using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test.View;
using test.ViewModel;
using Atacad = Autodesk.AutoCAD.ApplicationServices.Application;
namespace test.Actions
{
    internal class Loginaction
    {
        public void Execute(){
            InitView();
        }

        private void InitView()
        {
      
            LoginViewModel vm = new LoginViewModel();

           
            LoginForm view = new LoginForm();
            view.DataContext = vm;
            vm.RequestClose += () =>
            {   
                vm.Dispose();
                view.DataContext = null;
                view.Close();
                
            };
            System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(view);

            helper.Owner = Atacad.MainWindow.Handle;

            view.ShowDialog();

        }
    }
}
