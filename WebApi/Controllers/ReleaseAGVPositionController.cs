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
    public class ReleaseAGVPositionController : ControllerBase
    {
        [HttpPost(Name = "ReleaseAGVPosition")]
        [Produces("application/json")]
        //public ReleaseAGVPositionRspMsg ReleaseAGVPositionRequestProcess(dynamic releaseAGVPositionReqMsgStr)
        //{
        //    ReleaseAGVPositionReqMsg releaseAGVPositionReqMsg = JsonSerializer.Deserialize<ReleaseAGVPositionReqMsg>(releaseAGVPositionReqMsgStr);
        //    ReleaseAGVPositionRspMsg releaseAGVPositionRsp = AgvMoveRequestProcess(releaseAGVPositionReqMsg);
        //    JsonSerializerOptions options = new JsonSerializerOptions();
        //    options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All); //解决中文序列化被编码的问题
        //    options.Converters.Add(new DateTimeConverter()); //解决时间格式序列化的问题
        //    string reqstr = JsonSerializer.Serialize(releaseAGVPositionReqMsg, options);
        //    string rspstr = JsonSerializer.Serialize(releaseAGVPositionRsp, options);
        //    //InserIntoWMSJsonMsgLog(1, reqstr, rspstr);
        //    Task.Factory.StartNew(() => { InserIntoWMSJsonMsgLog("taskCancelRequest", 1, reqstr, rspstr); });
        //    return releaseAGVPositionRsp;
        //    //return JsonSerializer.Serialize(wMSTaskReqMsg);
        //}

        //public ReleaseAGVPositionRspMsg AgvMoveRequestProcess(ReleaseAGVPositionReqMsg releaseAGVPositionReq)
        //{
        //    ReleaseAGVPositionRspMsg releaseAGVPositionRsp = new ReleaseAGVPositionRspMsg();
        //    AgvPosition agvPosition = AgvManager.Instance.AgvPositionList.Find(x => x.Position == releaseAGVPositionReq.location);
        //    if(agvPosition == null )
        //    {
        //        releaseAGVPositionRsp.result = false;
        //        releaseAGVPositionRsp.code = 0;
        //        releaseAGVPositionRsp.msg = "AGV点位不存在！";
        //    }
        //    WMSTask aGVTask = new()
        //    {
        //        taskId = releaseAGVPositionReq.taskId,
        //        PalletNum = releaseAGVPositionReq.palletCode,
        //        FmLocation = releaseAGVPositionReq.location,
        //        TaskType = WMSTaskType.AGVPositionRelease,
        //        TaskStatus = WMSTaskStatus.AGVPositionChg,
        //        TaskSource = "WMS",
        //        WMSLocation = releaseAGVPositionReq.location,
        //    };
        //    WMSTasksManager.Instance.AddWMSTask(aGVTask);
        //    return releaseAGVPositionRsp;
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
