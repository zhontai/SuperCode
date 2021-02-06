using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using FreeSql;
using FreeSql.Aop;
using FreeSql.DataAnnotations;
using SuperCode.Utils;
using SuperCode.Configs;
using System.Collections.Generic;
using System.Reflection;
using SuperCode.Entities;

namespace SuperCode.Db
{
    public class DbHelper
    {
        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="dbConfig"></param>
        /// <returns></returns>
        public async static Task CreateDatabase(DbConfig dbConfig)
        {
            if (!dbConfig.CreateDb || dbConfig.Type == DataType.Sqlite)
            {
                return;
            }

            var db = new FreeSqlBuilder()
                    .UseConnectionString(dbConfig.Type, dbConfig.CreateDbConnectionString)
                    .Build();

            try
            {
                Console.WriteLine("\r\n create database started");
                await db.Ado.ExecuteNonQueryAsync(dbConfig.CreateDbSql);
                Console.WriteLine(" create database succeed");
            }
            catch (Exception e)
            {
                Console.WriteLine($" create database failed.\n {e.Message}");
            }
        }

        /// <summary>
        /// 获得指定程序集表实体
        /// </summary>
        /// <returns></returns>
        public static Type[] GetEntityTypes()
        {
            List<string> assemblyNames = new List<string>()
            {
                "SuperCode"
            };

            List<Type> entityTypes = new List<Type>();

            foreach (var assemblyName in assemblyNames)
            {
                foreach (Type type in Assembly.Load(assemblyName).GetExportedTypes())
                {
                    foreach (Attribute attribute in type.GetCustomAttributes())
                    {
                        if (attribute is TableAttribute tableAttribute)
                        {
                            if (tableAttribute.DisableSyncStructure == false)
                            {
                                entityTypes.Add(type);
                            }
                        }
                    }
                }
            }

            return entityTypes.ToArray();
        }

        /// <summary>
        /// 同步结构
        /// </summary>
        public static void SyncStructure(IFreeSql db, string msg = null, DbConfig dbConfig = null)
        {
            //打印结构比对脚本
            //var dDL = db.CodeFirst.GetComparisonDDLStatements<PermissionEntity>();
            //Console.WriteLine("\r\n " + dDL);

            //打印结构同步脚本
            //db.Aop.SyncStructureAfter += (s, e) =>
            //{
            //    if (e.Sql.NotNull())
            //    {
            //        Console.WriteLine(" sync structure sql:\n" + e.Sql);
            //    }
            //};

            //获得指定程序集表实体
            var entityTypes = GetEntityTypes();

            var dbType = dbConfig.Type.ToString();
            Console.WriteLine($"\r\n {(msg.NotNull() ? msg : $"sync {dbType} structure")} started");
            
            // 同步结构
            if(entityTypes.Length > 0)
            {
                if (dbConfig.Type == DataType.Oracle)
                {
                    db.CodeFirst.IsSyncStructureToUpper = true;
                }
                db.CodeFirst.SyncStructure(entityTypes);
            }
            Console.WriteLine($" {(msg.NotNull() ? msg : $"sync {dbType} structure")} succeed");
        }

        /// <summary>
        /// 初始化数据表数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="data"></param>
        /// <param name="tran"></param>
        /// <param name="dbConfig"></param>
        /// <returns></returns>
        private static async Task InitDtData<T>(
            IFreeSql db, 
            T[] data, 
            System.Data.Common.DbTransaction tran, 
            DbConfig dbConfig = null
        ) where T : class
        {
            var table = typeof(T).GetCustomAttributes(typeof(TableAttribute),false).FirstOrDefault() as TableAttribute;
            var tableName = table.Name;

            try
            {
                if (!await db.Queryable<T>().AnyAsync())
                {
                    if (data?.Length > 0)
                    {
                        var insert = db.Insert<T>();

                        if(tran != null)
                        {
                            insert = insert.WithTransaction(tran);
                        }

                        if(dbConfig.Type == DataType.SqlServer)
                        {
                            var insrtSql = insert.AppendData(data).InsertIdentity().ToSql();
                            await db.Ado.ExecuteNonQueryAsync($"SET IDENTITY_INSERT {tableName} ON\n {insrtSql} \nSET IDENTITY_INSERT {tableName} OFF");
                        }
                        else
                        {
                            await insert.AppendData(data).InsertIdentity().ExecuteAffrowsAsync();
                        }
                        
                        Console.WriteLine($" table: {tableName} sync data succeed");
                    }
                    else
                    {
                        Console.WriteLine($" table: {tableName} import data []");
                    }
                }
                else
                {
                    Console.WriteLine($" table: {tableName} record already exists");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" table: {tableName} sync data failed.\n{ex.Message}");
            }
        }

        /// <summary>
        /// 同步数据审计方法
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private static void SyncDataAuditValue(object s, AuditValueEventArgs e)
        {
            //if (e.AuditValueType == AuditValueType.Insert)
            //{
            //    switch (e.Property.Name)
            //    {
            //        case "CreatedUserId":
            //            e.Value = 2;
            //            break;
            //        case "CreatedUserName":
            //            e.Value = "admin";
            //            break;
            //    }
            //}
            //else if (e.AuditValueType == AuditValueType.Update)
            //{
            //    switch (e.Property.Name)
            //    {
            //        case "ModifiedUserId":
            //            e.Value = 2;
            //            break;
            //        case "ModifiedUserName":
            //            e.Value = "admin";
            //            break;
            //    }
            //}
        }

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <returns></returns>
        public static async Task SyncData(IFreeSql db, DbConfig dbConfig = null)
        {
            try
            {
                //db.Aop.CurdBefore += (s, e) =>
                //{
                //    Console.WriteLine($"{e.Sql}\r\n");
                //};

                Console.WriteLine("\r\n sync data started");

                db.Aop.AuditValue += SyncDataAuditValue;
               
                var filePath = Path.Combine(AppContext.BaseDirectory, "Db/Data/data.json").ToPath();
                var jsonData = FileHelper.ReadFile(filePath);
                var data = JsonConvert.DeserializeObject<Data>(jsonData);

                using (var uow = db.CreateUnitOfWork())
                using (var tran = uow.GetOrBeginTransaction())
                {
                    await InitDtData(db, data.Connections, tran, dbConfig);
                    await InitDtData(db, data.OnlineTemplateTools, tran, dbConfig);

                    uow.Commit();
                }

                db.Aop.AuditValue -= SyncDataAuditValue;

                Console.WriteLine(" sync data succeed");
            }
            catch (Exception ex)
            {
                throw new Exception($" sync data failed.\n{ex.Message}");
            }
        }

        /// <summary>
        /// 生成极简数据
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static async Task GenerateSimpleJsonData(IFreeSql db)
        {
            try
            {
                Console.WriteLine("\r\n generate data started");

                #region 数据表

                #region 连接管理
                var connections = await db.Queryable<ConnectionEntity>().ToListAsync(a => new
                {
                    a.Id,
                    a.ConnectionName,
                    a.DbType,
                    a.ConnectionString
                });
                #endregion

                #region 在线模板工具管理
                var onlineTemplateTools = await db.Queryable<OnlineTemplateToolEntity>().ToListAsync(a => new
                {
                    a.Id,
                    a.Name,
                    a.InstallCommand,
                    a.UnInstallCommand,
                    a.CreateCommand
                });
                #endregion

                #endregion

                #region 生成数据
                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                var jsonData = JsonConvert.SerializeObject(new
                {
                    connections,
                    onlineTemplateTools
                },
                Formatting.Indented,
                settings
                );
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Db/Data/data.json").ToPath();
                FileHelper.WriteFile(filePath, jsonData);
                #endregion

                Console.WriteLine(" generate data succeed\r\n");
            }
            catch (Exception ex)
            {
                throw new Exception($" generate data failed。\n{ex.Message}\r\n");
            }
        }
    }
}
