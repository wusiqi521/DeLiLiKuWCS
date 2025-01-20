using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.Tools;
using BMHRI.WCS.Server.UserControls;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BMHRI.WCS.Server.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UpdateAgvTaskStatusController : ControllerBase
    {
        [HttpPost(Name = "UpdateAgvTaskStatus")]
        [Produces("application/json")]
        public HKAgvToWCSRspMessage UpdateAgvTaskStatus(dynamic hKAgvReqMessage)
        {
            HKAgvToWCSRspMessage hKAgvRspMessage = new HKAgvToWCSRspMessage();
            try
            {
                JsonNode jsonNode = JsonNode.Parse(hKAgvReqMessage.ToString())!;

                if (jsonNode != null)
                {
                    string robotCode = jsonNode["robotCode"].ToString();
                    string taskCode = jsonNode["taskCode"].ToString();
                    string method = jsonNode["method"].ToString();
                    string reqCode = jsonNode["reqCode"].ToString();
                    string currentPosition = jsonNode["currentPositionCode"].ToString();
                    switch (method)
                    {
                        case "end":
                            WCSTask wCSTasken = WCSTaskManager.Instance.WCSTaskList.Find(x => x.WCSSeqID == taskCode);
                            if (wCSTasken != null)
                            {
                                AgvManager.Instance.UpdateAGVPositionPalletNoStatus(wCSTasken.ToLocation, wCSTasken.PalletNum, AgvPositionStatus.InPlace);
                                WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTasken, WCSTaskStatus.Done);
                                AgvPosition agvPosition = AgvManager.Instance.AgvPositionList.Find(x => x.Position == wCSTasken.ToLocation);
                                if (agvPosition != null && agvPosition.PositionType != AgvPositionSystemType.None && agvPosition.PositionType != AgvPositionSystemType.GoodLocation)
                                {
                                    SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == agvPosition.SSJID);
                                    sSJDevice.PutDone(wCSTasken.PalletNum, agvPosition.SSJPositon);
                                }
                            }
                            break;
                        //case "finish":
                        //    WCSTask wCSTaskfi = WCSTaskManager.Instance.WCSTaskList.Find(x => x.WCSSeqID == taskCode);
                        //    if (wCSTaskfi != null)
                        //        AgvManager.Instance.UpdateAGVPositionPalletNoStatus(wCSTaskfi.FmLocation, null, AgvPositionStatus.None);
                        //    break;
                        case "outbin":
                            WCSTask wCSTaskou = WCSTaskManager.Instance.WCSTaskList.Find(x => x.WCSSeqID == taskCode);
                            if (wCSTaskou != null)
                            {
                                AgvPosition agvPosition = AgvManager.Instance.AgvPositionList.Find(x => x.Position == wCSTaskou.FmLocation);
                                if (agvPosition != null)
                                {
                                    AgvManager.Instance.UpdateAGVPositionPalletNoStatus(agvPosition.Position, null, AgvPositionStatus.None);
                                    if(agvPosition.PositionType != AgvPositionSystemType.None && agvPosition.PositionType != AgvPositionSystemType.GoodLocation)
                                    {
                                        SSJDevice sSJDevice = PLCDeviceManager.Instance.FindSSJDevice(agvPosition.SSJPositon);
                                        if (sSJDevice != null)
                                            sSJDevice?.GetDone(wCSTaskou.PalletNum,agvPosition.SSJPositon);
                                    }
                                }
                                WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTaskou, WCSTaskStatus.AGVPickUped);
                            }
                            break;
                        //case "apply_pick":
                        //    WCSTask wCSTaskapick = WCSTaskManager.Instance.WCSTaskList.Find(x => x.WCSSeqID == taskCode);
                        //    if (wCSTaskapick != null)
                        //    {
                        //        WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTaskapick, WCSTaskStatus.AGVApplyPick);
                        //    }
                        //    break;
                        //case "apply_put":
                        //    WCSTask wCSTaskaput = WCSTaskManager.Instance.WCSTaskList.Find(x => x.WCSSeqID == taskCode);
                        //    if (wCSTaskaput != null)
                        //    {
                        //        WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTaskaput, WCSTaskStatus.AGVApplyPut);
                        //    }
                        //    break;
                        case "apply":
                            WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.WCSSeqID == taskCode && (x.FmLocation == currentPosition || x.ToLocation == currentPosition));
                            if(wCSTask != null)
                            {
                                
                                if (wCSTask.FmLocation == currentPosition)
                                    WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.AGVApplyPick);
                                else
                                {
                                    WCSTaskManager.Instance.UpdateWCSTaskStatus(wCSTask, WCSTaskStatus.AGVApplyPut);

                                    AgvPosition agvPositionTo = AgvManager.Instance.AgvPositionList.Find(x => x.Position == wCSTask.ToLocation);
                                    if (agvPositionTo != null&&(agvPositionTo.PositionType== AgvPositionSystemType.SSJDeviceBlockOut|| agvPositionTo.PositionType == AgvPositionSystemType.SSJDeviceBlockIn))
                                    {
                                        SSJDevice sSJDevice = PLCDeviceManager.Instance.SSJDeviceList.Find(x => x.PLCID == agvPositionTo.SSJID);
                                        if (sSJDevice == null || sSJDevice.SSJWorkState == SSJDeviceWorkState.None) break;
                                        SSJDeviceBlock sSJDeviceBlock = sSJDevice.DeviceBlockList.Find(x => x.Position == agvPositionTo.SSJPositon);
                                        if (sSJDeviceBlock == null) break;
                                        sSJDevice.ApplyPut(wCSTask.PalletNum,agvPositionTo.SSJPositon);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    hKAgvRspMessage.reqCode = reqCode;
                }
            }
            catch (Exception ex)
            {
                hKAgvRspMessage.code = "1";
                hKAgvRspMessage.message = ex.Message;
                LogHelper.WriteLog("UpdateAgvTaskStatusController", ex);
            }
            string rspStr = JsonHelp.Serialize(hKAgvRspMessage);
            //InsertIntoWCS2HKLog(string msgDesc, string reqMsg, string rspMsg, MsgDirection msgdir)
            Task.Factory.StartNew(() => { AgvManager.Instance.InsertIntoWCS2HKLog("UpdateAgvTaskStatus", hKAgvReqMessage.ToString(), rspStr, AGVMsgDirection.Input); });
            return hKAgvRspMessage;
        }

    }
    public class HKAgvToWCSReqMessage
    {
        //{"action":"","areaCode":"","berthCode":"","callCode":"LK_2L_CK_01","callTyp":"",
        //"clientCode":"","cooX":48500.0,"cooY":24054.0,"ctnrCode":"","ctnrTyp":"",
        //"currentCallCode":"LK_2L_QY_01","currentPositionCode":"LK_2L_QY_01",
        //"data":null,"eqpCode":"","indBind":"","mapCode":"BB","mapDataCode":"048500BB024054",
        //"mapShortName":"","materialLot":"","method":"end","orgCode":"","podCode":"",
        //"podDir":"","podNum":"","podTyp":"","reqCode":"18B1CB504581JW3","reqTime":"2023-10-11 11:08:27",
        //"roadWayCode":"","robotCode":"3125","seq":"","stgBinCode":"","subTaskNum":"",
        //"taskCode":"eb9ba5f8","taskTyp":"","tokenCode":"","username":"","wbCode":"LK_2L_CK_01"}"
        public string reqCode { get; set; }//必填 请求编号，每个请求都要一个唯一编号， 同一个请求重复提交， 使用同一编号。由上层系统设定。
        public string reqTime { get; set; } //选填 请求时间截 格式: “yyyy-MM-dd HH:mm:ss”。由上层系统设定。
        public string clientCode { get; set; } //选填 客户端编号，如 PDA， HCWMS 等。如果填写，需先在RCS-2000 系统配置，上层系统调用时进行填写，当多系统调用时，调度系统可以进行调用方区分。
        public string taskCode { get; set; } //令牌号, 由调度系统颁发。如果填写，需先在 RCS-2000 系统配置，上层系统调用时进行填写。
        public string taskStatus { get; set; }
    }

    public class HKAgvToWCSRspMessage
    {
        //{ "code": "0",  "message": "成功",  "reqCode": "1541954B96B1112"}
        public string reqCode { get; set; } //必填 请求编号返回，形成一一对应
        public string code { get; set; }// 必填 返回编号， “0”：成功， 1~N：失败
        public string message { get; set; }//必填 “0”：成功 1~N：其他的详细描述
        public HKAgvToWCSRspMessage()
        {
            code = "0";
            message = "成功";
            reqCode = DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
    public enum AGVMsgDirection
    {
        Output,
        Input,
    }
}
