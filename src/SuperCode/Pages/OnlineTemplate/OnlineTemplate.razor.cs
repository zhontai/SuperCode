using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using SuperCode.Configs;
using SuperCode.Entities;
using SuperCode.Pages.OnlineTemplate;
using SuperCode.Services;
using SuperCode.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperCode.Pages
{
    public class OnlineTemplateBase : BComponentBase
    {
        internal List<OnlineTemplateToolEntity> Lists { get; private set; } = new List<OnlineTemplateToolEntity>();
        internal BTable table;
        internal BForm searchForm;
        internal bool isLoading;

        [Inject]
        public IOnlineTemplateService OnlineTemplateService { get; set; }

        [Inject]
        public IOptions<CodeSettings> CodeSettingsOptions { get; set; }
        public CodeSettings CodeSettings { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            CodeSettings = CodeSettingsOptions.Value;
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
            var condition = searchForm.GetValue<OnlineTemplateToolEntity>();
            Lists = await OnlineTemplateService.GetListAsync(condition.Name);
            isLoading = false;
            table.MarkAsRequireRender();
            RequireRender = true;
            StateHasChanged();
            return Task.CompletedTask;
        }

        public async Task AddAsync()
        {
            await DialogService.ShowDialogAsync<OnlineTemplateEdit>("新增模板工具", new Dictionary<string, object>());
            await RefreshAsync();
        }

        public async Task EditAsync(object data)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add(nameof(OnlineTemplateEdit.OnlineTemplate), data);
            await DialogService.ShowDialogAsync<OnlineTemplateEdit>("编辑模板工具", parameters);
            await RefreshAsync();
        }

        public async Task DeleteAsync(object data)
        {
            var confirm = await ConfirmAsync("确认删除该模板工具？");
            if (confirm != MessageBoxResult.Ok)
            {
                return;
            }
            var result = await OnlineTemplateService.DeleteAsync(((OnlineTemplateToolEntity)data).Id);
           
            //Toast(result);
            await RefreshAsync();
        }

        public async Task InstallToolAsync(object data)
        {
            var onlineTemplate = (OnlineTemplateToolEntity)data;
            var command = onlineTemplate.InstallCommand;
            if (command.NotNull())
            {
                await Task.Run(() =>
                {
                    onlineTemplate._InstallLoading = true;
                    try
                    {
                        var result = DosCommandHelper.Execute(command);
                        Toast(result, MessageType.Success);
                    }
                    catch (Exception ex)
                    {
                        Toast(ex.Message, MessageType.Error);
                    }
                    onlineTemplate._InstallLoading = false;
                });
            }
        }

        public async Task UnInstallToolAsync(object data)
        {
            var onlineTemplate = (OnlineTemplateToolEntity)data;
            var command = onlineTemplate.UnInstallCommand;
            if (command.NotNull())
            {
                await Task.Run(() =>
                {
                    onlineTemplate._UnInstallLoading = true;
                    try
                    {
                        var result = DosCommandHelper.Execute(command);
                        Toast(result, MessageType.Success);
                    }
                    catch (Exception ex)
                    {
                        Toast(ex.Message, MessageType.Error);
                    }
                    onlineTemplate._UnInstallLoading = false;
                });
            }
        }

        public async Task CreateToolAsync(object data)
        {
            var onlineTemplate = (OnlineTemplateToolEntity)data;
            var command = onlineTemplate.CreateCommand;
            if (command.NotNull())
            {
                await Task.Run(async () =>
                {
                    onlineTemplate._CreateLoading = true;
                    try
                    {
                        var isOutput = command.Contains("-Output ");
                        var savePath = string.Empty;
                        if (!isOutput)
                        {
                            savePath = CodeSettings.SavePath;
                            if (savePath.IsNull())
                            {
                                savePath = await ElectronHelper.GetSelectPath();
                            }
                            command = $"{command} -Output \"{savePath}\"";
                        }
                        var result = DosCommandHelper.Execute(command);
                        Toast(result, MessageType.Success);
                        if (savePath.NotNull())
                        {
                            await ElectronHelper.OpenSavePathAsync(savePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Toast(ex.Message, MessageType.Error);
                    }
                    onlineTemplate._CreateLoading = false;
                });
            }
        }
    }
}
