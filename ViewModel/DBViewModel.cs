using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acadapp = Autodesk.AutoCAD.ApplicationServices.Application;
namespace test.ViewModel
{
    internal class DBViewModel : ViewModelBase
    { 
        
        public RelayCommand ChangeDBstretch => new RelayCommand(execute =>{
            ChangeStretchValue();
        });
        public RelayCommand GetBlockPropCommand => new RelayCommand(execute => {
            getBlockProp();
        });
        public DBViewModel()
        {
        }
        private double distance;

        public double Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        public void ChangeStretchValue()
        {
            Document doc = Acadapp.DocumentManager.MdiActiveDocument;

            try
            {
                using (DocumentLock dl = doc.LockDocument())
                {
                    Database db = doc.Database;
                    Editor ed = doc.Editor;

                    PromptEntityOptions peo = new PromptEntityOptions("\nSelect a block: ");
                    peo.SetRejectMessage("\nOnly blocks allowed!");
                    peo.AddAllowedClass(typeof(BlockReference), true);

                    PromptEntityResult per = ed.GetEntity(peo);

                    if (per.Status != PromptStatus.OK)
                        return;

                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        BlockReference blref = tr.GetObject(per.ObjectId, OpenMode.ForRead) as BlockReference;

                        if (blref != null && blref.IsDynamicBlock)
                        {
                            blref.UpgradeOpen();

                            foreach (DynamicBlockReferenceProperty prop in blref.DynamicBlockReferencePropertyCollection)
                            {
                                if (prop.PropertyName == "Distance1")
                                {
                                    prop.Value = Distance; // nhớ đảm bảo Distance đúng kiểu (double)
                                }
                            }
                        }
                        else
                        {
                            ed.WriteMessage("\nSelected block is not a dynamic block.");
                        }

                        tr.Commit();
                    }
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                Acadapp.ShowAlertDialog("AutoCAD error: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                Acadapp.ShowAlertDialog("System error: " + ex.Message);
            }
        }

        public void getBlockProp()
        {
            Document doc = Acadapp.DocumentManager.MdiActiveDocument;

            using (DocumentLock dl = doc.LockDocument())
            {
                Database db = doc.Database;
                Editor ed = doc.Editor;
                PromptEntityOptions peo = new PromptEntityOptions("\nSelect a block: ");
                peo.SetRejectMessage("\nOnly blocks allowed!");
                peo.AddAllowedClass(typeof(BlockReference), true);
                PromptEntityResult per = ed.GetEntity(peo);
                if (per.Status != PromptStatus.OK)
                    return;
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockReference blref = tr.GetObject(per.ObjectId, OpenMode.ForRead) as BlockReference;
                    if (blref != null && blref.IsDynamicBlock)
                    {
                        foreach (DynamicBlockReferenceProperty prop in blref.DynamicBlockReferencePropertyCollection)
                        {
                            ed.WriteMessage($"\nProperty Name: {prop.PropertyName}, Value: {prop.Value}");
                        }
                    }
                    else
                    {
                        ed.WriteMessage("\nSelected block is not a dynamic block.");
                    }
                    tr.Commit();
                }
            }

        }
    }
}
