using ElectronNET.API;
using ElectronNET.API.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SuperCode.Utils
{
    public class ElectronHelper : IDisposable
    {
        private bool _alreadyDispose = false;

        public ElectronHelper()
        {
        }
        ~ElectronHelper()
        {
            Dispose();
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_alreadyDispose) return;
            _alreadyDispose = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static async Task<string> GetSelectPath()
        {
            var mainWindow = Electron.WindowManager.BrowserWindows.First();
            var options = new OpenDialogOptions
            {
                Properties = new OpenDialogProperty[]
                {
                        OpenDialogProperty.openFile,
                        OpenDialogProperty.openDirectory
                }
            };

            string[] files = await Electron.Dialog.ShowOpenDialogAsync(mainWindow, options);
            return string.Join("", files);
        }

        public static async Task OpenSavePathAsync(string path)
        {
            if (path.NotNull())
            {
                await Electron.Shell.OpenPathAsync(path);
            }
        }
    }
}
