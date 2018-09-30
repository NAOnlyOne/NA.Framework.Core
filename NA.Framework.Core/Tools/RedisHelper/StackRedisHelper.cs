using ProtoBuf;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NA.Framework.Core.Tools
{
    public partial class StackRedisHelper
    {
        /// <summary>
        /// Redis连接对象
        /// </summary>
        private static ConnectionMultiplexer _conn;

        /// <summary>
        /// 当前使用第几个数据库
        /// </summary>
        private static int _dbIndex;

        /// <summary>
        /// Key前缀
        /// </summary>
        private static string _prefix;

        /// <summary>
        /// 日志记录器
        /// </summary>
        private static StackBaseLogger _logger;

        /// <summary>
        /// 当前数据库
        /// </summary>
        public static IDatabase Database
        {
            get
            {
                if (_conn == null)
                {
                    throw new Exception("Redis连接未打开，请调用Init函数初始化本工具类");
                }

                if (_database == null)
                {
                    _database = _conn.GetDatabase(_dbIndex);
                }

                return _database;
            }
        }
        private static IDatabase _database;

        /// <summary>
        /// 初始化Redis连接
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="prefix"></param>
        /// <param name="logger"></param>
        /// <param name="dbIndex"></param>
        public static void Init(string connStr, string prefix, int dbIndex = 0, StackBaseLogger logger = null)
        {
            if (string.IsNullOrWhiteSpace(connStr))
            {
                throw new Exception("Redis连接字符串不能为空");
            }

            _conn = ConnectionMultiplexer.Connect(connStr, logger);

            _prefix = prefix;
            _dbIndex = dbIndex;
            _logger = logger;

            //注册事件
            _conn.ConnectionFailed += Redis_ConnectionFailed;
            _conn.ErrorMessage += Redis_ErrorMessage;
            _conn.ConnectionRestored += Redis_ConnectionRestored;
        }

        /// <summary>
        /// 初始化Redis连接
        /// </summary>
        /// <param name="config"></param>
        /// <param name="prefix"></param>
        /// <param name="logger"></param>
        /// <param name="dbIndex"></param>
        public static void Init(ConfigurationOptions config, string prefix, int dbIndex = 0, StackBaseLogger logger = null)
        {
            Init(config.ToString(), prefix, dbIndex, logger);
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public static void Dispose()
        {
            _conn?.Close();
            _dbIndex = -1;
            _logger = null;
            _prefix = null;
        }
    }

    /// <summary>
    /// 公有方法
    /// </summary>
    public partial class StackRedisHelper
    {
        public static string MergeKey(string key)
        {
            return $"{_prefix}_{key}";
        }

        public static string GetString(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            else
                return Database.StringGet(MergeKey(key));
        }

        public static T GetObj<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return default(T);

            var value = Database.StringGet(MergeKey(key));
            if (string.IsNullOrWhiteSpace(value))
                return default(T);

            using (MemoryStream ms = new MemoryStream())
            {
                return Serializer.Deserialize<T>(ms);
            }
        }

        public static string HashGet(string key, string field)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(field))
                return null;
            else
                return Database.HashGet(MergeKey(key), field);
        }

        public static Dictionary<string, string> HashGet(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            var values = Database.HashGetAll(MergeKey(key));
            return values.ToStringDictionary();
        }

        public static bool HashSet(string key, Dictionary<string, string> values)
        {
            if (string.IsNullOrWhiteSpace(key) || values == null)
                return false;

            List<HashEntry> entry = new List<HashEntry>();
            foreach (var item in values)
            {
                entry.Add(new HashEntry(item.Key, item.Value));
            }
            Database.HashSet(MergeKey(key), entry.ToArray());
            return true;
        }

        public static bool HashSet(string key, string field, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(field) || string.IsNullOrWhiteSpace(value))
                return false;
            else
                return Database.HashSet(MergeKey(key), field, value);
        }

        public static bool HasKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;
            else
                return Database.KeyExists(MergeKey(key));
        }

        public static bool KeyExpire(string key, int timeout)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;
            else
                return Database.KeyExpire(MergeKey(key), GetTimeSpan(timeout));
        }

        public static bool RemoveKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;
            else
                return Database.KeyDelete(MergeKey(key));
        }

        /// <summary>
        /// 缓存字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeout">过期时间，单位：秒</param>
        /// <returns></returns>
        public static bool SetString(string key, string value, int timeout = 0)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;
            else
                return Database.StringSet(MergeKey(key), value, GetTimeSpan(timeout));
        }

        public static bool SetString(string key, string value, TimeSpan? timeSpan = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;
            else
                return Database.StringSet(MergeKey(key), value, timeSpan);
        }

        /// <summary>
        /// 以Protobuf序列化方式缓存对象
        /// 注意：自定义类必须添加Proto特性（[ProtoContract] [ProtoMember]）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeout">有效时长（单位：秒）</param>
        /// <returns></returns>
        public static bool SetObj<T>(string key, T value, int timeout = 0)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize(ms, value);
                return Database.StringSet(MergeKey(key), ms.ToArray(), GetTimeSpan(timeout));
            }
        }

        /// <summary>
        /// 以Protobuf序列化方式缓存对象
        /// 注意：自定义类必须添加Proto特性（[ProtoContract] [ProtoMember]）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static bool SetObj<T>(string key, T value, TimeSpan? timeSpan = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize(ms, value);
                return Database.StringSet(MergeKey(key), ms.ToArray(), timeSpan);
            }
        }

        public static void SetDbIndex(int dbIndex)
        {
            _dbIndex = dbIndex;

            if (_conn != null)
                _database = _conn.GetDatabase(dbIndex);
        }

        public static long StringDecrement(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return 0;
            else
                return Database.StringDecrement(key);
        }

        public static long StringIncrement(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return 0;
            else
                return Database.StringIncrement(key);
        }
    }

    /// <summary>
    /// 私有方法
    /// </summary>
    public partial class StackRedisHelper
    {
        private static TimeSpan? GetTimeSpan(int timeout)
        {
            if (timeout > 0)
                return TimeSpan.FromSeconds(timeout);
            else
                return null;
        }
    }

    /// <summary>
    /// 事件
    /// </summary>
    public partial class StackRedisHelper
    {
        private static void Redis_ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            _logger?.Log($"Redis重新连接成功，详细信息：{e.Exception.Message}");
        }

        private static void Redis_ErrorMessage(object sender, RedisErrorEventArgs e)
        {
            _logger?.Log($"Redis服务响应失败，错误信息：{e.Message}");
        }

        private static void Redis_ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            _logger?.Log($"Redis连接失败，错误信息：{e.Exception.Message}");
        }
    }
}
