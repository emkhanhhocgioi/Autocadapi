using Autodesk.AutoCAD.Geometry;

namespace test.Model
{
    public class MoveDoorModel
    {
        public Point3d StartPoint { get; set; }
        public Point3d EndPoint { get; set; }

        public double WitdhBetween { get; set; }
        public double AngleRotate { get; set; }

        public double Flipstate1 { get; set; }
        public double Flipstate2 { get; set; }

        public string LineText
        {
            get => $"({StartPoint.X:F2}, {StartPoint.Y:F2})  →  ({EndPoint.X:F2}, {EndPoint.Y:F2})";
        }

        public MoveDoorModel()
        {
            StartPoint = Point3d.Origin;
            EndPoint = Point3d.Origin;
        }
    }
}
