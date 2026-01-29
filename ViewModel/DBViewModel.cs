using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using Acadapp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace test.ViewModel
{
    internal class DBViewModel : ViewModelBase
    {
        
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

                    // QUAN TRỌNG: Các bước để update hình
                    // 1. Đánh dấu block đã thay đổi
                    blref.RecordGraphicsModified(true);

                    // 2. Commit transaction trước
                    tr.Commit();

                    // 3. Sau khi commit, regen để cập nhật hiển thị
                    ed.Regen();

                    // Hoặc có thể dùng UpdateDisplay nếu regen không hiệu quả
                    // doc.Editor.UpdateScreen();

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

                // GỌI FlipCheck() MỘT LẦN Ở ĐÂY
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
