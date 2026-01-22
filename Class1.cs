using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test.View;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;


namespace test
{
    public class Class1
    {


        [CommandMethod("DRAWRECT")]
        public void draw2line ()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor editor = doc.Editor;

            PromptDoubleResult witdh = editor.GetDouble("Witdh :");

            if (witdh.Status != PromptStatus.OK) return;

            PromptDoubleResult height = editor.GetDouble("Height :");

            double wh = witdh.Value;
            double hh = height.Value;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt =
                   (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);

                BlockTableRecord btr =
                    (BlockTableRecord)trans.GetObject(
                        bt[BlockTableRecord.ModelSpace],
                        OpenMode.ForWrite);

                Polyline rect = new Polyline();
                rect.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
                rect.AddVertexAt(1, new Point2d(wh, 0), 0, 0, 0);
                rect.AddVertexAt(2, new Point2d(wh, hh), 0, 0, 0);
                rect.AddVertexAt(3, new Point2d(0, hh), 0, 0, 0);
                rect.Closed = true;

                btr.AppendEntity(rect);
                trans.AddNewlyCreatedDBObject(rect, true);

                trans.Commit();
            }
            double area = wh * hh;
            editor.WriteMessage($"\nRectangle area = {area}");
        }
        [CommandMethod("LOGIN")] 
        public void LOGIN()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor editor = doc.Editor;

            LoginForm lgform = new LoginForm();
            System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(lgform);

            helper.Owner = Application.MainWindow.Handle;

            lgform.ShowDialog();


        }
        

        
    } 
}
