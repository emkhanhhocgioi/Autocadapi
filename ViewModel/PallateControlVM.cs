using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using test.Model;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
namespace test.ViewModel
{
    internal class PallateControlVM : ViewModelBase
    {
        

        public RelayCommand DrawLineCM => new RelayCommand(execute =>DrawLine());
        public RelayCommand DrawCircle => new RelayCommand(execute =>DrawCircleCmd());

        public RelayCommand DrawRectOnMouse => new RelayCommand(execute => DrawRectangle());
        public RelayCommand ResetWH => new RelayCommand(execute => {
            RectW = 0;
            RectH = 0;
        });

        public PallateControlVM()
        {
            
        }
        private int rectW;

        public int RectW
        {
            get { return rectW; }
            set { rectW = value;
            OnPropertyChanged();
            }
        }

        private int rectH;

        public int RectH
        {
            get { return rectH; }
            set
            {
                rectH = value;
                OnPropertyChanged();
            }
        }

        public void DrawRectangle()
        {
           if(RectW <=0 || RectH <=0 )
            {
                MessageBox.Show("Width and Height must be greater than zero.");
                return;
            }
            else
            {
                Document doc = AcadApp.DocumentManager.MdiActiveDocument;


                using (DocumentLock dl = doc.LockDocument())
                {
                    Database db = doc.Database;
                    Editor editor = doc.Editor;

                    PromptPointOptions ppo = new PromptPointOptions("\nSpecify lower-left corner point: ");
                    PromptPointResult ppr = editor.GetPoint(ppo);
                    if (ppr.Status != PromptStatus.OK) return;
                    Autodesk.AutoCAD.Geometry.Point3d lowerLeftPt = ppr.Value;

                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                        Polyline rectangle = new Polyline();
                        rectangle.AddVertexAt(0, new Autodesk.AutoCAD.Geometry.Point2d(lowerLeftPt.X, lowerLeftPt.Y), 0, 0, 0);
                        rectangle.AddVertexAt(1, new Autodesk.AutoCAD.Geometry.Point2d(lowerLeftPt.X + RectW, lowerLeftPt.Y), 0, 0, 0);
                        rectangle.AddVertexAt(2, new Autodesk.AutoCAD.Geometry.Point2d(lowerLeftPt.X + RectW, lowerLeftPt.Y + RectH), 0, 0, 0);
                        rectangle.AddVertexAt(3, new Autodesk.AutoCAD.Geometry.Point2d(lowerLeftPt.X, lowerLeftPt.Y + RectH), 0, 0, 0);
                        rectangle.Closed = true;
                        btr.AppendEntity(rectangle);
                        tr.AddNewlyCreatedDBObject(rectangle, true);
                        tr.Commit();
                    }


                }
            }
               

        }   
        public void DrawCircleCmd()
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            using (DocumentLock docLock = doc.LockDocument())
            {
                Editor editor = doc.Editor;
                Database db = doc.Database;
                
                PromptPointOptions ppo = new PromptPointOptions("\nSpecify center point: ");

                PromptPointResult ppr1 = editor.GetPoint(ppo);
                if (ppr1.Status != PromptStatus.OK) return;

                Autodesk.AutoCAD.Geometry.Point3d centerPt = ppr1.Value;


                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                   
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                   
                    Circle circle = new Circle
                    {
                        Center = centerPt,
                        Radius = 5.0, 
                        Normal = Autodesk.AutoCAD.Geometry.Vector3d.ZAxis
                    };
                    
                    btr.AppendEntity(circle);
                    
                    tr.AddNewlyCreatedDBObject(circle, true);
                    tr.Commit();
                }
            }
        }
        public void DrawLine()
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
          
            using (DocumentLock docLock = doc.LockDocument())
            {
                Editor editor = doc.Editor;
                Database db = doc.Database;

                PromptPointOptions ppo = new PromptPointOptions("\nSpecify first point: ");
                PromptPointResult ppr1 = editor.GetPoint(ppo);
                if (ppr1.Status != PromptStatus.OK) return;

                Autodesk.AutoCAD.Geometry.Point3d pt1 = ppr1.Value;

                ppo.Message = "\nSpecify second point: ";
                ppo.UseBasePoint = true;
                ppo.BasePoint = pt1;

                PromptPointResult ppr2 = editor.GetPoint(ppo);
                if (ppr2.Status != PromptStatus.OK) return;

                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);

                    // Get the Model Space block table record
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    // Create the line
                    Line line = new Line(pt1, ppr2.Value);

                    // Add the line to Model Space
                    btr.AppendEntity(line);

                    // Add the line to the transaction
                    trans.AddNewlyCreatedDBObject(line, true);

                    trans.Commit();
                }
            }

        }
    }
}
