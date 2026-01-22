using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Model;

namespace test.ViewModel
{
    internal class LoginViewModel : ViewModelBase
    {
        public User tesuser { get; set; }

        public RelayCommand LoginCommand => new RelayCommand(execute => Login());
        public RelayCommand ExitCommand => new RelayCommand(execute => Exit());


        public LoginViewModel()
        {
            tesuser = new User();
        }

        public void Login()
        {

        }

        public void Exit() { }
    }
}
