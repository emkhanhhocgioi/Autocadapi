using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;

using System.Collections.ObjectModel;

using System.Text;
using System.Windows;
using test.Model;
using Acadapp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace test.ViewModel
{
    internal class DBViewModel : ViewModelBase
    {
        public ObservableCollection<short> FlipStates { get; set; }
     = new ObservableCollection<short>
     {
        0,
        1
     };
        private ObjectId? _currentBlockId;

        public RelayCommand SetBlockPropCommand => new RelayCommand(execute =>
        {
            SetDynamicBlockValue();
        });
        public RelayCommand ChangeStretchCommand => new RelayCommand(execute =>
        {
            ChangeStretchValue();
        });

        public RelayCommand GetBlockPropCommand => new RelayCommand(execute =>
        {
            GetBlockProp();
        });
        public RelayCommand ApplyRotateCommand => new RelayCommand(execute =>
        {
            ChangeAngleValue();
        });
        public RelayCommand ApplyFlipCommand => new RelayCommand(execute =>
        {
            ChangeFlipState();
        });
        public RelayCommand MoveDoorCommand => new RelayCommand(execute =>
        {
            MoveDoor();
        });
        public DBViewModel()
        {
        }


        #region Propertiers


        public class DynamicBlockProperty
        {
            public string PropertyName { get; set; }
            public object Value { get; set; }
        }

        private const double AspectRatio = 1.04;
        private bool _isUpdating = false;

        private double _distance1;
        public double Distance1
        {
            get => _distance1;
            set
            {
                if (_distance1 != value)
                {
                    _distance1 = value;
                    OnPropertyChanged();

                    if (_isUpdating) return;

                    _isUpdating = true;
                    DistanceW = Math.Round(_distance1 * AspectRatio, 2);
                    _isUpdating = false;
                }
            }
        }

        private double _distanceW;
        public double DistanceW
        {
            get => _distanceW;
            set
            {
                if (_distanceW != value)
                {
                    _distanceW = value;
                    OnPropertyChanged();

                    if (_isUpdating) return;

                    _isUpdating = true;
                    Distance1 = Math.Round(_distanceW / AspectRatio, 2);
                    _isUpdating = false;
                }
            }
        }


        private double _angle2;

        public double Angle2
        {
            get { return _angle2; }
            set { _angle2 = value;
            OnPropertyChanged();
            }
        }

        private short flipstate1;

        public short Flipstate1
        {
            get { return flipstate1; }
            set { flipstate1 = value; OnPropertyChanged(); }
        }

        private short flipstate2;

        public short Flipstate2
        {
            get { return flipstate2; }
            set { flipstate2 = value; OnPropertyChanged(); }
        }

        private string flipDirection;

        public string FlipDirection
        {
            get { return flipDirection; }
            set { flipDirection = value; OnPropertyChanged(); }
        }





        #endregion

        private MoveDoorModel _currentDoor;

        public MoveDoorModel CurrentDoor
        {
            get { return _currentDoor; }
            set { _currentDoor = value;
            OnPropertyChanged();
           
            }
        }
        

        public ObservableCollection<DynamicBlockProperty> DynamicBlockProperties { get; set; }
            = new ObservableCollection<DynamicBlockProperty>();

     

   
      
        public void SetDynamicBlockValue()
        {
            try
            {
                Document doc = Acadapp.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;
                Editor ed = doc.Editor;

                // Kiểm tra xem đã có block được chọn trước đó chưa
                if (_currentBlockId == null || _currentBlockId.Value.IsNull)
                {
                    MessageBox.Show("Vui lòng chọn block trước (dùng Get Block Prop).", "Thông báo");
                    return;
                }

                using (DocumentLock dl = doc.LockDocument())
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockReference blref =
                        tr.GetObject(_currentBlockId.Value, OpenMode.ForWrite) as BlockReference;

                    if (blref == null)
                    {
                        MessageBox.Show("Không thể lấy BlockReference.", "Lỗi");
                        return;
                    }

                    if (!blref.IsDynamicBlock)
                    {
                        MessageBox.Show("Block được chọn không phải Dynamic Block.", "Lỗi");
                        return;
                    }

                    int updatedCount = 0;
                    StringBuilder updatedProps = new StringBuilder();
                    var props = blref.DynamicBlockReferencePropertyCollection;

                    foreach (DynamicBlockReferenceProperty prop in props)
                    {
                        if (prop.ReadOnly)
                            continue;

                        foreach (DynamicBlockProperty dbprop in DynamicBlockProperties)
                        {
                            if (prop.PropertyName == dbprop.PropertyName)
                            {
                                try
                                {
                                    prop.Value = dbprop.Value;
                                    updatedCount++;
                                    updatedProps.AppendLine($"- {prop.PropertyName}: {dbprop.Value}");
                                }
                                catch (Exception ex)
                                {
                                    ed.WriteMessage(
                                        $"\nKhông thể set '{prop.PropertyName}': {ex.Message}");
                                }
                                break;
                            }
                        }
                    }

                   
                    blref.RecordGraphicsModified(true);

               
                    tr.Commit();

                
                    ed.Regen();

                 

                    if (updatedCount > 0)
                    {
                        MessageBox.Show(
                            $"Đã cập nhật {updatedCount} thuộc tính:\n\n{updatedProps}",
                            "Thành công",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Không có property nào được cập nhật.",
                            "Thông báo",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"Lỗi:\n{e.Message}\n\n{e.StackTrace}",
                    "Exception",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public void GetBlockProp()
        {
            Document doc = Acadapp.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            // KHÔNG LockDocument khi GetEntity
            PromptEntityOptions peo = new PromptEntityOptions("\nSelect a block: ");
            peo.SetRejectMessage("\nOnly blocks allowed!");
            peo.AddAllowedClass(typeof(BlockReference), true);

            PromptEntityResult res = ed.GetEntity(peo);
            if (res.Status != PromptStatus.OK)
                return;

            _currentBlockId = res.ObjectId;

            using (DocumentLock dl = doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockReference blref =
                    tr.GetObject(res.ObjectId, OpenMode.ForRead) as BlockReference;

                if (blref != null && blref.IsDynamicBlock)
                {
                    DynamicBlockProperties.Clear();

                    
                    foreach (DynamicBlockReferenceProperty prop
                        in blref.DynamicBlockReferencePropertyCollection)
                    {
                       
                            DynamicBlockProperties.Add(new DynamicBlockProperty
                            {
                                PropertyName = prop.PropertyName,
                                Value = prop.Value
                            });
                            if(prop.PropertyName == "Distance1")
                            {
                                Distance1 = Convert.ToDouble(prop.Value);
                            }
                            if(prop.PropertyName == "W")
                            {
                                DistanceW = Convert.ToDouble(prop.Value);
                            }
                            if(prop.PropertyName == "Angle2")
                            {
                                Angle2 = Convert.ToDouble(prop.Value);
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


        public void ChangeStretchValue()
        {
            try
            {
                Document doc = Acadapp.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;
                Editor ed = doc.Editor;

                // Kiểm tra xem đã có block được chọn trước đó chưa
                if (_currentBlockId == null || _currentBlockId.Value.IsNull)
                {
                    MessageBox.Show("Vui lòng chọn block trước (dùng Get Block Prop).", "Thông báo");
                    return;
                }

                using (DocumentLock dl = doc.LockDocument())
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockReference blref =
                        tr.GetObject(_currentBlockId.Value, OpenMode.ForWrite) as BlockReference;

                    if (blref == null)
                    {
                        MessageBox.Show("Không thể lấy BlockReference.", "Lỗi");
                        return;
                    }

                    if (!blref.IsDynamicBlock)
                    {
                        MessageBox.Show("Block được chọn không phải Dynamic Block.", "Lỗi");
                        return;
                    }

                    int updatedCount = 0;
                    StringBuilder updatedProps = new StringBuilder();

                    foreach (DynamicBlockReferenceProperty prop in blref.DynamicBlockReferencePropertyCollection)
                    {
                        if (prop.ReadOnly)
                            continue;

                        try
                        {
                            if (prop.PropertyName == "Distance1")
                            {
                                prop.Value = Distance1;
                                updatedCount++;
                                updatedProps.AppendLine($"- {prop.PropertyName}: {Distance1}");
                            }
                            else if (prop.PropertyName == "W")
                            {
                                prop.Value = DistanceW;
                                updatedCount++;
                                updatedProps.AppendLine($"- {prop.PropertyName}: {DistanceW}");
                            }
                        }
                        catch (Exception ex)
                        {
                            ed.WriteMessage(
                                $"\nKhông thể set '{prop.PropertyName}': {ex.Message}");
                        }
                    }

                    // QUAN TRỌNG: Các bước để update hình
                    // 1. Đánh dấu block đã thay đổi
                    blref.RecordGraphicsModified(true);

                    // 2. Commit transaction trước
                    tr.Commit();

                    // 3. Sau khi commit, regen để cập nhật hiển thị
                    ed.Regen();

                    if (updatedCount > 0)
                    {
                        MessageBox.Show(
                            $"Đã cập nhật {updatedCount} thuộc tính:\n\n{updatedProps}",
                            "Thành công",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Không có property nào được cập nhật.",
                            "Thông báo",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"Lỗi:\n{e.Message}\n\n{e.StackTrace}",
                    "Exception",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
         }
        
        public void ChangeAngleValue()
        {
            try
            {
                Document doc = Acadapp.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;
                Editor ed = doc.Editor;

                // Kiểm tra xem đã có block được chọn trước đó chưa
                if (_currentBlockId == null || _currentBlockId.Value.IsNull)
                {
                    MessageBox.Show("Vui lòng chọn block trước (dùng Get Block Prop).", "Thông báo");
                    return;
                }

                using (DocumentLock dl = doc.LockDocument())
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockReference blref =
                        tr.GetObject(_currentBlockId.Value, OpenMode.ForWrite) as BlockReference;

                    if (blref == null)
                    {
                        MessageBox.Show("Không thể lấy BlockReference.", "Lỗi");
                        return;
                    }

                    if (!blref.IsDynamicBlock)
                    {
                        MessageBox.Show("Block được chọn không phải Dynamic Block.", "Lỗi");
                        return;
                    }

                    int updatedCount = 0;
                    StringBuilder updatedProps = new StringBuilder();

                    foreach (DynamicBlockReferenceProperty prop in blref.DynamicBlockReferencePropertyCollection)
                    {
                        if (prop.ReadOnly)
                            continue;

                        try
                        {   
                            if (prop.PropertyName == "Angle2")
                            {
                                prop.Value = DegreeToRadian(Angle2);
                                updatedCount++;
                                updatedProps.AppendLine($"- {prop.PropertyName}: {Angle2}");
                            }
                         
                        }
                        catch (Exception ex)
                        {
                            ed.WriteMessage(
                                $"\nKhông thể set '{prop.PropertyName}': {ex.Message}");
                        }
                    }

                    // QUAN TRỌNG: Các bước để update hình
                    // 1. Đánh dấu block đã thay đổi
                    blref.RecordGraphicsModified(true);

                    // 2. Commit transaction trước
                    tr.Commit();

                    // 3. Sau khi commit, regen để cập nhật hiển thị
                    ed.Regen();

                    if (updatedCount > 0)
                    {
                        MessageBox.Show(
                            $"Đã cập nhật {updatedCount} thuộc tính:\n\n{updatedProps}",
                            "Thành công",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Không có property nào được cập nhật.",
                            "Thông báo",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"Lỗi:\n{e.Message}\n\n{e.StackTrace}",
                    "Exception",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public void ChangeFlipState()
        {
            Document doc = Acadapp.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            if (_currentBlockId == null || _currentBlockId.Value.IsNull)
            {
                MessageBox.Show("Vui lòng chọn block trước (dùng Get Block Prop).", "Thông báo");
                return;
            }

            using (DocumentLock dl = doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockReference blref = tr.GetObject(_currentBlockId.Value, OpenMode.ForWrite) as BlockReference;

                if (blref == null)
                {
                    MessageBox.Show("Không thể lấy BlockReference.", "Lỗi");
                    return;
                }

                if (!blref.IsDynamicBlock)
                {
                    MessageBox.Show("Block được chọn không phải Dynamic Block.", "Lỗi");
                    return;
                }

              
                FlipCheck();

                int updatedCount = 0;
                StringBuilder updatedProps = new StringBuilder();

                foreach (DynamicBlockReferenceProperty prop in blref.DynamicBlockReferencePropertyCollection)
                {
                    if (prop.ReadOnly)
                        continue;

                    try
                    {
                        if (prop.PropertyName == "Flip state1")
                        {
                            prop.Value = Flipstate1;
                            updatedCount++;
                            updatedProps.AppendLine($"- {prop.PropertyName}: {Flipstate1}");
                           
                        }
                        else if (prop.PropertyName == "Flip state2")
                        {
                            prop.Value = Flipstate2;
                            updatedCount++;
                            updatedProps.AppendLine($"- {prop.PropertyName}: {Flipstate2}");
                          
                        }
                    }
                    catch (Exception ex)
                    {
                        ed.WriteMessage($"\nKhông thể set '{prop.PropertyName}': {ex.Message}");
                    }
                }

                blref.RecordGraphicsModified(true);
                tr.Commit();
                ed.Regen();

                if (updatedCount > 0)
                {
                    MessageBox.Show(
                        $"Đã cập nhật {updatedCount} thuộc tính:\n\n{updatedProps}",
                        "Thành công",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(
                        "Không có property nào được cập nhật.",
                        "Thông báo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
        }

        public void MoveDoor()
        {
            try
            {
                Document document = Acadapp.DocumentManager.MdiActiveDocument;
                Database db = document.Database;
                Editor ed = document.Editor;

                // Di chuyển GetPoint ra ngoài DocumentLock
                PromptPointOptions ppo = new PromptPointOptions("\nSpecify start point: ");
                PromptPointResult starp = ed.GetPoint(ppo);
                if (starp.Status != PromptStatus.OK) return;

                PromptPointOptions ppo2 = new PromptPointOptions("\nSpecify end point: ");
                PromptPointResult endp = ed.GetPoint(ppo2);
                if (endp.Status != PromptStatus.OK) return;

                // Di chuyển GetEntity ra ngoài DocumentLock và Transaction
                PromptEntityOptions pet = new PromptEntityOptions("\nChoose a door to clone: ");
                PromptEntityResult per = ed.GetEntity(pet);
                if (per.Status != PromptStatus.OK) return;

                using (DocumentLock dl = document.LockDocument())
                {
                    double widthBetween = endp.Value.DistanceTo(starp.Value);

                    CurrentDoor = new MoveDoorModel
                    {
                        StartPoint = starp.Value,
                        EndPoint = endp.Value,
                        WitdhBetween = widthBetween,
                        Flipstate1 = Flipstate1,
                        Flipstate2 = Flipstate2,
                    };

                    ed.WriteMessage($"{starp.Value} and {endp.Value}");

                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                        BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                        BlockReference originalBr = tr.GetObject(per.ObjectId, OpenMode.ForRead) as BlockReference;

                        if (originalBr != null)
                        {
                            ObjectId blockId = originalBr.IsDynamicBlock ? originalBr.DynamicBlockTableRecord : originalBr.BlockTableRecord;
                            BlockReference newBr = new BlockReference(originalBr.Position, blockId);

                            newBr.Layer = originalBr.Layer;
                            newBr.Rotation = originalBr.Rotation;
                            newBr.ScaleFactors = originalBr.ScaleFactors;

                            btr.AppendEntity(newBr);
                            tr.AddNewlyCreatedDBObject(newBr, true);

                            double blaspectRatio = 0;
                            double originalW = 0;
                            double originalDistance2 = 0;

                            Vector3d v = CurrentDoor.StartPoint - CurrentDoor.EndPoint;
                            double angle = v.AngleOnPlane(new Plane(Point3d.Origin, Vector3d.ZAxis));
                            double deg = angle * 180 / Math.PI;
                            ed.WriteMessage($"Angle : {deg}");

                            if (originalBr.IsDynamicBlock && newBr.IsDynamicBlock)
                            {
                                DynamicBlockReferencePropertyCollection originalProps = originalBr.DynamicBlockReferencePropertyCollection;

                                // THÊM dòng này để upgrade newBr sang ForWrite
                                newBr.UpgradeOpen();

                                DynamicBlockReferencePropertyCollection newProps = newBr.DynamicBlockReferencePropertyCollection;

                                foreach (DynamicBlockReferenceProperty originalProp in originalProps)
                                {
                                    if (originalProp.PropertyName == "W")
                                        originalW = Convert.ToDouble(originalProp.Value);

                                    if (originalProp.PropertyName == "Distance2")
                                        originalDistance2 = Convert.ToDouble(originalProp.Value);
                                }

                                if (originalDistance2 != 0)
                                {
                                    blaspectRatio = originalW / originalDistance2;
                                }

                                foreach (DynamicBlockReferenceProperty originalProp in originalProps)
                                {
                                    foreach (DynamicBlockReferenceProperty np in newProps)
                                    {
                                        if (np.PropertyName == originalProp.PropertyName && !np.ReadOnly)
                                        {
                                            if (np.PropertyName == "Angle2")
                                            {
                                                np.Value = (deg == 90) ? DegreeToRadian(90) : DegreeToRadian(0);
                                            }
                                            else if (np.PropertyName == "Flip state1")
                                            {
                                                np.Value = (short) CurrentDoor.Flipstate1;
                                            }
                                            else if (np.PropertyName == "Flip state2")
                                            {
                                                np.Value = (short) CurrentDoor.Flipstate2;
                                            }
                                            else if (np.PropertyName == "W")
                                            {
                                                np.Value = CurrentDoor.WitdhBetween;
                                            }
                                            else if (np.PropertyName == "Distance2")
                                            {
                                                np.Value = (blaspectRatio != 0) ? CurrentDoor.WitdhBetween / blaspectRatio : originalProp.Value;
                                            }
                                            else
                                            {
                                                np.Value = originalProp.Value;
                                            }
                                        }
                                    }
                                }
                            }

                            Vector3d moveVector = originalBr.Position.GetVectorTo(CurrentDoor.StartPoint);
                            newBr.TransformBy(Matrix3d.Displacement(moveVector));

                            ed.WriteMessage($"\nNew door created at {CurrentDoor.StartPoint}");
                            newBr.RecordGraphicsModified(true);
                        }

                        tr.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"Lỗi:\n{e.Message}\n\n{e.StackTrace}",
                    "Exception",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        #region HelperFunc
        public static double DegreeToRadian(double deg)
        {
            return deg * Math.PI / 180.0;
        }

        public  void  FlipCheck()
        {
            if (FlipDirection== "Horizontal")
            {
                Flipstate1 = 0;
                Flipstate2 = 1;
            }
            else if ((FlipDirection == "Vertical"))
            {
                Flipstate1 = 1;
                Flipstate2 = 0;
            }else if (FlipDirection == "Default"){
                Flipstate1 = 0;
                Flipstate2 = 0;
            }
        }
        #endregion 
    }

}
