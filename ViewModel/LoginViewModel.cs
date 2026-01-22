using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using test.Application;
using test.Model;
using test.Service;

namespace test.ViewModel
{
    internal class LoginViewModel : ViewModelBase
    {
        public event Action RequestClose;

        public RelayCommand LoginCommand => new RelayCommand(execute => Login());
        public RelayCommand ExitCommand => new RelayCommand(execute => Exit());


        public LoginViewModel()
        {
           
        }
        private string account;

        public string Account
        {
            get { return account; }
            set { account = value; }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public void Login()
        {   

            authService authService = new authService();
            bool log = authService.AuthLogin(Account,Password);

            if (log)
            {
                MessageBox.Show("Login successful");
                RibbonManager.UpdateRibbonState();
                RequestClose?.Invoke();
            }
            else
            {
                MessageBox.Show("Login failed");
            }


        }
        public void Dispose()
        {
           Password = null;
           Account = null;

        }
        public void Exit() {
            RequestClose?.Invoke();
        }
    }
}
