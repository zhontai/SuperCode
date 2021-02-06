using ElectronNET.API;
using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SuperCode.Configs;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SuperCode.Utils
{
    public class SuperCodeHelper : IDisposable
    {
        private bool _alreadyDispose = false;

        public SuperCodeHelper()
        {
        }
        ~SuperCodeHelper()
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

        public static void SaveSettings(CodeSettings codeSettings)
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var jsonData = JsonConvert.SerializeObject(codeSettings, Formatting.Indented, settings);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "codesettings.json").ToPath();
            FileHelper.WriteFile(filePath, jsonData);
        }
    }
}
