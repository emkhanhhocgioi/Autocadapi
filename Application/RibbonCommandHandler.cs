using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
namespace test.Application
{
    public class RibbonCommandHandler : ICommand
    {
        private readonly string _commandName;



        public RibbonCommandHandler(string _commandName)
        {
         this._commandName = _commandName;
        }
        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            doc.SendStringToExecute(
             _commandName + " ",
             true,
             false,
             false
       );
        }

    }
}
