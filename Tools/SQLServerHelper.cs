using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Generic;

namespace BMHRI.WCS.Server.Tools
{
    public class SQLServerHelper
    {
        private static string _connectionStr;
        public static string ConnectionStr
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionStr))
                {
                    Configuration appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    _connectionStr = appConfig.AppSettings.Settings["SqlServerConnectionString"].Value;
                }
                return _connectionStr;
            }
        }//连接字符串
        private static bool IsOperationEx = false;
        public static bool isConnected = true;                 //是否连接
        public static bool IsConnected
        {
            get
            {
                return isConnected;
            }
            set
            {
                if (isConnected != value)
                {
                    isConnected = value;
                    RaiseWCSDBConnectionCharnged(value);
                }
            }
        }

        /// <summary>
        /// 构造 连接字符串
        /// </summary>
        public SQLServerHelper()
        {

        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <returns></returns>
        public static void ConnectTest()
        {
            if (IsConnected && !IsOperationEx) return;
            SqlConnection SqlConn = new SqlConnection(ConnectionStr);
            try
            {
                if (SqlConn.State != ConnectionState.Open)
                {
                    SqlConn.Open();
                }

            }
            catch (Exception)
            {
                //ConnectTestRe = false;
            }
            finally
            {
                if (SqlConn.State == ConnectionState.Open)
                {
                    IsConnected = true;
                }
                else
                {
                    IsConnected = false;
                }
                SqlConn.Dispose();
                IsOperationEx = false;
            }
        }

        /// <summary>
        /// 读取到表格
        /// </summary>
        /// <param name="DBReadStr"></param>
        /// <returns></returns>
        public static DataTable? DataBaseReadToTable(string DBReadStr)
        {
            if (!IsConnected)
            {
                return null;
            }

            DataTable? dt = new DataTable();
            SqlConnection SqlConn = new SqlConnection(ConnectionStr);
            SqlDataAdapter Sqlda1 = new SqlDataAdapter(DBReadStr, SqlConn);
            try
            {

                if (SqlConn.State != ConnectionState.Open)
                {
                    SqlConn.Open();
                }
                Sqlda1.SelectCommand.CommandTimeout = 180;
                Sqlda1.Fill(dt);
            }
            catch (Exception ex)
            {
                dt = null;
                //MessageBox.Show("数据库表读取失败！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                IsOperationEx = true;
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame? sf = st.GetFrame(0);
                //WriteErrorLog(ex.Message.ToString() + " " + sf.GetFileName() + " " + sf.GetMethod().Name + " " + ex.StackTrace.ToString());
                //LogHelper.WriteLog(ex.Message.ToString() + " " + sf.GetFileName() + " " + sf.GetMethod().Name + " " + ex.StackTrace.ToString(), ex);
                throw new Exception(ex.Message.ToString() + " " + sf.GetFileName() + " " + sf.GetMethod().Name + " " + ex.StackTrace.ToString());
            }
            finally
            {
                Sqlda1.Dispose();
                SqlConn.Dispose();
            }

            return dt;
        }
        public static object DataBaseReadToObject(string query, List<SqlParameter> parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // 添加参数（如果有）
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }

                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }
        /// <summary>
        /// 读取到表格
        /// </summary>
        /// <param name="DBReadStr"></param>
        /// <returns></returns>
        public static object? DataBaseReadToObject(string DBReadStr)
        {
            object? obj = null;
            try
            {
                DataTable? dataTable = DataBaseReadToTable(DBReadStr);
                if (dataTable.Rows.Count > 0)
                {
                    obj = dataTable.Rows[0][0];
                }
            }
            catch { return null; }
            return obj;
        }


        /// <summary>
        /// 读取到数据
        /// </summary>
        /// <param name="DBReadStr"></param>
        /// <returns></returns>
        public static DataSet? DataBaseReadToDataSet(string DBReadStr)
        {
            if (!IsConnected)
            {
                return null;
            }
            DataSet ds = new DataSet();
            SqlConnection SqlConn = new SqlConnection(ConnectionStr);
            SqlDataAdapter Sqlda1 = new SqlDataAdapter(DBReadStr, SqlConn);
            //bool ConnectTestRe = true;
            try
            {
                //SqlDataAdapter Sqlda1 = new SqlDataAdapter(DBReadStr, ConnectionStr);
                if (SqlConn.State != ConnectionState.Open)
                {
                    SqlConn.Open();
                }

                Sqlda1.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
                IsOperationEx = true;

                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                //WriteErrorLog(ex.Message.ToString() + " " + sf.GetFileName() + " " + sf.GetMethod().Name + " " + ex.StackTrace.ToString());
                //LogHelper.WriteLog(ex.Message.ToString() + " " + sf.GetFileName() + " " + sf.GetMethod().Name + " " + ex.StackTrace.ToString(), ex);
                throw new Exception(ex.Message.ToString() + " " + sf.GetFileName() + " " + sf.GetMethod().Name + " " + ex.StackTrace.ToString());
            }
            finally
            {

                Sqlda1.Dispose();
                SqlConn.Dispose();
            }
            return ds;
        }

        /// <summary>
        /// 数据库 增、插、删、改操作
        /// </summary>
        /// <param name="ExecuteStr"></param>
        /// <returns></returns>
        public static bool DataBaseExecute(string ExecuteStr)
        {
            if (!IsConnected)
            {
                return false;
            }
            bool isExecuted = false;
            //bool ConnectTestRe = true;
            SqlConnection SqlConn = new SqlConnection(ConnectionStr);
            SqlCommand SqlCmd = new SqlCommand(ExecuteStr, SqlConn)
            {
                CommandTimeout = 180
            };
            try
            {

                if (SqlConn.State != ConnectionState.Open)
                {
                    SqlConn.Open();
                }

                int i = SqlCmd.ExecuteNonQuery();
                isExecuted = true;
            }
            catch (Exception ex)
            {
                isExecuted = false;
                IsOperationEx = true;

                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame? sf = st.GetFrame(0);
                //WriteErrorLog(ExecuteStr + "|" + ex.Message.ToString() + " " + sf.GetFileName() + " " + sf.GetMethod().Name + " " + ex.StackTrace.ToString());
                //MessageBox.Show("数据库操作失败！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //LogHelper.WriteLog(ex.Message.ToString() + " " + sf.GetFileName() + " " + sf.GetMethod().Name + " " + ex.StackTrace.ToString(), ex);
                throw new Exception(ex.Message.ToString() + " " + sf.GetFileName() + " " + sf.GetMethod().Name + " " + ex.StackTrace.ToString());
            }
            finally
            {
                SqlCmd.Dispose();
                SqlConn.Dispose();
            }
            return isExecuted;
        }

        /// <summary>
        /// 执行存储过程，返回Output输出参数值        
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>object</returns>
        public static object ExeProcedure(string storedProcName, IDataParameter[] paramenters)
        {
            if (!IsConnected) return null;
            object obj = null;
            SqlConnection SqlConn = new SqlConnection(ConnectionStr);
            SqlCommand command = new SqlCommand(storedProcName, SqlConn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 180
            };
            try
            {
                if (SqlConn.State != ConnectionState.Open)
                {
                    SqlConn.Open();
                }
                command.Parameters.AddRange(paramenters);
                command.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
                command.ExecuteNonQuery();
                obj = command.Parameters["@ReturnValue"].Value; //@Output_Value和具体的存储过程参数对应
                if (Equals(obj, null) || (Equals(obj, DBNull.Value)))
                {
                    return null;
                }
                else
                {
                    return obj;
                }

            }
            catch (Exception ex)
            {
                IsOperationEx = true;
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                //WriteErrorLog(e.Message.ToString() + " " + sf.GetFileName() + " " + sf.GetMethod().Name + " " + e.StackTrace.ToString());
                //LogHelper.WriteLog(ex.Message.ToString() + " " + sf.GetFileName() + " " + sf.GetMethod().Name + " " + ex.StackTrace.ToString(), ex);
                throw new Exception(ex.Message.ToString() + " " + sf.GetFileName() + " " + sf.GetMethod().Name + " " + ex.StackTrace.ToString());
                //return null;
            }
            finally
            {
                SqlConn.Dispose();
                command.Dispose();
            }
        }

        /// <summary>
        /// 执行操作，带参数，返回影响行数        
        /// </summary>
        /// <param name="sqlstr">操作语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>object</returns>
        public static object ExeSQLStringWithParam(string sqlstr, IDataParameter[] paramenters)
        {
            if (!IsConnected) return null;
            object obj = null;
            SqlConnection SqlConn = new SqlConnection(ConnectionStr);
            SqlCommand command = new SqlCommand(sqlstr, SqlConn)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 180
            };
            try
            {
                if (SqlConn.State != ConnectionState.Open)
                {
                    SqlConn.Open();
                }
                command.Parameters.AddRange(paramenters);
                //command.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
                obj = command.ExecuteNonQuery();
                //obj = command.Parameters["@ReturnValue"].Value; //@Output_Value和具体的存储过程参数对应
                if (Equals(obj, null) || (Equals(obj, DBNull.Value)))
                {
                    return null;
                }
                else
                {
                    return obj;
                }

            }
            catch (Exception ex)
            {
                IsOperationEx = true;
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                //WriteErrorLog(e.Message.ToString() + " " + sf.GetFileName() + " " + sf.GetMethod().Name + " " + e.StackTrace.ToString());
                //LogHelper.WriteLog(ex.Message.ToString() + " " + sf.GetFileName() + " " + sf.GetMethod().Name + " " + ex.StackTrace.ToString(), ex);
                throw new Exception(ex.Message.ToString() + " " + sf.GetFileName() + " " + sf.GetMethod().Name + " " + ex.StackTrace.ToString());
                //return null;
            }
            finally
            {
                SqlConn.Dispose();
                command.Dispose();
            }
        }

        //public static void WriteErrorLog(string errorString)
        //{

        //}

        //public static void WriteSqlLog(string sqlString)
        //{

        //}



        #region reconnect
        public static void CreateTestTimer()
        {

        }

        #endregion

        /// <summary>
        /// 触发连接状态事件
        /// </summary>
        /// <param name="status"></param>
        private static void RaiseWCSDBConnectionCharnged(bool isConnected)
        {
            if (WCSDBConnectionCharnged is null) return;
            WCSDBConnectionCharnged(isConnected);
        }

        public static event Action<bool> WCSDBConnectionCharnged;

        public static bool BulkToDB(DataTable dt, string tablName)
        {
            SqlConnection sqlConn = new SqlConnection(ConnectionStr);
            SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConn)
            {
                DestinationTableName = tablName,
                BatchSize = dt.Rows.Count
            };

            try
            {
                sqlConn.Open();
                if (dt != null && dt.Rows.Count != 0)
                {
                    bulkCopy.WriteToServer(dt);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                IsOperationEx = true;
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                //WriteErrorLog(e.Message.ToString() + " " + sf.GetFileName() + " " + sf.GetMethod().Name + " " + e.StackTrace.ToString());
                //LogHelper.WriteLog(ex.Message.ToString() + " " + sf.GetFileName() + " " + sf.GetMethod().Name + " " + ex.StackTrace.ToString(), ex);
                throw new Exception(ex.Message.ToString() + " " + sf.GetFileName() + " " + sf.GetMethod().Name + " " + ex.StackTrace.ToString());
                //return false;
            }
            finally
            {
                sqlConn.Close();
                if (bulkCopy != null)
                    bulkCopy.Close();
            }
        }
    }
}
