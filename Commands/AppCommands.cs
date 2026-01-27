using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;

using System.IO;

using System.Reflection;
using System.Windows;
using test.Actions;
using test.View;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace test.Commands
{
    public class AppCommands
    {

        [CommandMethod("LOGIN")]
        public void LOGIN()
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor editor = doc.Editor;

            Loginaction la = new Loginaction();
            la.Execute();

            editor.WriteMessage("\nLogin command executed.");



        }
        [CommandMethod("LOGOUT")]
        public void lOGOUT()
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor editor = doc.Editor;

            LogoutAction logout = new LogoutAction();
            logout.Execute();

            editor.WriteMessage("\nLogout command executed.");



        }
        [CommandMethod("PROJECT_SELECT")]
        public void PROJECTSELECT()
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor editor = doc.Editor;
            ProjectSelect ps = new ProjectSelect();
            ps.Execute();
            editor.WriteMessage("\nProject Select command executed.");
        }
        [CommandMethod("SHOWPALETTE")]
        public static void SHOWPALETTE()
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor editor = doc.Editor;
            OpenPalette opensp = new OpenPalette();
            PalleteControl pc = new PalleteControl();
            pc.DataContext = new ViewModel.PallateControlVM();
            opensp.Execute(pc);
            editor.WriteMessage("\nShow Palette command executed.");
        }
        [CommandMethod("ADDLO")]
        public void ADDLO()
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor editor = doc.Editor;
            AddLo addlo = new AddLo();
            addlo.execute();
            editor.WriteMessage("\nAddLo command executed.");

        }
        [CommandMethod("DBCONTROL")]
        public void DBCONTROL()
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor editor = doc.Editor;
            DynamicBlockControl dbcontrol = new DynamicBlockControl();
            dbcontrol.DataContext = new ViewModel.DBViewModel();
            DBPallete dbpallete = new DBPallete();
            dbpallete.execute(dbcontrol);
            editor.WriteMessage("\nDB Control command executed.");
        }
    }
}

