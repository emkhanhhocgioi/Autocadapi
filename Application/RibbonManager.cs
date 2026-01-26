using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Windows;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using test.Helper;
using test.Service;

namespace test.Application
{
    public class RibbonManager
    {
        public static string loginIcon = @"C:\Users\hidra\source\repos\test\Assets\login.png";
        public static string logoutIcon = @"C:\Users\hidra\source\repos\test\Assets\logout.png";


        private static RibbonButton _LoginInBtn;
        private static RibbonButton _LoginOutBtn;
        private static RibbonButton _ProjectSelect;

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
                Title = "Hệ Thống"
            };
            RibbonPanelSource panelSource2 = new RibbonPanelSource
            {
                Title = "Dự Án"
            };
            RibbonPanel panel = new RibbonPanel
            {
                Source = panelSource

            };
            RibbonPanel panel2 = new RibbonPanel
            {
                Source = panelSource2
            };


            _LoginInBtn = new RibbonButton
            {
                Text = "Đăng Nhập",
                LargeImage = imageLoader.LoadFromFile(loginIcon),

                CommandHandler = new RibbonCommandHandler("LOGIN"),
                IsEnabled = true
            };

            _LoginOutBtn = new RibbonButton
            {
                Text = "Đăng Xuất",
                LargeImage = imageLoader.LoadFromFile(logoutIcon),
                CommandHandler = new RibbonCommandHandler("LOGOUT"),
                IsEnabled = false
            };
            _ProjectSelect = new RibbonButton
            {
                Text = "Chọn Dự Án",
                LargeImage = imageLoader.LoadFromFile(logoutIcon),
                CommandHandler = new RibbonCommandHandler("PROJECT_SELECT"),
                IsEnabled = true
            };


            panelSource.Items.Add(_LoginInBtn);
            panelSource.Items.Add(_LoginOutBtn);
            panelSource2.Items.Add(_ProjectSelect);
            tab.Panels.Add(panel);
            tab.Panels.Add(panel2);
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

            if (_ProjectSelect != null)
            {
                _ProjectSelect.IsEnabled = authService.IsLoggedIn;

            }

        }
    }
}