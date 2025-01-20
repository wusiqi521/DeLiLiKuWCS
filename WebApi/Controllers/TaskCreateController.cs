using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.Tools;
using BMHRI.WCS.Server.UserControls;
using BMHRI.WCS.Server.WebApi.Protocols;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace BMHRI.WCS.Server.WebApi.Controllers
{
    [ApiController]
    [Route("WMSToWCS/[controller]")]
    public class TaskCreateController : ControllerBase
    {
        [HttpPost(Name = "TaskCreate")]
        [Produces("application/json")]
        public WCSRspWMSMsg TaskCreateReqMsgProcess(dynamic taskCreateReqMsgStr)
        {
            WMSReqWCSMsg wMSTOWCSReqMsg = JsonSerializer.Deserialize<WMSReqWCSMsg>(taskCreateReqMsgStr);
            WCSRspWMSMsg wMSTOWCSRspMsg = TaskCreateProcess(wMSTOWCSReqMsg);
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All); //解决中文序列化被编码的问题
            options.Converters.Add(new DateTimeConverter()); //解决时间格式序列化的问题
            string reqstr = JsonSerializer.Serialize(wMSTOWCSReqMsg, options);
            string rspstr = JsonSerializer.Serialize(wMSTOWCSRspMsg, options);
            //InserIntoWMSJsonMsgLog(1, reqstr, rspstr);
            Task.Factory.StartNew(() => { InserIntoWMSJsonMsgLog("TaskCreate", 1, reqstr, rspstr); });
            return wMSTOWCSRspMsg;
            //return JsonSerializer.Serialize(wMSTaskReqMsg);
        }

        public WCSRspWMSMsg TaskCreateProcess(WMSReqWCSMsg wMSTOWCSReqMsg)
        {
            WCSRspWMSMsg wMSTOWCSRspMsg = new WCSRspWMSMsg();
            if (wMSTOWCSReqMsg != null)
            {
                if (string.IsNullOrEmpty(wMSTOWCSReqMsg.TaskType.ToString()) || string.IsNullOrEmpty(wMSTOWCSReqMsg.Palno))
                {
                    wMSTOWCSRspMsg.Code = "1";
                    wMSTOWCSRspMsg.Message = "TaskType和Palno不能为空!";
                    return wMSTOWCSRspMsg;
                }
                switch (wMSTOWCSReqMsg.TaskType)
                {
                    case WMSToWCSTaskType.OutBoundTask:
                        WMSTask wMSOutTask = WMSTasksManager.Instance.WMSTaskList.Find(x => x.PalletNum == wMSTOWCSReqMsg.Palno);
                        if (wMSOutTask == null)
                        {
                            WMSTask wMSTask = new()
                            {
                                PalletNum = wMSTOWCSReqMsg.Palno,
                                FmLocation = ConverWMSLocToWCSLoc(wMSTOWCSReqMsg.BeginAddre),
                                WMSLocation = wMSTOWCSReqMsg.BeginAddre,
                                ToLocation = wMSTOWCSReqMsg.EndAddre,
                                Warehouse = wMSTOWCSReqMsg.Location,
                                TaskType = WMSTaskType.Outbound,
                                TaskStatus = WMSTaskStatus.TaskAssigned,
                                TaskSource = "WMS"
                            };
                            //if (wMSTask.ToLocation.Substring(0, 1) == "4")    //floor待确定
                            //    wMSTask.Floor = 2;
                            WMSTasksManager.Instance.AddWMSTask(wMSTask);
                            wMSTOWCSRspMsg.Code = "0";
                            wMSTOWCSRspMsg.Message = "成功";
                        }
                        else
                        {
                            wMSTOWCSRspMsg.Code = "1";
                            wMSTOWCSRspMsg.Message = "出库任务重复推送!";
                            wMSTOWCSRspMsg.Data = null;
                        }
                        break;
                    case WMSToWCSTaskType.MoveTask:
                        WMSTask wMSMoveTask = WMSTasksManager.Instance.WMSTaskList.Find(x => x.PalletNum == wMSTOWCSReqMsg.Palno);
                        if (wMSMoveTask == null)
                        {
                            WMSTask wMSTask = new()
                            {
                                PalletNum = wMSTOWCSReqMsg.Palno,
                                FmLocation = ConverWMSLocToWCSLoc(wMSTOWCSReqMsg.BeginAddre),
                                ToLocation = ConverWMSLocToWCSLoc(wMSTOWCSReqMsg.EndAddre),
                                WMSLocation = wMSTOWCSReqMsg.BeginAddre,
                                WMSLocation2 = wMSTOWCSReqMsg.EndAddre,
                                Warehouse = wMSTOWCSReqMsg.Location,
                                TaskType = WMSTaskType.Moving,
                                TaskStatus = WMSTaskStatus.TaskAssigned,
                                TaskSource = "WMS"
                                //Priority =9
                            };
                            //if (wMSTask.ToLocation.Substring(0, 1) == "4")    //floor待确定
                            //    wMSTask.Floor = 2;
                            WMSTasksManager.Instance.AddWMSTask(wMSTask);
                            wMSTOWCSRspMsg.Code = "0";
                            wMSTOWCSRspMsg.Message = "成功";
                        }
                        else
                        {
                            wMSTOWCSRspMsg.Code = "1";
                            wMSTOWCSRspMsg.Message = "倒库任务重复推送!";
                            wMSTOWCSRspMsg.Data = null;
                        }
                        break;
                    case WMSToWCSTaskType.TaskCancel:

                         WMSTask wMSTaskCanel = WMSTasksManager.Instance.WMSTaskList.Find(x => x.PalletNum == wMSTOWCSReqMsg.Palno);
                         if (wMSTaskCanel == null)   //只可以取消出库任务和倒库任务
                         {
                            wMSTOWCSRspMsg.Message = "该托盘号任务不存在";
                            wMSTOWCSRspMsg.Data = null;
                            wMSTOWCSRspMsg.Code = "1";                           
                         }
                         else
                         {
                            WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.PalletNum == wMSTaskCanel.PalletNum && (x.TaskType == WCSTaskTypes.DDJUnstack || x.TaskType == WCSTaskTypes.DDJStackMove));
                             if (wCSTask == null)
                             {                                  
                              wMSTOWCSRspMsg.Message = "入库任务无法取消，取消失败";
                              wMSTOWCSRspMsg.Data = null;
                              wMSTOWCSRspMsg.Code = "1";
                             }
                             else
                              {
                                    if (wCSTask.TaskStatus == WCSTaskStatus.Waiting)   //任务取消成功!
                                    {
                                    wMSTOWCSRspMsg.Message = "任务取消成功！";
                                    wMSTOWCSRspMsg.Data = null;
                                    wMSTOWCSRspMsg.Code = "0";
                                    WMSTasksManager.Instance.DeleteWMSAndWCSTask(wMSTaskCanel);
                                    }
                                    else
                                    {
                                    wMSTOWCSRspMsg.Message = "任务已经执行，无法取消";
                                    wMSTOWCSRspMsg.Data = null;
                                    wMSTOWCSRspMsg.Code = "1";
                                }
                              }

                         }
                        break;
                    case WMSToWCSTaskType.ApplyPut:
                        wMSTOWCSRspMsg.Message = "收到申请放货信息";
                        wMSTOWCSRspMsg.Data = null;
                        wMSTOWCSRspMsg.Code = "0";
                        WMSTask wMSApplyPutTask = new()
                        {
                            PalletNum = wMSTOWCSReqMsg.Palno,
                            Warehouse = wMSTOWCSReqMsg.Location,
                            TaskType = WMSTaskType.AGVApplyInput,
                            TaskStatus = WMSTaskStatus.AGVApplyInput,
                            TaskSource = "WMS",
                            FmLocation = wMSTOWCSReqMsg.Port
                        };
                        WMSTasksManager.Instance.AddWMSTask(wMSApplyPutTask);
                        break;
                    case WMSToWCSTaskType.PutFinish://此处应该传给ssj消息
                        wMSTOWCSRspMsg.Message = "收到放货完成信息";
                        wMSTOWCSRspMsg.Data = null;
                        wMSTOWCSRspMsg.Code = "0";
                        SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == WCSTaskManager.Instance.GetSSJID(wMSTOWCSReqMsg.Port));
                        SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x=>x.Position== wMSTOWCSReqMsg.Port);
                        sSJDevice.OutTaskLimitDataItem(sSJDeviceBlock.AGVPutMBAddr, (byte)sSJDeviceBlock.AGVPutMBAddr2, 0);
                        break;
                    case WMSToWCSTaskType.ApplyGet:
                        wMSTOWCSRspMsg.Message = "收到申请取货信息";
                        wMSTOWCSRspMsg.Data = null;
                        wMSTOWCSRspMsg.Code = "0";
                        WMSTask wMSApplyGetTask = new()
                        {
                            PalletNum = wMSTOWCSReqMsg.Palno,
                            Warehouse = wMSTOWCSReqMsg.Location,
                            TaskType = WMSTaskType.AGVApplyGet,
                            TaskStatus = WMSTaskStatus.AGVApplyGet,
                            TaskSource = "WMS",
                            FmLocation = wMSTOWCSReqMsg.Port
                        };

                        WMSTasksManager.Instance.AddWMSTask(wMSApplyGetTask);
                        break;
                    case WMSToWCSTaskType.GetFinish://此处应该传给ssj消息
                        wMSTOWCSRspMsg.Message = "收到取货完成信息";
                        wMSTOWCSRspMsg.Data = null;
                        wMSTOWCSRspMsg.Code = "0";
                        break;
                }

            }
            return wMSTOWCSRspMsg;
        }

        public void InserIntoWMSJsonMsgLog(string msgDesc, int direction, string req_msg, string rsp_msg)
        {
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@MsgDesc",SqlNull(msgDesc)),
                    new SqlParameter("@MsgDirection",direction),
                    new SqlParameter("@ReqMsg",req_msg),
                    new SqlParameter("@RspMsg",rsp_msg),
                };
                SQLServerHelper.ExeSQLStringWithParam("INSERT INTO [dbo].[WCS2WMSJsonLog] " +
                    "([MsgDesc],[MsgDirection],[ReqMsg],[RspMsg])VALUES" +
                    "(@MsgDesc, @MsgDirection , @ReqMsg ,@RspMsg)", sqlParameters);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("InserIntoWMSJsonMsgLog ", ex);
            }
        }
        public static object SqlNull(object obj)
        {
            if (obj == null)
                return DBNull.Value;
            return obj;
        }
        public string ConverWMSLocToWCSLoc(string wmsLoc)     //B501010101   2位仓库编码+2位区域编码+货架号（1~2位）+2位列数编码+2位层数编码
        {//G0100201
            string wcsLoc;
            if (wmsLoc.Length != 8) return "00000000";
            wcsLoc = wmsLoc.Substring(1, 2) + "0" + wmsLoc.Substring(3, 3) + wmsLoc.Substring(6, 2);
            return wcsLoc;
        }
    }
}
