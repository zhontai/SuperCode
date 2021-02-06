using ElectronNET.API;
using ElectronNET.API.Entities;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SuperCode.Configs;
using SuperCode.Utils;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SuperCode.Pages
{
    public class SettingsBase : BComponentBase
    {
        internal BForm form;
        internal bool isLoading = false;

        [Inject]
        public IOptions<CodeSettings> CodeSettingsOptions { get; set; }
        public CodeSettings CodeSettings { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            CodeSettings = CodeSettingsOptions.Value;
            //if (CodeSettings.SavePath.IsNull())
            //{
            //    CodeSettings.SavePath = await Electron.App.GetPathAsync(PathName.Home);
            //}
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender)
            {
                return;
            }
        }

        public async void ChangeSavePathAsync()
        {
            var savePath = await ElectronHelper.GetSelectPath();
            if (savePath.NotNull())
            {
                CodeSettings.SavePath = savePath;
                form.Refresh();
                SuperCodeHelper.SaveSettings(CodeSettings);
            }
        }

        public async void OpenSavePathAsync()
        {
            await ElectronHelper.OpenSavePathAsync(CodeSettings.SavePath);
        }

        public void OpenAtLoginChanged(bool value)
        {
            CodeSettings.OpenAtLogin = value;
            Electron.App.SetLoginItemSettings(new LoginSettings { OpenAtLogin = CodeSettings.OpenAtLogin });
            SuperCodeHelper.SaveSettings(CodeSettings);
        }

        public void CloseToTrayChanged(bool value)
        {
            CodeSettings.CloseToTray = value;
            SuperCodeHelper.SaveSettings(CodeSettings);
        }
    }
}
