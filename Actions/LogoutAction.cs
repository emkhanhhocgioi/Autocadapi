using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using test.Application;

namespace test.Actions
{
    internal class LogoutAction
    {
        public void Execute()
        {
            InitView();
        }

        private void InitView()
        {
            Service.authService.Logout();

            MessageBox.Show(
                "Bạn đã đăng xuất thành công!",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            RibbonManager.UpdateRibbonState();
        }

    }
}
