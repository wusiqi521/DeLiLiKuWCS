using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.Tools;
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
    public class OutboundAndMoveTaskController : ControllerBase
    {
        [HttpPost(Name = "OutboundAndMoveTask")]
        [Produces("application/json")]
        //public OutboundPushRspMsg OutboundPushReqMsgProcess(dynamic outboundPushReqMsgStr)
        //{
        //    OutboundPushReqMsg outboundPushReqMsg = JsonSerializer.Deserialize<OutboundPushReqMsg>(outboundPushReqMsgStr);
        //    OutboundPushRspMsg outboundPushRspMsg = OutBoundRequestProcess(outboundPushReqMsg);
        //    JsonSerializerOptions options = new JsonSerializerOptions();
        //    options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All); //解决中文序列化被编码的问题
        //    options.Converters.Add(new DateTimeConverter()); //解决时间格式序列化的问题
        //    string reqstr = JsonSerializer.Serialize(outboundPushReqMsg, options);
        //    string rspstr = JsonSerializer.Serialize(outboundPushRspMsg, options);
        //    //InserIntoWMSJsonMsgLog(1, reqstr, rspstr);
        //    Task.Factory.StartNew(() => { InserIntoWMSJsonMsgLog("OutboundPushRsp", 1, reqstr, rspstr); });
        //    return outboundPushRspMsg;
        //    //return JsonSerializer.Serialize(wMSTaskReqMsg);
        //}

        //public OutboundPushRspMsg OutBoundRequestProcess(OutboundPushReqMsg outboundPushReqMsg)
        //{
        //    OutboundPushRspMsg outboundPushRspMsg = new OutboundPushRspMsg();
        //    if (outboundPushReqMsg != null)
        //    {
        //        if (string.IsNullOrEmpty(outboundPushReqMsg.taskId)||string.IsNullOrEmpty(outboundPushReqMsg.taskType)||string.IsNullOrEmpty(outboundPushReqMsg.palletCode) || string.IsNullOrEmpty(outboundPushReqMsg.fmLocation) || string.IsNullOrEmpty(outboundPushReqMsg.toLocation))
        //        {
        //            outboundPushRspMsg.result = false;
        //            outboundPushRspMsg.code = 0;
        //            outboundPushRspMsg.msg = "部分参数为空!";
        //            outboundPushRspMsg.data = null;
        //            return outboundPushRspMsg;
        //        }
        //        if (outboundPushReqMsg.fmLocation.Length != 10)   //B501010101   2位仓库编码+2位区域编码+货架号（1~2位）+2位层数编码+2位列数编码
        //        {
        //            outboundPushRspMsg.result = false;
        //            outboundPushRspMsg.code = 0;
        //            outboundPushRspMsg.msg = "出库起始地格式错误!";
        //            outboundPushRspMsg.data = null;
        //            return outboundPushRspMsg;
        //        }
        //        DDJDevice dDJDevice = PLCDeviceManager.Instance.DDJDeviceList.Find(x => x.PLCID == GetDDJIDFromLocation(outboundPushReqMsg.fmLocation));
        //        if (dDJDevice == null)
        //        {
        //            outboundPushRspMsg.result = false;
        //            outboundPushRspMsg.code = 0;
        //            outboundPushRspMsg.msg = "出库起始地格式错误!";
        //            outboundPushRspMsg.data = null;
        //            return outboundPushRspMsg;
        //        }
        //        if (!dDJDevice.Available)
        //        {
        //            outboundPushRspMsg.result = false;
        //            outboundPushRspMsg.code = 0;
        //            outboundPushRspMsg.msg = "堆垛机不可用!";
        //            outboundPushRspMsg.data = null;
        //            return outboundPushRspMsg;
        //        }
        //        WMSTask wMSOutTask = WMSTasksManager.Instance.WMSTaskList.Find(x => x.taskId == outboundPushReqMsg.taskId && x.PalletNum == outboundPushReqMsg.palletCode);
        //        if (wMSOutTask == null)
        //        {
        //            if (outboundPushReqMsg.taskType == "O")
        //            {
        //                WMSTask wMSTask = new()
        //                {
        //                    taskId = outboundPushReqMsg.taskId,
        //                    PalletNum = outboundPushReqMsg.palletCode,
        //                    FmLocation = WMSWebApiClient.Instance.ConverWMSLocToWCSLoc(outboundPushReqMsg.fmLocation),
        //                    ToLocation = outboundPushReqMsg.toLocation,
        //                    TaskType = WMSTaskType.Outbound,
        //                    Priority = Convert.ToInt32(outboundPushReqMsg.priority),
        //                    TaskStatus = WMSTaskStatus.TaskAssigned,
        //                    TaskSource = "WMS",
        //                    WMSLocation = outboundPushReqMsg.fmLocation,
        //                    LedMessage = outboundPushReqMsg.comment
        //                };
        //                if (outboundPushReqMsg.toLocation[..2] == "10")
        //                    wMSTask.Floor = 1;
        //                else if (outboundPushReqMsg.toLocation[..2] == "11")
        //                    wMSTask.Floor = 2;
        //                else if (outboundPushReqMsg.toLocation[..2] == "20")
        //                    wMSTask.Floor = 3;
        //                else
        //                    wMSTask.Floor = 4;
        //                WMSTasksManager.Instance.AddWMSTask(wMSTask);
        //            }
        //            else
        //            {
        //                WMSTask wMSTask = new WMSTask()
        //                {
        //                    taskId = outboundPushReqMsg.taskId,
        //                    PalletNum = outboundPushReqMsg.palletCode,
        //                    FmLocation = WMSWebApiClient.Instance.ConverWMSLocToWCSLoc(outboundPushReqMsg.fmLocation),
        //                    ToLocation = WMSWebApiClient.Instance.ConverWMSLocToWCSLoc(outboundPushReqMsg.toLocation),
        //                    TaskType = WMSTaskType.Moving,
        //                    Priority = Convert.ToInt32(outboundPushReqMsg.priority),
        //                    TaskStatus = WMSTaskStatus.TaskAssigned,
        //                    TaskSource = "WMS",
        //                    WMSLocation = outboundPushReqMsg.fmLocation,
        //                    WMSLocation2=outboundPushReqMsg.toLocation,
        //                    LedMessage = ""
        //                };
        //                WMSTasksManager.Instance.AddWMSTask(wMSTask);
        //            }
        //        }
        //        else
        //        {
        //            outboundPushRspMsg.result = false;
        //            outboundPushRspMsg.code = 0;
        //            outboundPushRspMsg.msg = "重复推送相同任务!";
        //            outboundPushRspMsg.data = null;
        //            return outboundPushRspMsg;
        //        }
        //    }
        //    return outboundPushRspMsg;
        //}
        public string GetDDJIDFromLocation(string location)
        {
            GoodsLocation fm_goodlocation = GoodsLocationManager.GoodsLocationList.Find(x => x.WMSPosition == location);
            if (fm_goodlocation == null) return "0000";
            return fm_goodlocation.DDJID;
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
    }
}
