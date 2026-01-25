using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.View;
using test.ViewModel;
using Atacad = Autodesk.AutoCAD.ApplicationServices.Application;
namespace test.Actions
{
    public class ProjectSelect
    {
        public void Execute()
        {
           InitView();
        }

        public void InitView()
        {

            ProjectSelectViewModel vm = new ProjectSelectViewModel();

            ProjectForm pf = new ProjectForm();
            pf.DataContext = vm;
            vm.RequestClose += () =>
            {    

                vm.Dispose();
                pf.DataContext = null;
                pf.Close();


            };
            System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(pf);

            helper.Owner = Atacad.MainWindow.Handle;

            pf.ShowDialog();
        }
    }
}
