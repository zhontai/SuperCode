using Element;
using Microsoft.AspNetCore.Components;
using SuperCode.Entities;
using SuperCode.Services;
using System.Threading.Tasks;

namespace SuperCode.Pages
{
    public class OnlineTemplateEditBase : BComponentBase
    {
        private bool isCreate = false;
        internal BForm form;
        internal bool isLoading = false;

        [Parameter]
        public OnlineTemplateToolEntity OnlineTemplate { get; set; }

        [Inject]
        public IOnlineTemplateService OnlineTemplateService { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            isCreate = OnlineTemplate == null;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            form.MarkAsRequireRender();
        }

        public async Task SubmitAsync()
        {
            if (!form.IsValid())
            {
                return;
            }
            string error = string.Empty;
            isLoading = true;
            OnlineTemplate = form.GetValue<OnlineTemplateToolEntity>();
            if (isCreate)
            {
                await OnlineTemplateService.AddAsync(OnlineTemplate);
            }
            else
            {
                await OnlineTemplateService.UpdateAsync(OnlineTemplate);
            }

            isLoading = false;
            if (!string.IsNullOrWhiteSpace(error))
            {
                Toast(error);
                return;
            }
            _ = DialogService.CloseDialogAsync(this, (object)null);
        }
    }
}
