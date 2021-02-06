using Element;
using Microsoft.AspNetCore.Components;
using SuperCode.Entities;
using SuperCode.Services;

namespace SuperCode.Pages
{
    public class ConnectionEditBase : BComponentBase
    {
        internal BForm form;
        internal bool isLoading = false;

        [Parameter]
        public ConnectionEntity Connection { get; set; }

        [Parameter]
        public DialogOption Dialog { get; set; }

        [Inject]
        public IConnectionService ConnectionService { get; set; }

        private bool isCreate = false;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            isCreate = Connection == null;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            form.MarkAsRequireRender();
        }

        public async System.Threading.Tasks.Task SubmitAsync()
        {
            if (!form.IsValid())
            {
                return;
            }
            string error = string.Empty;
            isLoading = true;
            Connection = form.GetValue<ConnectionEntity>();
            if (isCreate)
            {
                await ConnectionService.AddAsync(Connection);
            }
            else
            {
                await ConnectionService.UpdateAsync(Connection);
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
