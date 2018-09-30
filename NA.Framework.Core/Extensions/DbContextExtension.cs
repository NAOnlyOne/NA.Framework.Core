using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace NA.Framework.Core.Extensions
{
    public static class DbContextExtension
    {
        public static IList<T> SqlQuery<T>(this DbContext db, string sql, Func<DbDataReader, T> map, params object[] parameters)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            if (sql == null)
            {
                throw new ArgumentNullException(nameof(sql));
            }
            if (map == null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            var conn = db.Database.GetDbConnection();
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    var props = typeof(T).GetProperties();
                    var result = new List<T>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var model = map(reader);
                            result.Add(model);
                        }
                    }
                    return result;
                }
            }
            finally
            {
                conn.Close();
            }
        }

        public static int ExecuteSql(this DbContext db, string sql, params DbParameter[] parameters)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            if (sql == null)
            {
                throw new ArgumentNullException(nameof(sql));
            }

            var conn = db.Database.GetDbConnection();
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
