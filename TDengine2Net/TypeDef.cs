using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDengine2Net
{
   public enum TSDB_CODE
    {
        SUCCESS = 0,
        ACTION_IN_PROGRESS = 1,
        LAST_SESSION_NOT_FINISHED = 5,
        INVALID_SESSION_ID = 6,
        INVALID_TRAN_ID = 7,
        INVALID_MSG_TYPE = 8,
        ALREADY_PROCESSED = 9,
        AUTH_FAILURE = 10,
        WRONG_MSG_SIZE = 11,
        UNEXPECTED_RESPONSE = 12,
        INVALID_RESPONSE_TYPE = 13,
        NO_RESOURCE = 14,
        INVALID_TIME_STAMP = 15,
        MISMATCHED_METER_ID = 16,
        ACTION_TRANS_NOT_FINISHED = 17,
        ACTION_NOT_ONLINE = 18,
        ACTION_SEND_FAILD = 19,
        NOT_ACTIVE_SESSION = 20,
        INVALID_VNODE_ID = 21,
        APP_ERROR = 22,
        INVALID_IE = 23,
        INVALID_VALUE = 24,
        REDIRECT = 25,
        ALREADY_THERE = 26,
        INVALID_METER_ID = 27,
        INVALID_SQL = 28,
        NETWORK_UNAVAIL = 29,
        INVALID_MSG_LEN = 30,
        INVALID_DB = 31,
        INVALID_TABLE = 32,
        DB_ALREADY_EXIST = 33,
        TABLE_ALREADY_EXIST = 34,
        INVALID_USER = 35,
        INVALID_ACCT = 36,
        INVALID_PASS = 37,
        DB_NOT_SELECTED = 38,
        MEMORY_CORRUPTED = 39,
        USER_ALREADY_EXIST = 40,
        NO_RIGHTS = 41,
        DISCONNECTED = 42,
        NO_MASTER = 43,
        NOT_CONFIGURED = 44,
        INVALID_OPTION = 45,
        NODE_OFFLINE = 46,
        SYNC_REQUIRED = 47,
        NO_ENOUGH_DNODES = 48,
        UNSYNCED = 49,
        TOO_SLOW = 50,
        OTHERS = 51,
        NO_REMOVE_MASTER = 52,
        WRONG_SCHEMA = 53,
        NOT_ACTIVE_VNODE = 54,
        TOO_MANY_USERS = 55,
        TOO_MANY_DATABSES = 56,
        TOO_MANY_TABLES = 57,
        TOO_MANY_DNODES = 58,
        TOO_MANY_ACCTS = 59,
        ACCT_ALREADY_EXIST = 60,
        DNODE_ALREADY_EXIST = 61,
        SDB_ERROR = 62,
        METRICMETA_EXPIRED = 63,   // local cached metric-meta expired causes error in metric query
        NOT_READY = 64,    // peer is not ready to process data
        MAX_SESSIONS = 65,    // too many sessions
        MAX_CONNECTIONS = 66,    // too many connections
        SESSION_ALREADY_EXIST = 67,
        NO_QSUMMARY = 68,
        SERV_OUT_OF_MEMORY = 69,
        INVALID_QHANDLE = 70,
        RELATED_TABLES_EXIST = 71,
        MONITOR_DB_FORBEIDDEN = 72,
        NO_DISK_PERMISSIONS = 73,
        VG_INIT_FAILED = 74,
        DATA_ALREADY_IMPORTED = 75,
        OPS_NOT_SUPPORT = 76,
        INVALID_QUERY_ID = 77,
        INVALID_STREAM_ID = 78,
        INVALID_CONNECTION = 79,
        ACTION_NOT_BALANCED = 80,
        CLI_OUT_OF_MEMORY = 81,
        DATA_OVERFLOW = 82,
        QUERY_CANCELLED = 83,
        GRANT_TIMESERIES_LIMITED = 84,
        GRANT_EXPIRED = 85,
        CLI_NO_DISKSPACE = 86,
        FILE_CORRUPTED = 87,
        INVALID_CLIENT_VERSION = 88,
        INVALID_ACCT_PARAMETER = 89,
        NOT_ENOUGH_TIME_SERIES = 90,
        NO_WRITE_ACCESS = 91,
        NO_READ_ACCESS = 92,
        GRANT_DB_LIMITED = 93,
        GRANT_USER_LIMITED = 94,
        GRANT_CONN_LIMITED = 95,
        GRANT_STREAM_LIMITED = 96,
        GRANT_SPEED_LIMITED = 97,
        GRANT_STORAGE_LIMITED = 98,
        GRANT_QUERYTIME_LIMITED = 99,
        GRANT_ACCT_LIMITED = 100,
        GRANT_DNODE_LIMITED = 101,
        GRANT_CPU_LIMITED = 102,
        SESSION_NOT_READY = 103,     // table NOT in ready state
        BATCH_SIZE_TOO_BIG = 104,
        TIMESTAMP_OUT_OF_RANGE = 105,
        INVALID_QUERY_MSG = 106,     // failed to validate the sql expression msg by vnode
        CACHE_BLOCK_TS_DISORDERED = 107,      // time stamp in cache block is disordered
        FILE_BLOCK_TS_DISORDERED = 108,      // time stamp in file block is disordered
        INVALID_COMMIT_LOG = 109,      // commit log init failed
        SERV_NO_DISKSPACE = 110,
        NOT_SUPER_TABLE = 111,      // operation only available for super table
        DUPLICATE_TAGS = 112,      // tags value for join not unique
        INVALID_SUBMIT_MSG = 113,
        NOT_ACTIVE_TABLE = 114,
        INVALID_TABLE_ID = 115,
        INVALID_VNODE_STATUS = 116,
        FAILED_TO_LOCK_RESOURCES = 117,
        TABLE_ID_MISMATCH = 118,
        QUERY_CACHE_ERASED = 119,

        MAX_ERROR_CODE = 120,
    }
    public enum TSDB_DATA_TYPE : byte
    {
        BOOL = 1,
        TINYINT = 2,
        SMALLINT = 3,
        INT = 4,
        BIGINT = 5,
        FLOAT = 6,
        DOUBLE = 7,
        BINARY = 8,
        TIMESTAMP = 9,
        NCHAR = 10
    }

    public enum TSDB_OPTION
    {
        LOCALE = 0,
        CHARSET = 1,
        TIMEZONE = 2,
        CONFIGDIR = 3,
        SHELL_ACTIVITY_TIMER = 4,
        SOCKET_TYPE = 5,
        TSDB_MAX_OPTIONS = 6
    }


}
