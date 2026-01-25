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
    }
}
