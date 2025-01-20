using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.Tools;
using BMHRI.WCS.Server.WebApi.Protocols;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SqlClient;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace BMHRI.WCS.Server.WebApi.Controllers
{
    [ApiController]
    [Route("WMSToWCS/[controller]")]
    public class AGVMoveTaskController : ControllerBase
    {
        [HttpPost(Name = "AGVMoveTask")]
        [Produces("application/json")]
        //public AGVMoveRspMsg AGVMoveRequestProcess(dynamic agvMoveReqMsgStr)
        //{
        //    AGVMoveReqMsg aGVMoveReqMsg = JsonSerializer.Deserialize<AGVMoveReqMsg>(agvMoveReqMsgStr);
        //    AGVMoveRspMsg aGVMoveRspMsg  = AgvMoveRequestProcess(aGVMoveReqMsg);
        //    JsonSerializerOptions options = new JsonSerializerOptions();
        //    options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All); //解决中文序列化被编码的问题
        //    options.Converters.Add(new DateTimeConverter()); //解决时间格式序列化的问题
        //    string reqstr = JsonSerializer.Serialize(aGVMoveReqMsg, options);
        //    string rspstr = JsonSerializer.Serialize(aGVMoveRspMsg, options);
        //    //InserIntoWMSJsonMsgLog(1, reqstr, rspstr);
        //    Task.Factory.StartNew(() => { InserIntoWMSJsonMsgLog("taskCancelRequest", 1, reqstr, rspstr); });
        //    return aGVMoveRspMsg;
        //    //return JsonSerializer.Serialize(wMSTaskReqMsg);
        //}

        //public AGVMoveRspMsg AgvMoveRequestProcess(AGVMoveReqMsg aGVMoveReqMsg)
        //{
        //    AGVMoveRspMsg aGVMoveRspMsg = new AGVMoveRspMsg();
        //    if (string.IsNullOrEmpty(aGVMoveReqMsg.taskId))
        //    {
        //        aGVMoveRspMsg.result = false;
        //        aGVMoveRspMsg.code = 0;
        //        aGVMoveRspMsg.msg = "taskId不能为空！";
        //        aGVMoveRspMsg.data = null;
        //        return aGVMoveRspMsg;
        //    }
        //    WMSTask wMSTask = WMSTasksManager.Instance.WMSTaskList.Find(x => x.taskId == aGVMoveReqMsg.taskId);
        //    if (wMSTask == null)
        //    {
        //        WMSTask aGVTask = new()
        //        {
        //            taskId = aGVMoveReqMsg.taskId,
        //            PalletNum = aGVMoveReqMsg.palletCode,
        //            FmLocation = aGVMoveReqMsg.fmLocation,
        //            ToLocation = aGVMoveReqMsg.toLocation,
        //            TaskType = WMSTaskType.AGVMove,
        //            TaskStatus = WMSTaskStatus.TaskAssigned,
        //            TaskSource = "WMS",
        //            WMSLocation = aGVMoveReqMsg.fmLocation,
        //            WMSLocation2 = aGVMoveReqMsg.toLocation,
        //        };
        //        WMSTasksManager.Instance.AddWMSTask(aGVTask);
        //    }
        //    else
        //    {
        //        aGVMoveRspMsg.result = false;
        //        aGVMoveRspMsg.code = 0;
        //        aGVMoveRspMsg.msg = "重复推送AGV任务！";
        //        aGVMoveRspMsg.data = null;
        //    }
        //    return aGVMoveRspMsg;
        //}


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
