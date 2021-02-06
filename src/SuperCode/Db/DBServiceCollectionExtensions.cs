using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using FreeSql;
using SuperCode.Configs;
using Microsoft.Extensions.Options;
using SuperCode.Utils;

namespace SuperCode.Db
{
    public static class DBServiceCollectionExtensions
    {
        /// <summary>
        /// 添加数据库
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbConfig"></param>
        /// <returns></returns>
        public async static Task AddDbAsync(this IServiceCollection services)
        {
            var codeSettings = new ConfigHelper().Get<CodeSettings>("codesettings", AppContext.BaseDirectory);
            var dbConfig = services.BuildServiceProvider().GetRequiredService<IOptions<DbConfig>>().Value;
            //创建数据库
            if (dbConfig.CreateDb && codeSettings.InitDb)
            {
                await DbHelper.CreateDatabase(dbConfig);
            }

            #region FreeSql
            var freeSqlBuilder = new FreeSqlBuilder()
                    .UseConnectionString(dbConfig.Type, dbConfig.ConnectionString)
                    .UseAutoSyncStructure(dbConfig.SyncStructure && codeSettings.InitDb)
                    .UseLazyLoading(false)
                    .UseNoneCommandParameter(true);

            #region 监听所有命令
            if (dbConfig.MonitorCommand && codeSettings.InitDb)
            {
                freeSqlBuilder.UseMonitorCommand(cmd => { }, (cmd, traceLog) =>
                {
                    //Console.WriteLine($"{cmd.CommandText}\n{traceLog}\r\n");
                    Console.WriteLine($"{cmd.CommandText}\r\n");
                });
            }
            #endregion

            var fsql = freeSqlBuilder.Build();
           
            #region 初始化数据库
            //同步结构
            if (dbConfig.SyncStructure && codeSettings.InitDb)
            {
                DbHelper.SyncStructure(fsql, dbConfig: dbConfig);
            }

            //同步数据
            if (dbConfig.SyncData && codeSettings.InitDb)
            {
                await DbHelper.SyncData(fsql, dbConfig);
            }
            #endregion

            //生成数据包
            if (dbConfig.GenerateData && !dbConfig.CreateDb && !dbConfig.SyncData)
            {
                await DbHelper.GenerateSimpleJsonData(fsql);
            }

            #region 监听Curd操作
            if (dbConfig.Curd && codeSettings.InitDb)
            {
                fsql.Aop.CurdBefore += (s, e) =>
                {
                    Console.WriteLine($"{e.Sql}\r\n");
                };
            }
            #endregion

            #region 审计数据
            /*
            //计算服务器时间
            var serverTime = fsql.Select<T>().Limit(1).First(a => DateTime.local);
            var timeOffset = DateTime.UtcNow.Subtract(serverTime);
            fsql.Aop.AuditValue += (s, e) =>
            {
                if (e.AuditValueType == FreeSql.Aop.AuditValueType.Insert)
                {
                    switch (e.Property.Name)
                    {
                        case "CreatedTime":
                            e.Value = DateTime.Now.Subtract(timeOffset);
                            break;
                    }
                }
                else if (e.AuditValueType == FreeSql.Aop.AuditValueType.Update)
                {
                    switch (e.Property.Name)
                    {
                        case "ModifiedTime":
                            e.Value = DateTime.Now.Subtract(timeOffset);
                            break;
                    }
                }
            };
            */
            #endregion

            services.AddSingleton(fsql);

            #endregion

            codeSettings.InitDb = false;
            SuperCodeHelper.SaveSettings(codeSettings);
        }
    }
}
