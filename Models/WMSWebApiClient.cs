using BMHRI.WCS.Server.Tools;
using BMHRI.WCS.Server.UserControls;
using BMHRI.WCS.Server.WebApi.Protocols;
using Newtonsoft.Json.Linq;
using NPOI.OpenXmlFormats.Spreadsheet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace BMHRI.WCS.Server.Models
{
    public class WMSWebApiClient
    {
        private static readonly Lazy<WMSWebApiClient> lazy = new Lazy<WMSWebApiClient>(() => new WMSWebApiClient());
        public static WMSWebApiClient Instance { get { return lazy.Value; } }

        readonly string WCSTOWMSURL = ConfigurationManager.AppSettings["WCSTOWMSURL"];
        public void Start()
        {
            Task.Factory.StartNew(() => ProcessWMSTask(), TaskCreationOptions.LongRunning);
        }

        private async void ProcessWMSTask()
        {
            while (true)
            {
                try
                {
                    List<WMSTask> wMSTasks = WMSTasksManager.Instance.WMSTaskList.FindAll(x =>
                    x.TaskStatus == WMSTaskStatus.TaskDone ||
                    x.TaskStatus == WMSTaskStatus.Double_Inbound ||
                    x.TaskStatus == WMSTaskStatus.SSJ_APP_IN ||
                    x.TaskStatus == WMSTaskStatus.UnStack_Empty ||
                    x.TaskStatus == WMSTaskStatus.Pick_Up_Stop ||
                    x.TaskStatus == WMSTaskStatus.DeviceStatusChg ||
                    x.TaskStatus == WMSTaskStatus.SSJ_APP_EM||
                    x.TaskStatus == WMSTaskStatus.EmptyCountUpdate||
                    x.TaskStatus == WMSTaskStatus.AGVApplyGet ||
                    x.TaskStatus == WMSTaskStatus.LightLED ||
                    x.TaskStatus == WMSTaskStatus.UnLightLED ||                   
                    x.TaskStatus == WMSTaskStatus.AGVApplyInput);
                    if (wMSTasks != null && wMSTasks.Count > 0)
                    {
                        foreach (WMSTask wMSTask in wMSTasks)
                        {
                            if (wMSTask.TaskSource == "WCS")
                            {
                                WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                                continue;
                            }
                            switch (wMSTask.TaskStatus)
                            {
                                //case WMSTaskStatus.DeviceStatusChg:
                                //    SendToWMSDeviceStatus(wMSTask);
                                //    break;
                                case WMSTaskStatus.TaskDone:
                                    switch (wMSTask.TaskType)
                                    {
                                        case WMSTaskType.Moving:
                                            TaskDone(wMSTask, "M", wMSTask.WMSLocation, wMSTask.WMSLocation2);
                                            break;
                                        case WMSTaskType.Outbound:
                                        case WMSTaskType.SSJOutbound:
                                        case WMSTaskType.InToOut:
                                        case WMSTaskType.Directe:
                                            TaskDone(wMSTask, "F", wMSTask.WMSLocation, wMSTask.ToLocation);
                                            break;
                                        case WMSTaskType.Inbound:
                                            SSJInBoundTaskDone(wMSTask);
                                            break;
                                        case WMSTaskType.Stacking:
                                            TaskDone(wMSTask, "E", wMSTask.FmLocation, wMSTask.WMSLocation);
                                            break;
                                        case WMSTaskType.AGVMove:
                                            TaskDone(wMSTask, "G", wMSTask.FmLocation, wMSTask.ToLocation);
                                            break;
                                        case WMSTaskType.NoTaskQuit:
                                            WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                                            break;
                                    }
                                    break;
                                case WMSTaskStatus.Double_Inbound:
                                    SendToWMSExceptionReport(wMSTask, WCSToWMSTaskType.DoubleInbound, "双重入库");
                                    break;
                                case WMSTaskStatus.Pick_Up_Stop:
                                    SendToWMSExceptionReport(wMSTask, WCSToWMSTaskType.FarOutboundNearHave, "远端出库近端有货");
                                    break;
                                case WMSTaskStatus.UnStack_Empty:
                                    SendToWMSExceptionReport(wMSTask, WCSToWMSTaskType.UnStack_Empty, "空出库");
                                    break;
                                case WMSTaskStatus.SSJ_APP_IN://巷道申请
                                    await SendToWMSInboundApplyAsync(wMSTask);
                                    break;
                                case WMSTaskStatus.SSJ_APP_EM:
                                    SendToWMSPalletOutApply(wMSTask);
                                    break;
                                case WMSTaskStatus.EmptyCountUpdate:
                                    //SendToWMSEmptyCount(wMSTask);
                                    break;
                                case WMSTaskStatus.AGVApplyGet:
                                    //SendToWMSEmptyCount(wMSTask);
                                    SendToWMSAGVApplyGet(wMSTask);
                                    break;
                                case WMSTaskStatus.AGVApplyInput:
                                    //SendToWMSEmptyCount(wMSTask);
                                    SendToWMSAGVApplyInput(wMSTask);
                                    break;
                                case WMSTaskStatus.LightLED:                                   
                                    SendToWMSLightLED(wMSTask);                                
                                    break;
                                case WMSTaskStatus.UnLightLED:
                                    SendToWMSUnLightLED(wMSTask);
                                    break;
                                case WMSTaskStatus.DeviceStatusChg:
                                     SendToWMSDeviceStatus(wMSTask);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("ProcessWMSTask ", ex);
                }
                finally
                {
                    Task.Delay(1000).Wait();
                }
            }
        }
        
        public void SSJInBoundTaskDone(WMSTask wMSTask)//12.29货位申请
        {
            try
            {
                if (wMSTask == null) return;
               
                string ddjid = "";
                if(wMSTask.Warehouse == "1519") { ddjid = "1"; }
                else { ddjid =  wMSTask.ToLocation.Substring(3, 1); }
                string ssj_id = GetSSJID(wMSTask.FmLocation);//获取输送机块的PLCID
                SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == ssj_id);//
                if (sSJDevice == null) return ;
                SSJDeviceBlock sSJFmDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == wMSTask.FmLocation);
                if (sSJFmDeviceBlock == null) return ;

                SSJDeviceBlock sSJToDeviceBlock; 

                if (sSJFmDeviceBlock.Floor == 1 && (sSJFmDeviceBlock.SystemType == DeviceSystemType.Picking || sSJFmDeviceBlock.SystemType == DeviceSystemType.TotalPort))
                {
                    sSJToDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Tunnel == wMSTask.ToLocation && (x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin || x.SystemType == DeviceSystemType.TotalPort) && x.Floor == sSJFmDeviceBlock.Floor);
                }
                else
                {
                    sSJToDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Tunnel == wMSTask.ToLocation && (x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin || x.SystemType == DeviceSystemType.InboundFinish) && x.Floor == sSJFmDeviceBlock.Floor);
                }


                WCSReqWMSMsg inboundApplySecondAskReqMsg = new()
                {
                    //taskId = wMSTask.taskId,
                    TaskType = WCSToWMSTaskType.InboundSecondApply,
                    Location = wMSTask.Warehouse,
                    Palno = wMSTask.PalletNum,
                    Port = sSJToDeviceBlock.Position,
                    
                    Deviceno = ddjid,
                    BeginAddre = sSJToDeviceBlock.Position,
                    //EndAddre = ""

                };

                string rspstr = JsonHelp.Serialize(inboundApplySecondAskReqMsg);
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, certificate2, chain, sslErrors) =>
                    {
                        // 忽略所有证书错误
                        return true;
                    }
                };
                using var MyWMSHttpClient_inboundApplySecondAsk = new HttpClient(handler);
                // using HttpClient MyWMSHttpClient_inboundApplySecondAsk = new();
                //MyWMSHttpClient_inboundApplySecondAsk.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginCheckManager.Instance.LoginCheckList.Find(x => x.DeviceID == "JuWeiWMS").Token);
                var sendData = new StringContent(rspstr, Encoding.UTF8, "application/json");
                HttpResponseMessage response = MyWMSHttpClient_inboundApplySecondAsk.PostAsync(WCSTOWMSURL, sendData).Result;

                string result = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(result))
                {
                    WMSTask wMSInboundTask = WMSTasksManager.Instance.WMSTaskList.Find(x => x.PalletNum == wMSTask.PalletNum && x.TaskType == WMSTaskType.Inbound);
                    if (wMSInboundTask == null) return;
                    InsertIntoWMS2WCSLog("SSJInboundDoneAndApplyGoodsLocation ", rspstr, result, MsgDirection.Output);
                    WMSRspWCSMsg inboundApplySecondAskRspMsg = JsonSerializer.Deserialize<WMSRspWCSMsg>(result);
                    if (inboundApplySecondAskRspMsg != null && inboundApplySecondAskRspMsg.Code == "0")
                    {
                        WMSTask wMSStackingTask = new()
                        {
                            PalletNum = inboundApplySecondAskRspMsg.Data.Palno,
                            FmLocation = inboundApplySecondAskRspMsg.Data.Port,//ConvertTunnelToDeviceBlock(wMSTask.ToLocation, wMSTask),//申请货位的起始地址是申请巷道的目的地址
                            ToLocation = ConverWMSLocToWCSLoc(inboundApplySecondAskRspMsg.Data.EndAddre),
                            WMSLocation = inboundApplySecondAskRspMsg.Data.EndAddre,
                            TaskType = WMSTaskType.Stacking,
                        };
                        wMSStackingTask.Priority = 9;
                        wMSStackingTask.TaskStatus = WMSTaskStatus.TaskAssigned;
                        wMSStackingTask.TaskSource = "WMS";
                       // wMSStackingTask.taskId = inboundApplySecondAskRspMsg.data.taskId;
                        wMSStackingTask.WMSLocation = inboundApplySecondAskRspMsg.Data.EndAddre;
                        wMSStackingTask.Floor = wMSInboundTask.Floor;
                        wMSStackingTask.GaoDiBZ = wMSTask.GaoDiBZ;
                        wMSStackingTask.Warehouse = inboundApplySecondAskRspMsg.Data.Location;
                        WMSTasksManager.Instance.AddWMSTask(wMSStackingTask);
                        WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                    }
                    else
                        WMSTasksManager.Instance.UpdateWMSTaskStatus(wMSTask.WMSSeqID, WMSTaskStatus.TaskFeedBackFailed);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("InBoundTaskDone ", ex);
            }
        }

        public void TaskDone(WMSTask wMSTask, string taskType, string fmLocation, string toLocation)//12.29改
        {
            try
            {
                string messDesc = "";
                WCSReqWMSMsg TaskDoneReqMsg = new WCSReqWMSMsg { };
                if (taskType == "E")
                {
                    messDesc = "InboundFinish ";
                    TaskDoneReqMsg.TaskType = WCSToWMSTaskType.InboundFinish;
                    TaskDoneReqMsg.Location = wMSTask.Warehouse;
                    TaskDoneReqMsg.Palno = wMSTask.PalletNum;
                    TaskDoneReqMsg.Port = wMSTask.FmLocation;
                    if (TaskDoneReqMsg.Location == "1519") { TaskDoneReqMsg.Deviceno = "0"; }
                    else { TaskDoneReqMsg.Deviceno = wMSTask.FmLocation.Substring(2, 1); }
                    TaskDoneReqMsg.BeginAddre = wMSTask.FmLocation;
                    TaskDoneReqMsg.EndAddre = ConverWCSLocToWMSLoc(wMSTask.ToLocation);
                }

                else if (taskType == "F")
                {
                    if(wMSTask.ToLocation == "0000" &&( wMSTask.TaskType == WMSTaskType.InToOut || wMSTask.TaskType == WMSTaskType.Directe))
                    {
                        WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                        return;
                    }
                    messDesc = "OutboundFinish ";
                    TaskDoneReqMsg.TaskType = WCSToWMSTaskType.OutboundFinish;
                    TaskDoneReqMsg.Location = wMSTask.Warehouse;
                    TaskDoneReqMsg.Palno = wMSTask.PalletNum;
                    TaskDoneReqMsg.Port = wMSTask.ToLocation;
                    if (TaskDoneReqMsg.Location == "1519") //针对模具库的出库任务
                    {
                        TaskDoneReqMsg.Deviceno = "1";
                        TaskDoneReqMsg.BeginAddre = ConverWCSLocToWMSLoc(wMSTask.FmLocation);
                        TaskDoneReqMsg.EndAddre = wMSTask.ToLocation;
                    }
                    else 
                    {
                        if(wMSTask.FmLocation.Length==4)//起始地址不是货位，针对提升机换层任务
                        {
                            // inboundFinishReqMsg.Deviceno = wMSTask.ToLocation.Substring(2, 1);
                            TaskDoneReqMsg.BeginAddre = wMSTask.FmLocation;
                            TaskDoneReqMsg.EndAddre = wMSTask.ToLocation;
                        }
                        else
                        {
                            TaskDoneReqMsg.Deviceno = wMSTask.ToLocation.Substring(2, 1);//针对托盘库的出库任务
                            TaskDoneReqMsg.BeginAddre = ConverWCSLocToWMSLoc(wMSTask.FmLocation);
                            TaskDoneReqMsg.EndAddre = wMSTask.ToLocation;
                        }
                        
                    }
                    
                }
                else if (taskType == "M")
                {
                    messDesc = "MoveFinish ";
                    TaskDoneReqMsg.TaskType = WCSToWMSTaskType.MoveFinish;
                    TaskDoneReqMsg.Location = wMSTask.Warehouse;
                    TaskDoneReqMsg.Palno = wMSTask.PalletNum;
                    if (TaskDoneReqMsg.Location == "1519") { TaskDoneReqMsg.Deviceno = "1"; }
                    else { TaskDoneReqMsg.Deviceno = wMSTask.ToLocation.Substring(2, 1); }
                    TaskDoneReqMsg.BeginAddre = ConverWCSLocToWMSLoc(wMSTask.FmLocation);
                    TaskDoneReqMsg.EndAddre = ConverWCSLocToWMSLoc(wMSTask.ToLocation);
                }
                else if (taskType == "G")
                    messDesc = "AGVMove ";
                else
                    messDesc = "";
                if (wMSTask == null) return;
                

                string rspstr = JsonHelp.Serialize(TaskDoneReqMsg);
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, certificate2, chain, sslErrors) =>
                    {
                        // 忽略所有证书错误
                        return true;
                    }
                };
                using var MyWMSHttpClient_inboundFinish = new HttpClient(handler);
                //using HttpClient MyWMSHttpClient_inboundFinish = new();
                //  MyWMSHttpClient_inboundFinish.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginCheckManager.Instance.LoginCheckList.Find(x => x.DeviceID == "JuWeiWMS").Token);
                var sendData = new StringContent(rspstr, Encoding.UTF8, "application/json");
                HttpResponseMessage response = MyWMSHttpClient_inboundFinish.PostAsync(WCSTOWMSURL, sendData).Result;

                string result = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(result))
                {
                    
                    InsertIntoWMS2WCSLog(messDesc, rspstr, result, MsgDirection.Output);
                    WMSRspWCSMsg inboundFinishRspMsg = JsonSerializer.Deserialize<WMSRspWCSMsg>(result);
                    if (inboundFinishRspMsg != null && inboundFinishRspMsg.Code == "0")
                        WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                    else
                        WMSTasksManager.Instance.UpdateWMSTaskStatus(wMSTask.WMSSeqID, WMSTaskStatus.TaskFeedBackFailed);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TaskDone ", ex);
            }
        }


        private void SendToWMSExceptionReport(WMSTask wMSTask, WCSToWMSTaskType exceptTypes, string description)
        {
            try
            {
                //if (IsTokenNearExpiry(threshold, LoginName))
                    //LoginCheck();
                if (wMSTask == null) return;
                WCSReqWMSMsg exceptionReportReqMsg = new()
                {
                    TaskType = exceptTypes,
                    Location = wMSTask.Warehouse,
                    Palno = wMSTask.PalletNum,
                    Port = wMSTask.ToLocation,
                    Deviceno = wMSTask.ToLocation.Substring(2, 1),
                    BeginAddre = wMSTask.FmLocation,
                    EndAddre = ConverWCSLocToWMSLoc(wMSTask.ToLocation)
                };

                string rspstr = JsonHelp.Serialize(exceptionReportReqMsg);
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, certificate2, chain, sslErrors) =>
                    {
                        // 忽略所有证书错误
                        return true;
                    }
                };
                using var MyWMSHttpClient_exceptionReport = new HttpClient(handler);
                // using HttpClient MyWMSHttpClient_exceptionReport = new();
                var data = new StringContent(rspstr, Encoding.UTF8, "application/json");

                HttpResponseMessage response = MyWMSHttpClient_exceptionReport.PostAsync(WCSTOWMSURL, data).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(result))
                {
                    InsertIntoWMS2WCSLog("SendToWMSExceptionReport ", rspstr, result, MsgDirection.Output);
                    WMSRspWCSMsg exceptionReportRspMsg = JsonSerializer.Deserialize<WMSRspWCSMsg>(result);
                    if (exceptionReportRspMsg != null && exceptionReportRspMsg.Code == "0")
                        if (exceptTypes != WCSToWMSTaskType.DoubleInbound)
                            if(exceptTypes == WCSToWMSTaskType.FarOutboundNearHave)
                            {
                                WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.WMSSeqID == wMSTask.WMSSeqID && x.TaskType == WCSTaskTypes.DDJUnstack);
                                //foreach(WCSTask wcstask in wCSTasks)
                                //{
                                WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.Waiting);//远端操作近端有货，将当前堆垛机任务变为waiting，wms应该下发近端的移库指令。
                               // }
                                WMSTasksManager.Instance.UpdateWMSTaskStatus(wMSTask.WMSSeqID, WMSTaskStatus.TaskDoingingConfirm);

                            }
                            else
                            {
                                WMSTasksManager.Instance.DeleteWMSAndWCSTask(wMSTask);
                            }                        
                        else
                        {
                            if (!string.IsNullOrEmpty(exceptionReportRspMsg.Data.EndAddre))
                            {
                                if (exceptionReportRspMsg.Data.EndAddre.Length == 8)
                                {
                                    WMSTasksManager.Instance.UpdateWMSTaskToLocation(wMSTask.WMSSeqID, ConverWMSLocToWCSLoc(exceptionReportRspMsg.Data.EndAddre), exceptionReportRspMsg.Data.EndAddre);
                                    WMSTasksManager.Instance.UpdateWMSTaskStatus(wMSTask.WMSSeqID, WMSTaskStatus.GoodLocChg);
                                }
                                
                            }
                        }

                    else
                        WMSTasksManager.Instance.UpdateWMSTaskStatus(wMSTask.WMSSeqID, WMSTaskStatus.TaskFeedBackFailed);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SendToWMSExceptionReport ", ex);
            }
        }
        private async Task SendToWMSInboundApplyAsync(WMSTask wMSTask)
        {
            if (wMSTask == null) return;

            try
            {
                // 创建请求消息
                WCSReqWMSMsg inboundApplyReqMsg = new()
                {
                    TaskType = WCSToWMSTaskType.InboundApply,
                    Location = wMSTask.Warehouse,
                    Palno = wMSTask.PalletNum,
                    Port = wMSTask.FmLocation,
                    HLAddre = wMSTask.GaoDiBZ.ToString().ToLower()
                };

                // 序列化为 JSON 字符串
                string rspstr = JsonHelp.Serialize(inboundApplyReqMsg);

                // 配置 HttpClientHandler，忽略所有证书错误
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, certificate2, chain, sslErrors) => true
                };

                using var MyWMSHttpClient_InboundApply = new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromSeconds(3) // 10秒超时
                };

                // 创建请求内容
                var data = new StringContent(rspstr, Encoding.UTF8, "application/json");

                // 创建一个 CancellationTokenSource，10 秒后取消
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

                // 异步发送 POST 请求，并传入 CancellationToken
                HttpResponseMessage response = await MyWMSHttpClient_InboundApply.PostAsync(WCSTOWMSURL, data, cts.Token);

                // 异步读取响应内容，并传入 CancellationToken
                string result = await response.Content.ReadAsStringAsync(cts.Token);

                if (!string.IsNullOrEmpty(result))
                {
                    // 记录日志
                    InsertIntoWMS2WCSLog("SendToWMSInboundApply ", rspstr, result, MsgDirection.Output);

                    // 反序列化响应消息
                    WMSRspWCSMsg inboundApplyRspMsg = JsonSerializer.Deserialize<WMSRspWCSMsg>(result);

                    if (inboundApplyRspMsg != null && inboundApplyRspMsg.Code == "0")
                    {
                        // 创建新的任务
                        WMSTask wMSTask1 = new WMSTask()
                        {
                            Warehouse = inboundApplyRspMsg.Data.Location,
                            PalletNum = inboundApplyRspMsg.Data.Palno,
                            FmLocation = inboundApplyRspMsg.Data.BeginAddre,
                            TaskStatus = WMSTaskStatus.TaskAssigned,
                            TaskSource = "WMS",
                        };

                        // 根据 EndType 设置任务类型和 ToLocation
                        if (inboundApplyRspMsg.Data.EndType == "3") // 提升机换层任务申请
                        {
                            wMSTask1.TaskType = WMSTaskType.Directe;
                            SSJDevice TosSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJID(inboundApplyRspMsg.Data.EndAddre));
                            SSJDeviceBlock TosSJDeviceblock = TosSJDevice.DeviceBlockList.Find(x => x.Position == inboundApplyRspMsg.Data.EndAddre);
                            wMSTask1.ToLocation = TosSJDeviceblock.CurrentMode != DeviceModeType.OutboundMode ? "0000" : inboundApplyRspMsg.Data.EndAddre;
                        }
                        else if (inboundApplyRspMsg.Data.EndType == "5") // ddj直出任务
                        {
                            wMSTask1.TaskType = WMSTaskType.InToOut;
                            SSJDevice TosSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJID(inboundApplyRspMsg.Data.EndAddre));
                            SSJDeviceBlock TosSJDeviceblock = TosSJDevice.DeviceBlockList.Find(x => x.Position == inboundApplyRspMsg.Data.EndAddre);
                            wMSTask1.ToLocation = TosSJDeviceblock.CurrentMode != DeviceModeType.OutboundMode ? "0000" : inboundApplyRspMsg.Data.EndAddre;
                        }
                        else // 普通入库逻辑
                        {
                            wMSTask1.ToLocation = inboundApplyRspMsg.Data.Location == "1519" ? "0003" : "000" + inboundApplyRspMsg.Data.Deviceno;
                            wMSTask1.TaskType = WMSTaskType.Inbound;
                        }

                        // 添加新任务，删除旧任务
                        WMSTasksManager.Instance.AddWMSTask(wMSTask1);
                        WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                    }
                    else
                    {
                        // 无任务退出逻辑
                        WMSTask wMSNoTask = new()
                        {
                            PalletNum = wMSTask.PalletNum,
                            FmLocation = wMSTask.FmLocation,
                            ToLocation = "0000",
                            TaskType = WMSTaskType.NoTaskQuit,
                            TaskStatus = WMSTaskStatus.TaskAssigned,
                            TaskSource = "WMS"
                        };
                        WMSTasksManager.Instance.AddWMSTask(wMSNoTask);
                        WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // 超时
                HandleTimeout(wMSTask);
            }
            catch (Exception ex)
            {
                // 其他异常
                LogHelper.WriteLog("SendToWMSInboundApplyAsync ", ex);
            }
        }


        //执行无任务退出
        private void HandleTimeout(WMSTask wMSTask)
        {
            LogHelper.WriteLog("SendToWMSInboundApply - Timeout occurred.");

            // 无任务退出逻辑
            WMSTask wMSNoTask = new()
            {
                PalletNum = wMSTask.PalletNum,
                FmLocation = wMSTask.FmLocation,
                ToLocation = "0000",
                TaskType = WMSTaskType.NoTaskQuit,
                TaskStatus = WMSTaskStatus.TaskAssigned,
                TaskSource = "WMS"
            };
            WMSTasksManager.Instance.AddWMSTask(wMSNoTask);
            WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
        }


        private void SendToWMSPalletOutApply(WMSTask wMSTask)
        {
            try
            {
                //if (IsTokenNearExpiry(threshold, LoginName))
                    //LoginCheck();
                if (wMSTask == null) return;
                WCSReqWMSMsg emptyPalletApplyReqMsg = new()
                {
                    Location = wMSTask.Warehouse,
                    //Palno = wMSTask.PalletNum,
                    Port = wMSTask.FmLocation
                };

                string rspstr = JsonHelp.Serialize(emptyPalletApplyReqMsg);
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, certificate2, chain, sslErrors) =>
                    {
                        // 忽略所有证书错误
                        return true;
                    }
                };
                using var MyWMSHttpClient_EmptyPalletApply = new HttpClient(handler);
                //  using HttpClient MyWMSHttpClient_EmptyPalletApply = new();
                var data = new StringContent(rspstr, Encoding.UTF8, "application/json");
               // MyWMSHttpClient_EmptyPalletApply.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginCheckManager.Instance.LoginCheckList.Find(x => x.DeviceID == "JuWeiWMS").Token);
                HttpResponseMessage response = MyWMSHttpClient_EmptyPalletApply.PostAsync(WCSTOWMSURL, data).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(result))
                {
                    InsertIntoWMS2WCSLog("SendToWMSEmptyPalletApply ", rspstr, result, MsgDirection.Output);
                    WMSRspWCSMsg emptyPalletApplyRspMsg = JsonSerializer.Deserialize<WMSRspWCSMsg>(result);
                    if (emptyPalletApplyRspMsg != null && emptyPalletApplyRspMsg.Code == "0")
                    {
                        WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                    }
                    else
                        WMSTasksManager.Instance.UpdateWMSTaskStatus(wMSTask.WMSSeqID, WMSTaskStatus.TaskFeedBackFailed);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SendToWMSInboundApply ", ex);
            }
        }

        private void SendToWMSAGVApplyGet(WMSTask wMSTask)//agv允许取货,目前的判断条件是agv与输送机托盘号是否一致
        {
            try
            {
                if (wMSTask == null) return;
                SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJID(wMSTask.FmLocation));
                if (sSJDevice == null) return;
                SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == wMSTask.FmLocation);
                WCSReqWMSMsg ApplyGetReqMsg = new WCSReqWMSMsg { };
                string messDesc ;
                if (sSJDeviceBlock.PalletNum != wMSTask.PalletNum)
                {
                    return;
                }

                    ApplyGetReqMsg.TaskType = WCSToWMSTaskType.GetAllowed;
                    ApplyGetReqMsg.Location = wMSTask.Warehouse;
                    ApplyGetReqMsg.Palno = wMSTask.PalletNum;
                    ApplyGetReqMsg.Port = wMSTask.FmLocation;
                    ApplyGetReqMsg.EndAddre = wMSTask.FmLocation;
                    messDesc = "AGVApplyGet";
                

                string rspstr = JsonHelp.Serialize(ApplyGetReqMsg);
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, certificate2, chain, sslErrors) =>
                    {
                        // 忽略所有证书错误
                        return true;
                    }
                };
                using var MyWMSHttpClient_inboundFinish = new HttpClient(handler);
                var sendData = new StringContent(rspstr, Encoding.UTF8, "application/json");
                HttpResponseMessage response = MyWMSHttpClient_inboundFinish.PostAsync(WCSTOWMSURL, sendData).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(result))
                {
                    InsertIntoWMS2WCSLog(messDesc, rspstr, result, MsgDirection.Output);
                    WMSRspWCSMsg inboundFinishRspMsg = JsonSerializer.Deserialize<WMSRspWCSMsg>(result);
                    if (inboundFinishRspMsg != null && inboundFinishRspMsg.Code == "0")
                        WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                    else
                        WMSTasksManager.Instance.UpdateWMSTaskStatus(wMSTask.WMSSeqID, WMSTaskStatus.TaskFeedBackFailed);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TaskDone ", ex);
            }
        }
        private void SendToWMSAGVApplyInput(WMSTask wMSTask)//
        {
            try
            {
                if (wMSTask == null) return;
                SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJID(wMSTask.FmLocation));
                if (sSJDevice == null) return;
                SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == wMSTask.FmLocation);
                //if (sSJDeviceBlock.PalletNum != null) return;
                if (!sSJDeviceBlock.Allow_AGV_Put)
                {
                    return;
                }
                sSJDevice.OutTaskLimitDataItem(sSJDeviceBlock.AGVPutMBAddr, (byte)sSJDeviceBlock.AGVPutMBAddr2, 1);
                WCSReqWMSMsg ApplyInputReqMsg = new WCSReqWMSMsg { };
                // messDesc = "AllowPut ";
                ApplyInputReqMsg.TaskType = WCSToWMSTaskType.PutAllowed;
                ApplyInputReqMsg.Location = wMSTask.Warehouse;
                ApplyInputReqMsg.Palno = wMSTask.PalletNum;
                ApplyInputReqMsg.Port = wMSTask.FmLocation;
                ApplyInputReqMsg.EndAddre = wMSTask.FmLocation;
                //SSJDeviceBlock sSJDeviceBlock = SSJDevice
                string messDesc = "AGVApplyInput";
                string rspstr = JsonHelp.Serialize(ApplyInputReqMsg);
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, certificate2, chain, sslErrors) =>
                    {
                        // 忽略所有证书错误
                        return true;
                    }
                };
                using var MyWMSHttpClient_inboundFinish = new HttpClient(handler);
                // using HttpClient MyWMSHttpClient_inboundFinish = new();
                //  MyWMSHttpClient_inboundFinish.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginCheckManager.Instance.LoginCheckList.Find(x => x.DeviceID == "JuWeiWMS").Token);

                var sendData = new StringContent(rspstr, Encoding.UTF8, "application/json");
                HttpResponseMessage response = MyWMSHttpClient_inboundFinish.PostAsync(WCSTOWMSURL, sendData).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(result))
                {
                    InsertIntoWMS2WCSLog(messDesc, rspstr, result, MsgDirection.Output);
                    WMSRspWCSMsg inboundFinishRspMsg = JsonSerializer.Deserialize<WMSRspWCSMsg>(result);
                    if (inboundFinishRspMsg != null && inboundFinishRspMsg.Code == "0")
                        WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                    else
                        WMSTasksManager.Instance.UpdateWMSTaskStatus(wMSTask.WMSSeqID, WMSTaskStatus.TaskFeedBackFailed);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TaskDone ", ex);
            }
        }

        private void SendToWMSLightLED(WMSTask wMSTask)//2025/01/04,WCStoWMS 点亮LED
        {
            try
            {
                if (wMSTask == null) return;
                SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJID(wMSTask.ToLocation));
                if (sSJDevice == null) return;
                SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == wMSTask.ToLocation);
                if (sSJDeviceBlock.PalletNum != wMSTask.PalletNum) return;
                WCSReqWMSMsg LightLEDReqMsg = new WCSReqWMSMsg { };
                // messDesc = "AllowPut ";
                LightLEDReqMsg.TaskType = WCSToWMSTaskType.LedOn;
                LightLEDReqMsg.Location = sSJDeviceBlock.WareHouse;
                LightLEDReqMsg.Palno = wMSTask.PalletNum;
                LightLEDReqMsg.Port = wMSTask.ToLocation;
                LightLEDReqMsg.Deviceno = wMSTask.ToLocation;
                LightLEDReqMsg.BeginAddre = ConverWCSLocToWMSLoc(wMSTask.FmLocation);
                LightLEDReqMsg.EndAddre = wMSTask.ToLocation;
                // LightLEDReqMsg.EndAddre = wMSTask.FmLocation;
                //SSJDeviceBlock sSJDeviceBlock = SSJDevice
                string messDesc = "LightLED";
                string rspstr = JsonHelp.Serialize(LightLEDReqMsg);
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, certificate2, chain, sslErrors) =>
                    {
                        // 忽略所有证书错误
                        return true;
                    }
                };
                using var MyWMSHttpClient_inboundFinish = new HttpClient(handler);
                //  using HttpClient MyWMSHttpClient_inboundFinish = new();
                //  MyWMSHttpClient_inboundFinish.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginCheckManager.Instance.LoginCheckList.Find(x => x.DeviceID == "JuWeiWMS").Token);
                var sendData = new StringContent(rspstr, Encoding.UTF8, "application/json");
                HttpResponseMessage response = MyWMSHttpClient_inboundFinish.PostAsync(WCSTOWMSURL, sendData).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(result))
                {
                    InsertIntoWMS2WCSLog(messDesc, rspstr, result, MsgDirection.Output);
                    WMSRspWCSMsg inboundFinishRspMsg = JsonSerializer.Deserialize<WMSRspWCSMsg>(result);
                    if (inboundFinishRspMsg != null && inboundFinishRspMsg.Code == "0")
                        WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                    else
                        WMSTasksManager.Instance.UpdateWMSTaskStatus(wMSTask.WMSSeqID, WMSTaskStatus.TaskFeedBackFailed);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TaskDone ", ex);
            }
        }   
        private void SendToWMSUnLightLED(WMSTask wMSTask)//2025/01/04,WCStoWMS 熄灭LED
        {
            try
            {
                if (wMSTask == null) return;
                SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == GetSSJID(wMSTask.ToLocation));
                if (sSJDevice == null) return;
                SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == wMSTask.ToLocation);
                //if (sSJDeviceBlock.PalletNum != wMSTask.PalletNum) return;
                WCSReqWMSMsg UnLightLEDReqMsg = new WCSReqWMSMsg { };
                // messDesc = "AllowPut ";
                UnLightLEDReqMsg.TaskType = WCSToWMSTaskType.LedOff;
                UnLightLEDReqMsg.Location = sSJDeviceBlock.WareHouse;
                UnLightLEDReqMsg.Palno = wMSTask.PalletNum;
                UnLightLEDReqMsg.Port = wMSTask.ToLocation;
                UnLightLEDReqMsg.Deviceno = wMSTask.ToLocation;
                // LightLEDReqMsg.EndAddre = wMSTask.FmLocation;
                //SSJDeviceBlock sSJDeviceBlock = SSJDevice
                string messDesc = "LightLEDOff";
                string rspstr = JsonHelp.Serialize(UnLightLEDReqMsg);
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, certificate2, chain, sslErrors) =>
                    {
                        // 忽略所有证书错误
                        return true;
                    }
                };
                using var MyWMSHttpClient_inboundFinish = new HttpClient(handler);
                // using HttpClient MyWMSHttpClient_inboundFinish = new();
                //  MyWMSHttpClient_inboundFinish.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginCheckManager.Instance.LoginCheckList.Find(x => x.DeviceID == "JuWeiWMS").Token);
                var sendData = new StringContent(rspstr, Encoding.UTF8, "application/json");
                HttpResponseMessage response = MyWMSHttpClient_inboundFinish.PostAsync(WCSTOWMSURL, sendData).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(result))
                {
                    InsertIntoWMS2WCSLog(messDesc, rspstr, result, MsgDirection.Output);
                    WMSRspWCSMsg inboundFinishRspMsg = JsonSerializer.Deserialize<WMSRspWCSMsg>(result);
                    if (inboundFinishRspMsg != null && inboundFinishRspMsg.Code == "0")
                        WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                    else
                        WMSTasksManager.Instance.UpdateWMSTaskStatus(wMSTask.WMSSeqID, WMSTaskStatus.TaskFeedBackFailed);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TaskDone ", ex);
            }
        }
        public void  SendToWMSDeviceStatus(WMSTask wMSTask)
        {
            try
            {
                WCSReqWMSMsg deviceStatusReqMsg = new WCSReqWMSMsg { };
                if (wMSTask.PalletNum.Length == 1)
                {
                    deviceStatusReqMsg.TaskType = WCSToWMSTaskType.DDJStatus;
                    deviceStatusReqMsg.Location = wMSTask.Warehouse;
                    deviceStatusReqMsg.Deviceno = wMSTask.PalletNum.ToString();
                    deviceStatusReqMsg.DevicenoStatu = wMSTask.ToLocation;
                }
                else//WCSReqWMSMsg deviceStatusReqMsg = new()
                {
                    deviceStatusReqMsg.TaskType = WCSToWMSTaskType.DDJStatus;
                    deviceStatusReqMsg.Location = wMSTask.Warehouse;
                    deviceStatusReqMsg.Port = wMSTask.PalletNum.ToString();
                    deviceStatusReqMsg.DevicenoStatu = wMSTask.ToLocation;
                };
                string rspstr = JsonHelp.Serialize(deviceStatusReqMsg);
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, certificate2, chain, sslErrors) =>
                    {
                        // 忽略所有证书错误
                        return true;
                    }
                };
                using var MyWMSHttpClient_deviceStatus = new HttpClient(handler);
                // 后续发送请求…
              //  using HttpClient MyWMSHttpClient_deviceStatus = new();

                //MyWMSHttpClient_deviceStatus.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginCheckManager.Instance.LoginCheckList.Find(x => x.DeviceID == "JuWeiWMS").Token);
                var sendData = new StringContent(rspstr, Encoding.UTF8, "application/json");
                HttpResponseMessage response = MyWMSHttpClient_deviceStatus.PostAsync(WCSTOWMSURL, sendData).Result;

                

                string result = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(result))
                {
                    InsertIntoWMS2WCSLog("DeviceStatus ", rspstr, result, MsgDirection.Output);
                    WMSRspWCSMsg deviceStatusRsqMsg = JsonSerializer.Deserialize<WMSRspWCSMsg>(result);
                    if (deviceStatusRsqMsg != null && deviceStatusRsqMsg.Code == "0")
                        WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
                    else
                        WMSTasksManager.Instance.UpdateWMSTaskStatus(wMSTask.WMSSeqID, WMSTaskStatus.TaskFeedBackFailed);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SendToWMSDeviceStatus ", ex);
            }
        }
        private string GetSSJID(string toLocation)
        {
            //string ssjid = "";
            //if (string.IsNullOrEmpty(toLocation)) return null;
            //ssjid = string.Concat("SSJ0", toLocation.AsSpan(0, 1));
            //return ssjid;
            string plcid = null;
            string sqlstr = string.Format("select PLCID from dbo.ConveyerBlocks where Position='{0}'", toLocation);
            object obj = SQLServerHelper.DataBaseReadToObject(sqlstr);
            if (obj == null) return "";
            if (string.IsNullOrEmpty(obj.ToString())) return "";
            plcid = obj.ToString();
            return plcid;
        }
        //private void SendToWMSEmptyCount(WMSTask wMSTask)
        //{
        //    try
        //    {
        //        //if (IsTokenNearExpiry(threshold, LoginName))
        //            //LoginCheck();
        //        if (wMSTask == null) return;
        //        EmptyCountReqMsg emptyCountReq = new()
        //        {
        //            wPalletCode = wMSTask.PalletNum,
        //            emptyPalletCount=wMSTask.DiePanJiCount
        //        };

        //        string rspstr = JsonHelp.Serialize(emptyCountReq);
        //        using HttpClient MyWMSHttpClient_EmptyCount = new();
        //        var data = new StringContent(rspstr, Encoding.UTF8, "application/json");
        //        MyWMSHttpClient_EmptyCount.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginCheckManager.Instance.LoginCheckList.Find(x => x.DeviceID == "JuWeiWMS").Token);
        //        HttpResponseMessage response = MyWMSHttpClient_EmptyCount.PostAsync(UpdateEmptyCountUrl, data).Result;
        //        string result = response.Content.ReadAsStringAsync().Result;
        //        if (!string.IsNullOrEmpty(result))
        //        {
        //            InsertIntoWMS2WCSLog("SendToWMSEmptyCountUpdate ", rspstr, result, MsgDirection.Output);
        //            EmptyCountRspMsg emptyCountRsp = JsonSerializer.Deserialize<EmptyCountRspMsg>(result);
        //            if (emptyCountRsp != null && emptyCountRsp.code == 1)
        //            {
        //                WMSTasksManager.Instance.DeleteWMSTaskAtID(wMSTask.WMSSeqID);
        //            }
        //            else
        //                WMSTasksManager.Instance.UpdateWMSTaskStatus(wMSTask.WMSSeqID, WMSTaskStatus.TaskFeedBackFailed);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.WriteLog("SendToWMSInboundApply ", ex);
        //    }
        //}


        #region Java_registerNo
        /// <summary>
        /// 返回Signature
        /// </summary>
        /// <param name="registerNo"></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name="timestamp"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        // 入参：registerNo = "6752b78c-f6f1-4f31-9077-d7f0a4bb982b" ; params = "wmsTaskNo&wmsTaskType&wcsTaskType&palletNo&fromLoc&toLoc";   Command是一个Map KEYVALUE对象 HashMap<String, Object>
        //  Command 为一个KEYVALUE的map对象
        //将要传给大参林的参数作为键值对存储进来
        //例如 :
        // Command<string, Object> map = Command<string, Object>();
        // map.Add("wmsTaskNo", "PA0009563556");
        // map.Add("wmsTaskType", "IN");
        // map.Add("wcsTaskType", "I");
        // map.Add("palletNo", "80005006");
        // map.Add("fromLoc", "11");
        // map.Add("toLoc", "8101790");
        //public static String digestSign(String registerNo, String params111, long timestamp, Command command)
        //{
        //    StringBuilder sb = new StringBuilder();                     // 定义一个字符串对象
        //    sb.append("registerNo=").append(registerNo).append("&");    // 将入参拼接,该方法的目的是为了实现该效果: registerNo=6752b78c-f6f1-4f31-9077-d7f0a4bb982b&
        //    if (!StringUtils.isEmpty(params))
        //    {  // 判断入参不为空
        //        String[] keys = params.split("&");  // 从入参params根据 & 进行分组拆分  得到的值为  arrylist: ['wmsTaskNo','wmsTaskType','wcsTaskType','palletNo','fromLoc','toLoc']
        //        for (String key : keys)
        //        {   // 循环遍历 
        //            if (command.containsKey(key))
        //            {  // 根据上面的拆分list对象循环取key,得到Command value值进行拼接
        //                sb.append(key).append("=").append(command.get(key)).append("&");   //拼接字符串并追加到地上代码： registerNo=6752b78c-f6f1-4f31-9077-d7f0a4bb982b&wmsTaskNo=PA0009563556&wmsTaskType=IN&........
        //            }
        //        }
        //    }
        //    sb.append("timestamp=").append(timestamp); //最后拼接一个当前时间戳 registerNo = 6752b78c - f6f1 - 4f31 - 9077 - d7f0a4bb982b & wmsTaskNo = PA0009563556 & wmsTaskType = IN & ........&timestamp = 1698399121000
        //    return DigestUtils.md5DigestAsHex(sb.toString().getBytes()); //将一开始定义的字符串对象进行MD5加密
        //}
        #endregion

        static string GetSignature(string msgId, string customerId, string appKey, string body)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(msgId).Append(customerId).Append(appKey).Append(body);

            // 使用 MD5 加密算法32位大写的md5值
            using (MD5 md5 = MD5.Create())
            {
                byte[] data = Encoding.UTF8.GetBytes(sb.ToString());
                byte[] hashBytes = md5.ComputeHash(data);
                StringBuilder sb2 = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb2.Append(hashBytes[i].ToString("X2"));
                }
                return sb2.ToString();
            }
        }
        public string ConverWMSLocToWCSLoc(string wmsLoc)     //B501010101   2位仓库编码+2位区域编码+货架号（1~2位）+2位列数编码+2位层数编码
        {//G0100201
            string wcsLoc;
            if (wmsLoc.Length != 8) return "00000000";
            wcsLoc = wmsLoc.Substring(1, 2) + "0" + wmsLoc.Substring(3, 3) + wmsLoc.Substring(6, 2);
            return wcsLoc;
        }

        public string ConverWCSLocToWMSLoc(string wmsLoc)     //B501010101   2位仓库编码+2位区域编码+货架号（1~2位）+2位列数编码+2位层数编码
        {//G0100201 01000201
            string wcsLoc;
            if (wmsLoc.Length != 8) return "00000000";
            wcsLoc = "G"+wmsLoc.Substring(0, 2) + wmsLoc.Substring(3, 3) + wmsLoc.Substring(6, 2) ;
            return wcsLoc;
        }

        private string ConvertTunnelToDeviceBlock(string tunnel, WMSTask wMSTask)
        {
            WMSTask wMSTask1 = WMSTasksManager.Instance.WMSTaskList.Find(x => x.WMSSeqID == wMSTask.WMSSeqID);
            if (wMSTask1 == null) return null;
            SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == "SSJ0" + wMSTask1.Floor.ToString());
            if (sSJDevice == null) return null;
            SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Tunnel == tunnel && (x.SystemType == DeviceSystemType.InboundFinish || x.SystemType == DeviceSystemType.InboundFinishOrOutboundBegin));
            if (sSJDeviceBlock == null) return null;
            return sSJDeviceBlock.Position;
        }
        public long ConvertDateTimeInt(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalMilliseconds;
        }

        private static void InsertIntoWMS2WCSLog(string msgDesc, string reqMsg, string rspMsg, MsgDirection msgdir)
        {
            try
            {
                SystemManager.Instance.UpdateWmsConnectStatus("WMS", SystemStatus.Connected);
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@MsgDesc",SqlNull(msgDesc)),
                    new SqlParameter("@MsgDirection",SqlNull(msgdir)),
                    new SqlParameter("@ReqMsg",SqlNull(reqMsg)),
                    new SqlParameter("@RspMsg",SqlNull(rspMsg)),
                };
                SQLServerHelper.ExeSQLStringWithParam("INSERT INTO [dbo].[WCS2WMSJsonLog] " +
                    "([MsgDesc],[MsgDirection],[ReqMsg],[RspMsg])VALUES" +
                    "(@MsgDesc,@MsgDirection , @ReqMsg ,@RspMsg)", sqlParameters);
            }
            catch (Exception ex)
            {
                SystemManager.Instance.UpdateWmsConnectStatus("WMS", SystemStatus.DisConnected);
                LogHelper.WriteLog("InsertIntoWMS2WCSLog ", ex);
            }
        }
        public static object SqlNull(object obj)
        {
            if (obj == null)
                return DBNull.Value;
            return obj;
        }
    }
}
