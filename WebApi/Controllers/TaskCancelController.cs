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
    public class TaskCancelController : ControllerBase
    {
        [HttpPost(Name = "CancelTask")]
        [Produces("application/json")]
        //public CancelTaskRspMsg TaskCancelRequestProcess(dynamic taskCancelReqMsgStr)
        //{
        //    CancelTaskReqMsg taskCancelReqMsg = JsonSerializer.Deserialize<CancelTaskReqMsg>(taskCancelReqMsgStr);
        //    CancelTaskRspMsg cancelTaskRspMsg = TaskCancelRequestProcess(taskCancelReqMsg);
        //    JsonSerializerOptions options = new JsonSerializerOptions();
        //    options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All); //解决中文序列化被编码的问题
        //    options.Converters.Add(new DateTimeConverter()); //解决时间格式序列化的问题
        //    string reqstr = JsonSerializer.Serialize(taskCancelReqMsg, options);
        //    string rspstr = JsonSerializer.Serialize(cancelTaskRspMsg, options);
        //    //InserIntoWMSJsonMsgLog(1, reqstr, rspstr);
        //    Task.Factory.StartNew(() => { InserIntoWMSJsonMsgLog("taskCancelRequest", 1, reqstr, rspstr); });
        //    return cancelTaskRspMsg;
        //    //return JsonSerializer.Serialize(wMSTaskReqMsg);
        //}

        //public CancelTaskRspMsg TaskCancelRequestProcess(CancelTaskReqMsg taskCancelReqMsg)
        //{
        //    CancelTaskRspMsg taskCancelRspMsg = new CancelTaskRspMsg();
        //    WMSTask wMSTask = WMSTasksManager.Instance.WMSTaskList.Find(x => x.taskId == taskCancelReqMsg.taskId);
        //    if (wMSTask == null)   //只可以取消出库任务和倒库任务
        //    {
        //        taskCancelRspMsg.msg = "WCS任务已取消，任务取消失败";
        //        taskCancelRspMsg.data = null;
        //        taskCancelRspMsg.code = 0;
        //        taskCancelRspMsg.result = true;
        //    }
        //    else
        //    {
        //        WCSTask wCSTask = WCSTaskManager.Instance.WCSTaskList.Find(x => x.WMSSeqID == wMSTask.WMSSeqID && (x.TaskType == WCSTaskTypes.DDJUnstack || x.TaskType == WCSTaskTypes.DDJStackMove));
        //        if (wCSTask == null)
        //        {
        //            taskCancelRspMsg.msg = "入库任务无法取消，取消失败";
        //            taskCancelRspMsg.data = null;
        //            taskCancelRspMsg.code = 0;
        //            taskCancelRspMsg.result = true;
        //        }
        //        else
        //        {
        //            if (wCSTask.TaskStatus == WCSTaskStatus.Waiting)   //任务取消成功!
        //            {
        //                WMSTasksManager.Instance.DeleteWMSAndWCSTask(wMSTask);
        //            }
        //            else
        //            {
        //                taskCancelRspMsg.msg = "任务已经执行，无法取消";
        //                taskCancelRspMsg.data = null;
        //                taskCancelRspMsg.code = 0;
        //                taskCancelRspMsg.result = true;
        //            }
        //        }
        //    }
        //    return taskCancelRspMsg;
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
