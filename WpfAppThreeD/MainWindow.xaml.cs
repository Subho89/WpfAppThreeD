using HelixToolkit.Wpf;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WpfAppThreeD
{
    public partial class MainWindow : Window
    {
        private SphereVisual3D pointSphere;
        private LinesVisual3D lineToX, lineToY, lineToZ;
        private SphereVisual3D sliderThumbX, sliderThumbY, sliderThumbZ;

        // Drag state
        private bool isDragging = false;
        private string draggingAxis = null;

        public MainWindow()
        {
            InitializeComponent();
            DrawAxes();
            DrawAxisLabels();
            AddPoint(0, 0, 0);
            Loaded += (s, e) => DrawGrid(); // wait until controls are ready
        }

        private void DrawAxes()
        {
            var group = new Model3DGroup();

            double xmin = double.Parse(txtXMin.Text);
            double xmax = double.Parse(txtXMax.Text);
            double ymin = double.Parse(txtYMin.Text);
            double ymax = double.Parse(txtYMax.Text);
            double zmin = double.Parse(txtZMin.Text);
            double zmax = double.Parse(txtZMax.Text);

            group.Children.Add(new PipeVisual3D { Point1 = new Point3D(xmin, 0, 0), Point2 = new Point3D(xmax, 0, 0), Diameter = 0.05, Fill = Brushes.Red }.Content);
            group.Children.Add(new PipeVisual3D { Point1 = new Point3D(0, ymin, 0), Point2 = new Point3D(0, ymax, 0), Diameter = 0.05, Fill = Brushes.Green }.Content);
            group.Children.Add(new PipeVisual3D { Point1 = new Point3D(0, 0, zmin), Point2 = new Point3D(0, 0, zmax), Diameter = 0.05, Fill = Brushes.Blue }.Content);

            AxesVisual.Content = group;
        }

        private void DrawGrid()
        {
            BoundingBoxVisual.Children.Clear();

            int xMin = int.Parse(txtXMin.Text);
            int xMax = int.Parse(txtXMax.Text);
            int yMin = int.Parse(txtYMin.Text);
            int yMax = int.Parse(txtYMax.Text);
            int zMin = int.Parse(txtZMin.Text);
            int zMax = int.Parse(txtZMax.Text);

            var gridLines = new LinesVisual3D
            {
                Thickness = 0.5,
                Color = Colors.Gray
            };

            // XY plane
            for (int x = xMin; x <= xMax; x++)
            {
                gridLines.Points.Add(new Point3D(x, yMin, 0));
                gridLines.Points.Add(new Point3D(x, yMax, 0));
            }
            for (int y = yMin; y <= yMax; y++)
            {
                gridLines.Points.Add(new Point3D(xMin, y, 0));
                gridLines.Points.Add(new Point3D(xMax, y, 0));
            }

            // XZ plane
            for (int x = xMin; x <= xMax; x++)
            {
                gridLines.Points.Add(new Point3D(x, 0, zMin));
                gridLines.Points.Add(new Point3D(x, 0, zMax));
            }
            for (int z = zMin; z <= zMax; z++)
            {
                gridLines.Points.Add(new Point3D(xMin, 0, z));
                gridLines.Points.Add(new Point3D(xMax, 0, z));
            }

            // YZ plane
            for (int y = yMin; y <= yMax; y++)
            {
                gridLines.Points.Add(new Point3D(0, y, zMin));
                gridLines.Points.Add(new Point3D(0, y, zMax));
            }
            for (int z = zMin; z <= zMax; z++)
            {
                gridLines.Points.Add(new Point3D(0, yMin, z));
                gridLines.Points.Add(new Point3D(0, yMax, z));
            }

            BoundingBoxVisual.Children.Add(gridLines);
        }

        private void DrawAxisLabels()
        {
            AxisLabelsVisual.Children.Clear();

            double xmin = double.Parse(txtXMin.Text);
            double xmax = double.Parse(txtXMax.Text);
            double ymin = double.Parse(txtYMin.Text);
            double ymax = double.Parse(txtYMax.Text);
            double zmin = double.Parse(txtZMin.Text);
            double zmax = double.Parse(txtZMax.Text);

            AxisLabelsVisual.Children.Add(new BillboardTextVisual3D { Position = new Point3D(xmax + 0.5, 0, 0), Text = "X", Foreground = Brushes.Red });
            AxisLabelsVisual.Children.Add(new BillboardTextVisual3D { Position = new Point3D(0, ymax + 0.5, 0), Text = "Y", Foreground = Brushes.Green });
            AxisLabelsVisual.Children.Add(new BillboardTextVisual3D { Position = new Point3D(0, 0, zmax + 0.5), Text = "Z", Foreground = Brushes.Blue });

            for (int i = (int)xmin; i <= xmax; i++)
                AxisLabelsVisual.Children.Add(new BillboardTextVisual3D { Position = new Point3D(i, 0, 0), Text = i.ToString(), Foreground = Brushes.Red, FontSize = 10 });

            for (int i = (int)ymin; i <= ymax; i++)
                AxisLabelsVisual.Children.Add(new BillboardTextVisual3D { Position = new Point3D(0, i, 0), Text = i.ToString(), Foreground = Brushes.Green, FontSize = 10 });

            for (int i = (int)zmin; i <= zmax; i++)
                AxisLabelsVisual.Children.Add(new BillboardTextVisual3D { Position = new Point3D(0, 0, i), Text = i.ToString(), Foreground = Brushes.Blue, FontSize = 10 });
        }

        private void AddPoint(double x, double y, double z)
        {
            pointSphere = new SphereVisual3D { Center = new Point3D(x, y, z), Radius = 0.3, Fill = Brushes.Yellow };
            PointVisual.Children.Clear();
            PointVisual.Children.Add(pointSphere);

            lineToX = new LinesVisual3D { Color = Colors.Gray, Thickness = 1 };
            lineToY = new LinesVisual3D { Color = Colors.Gray, Thickness = 1 };
            lineToZ = new LinesVisual3D { Color = Colors.Gray, Thickness = 1 };
            PointVisual.Children.Add(lineToX);
            PointVisual.Children.Add(lineToY);
            PointVisual.Children.Add(lineToZ);

            sliderThumbX = new SphereVisual3D { Radius = 0.2, Fill = Brushes.Red };
            sliderThumbY = new SphereVisual3D { Radius = 0.2, Fill = Brushes.Green };
            sliderThumbZ = new SphereVisual3D { Radius = 0.2, Fill = Brushes.Blue };
            PointVisual.Children.Add(sliderThumbX);
            PointVisual.Children.Add(sliderThumbY);
            PointVisual.Children.Add(sliderThumbZ);

            UpdatePoint(x, y, z);
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtX.Text, out double x) &&
                double.TryParse(txtY.Text, out double y) &&
                double.TryParse(txtZ.Text, out double z))
            {
                x = Clamp(x, double.Parse(txtXMin.Text), double.Parse(txtXMax.Text));
                y = Clamp(y, double.Parse(txtYMin.Text), double.Parse(txtYMax.Text));
                z = Clamp(z, double.Parse(txtZMin.Text), double.Parse(txtZMax.Text));

                UpdatePoint(x, y, z);

                sliderX.Value = x;
                sliderY.Value = y;
                sliderZ.Value = z;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdatePoint(sliderX.Value, sliderY.Value, sliderZ.Value);

            txtX.Text = sliderX.Value.ToString("F2");
            txtY.Text = sliderY.Value.ToString("F2");
            txtZ.Text = sliderZ.Value.ToString("F2");
        }

        private void UpdatePoint(double x, double y, double z)
        {
            if (pointSphere != null)
                pointSphere.Center = new Point3D(x, y, z);

            lineToX.Points.Clear();
            lineToY.Points.Clear();
            lineToZ.Points.Clear();

            lineToX.Points.Add(new Point3D(0, y, z));
            lineToX.Points.Add(new Point3D(x, y, z));

            lineToY.Points.Add(new Point3D(x, 0, z));
            lineToY.Points.Add(new Point3D(x, y, z));

            lineToZ.Points.Add(new Point3D(x, y, 0));
            lineToZ.Points.Add(new Point3D(x, y, z));

            sliderThumbX.Center = new Point3D(x, 0, 0);
            sliderThumbY.Center = new Point3D(0, y, 0);
            sliderThumbZ.Center = new Point3D(0, 0, z);

            sliderX.Value = x;
            sliderY.Value = y;
            sliderZ.Value = z;

            txtX.Text = x.ToString("F2");
            txtY.Text = y.ToString("F2");
            txtZ.Text = z.ToString("F2");
        }

        private double Clamp(double val, double min, double max) =>
            Math.Min(Math.Max(val, min), max);

        // ---------------- DRAG HANDLING ----------------
        private void view1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(view1);

            var hitResult = VisualTreeHelper.HitTest(view1.Viewport, mousePos) as RayHitTestResult;
            if (hitResult is RayMeshGeometry3DHitTestResult meshHit)
            {
                var hitModel = meshHit.VisualHit as ModelVisual3D;
                if (hitModel?.Content == sliderThumbX.Content) { isDragging = true; draggingAxis = "X"; }
                else if (hitModel?.Content == sliderThumbY.Content) { isDragging = true; draggingAxis = "Y"; }
                else if (hitModel?.Content == sliderThumbZ.Content) { isDragging = true; draggingAxis = "Z"; }

                if (isDragging)
                    Mouse.Capture(view1);
            }
        }

        private void view1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging || draggingAxis == null) return;

            Point pos = e.GetPosition(view1);
            var ray = Viewport3DHelper.Point2DtoRay3D(view1.Viewport, pos);
            if (ray == null) return;

            if (draggingAxis == "X")
            {
                if (RayPlaneIntersection(ray, new Point3D(0, pointSphere.Center.Y, pointSphere.Center.Z), new Vector3D(0, 1, 0), out var hit))
                {
                    double value = Clamp(hit.X, double.Parse(txtXMin.Text), double.Parse(txtXMax.Text));
                    UpdatePoint(value, pointSphere.Center.Y, pointSphere.Center.Z);
                }
            }
            else if (draggingAxis == "Y")
            {
                if (RayPlaneIntersection(ray, new Point3D(pointSphere.Center.X, 0, pointSphere.Center.Z), new Vector3D(1, 0, 0), out var hit))
                {
                    double value = Clamp(hit.Y, double.Parse(txtYMin.Text), double.Parse(txtYMax.Text));
                    UpdatePoint(pointSphere.Center.X, value, pointSphere.Center.Z);
                }
            }
            else if (draggingAxis == "Z")
            {
                if (RayPlaneIntersection(ray, new Point3D(pointSphere.Center.X, pointSphere.Center.Y, 0), new Vector3D(0, 0, 1), out var hit))
                {
                    double value = Clamp(hit.Z, double.Parse(txtZMin.Text), double.Parse(txtZMax.Text));
                    UpdatePoint(pointSphere.Center.X, pointSphere.Center.Y, value);
                }
            }
        }

        private void view1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            draggingAxis = null;
            Mouse.Capture(null);
        }

        // ---------- Utility: Ray/Plane intersection ----------
        private static bool RayPlaneIntersection(Ray3D ray, Point3D planePoint, Vector3D planeNormal, out Point3D intersection)
        {
            intersection = new Point3D();
            double denom = Vector3D.DotProduct(planeNormal, ray.Direction);
            if (Math.Abs(denom) < 1e-6) return false; // Parallel

            Vector3D p0l0 = planePoint - ray.Origin;
            double t = Vector3D.DotProduct(p0l0, planeNormal) / denom;
            if (t < 0) return false; // Behind origin

            intersection = ray.Origin + t * ray.Direction;
            return true;
        }

       
    }
}
