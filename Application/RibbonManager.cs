using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Windows;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Media.Imaging;
using test.Helper;
using test.Service;

namespace test.Application
{
    public class RibbonManager
    {
        public static string loginIcon = "test.Assets.login.png";
        public static string logoutIcon = "test.Assets.logout.png";

        private static RibbonButton _LoginInBtn;
        private static RibbonButton _LoginOutBtn;

        public static void InitializeRibbon()
        {
            ImageLoader imageLoader = new ImageLoader();
            RibbonControl ribbon = ComponentManager.Ribbon;

            if (ribbon == null) return;

            RibbonTab tab = new RibbonTab
            {
                Title = "Custom PLUGIN",
                Id = "MY_PLUGIN_TAB"
            };

            RibbonPanelSource panelSource = new RibbonPanelSource
            {
                Title = "Tools"
            };

            RibbonPanel panel = new RibbonPanel
            {
                Source = panelSource
            };

            _LoginInBtn = new RibbonButton
            {
                Text = "Đăng Nhập",
                
                CommandHandler = new RibbonCommandHandler("LOGIN"),
                IsEnabled = true
            };

            _LoginOutBtn = new RibbonButton
            {
                Text = "Đăng Xuất",
                CommandHandler = new RibbonCommandHandler("LOGOUT"),
                IsEnabled = false
            };

            panelSource.Items.Add(_LoginInBtn);
            panelSource.Items.Add(_LoginOutBtn);
            tab.Panels.Add(panel);
            ribbon.Tabs.Add(tab);
        }

        public static void UpdateRibbonState()
        {
            if (_LoginInBtn != null)
            {
                _LoginInBtn.IsEnabled = !authService.IsLoggedIn;
            }

            if (_LoginOutBtn != null)
            {
                _LoginOutBtn.IsEnabled = authService.IsLoggedIn;
            }
        }

    }
}