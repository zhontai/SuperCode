
using Element;
using Microsoft.AspNetCore.Components;
using SuperCode.Models;
using SuperCode.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SuperCode
{
    public record MenuDataItem(string Path="",string Label="");

    public partial class BasicLayout
    {
        [Inject] protected NavigationManager NavigationManager { get; set; }

        protected List<MenuModel> Menus { get; set; } = new List<MenuModel>();

        protected Dictionary<string, Pane> Panes = new Dictionary<string, Pane>();

        protected ObservableCollection<TabOption> Tabs = new ObservableCollection<TabOption>();

        protected string MenuTitle;

        protected bool ToActivePane = false;

        public BTab Tab { get; set; }
        public BMenu Menu { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            var path = new Uri(NavigationManager.Uri).LocalPath;
            Menus.Add(new MenuModel()
            {
                Label = "模板管理",
                Name = "template",
                Icon = "el-icon-menu",
                IsOpen = true,
                Children = new List<MenuModel>()
                {
                    //new MenuModel(){
                    //     Label="本地模板",
                    //     Icon="el-icon-document",
                    //     Route="/local-template"
                    //},
                    new MenuModel(){
                         Label="在线模板工具",
                         Icon="el-icon-document",
                         Route="/online-template"
                    }
                }
            });
            Menus.Add(new MenuModel()
            {
                Label = "连接管理",
                Name = "connection",
                Icon = "el-icon-menu",
                IsOpen = true,
                Children = new List<MenuModel>()
                {

                    new MenuModel(){
                         Label="连接管理",
                         Icon="el-icon-link",
                         Route="/connection"
                    }
                }
            });
            Menus.Add(new MenuModel()
            {
                Label = "系统管理",
                Name = "system",
                Icon = "el-icon-menu",
                IsOpen = true,
                Children = new List<MenuModel>()
                {

                    new MenuModel(){
                         Label="系统设置",
                         Icon="el-icon-setting",
                         Route="/settings"
                    }
                }
            });

            FixMenuInfo(Menus);

            FindMenuTitle(Menus, path);

            if(path == "/")
            {
                NavigationManager.NavigateTo("/online-template");
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            OpenSubMenu(Menus);
            base.OnAfterRender(firstRender);
        }

        private Pane GetAnotherPane(Pane pane)
        {
            Pane anotherPane = null;

            if (ToActivePane)
            {
                anotherPane = Panes.Values.OrderByDescending(v => v.ActiveTime).FirstOrDefault(v => v.Url != pane.Url);
            }
            else
            {
                var paneIndex = -1;
                var panes = Panes.Values.OrderByDescending(v => v.StartTime).ToArray();
                for (int i = 0, len = panes.Length; i < len; i++)
                {
                    if (panes[i].Url == pane.Url)
                    {
                        paneIndex = i;
                        break;
                    }
                }

                if (paneIndex >= 0)
                {
                    var otherPanes = Panes.Values.Where(v => v.Url != pane.Url).OrderByDescending(v => v.StartTime).ToArray();
                    anotherPane = paneIndex > otherPanes.Length - 1 ? otherPanes.LastOrDefault() : otherPanes[paneIndex];
                }
            }

            return anotherPane;
        }

        void FixMenuInfo(List<MenuModel> menus)
        {
            foreach (var menu in menus)
            {
                menu.Name ??= menu.Route;
                menu.Title ??= menu.Label;
                FixMenuInfo(menu.Children);
            }
        }

        void OpenSubMenu(List<MenuModel> menus)
        {
            foreach (var menu in menus)
            {
                if (menu.IsOpen)
                {
                    menu.SubMenu?.Activate();
                }
                OpenSubMenu(menu.Children);
            }
        }

        void FindMenuTitle(List<MenuModel> menus, string path)
        {
            foreach (var menu in menus)
            {
                if (menu.Route == path)
                {
                    MenuTitle = menu.Title ?? menu.Label;
                    return;
                }
                FindMenuTitle(menu.Children, path);
            }
        }

        private void AddPane()
        {
            string currentUrl = NavigationManager.Uri;
            var path = new Uri(currentUrl).LocalPath;
            if(path != "/")
            {
                if (Panes.TryGetValue(currentUrl, out Pane curPane))
                {
                    curPane.ActiveTime = DateTime.Now;
                }
                else
                {
                    string title;
                    FindMenuTitle(Menus, path);
                    title = MenuTitle;
                    curPane = new Pane
                    {
                        Url = currentUrl,
                        Title = title,
                        Body = Body
                    };
                    Panes[currentUrl] = curPane;
                    if (PageType != null)
                    {
                        curPane.BuildCustomBodyRenderer(PageType, RouteValues);
                    }
                }
            }

            foreach (Pane pane in Panes.Values.ToArray())
            {
                if (pane.Url != currentUrl && pane.IsClosed)
                {
                    Panes.Remove(pane.Url);
                }
            }

            Tabs = new ObservableCollection<TabOption>();
            foreach (Pane pane in Panes.Values.OrderBy(a=>a.StartTime).ToArray())
            {
                Tabs.Add(new TabOption 
                { 
                    Name = pane.Url,
                    Title = pane.Title,
                    IsActive = currentUrl == pane.Url,
                    IsClosable = true
                });
            }

            if (Panes.Count == 0)
            {
                Menu?.DeActiveItem();
            }

            Tab?.MarkAsRequireRender();
            Tab?.Refresh();
        }

        private void OnTabClose(BTabPanel bTabPanel)
        {
            var pane = Panes.FirstOrDefault(a=>a.Value.Url == bTabPanel.Name).Value;
            if (NavigationManager.Uri == pane.Url)
            {
                var anotherItem = GetAnotherPane(pane);
                if (anotherItem != null)
                {
                    pane.IsClosed = true;
                    NavigationManager.NavigateTo(anotherItem.Url);
                }
                else
                {
                    pane.IsClosed = true;
                    NavigationManager.NavigateTo("/");
                }
            }
            else
            {
                Panes.Remove(pane.Url);
            }
        }

        protected void OnActiveTabChanged(BChangeEventArgs<BTabPanel> eventArgs)
        {
            if(eventArgs.NewValue != null && NavigationManager.Uri != eventArgs.NewValue.Name)
            {
                NavigationManager.NavigateTo(eventArgs.NewValue.Name);
            }
        }
    }
}