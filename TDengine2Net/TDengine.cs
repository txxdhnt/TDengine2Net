

using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;

namespace TDengine2Net
{
   public class TDengine:IDisposable
    {
        #region DLL
        [DllImport("taos.dll", EntryPoint = "taos_init", CallingConvention = CallingConvention.StdCall)]
        private static extern  void taos_init();
        [DllImport("taos.dll", EntryPoint = "taos_options", CallingConvention = CallingConvention.StdCall)]
        static extern private void taos_options(int option, string value);
        [DllImport("taos.dll", EntryPoint = "taos_connect", CallingConvention = CallingConvention.StdCall)]
        static extern private long taos_connect(string ip, string user, string password, string db, int port);
        [DllImport("taos.dll", EntryPoint = "taos_query", CallingConvention = CallingConvention.StdCall)]
        static extern private int taos_query(long taos, string sqlstr);

       
        [DllImport("taos.dll", EntryPoint = "taos_use_result", CallingConvention = CallingConvention.StdCall)]
        static extern private long taos_use_result(long taos);

        [DllImport("taos.dll", EntryPoint = "taos_field_count", CallingConvention = CallingConvention.StdCall)]
        static extern public int taos_field_count(long taos);

        [DllImport("taos.dll", EntryPoint = "taos_fetch_fields", CallingConvention = CallingConvention.StdCall)]
        static extern private IntPtr taos_fetch_fields(long res);
        [DllImport("taos.dll", EntryPoint = "taos_fetch_row", CallingConvention = CallingConvention.StdCall)]
        static extern public IntPtr taos_fetch_row(long res);

        [DllImport("taos.dll", EntryPoint = "taos_free_result", CallingConvention = CallingConvention.StdCall)]
        static extern public IntPtr taos_free_result(long res);

        [DllImport("taos.dll", EntryPoint = "taos_close", CallingConvention = CallingConvention.StdCall)]
        static extern public int taos_close(long taos);

        [DllImport("taos.dll", EntryPoint = "taos_get_client_info", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr taos_get_client_info();

        [DllImport("taos.dll", EntryPoint = "taos_get_server_info", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr taos_get_server_info(long taos);
        [DllImport("taos.dll", EntryPoint = "taos_errstr", CallingConvention = CallingConvention.StdCall)]
        static extern private IntPtr taos_errstr(long taos);
        [DllImport("taos.dll", EntryPoint = "taos_errno", CallingConvention = CallingConvention.StdCall)]
        static extern public int taos_errno(long taos);





        [DllImport("taos.dll", EntryPoint = "taos_affected_rows", CallingConvention = CallingConvention.StdCall)]
        static extern private int taos_affected_rows(long taos);
        /*
         * taos_close_stream
         * taos_consume
         * taos_fetch_block
         * taos_fetch_row_a
         * taos_fetch_rows_a
         * taos_fetch_subfields
         * taos_num_fields
         * taos_open_stream
         * taos_print_row
         * taos_query_a
         *taos_result_precision
         *taos_select_db
         *taos_stop_query
         *taos_subfields_count
         *taos_subscribe
         *taos_unsubscribe
        */
        #endregion

        private string  _host;
        private int     _port = 6030;       
        private string  _user = "root";
        private string  _password = "taosdata";       
        private string  _db="";

        private long        _taosConn=0;
        private TSDB_CODE   _taosErroNo= TSDB_CODE.SUCCESS;
        private string      _taosErrorReaon="";

        private string      _configDir="";
        
        /// <summary>
        /// 注意：１、当前目录存在taos.dll；２.taos.dll的版本和服务器上的taos版本一致。
        /// </summary>
        public TDengine()
        {
            try
            {
                //taos_options((int)TDengineInitOption.TDDB_OPTION_CONFIGDIR, "C:/TDengine/cfg");
                //taos_options((int)TDengineInitOption.TDDB_OPTION_SHELL_ACTIVITY_TIMER, "60");
                taos_init();
                Console.WriteLine("taos_init finished! "  + GetClientInfo());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 连接到TAOSDB服务器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="db"></param>
        public TSDB_CODE Connect(string host, int port, string user, string password,string db="")
        {
            _host = host;
            _user = user;
            _password = password;
            _port = port;
            _db = db;
            try
            {
                _taosConn = taos_connect(_host, _user, _password, _db,_port);
                if (_taosConn != 0)
                {
                    Console.WriteLine("Connect to TDengine success!" + GetServerInfo());
                    return TSDB_CODE.SUCCESS;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            Console.WriteLine("Connect to TDengine failed");
            return TSDB_CODE.INVALID_CONNECTION;

        }
        /// <summary>
        ///  创建数据库
        /// </summary>
        /// <param name="db">数据库名</param>
        /// <returns>TSDB_CODE</returns>
        public TSDB_CODE CreateDB(string db)
        {
            return  SQLCommand("create database if not exists " + db);
        }
        /// <summary>
        /// 切换当前数据库
        /// </summary>
        /// <param name="db">数据库名</param>
        /// <returns></returns>
        public TSDB_CODE UserDB(string db)
        {
            return SQLCommand("use " + db);
        }
        /// <summary>
        /// 在***当前数据库***中建新表
        /// </summary>
        /// <param name="tb">表名</param>
        /// <param name="fields">字段数组</param>
        /// <returns>TSDB_CODE</returns>
        public TSDB_CODE CreateTable(string tb,TSDB_FIELD[] fields)
        {
            if (_taosConn != 0 && _db!= "" && tb != ""&& fields.Length>1)
            {
                if(fields[0].type!= (byte)TSDB_DATA_TYPE.TIMESTAMP) return TSDB_CODE.OTHERS; 
                StringBuilder sql = new StringBuilder();
                sql.Append("create table if not exists ").Append(tb).Append("(");
                sql.Append(fields[0].name + " " + fields[0].TSDBDataTypeName());
                for (int i = 1; i < fields.Length; i++)
                {
                    sql.Append(","+fields[i].name + " " + fields[i].TSDBDataTypeName());
                }
                sql.Append(");");
                return SQLCommand(sql.ToString());
            }
            return TSDB_CODE.OTHERS;
        }

        /// <summary>
        /// SQl查询返回System.Data.DataTable
        /// </summary>
        /// <param name="sql">SQl查询语句</param>
        /// <returns></returns>
        public DataTable QueryToDataTable(string sql)
        {

            DataTable dt = new DataTable();
            if (_taosConn == 0) return new DataTable();
            try
            {
                if (SQLCommand(sql)==TSDB_CODE.SUCCESS)
                {
                    int fieldCount = TDengine.taos_field_count(_taosConn);

                    Console.WriteLine("field count: " + fieldCount);

                    List<TSDB_FIELD> fields = FetchFields(_taosConn);
                    foreach (var f in fields)
                    {
                        dt.Columns.Add(f.name, f.CSharpTypeName());
                        #if (DEBUG)
                        Console.WriteLine(string.Format("name:{0} type:{1} name:{2} bytes:{3}", f.name.PadRight(20), f.type.ToString().PadRight(5), f.TSDBDataTypeName().PadRight(10), f.bytes.ToString().PadRight(5)));
                        #endif
                    }

                    long result = taos_use_result(_taosConn);
                    if (result == 0)
                    {
                        Console.WriteLine(sql + " result set is null");
                        return dt;
                    }
                    IntPtr rowdata;
                    long queryRows = 0;
                    while ((rowdata = taos_fetch_row(result)) != IntPtr.Zero)
                    {
                        queryRows++;
                        DataRow dr = dt.NewRow();
                        Console.Write(queryRows +"\t");
                        for (int index = 0; index < fieldCount; ++index)
                        {
                            TSDB_FIELD field = fields[index];
                            int offset = 8 * index;

                            IntPtr data = Marshal.ReadIntPtr(rowdata, offset);

                          

                            if (data == IntPtr.Zero)
                            {
                                Console.Write("NULL");
                                continue;
                            }
                            dynamic v=new object();
                            switch ((TSDB_DATA_TYPE)field.type)
                            {
                                case TSDB_DATA_TYPE.BOOL:
                                    //bool v1 = Marshal.ReadByte(data) == 0 ? false : true;
                                    //Console.Write(v1);
                                    v = Marshal.ReadByte(data) == 0 ? false : true;
                                    break;
                                case TSDB_DATA_TYPE.TINYINT:
                                    // byte v2 = Marshal.ReadByte(data);
                                    //Console.Write(v2);
                                    v = Marshal.ReadByte(data);
                                    break;
                                case TSDB_DATA_TYPE.SMALLINT:
                                    //short v3 = Marshal.ReadInt16(data);
                                    //Console.Write(v3);
                                    v= Marshal.ReadInt16(data);
                                    break;
                                case TSDB_DATA_TYPE.INT:
                                    //int v4 = Marshal.ReadInt32(data);
                                    //Console.Write(v4);
                                    v = Marshal.ReadInt32(data);
                                    break;
                                case TSDB_DATA_TYPE.BIGINT:
                                    //long v5 = Marshal.ReadInt64(data);
                                    //Console.Write(v5);
                                    v = Marshal.ReadInt64(data);
                                    break;
                                case TSDB_DATA_TYPE.FLOAT:
                                    //float v6 = (float)Marshal.PtrToStructure(data, typeof(float));
                                    //Console.Write(v6);
                                    v = (float)Marshal.PtrToStructure(data, typeof(float));
                                    break;
                                case TSDB_DATA_TYPE.DOUBLE:
                                    //double v7 = (double)Marshal.PtrToStructure(data, typeof(double));
                                    //Console.Write(v7);
                                    v = (double)Marshal.PtrToStructure(data, typeof(double));
                                    break;
                                case TSDB_DATA_TYPE.BINARY:
                                    //string v8 = Marshal.PtrToStringAnsi(data);
                                    //Console.Write(v8);
                                    v = Marshal.PtrToStringAnsi(data);
                                    break;
                                case TSDB_DATA_TYPE.TIMESTAMP:
                                    long v9 = Marshal.ReadInt64(data);
                                    System.DateTime time = System.DateTime.MinValue;
                                    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0));
                                    time = startTime.AddMilliseconds(v9);
                                    //Console.Write(Convert.ToDateTime(time));
                                    v = time;
                                    break;
                                case TSDB_DATA_TYPE.NCHAR:
                                    //string v10 = Marshal.PtrToStringAnsi(data);
                                    //Console.Write(v10);
                                    v = Marshal.PtrToStringAnsi(data);
                                    break;
                            }
                            Console.Write(v + "\t");
                            dr[index] = v;

                        }
                        Console.WriteLine("");
                        dt.Rows.Add(dr);
                    }

                    if (taos_errno(_taosConn) != 0)
                    {
                        Console.WriteLine("Query is not complete， Error {0:G}", taos_errno(_taosConn), Error(_taosConn));
                    }
                    else
                    {
                        Console.WriteLine("Query is complete");
                    }
                    taos_free_result(result);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);           
            }
            return dt;

        }

        /// <summary>
        /// SQl查询返回JSON
        /// </summary>
        /// <param name="sql">SQl查询语句</param>
        /// <returns></returns>
        public string   QueryToJson(string sql)
        {
            return DataTableToJson(QueryToDataTable(sql));
        }
        /// <summary>
        /// 执行非查询ＳＱＬ
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>执行返回代码,见 enum TSDB_CODE</returns>
        public TSDB_CODE NonQuery(string sql)
        {
            return SQLCommand(sql);
        }
        /// <summary>
        /// 客户端版本号
        /// </summary>
        /// <returns>客户端版本号</returns>
        public string GetClientInfo()
        {
            return IntPtrToString(taos_get_client_info());
        }
        /// <summary>
        /// 服务器端版本号
        /// </summary>
        /// <returns>服务器端</returns>
        public string GetServerInfo()
        {
            if (_taosConn == 0) return "Server UnConnected"; 
            return IntPtrToString(taos_get_server_info(_taosConn));
        }

        public void Dispose()
        {
            if(_taosConn!=0)taos_close(_taosConn);
        }
        #region Private 
        private TSDB_CODE SQLCommand(string sql)
        {
            if (_taosConn == 0) return TSDB_CODE.INVALID_CONNECTION;
            if (taos_query(_taosConn, sql) == 0)
            {
                _taosErroNo = TSDB_CODE.SUCCESS;
                _taosErrorReaon ="Success";
                return TSDB_CODE.SUCCESS;
            }
            else
            {
                _taosErroNo = (TSDB_CODE)taos_errno(_taosConn);
                _taosErrorReaon = Error(_taosConn);
                Console.WriteLine("ErrorNo: {0} Reason:{1}. SQLCommand={2}",(int) _taosErroNo, _taosErrorReaon,sql);
                return _taosErroNo;
            }
        }

        private  string Error(long conn)
        {
            IntPtr errPtr = taos_errstr(conn);
            return Marshal.PtrToStringAnsi(errPtr);
        }
       
        private string IntPtrToString(IntPtr ptr)
        {
            return Marshal.PtrToStringAnsi(ptr);
        }
        private List<TSDB_FIELD> FetchFields(long taos)
        {
            const int fieldSize = 68;

            List<TSDB_FIELD> fields = new List<TSDB_FIELD>();
            long result = taos_use_result(taos);
            if (result == 0)
            {
                return fields;
            }
            int fieldCount = taos_field_count(taos);
            IntPtr fieldsPtr = taos_fetch_fields(result);

            for (int i = 0; i < fieldCount; ++i)
            {
                int offset = i * fieldSize;
                TSDB_FIELD meta = new TSDB_FIELD();
                meta.name = Marshal.PtrToStringAnsi(fieldsPtr + offset);
                meta.bytes = Marshal.ReadInt16(fieldsPtr + offset + 64);
                meta.type = Marshal.ReadByte(fieldsPtr + offset + 66);
                fields.Add(meta);
            }
            return fields;
        }
        private string DataTableToJson(DataTable table)
        {
            var JsonString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JsonString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JsonString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JsonString.Append("}");
                    }
                    else
                    {
                        JsonString.Append("},");
                    }
                }
                JsonString.Append("]");
            }
            return JsonString.ToString();
        }

       
        #endregion

    }
    /// <summary>
    /// TSDB 字段类：
    /// １.服务器内部类型　转换成　ＳＱＬ数据类型
    ///  2.ＳＱＬ数据类型  转换    C# 数据类型
    /// </summary>
   public  class TSDB_FIELD
    {
        public string name;
        public short bytes;
        public byte type;
        public TSDB_FIELD()
        {

        }
        public TSDB_FIELD(string name, TSDB_DATA_TYPE type, short bytes = 0)
        {
            this.name = name;
            this.bytes = bytes;
            this.type = (byte)type;
        }

        /// <summary>
        /// 服务器内部类型　转换成　ＳＱＬ数据类型
        /// </summary>
        /// <returns></returns>
        public string TSDBDataTypeName()
        {
            switch ((TSDB_DATA_TYPE)type)
            {
                case TSDB_DATA_TYPE.BOOL:
                    return "BOOL";
                case TSDB_DATA_TYPE.TINYINT:
                    return "TINYINT";
                case TSDB_DATA_TYPE.SMALLINT:
                    return "SMALLINT";
                case TSDB_DATA_TYPE.INT:
                    return "INT";
                case TSDB_DATA_TYPE.BIGINT:
                    return "BIGINT";
                case TSDB_DATA_TYPE.FLOAT:
                    return "FLOAT";
                case TSDB_DATA_TYPE.DOUBLE:
                    return "DOUBLE";
                case TSDB_DATA_TYPE.BINARY:
                    return "BINARY(" + bytes + ")";
                case TSDB_DATA_TYPE.TIMESTAMP:
                    return "TIMESTAMP";
                case TSDB_DATA_TYPE.NCHAR:
                    return "NCHAR(" + bytes + ")";
                default:
                    return "undefine";
            }
        }
        /// <summary>
        /// ＳＱＬ数据类型  转换    C# 数据类型
        /// </summary>
        /// <returns></returns>
        public Type CSharpTypeName()
        {
            switch ((TSDB_DATA_TYPE)type)
            {
                case TSDB_DATA_TYPE.BOOL:
                    return typeof(bool);
                case TSDB_DATA_TYPE.TINYINT:
                    return typeof(byte);
                case TSDB_DATA_TYPE.SMALLINT:
                    return typeof(Int16);
                case TSDB_DATA_TYPE.INT:
                    return typeof(Int32);
                case TSDB_DATA_TYPE.BIGINT:
                    return typeof(Int64);
                case TSDB_DATA_TYPE.FLOAT:
                    return typeof(float);
                case TSDB_DATA_TYPE.DOUBLE:
                    return typeof(double);
                case TSDB_DATA_TYPE.BINARY:
                    return typeof(string);
                case TSDB_DATA_TYPE.TIMESTAMP:
                    return typeof(DateTime);
                case TSDB_DATA_TYPE.NCHAR:
                    return typeof(string);
                default:
                    return typeof(string);
            }
        }
    }
    /// <summary>
    /// Taos DB C# Driver
    /// </summary>
}