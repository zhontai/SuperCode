using Element;
using FreeSql;
using Microsoft.AspNetCore.Components;
using SuperCode.Entities;
using SuperCode.Pages.Connection;
using SuperCode.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperCode.Pages
{
    public class ConnectionBase : BComponentBase
    {
        protected List<ConnectionEntity> Connections { get; private set; } = new List<ConnectionEntity>();
        protected BTable table;
        internal BForm searchForm;
        internal bool isLoading;

        [Inject]
        public IConnectionService ConnectionService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender)
            {
                return;
            }
            await RefreshAsync();
        }

        public async Task<Task> RefreshAsync()
        {
            if (table == null)
            {
                return Task.CompletedTask;
            }

            isLoading = true;
            var condition = searchForm.GetValue<ConnectionEntity>();
            Connections = await ConnectionService.GetListAsync(condition.ConnectionName);
            isLoading = false;
            table.MarkAsRequireRender();
            RequireRender = true;
            StateHasChanged();
            return Task.CompletedTask;
        }

        public async Task AddAsync()
        {
            await DialogService.ShowDialogAsync<ConnectionEdit>("新增连接", new Dictionary<string, object>());
            await RefreshAsync();
        }

        public async Task EditAsync(object connection)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add(nameof(ConnectionEdit.Connection), connection);
            await DialogService.ShowDialogAsync<ConnectionEdit>("编辑连接", parameters);
            await RefreshAsync();
        }

        public async Task DeleteAsync(object connection)
        {
            var confirm = await ConfirmAsync("确认删除该连接？");
            if (confirm != MessageBoxResult.Ok)
            {
                return;
            }
            var result = await ConnectionService.DeleteAsync(((ConnectionEntity)connection).Id);
           
            //Toast(result);
            await RefreshAsync();
        }

        public async Task TestConnectionAsync(object data)
        {
            await Task.Run(async () =>
            {
                var connection = (ConnectionEntity)data;
                connection._TestConnectioinLoading = true;
                using (var fsql = new FreeSqlBuilder().UseConnectionString(connection.DbType, connection.ConnectionString).Build())
                {
                    var isConnect = await fsql.Ado.ExecuteConnectTestAsync();
                    if (isConnect)
                    {
                        Toast("连接成功", MessageType.Success);
                    }
                    else
                    {
                        Toast("连接失败", MessageType.Error);
                    }
                }
                connection._TestConnectioinLoading = false;
            });
        }
    }
}
