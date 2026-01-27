using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Geometry;
using Acadapp =  Autodesk.AutoCAD.ApplicationServices.Application;

namespace test.Actions
{
    internal class AddLo
    {
        public void execute()
        {
           Document doc = Acadapp.DocumentManager.MdiActiveDocument;
           Database db = doc.Database;
           Editor ed = doc.Editor;
           
           PromptPointOptions ppo = new PromptPointOptions("\nSpecify A point: ");
           PromptPointResult ppr = ed.GetPoint(ppo);
           PromptPointOptions ppo2 = new PromptPointOptions("\nSpecify B point: ");
           PromptPointResult ppr2 = ed.GetPoint(ppo2);


            Point3d A = ppr.Value;
            Point3d B = ppr2.Value;

            // Line tim lộ
            Line centerLine = new Line(A, B);

            // Vector chỉ phương
            Vector3d dir = B - A;

            // Vector vuông góc (trong mặt phẳng XY)
            Vector3d normal = new Vector3d(-dir.Y, dir.X, 0).GetNormal();

            // Khoảng cách offset
            double d = 3.0;

            // Vector offset
            Vector3d offsetVec = normal * d;

            // 2 line offset
            Line leftLine = new Line(A + offsetVec, B + offsetVec);
            Line rightLine = new Line(A - offsetVec, B - offsetVec);
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId,OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                btr.AppendEntity(centerLine);
                tr.AddNewlyCreatedDBObject(centerLine, true);
                btr.AppendEntity(leftLine);
                tr.AddNewlyCreatedDBObject(leftLine, true);
                btr.AppendEntity(rightLine);
                tr.AddNewlyCreatedDBObject(rightLine, true);

                tr.Commit();
            }

        }   
    
    }
}
