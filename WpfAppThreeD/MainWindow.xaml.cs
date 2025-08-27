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
        private ModelVisual3D pointVisual;
        private SphereVisual3D pointSphere;

        private bool isDragging = false;
        private Point lastMousePos;

        private double minX = -10, maxX = 10;
        private double minY = -10, maxY = 10;
        private double minZ = -10, maxZ = 10;

        public MainWindow()
        {
            InitializeComponent();
            DrawGrid();
            AddPoint(0, 0, 0);
        }

        //private void DrawGrid()
        //{
        //    view1.Children.Clear();
        //    view1.Children.Add(new DefaultLights());

        //    // ---------------- GRID PLANES ----------------
        //    var gridXY = new LinesVisual3D { Color = Colors.LightGray, Thickness = 0.5 };
        //    for (int y = (int)minY; y <= (int)maxY; y++)
        //    {
        //        gridXY.Points.Add(new Point3D(minX, y, 0));
        //        gridXY.Points.Add(new Point3D(maxX, y, 0));
        //    }
        //    for (int x = (int)minX; x <= (int)maxX; x++)
        //    {
        //        gridXY.Points.Add(new Point3D(x, minY, 0));
        //        gridXY.Points.Add(new Point3D(x, maxY, 0));
        //    }
        //    view1.Children.Add(gridXY);

        //    var gridXZ = new LinesVisual3D { Color = Colors.LightGray, Thickness = 0.5 };
        //    for (int z = (int)minZ; z <= (int)maxZ; z++)
        //    {
        //        gridXZ.Points.Add(new Point3D(minX, 0, z));
        //        gridXZ.Points.Add(new Point3D(maxX, 0, z));
        //    }
        //    for (int x = (int)minX; x <= (int)maxX; x++)
        //    {
        //        gridXZ.Points.Add(new Point3D(x, 0, minZ));
        //        gridXZ.Points.Add(new Point3D(x, 0, maxZ));
        //    }
        //    view1.Children.Add(gridXZ);

        //    var gridYZ = new LinesVisual3D { Color = Colors.LightGray, Thickness = 0.5 };
        //    for (int z = (int)minZ; z <= (int)maxZ; z++)
        //    {
        //        gridYZ.Points.Add(new Point3D(0, minY, z));
        //        gridYZ.Points.Add(new Point3D(0, maxY, z));
        //    }
        //    for (int y = (int)minY; y <= (int)maxY; y++)
        //    {
        //        gridYZ.Points.Add(new Point3D(0, y, minZ));
        //        gridYZ.Points.Add(new Point3D(0, y, maxZ));
        //    }
        //    view1.Children.Add(gridYZ);

        //    // ---------------- AXES (separate for each color) ----------------
        //    var axisX = new LinesVisual3D { Color = Colors.Red, Thickness = 2 };
        //    axisX.Points.Add(new Point3D(minX, 0, 0));
        //    axisX.Points.Add(new Point3D(maxX, 0, 0));
        //    view1.Children.Add(axisX);

        //    var axisY = new LinesVisual3D { Color = Colors.Green, Thickness = 2 };
        //    axisY.Points.Add(new Point3D(0, minY, 0));
        //    axisY.Points.Add(new Point3D(0, maxY, 0));
        //    view1.Children.Add(axisY);

        //    var axisZ = new LinesVisual3D { Color = Colors.Blue, Thickness = 2 };
        //    axisZ.Points.Add(new Point3D(0, 0, minZ));
        //    axisZ.Points.Add(new Point3D(0, 0, maxZ));
        //    view1.Children.Add(axisZ);

        //    // ---------------- LABELS ----------------
        //    view1.Children.Add(new BillboardTextVisual3D { Text = "X", Position = new Point3D(maxX + 0.5, 0, 0), Foreground = Brushes.Red });
        //    view1.Children.Add(new BillboardTextVisual3D { Text = "Y", Position = new Point3D(0, maxY + 0.5, 0), Foreground = Brushes.Green });
        //    view1.Children.Add(new BillboardTextVisual3D { Text = "Z", Position = new Point3D(0, 0, maxZ + 0.5), Foreground = Brushes.Blue });

        //    // ---------------- RULER TICKS ----------------
        //    for (double x = minX; x <= maxX; x++)
        //    {
        //        view1.Children.Add(new LinesVisual3D { Color = Colors.Red, Points = new Point3DCollection { new Point3D(x, -0.1, 0), new Point3D(x, 0.1, 0) } });
        //        view1.Children.Add(new BillboardTextVisual3D { Text = x.ToString(), Position = new Point3D(x, -0.3, 0), Foreground = Brushes.Red });
        //    }
        //    for (double y = minY; y <= maxY; y++)
        //    {
        //        view1.Children.Add(new LinesVisual3D { Color = Colors.Green, Points = new Point3DCollection { new Point3D(-0.1, y, 0), new Point3D(0.1, y, 0) } });
        //        view1.Children.Add(new BillboardTextVisual3D { Text = y.ToString(), Position = new Point3D(-0.3, y, 0), Foreground = Brushes.Green });
        //    }
        //    for (double z = minZ; z <= maxZ; z++)
        //    {
        //        view1.Children.Add(new LinesVisual3D { Color = Colors.Blue, Points = new Point3DCollection { new Point3D(0, -0.1, z), new Point3D(0, 0.1, z) } });
        //        view1.Children.Add(new BillboardTextVisual3D { Text = z.ToString(), Position = new Point3D(0, -0.3, z), Foreground = Brushes.Blue });
        //    }

        //    // ---------------- Restore Sphere & Guides ----------------
        //    if (pointSphere != null)
        //    {
        //        if (view1.Children.Contains(pointSphere))
        //            view1.Children.Remove(pointSphere);

        //        view1.Children.Add(pointSphere);

        //        Point3D p = pointSphere.Transform.Value.Transform(new Point3D(0, 0, 0));

        //        // --- Projection guides (yellow box) ---
        //        // Project to XY plane
        //        Point3D pXY = new Point3D(p.X, p.Y, 0);
        //        view1.Children.Add(new LinesVisual3D
        //        {
        //            Color = Colors.Yellow,
        //            Thickness = 1,
        //            Points = new Point3DCollection { p, pXY }
        //        });

        //        // Project to XZ plane
        //        Point3D pXZ = new Point3D(p.X, 0, p.Z);
        //        view1.Children.Add(new LinesVisual3D
        //        {
        //            Color = Colors.Yellow,
        //            Thickness = 1,
        //            Points = new Point3DCollection { p, pXZ }
        //        });

        //        // Project to YZ plane
        //        Point3D pYZ = new Point3D(0, p.Y, p.Z);
        //        view1.Children.Add(new LinesVisual3D
        //        {
        //            Color = Colors.Yellow,
        //            Thickness = 1,
        //            Points = new Point3DCollection { p, pYZ }
        //        });

        //        // --- Drop from planes to axes ---
        //        view1.Children.Add(new LinesVisual3D
        //        {
        //            Color = Colors.Yellow,
        //            Thickness = 1,
        //            Points = new Point3DCollection { pXY, new Point3D(p.X, 0, 0) }
        //        });
        //        view1.Children.Add(new LinesVisual3D
        //        {
        //            Color = Colors.Yellow,
        //            Thickness = 1,
        //            Points = new Point3DCollection { pXZ, new Point3D(p.X, 0, 0) }
        //        });
        //        view1.Children.Add(new LinesVisual3D
        //        {
        //            Color = Colors.Yellow,
        //            Thickness = 1,
        //            Points = new Point3DCollection { pYZ, new Point3D(0, p.Y, 0) }
        //        });

        //        // Final connection to Z axis origin
        //        view1.Children.Add(new LinesVisual3D
        //        {
        //            Color = Colors.Yellow,
        //            Thickness = 1,
        //            Points = new Point3DCollection { new Point3D(p.X, 0, 0), new Point3D(0, 0, 0) }
        //        });
        //        view1.Children.Add(new LinesVisual3D
        //        {
        //            Color = Colors.Yellow,
        //            Thickness = 1,
        //            Points = new Point3DCollection { new Point3D(0, p.Y, 0), new Point3D(0, 0, 0) }
        //        });
        //        view1.Children.Add(new LinesVisual3D
        //        {
        //            Color = Colors.Yellow,
        //            Thickness = 1,
        //            Points = new Point3DCollection { new Point3D(0, 0, p.Z), new Point3D(0, 0, 0) }
        //        });
        //    }
        //}


        //private void DrawGrid()
        //{
        //    view1.Children.Clear();
        //    view1.Children.Add(new DefaultLights());

        //     double.TryParse(txtStep.Text,out double step); // <-- set from txtStep

        //    // ---------------- GRID PLANES ----------------
        //    var gridXY = new LinesVisual3D { Color = Colors.LightGray, Thickness = 0.5 };
        //    for (double y = minY; y <= maxY; y += step)
        //    {
        //        gridXY.Points.Add(new Point3D(minX, y, 0));
        //        gridXY.Points.Add(new Point3D(maxX, y, 0));
        //    }
        //    for (double x = minX; x <= maxX; x += step)
        //    {
        //        gridXY.Points.Add(new Point3D(x, minY, 0));
        //        gridXY.Points.Add(new Point3D(x, maxY, 0));
        //    }
        //    view1.Children.Add(gridXY);

        //    var gridXZ = new LinesVisual3D { Color = Colors.LightGray, Thickness = 0.5 };
        //    for (double z = minZ; z <= maxZ; z += step)
        //    {
        //        gridXZ.Points.Add(new Point3D(minX, 0, z));
        //        gridXZ.Points.Add(new Point3D(maxX, 0, z));
        //    }
        //    for (double x = minX; x <= maxX; x += step)
        //    {
        //        gridXZ.Points.Add(new Point3D(x, 0, minZ));
        //        gridXZ.Points.Add(new Point3D(x, 0, maxZ));
        //    }
        //    view1.Children.Add(gridXZ);

        //    var gridYZ = new LinesVisual3D { Color = Colors.LightGray, Thickness = 0.5 };
        //    for (double z = minZ; z <= maxZ; z += step)
        //    {
        //        gridYZ.Points.Add(new Point3D(0, minY, z));
        //        gridYZ.Points.Add(new Point3D(0, maxY, z));
        //    }
        //    for (double y = minY; y <= maxY; y += step)
        //    {
        //        gridYZ.Points.Add(new Point3D(0, y, minZ));
        //        gridYZ.Points.Add(new Point3D(0, y, maxZ));
        //    }
        //    view1.Children.Add(gridYZ);

        //    // ---------------- AXES ----------------
        //    view1.Children.Add(new LinesVisual3D { Color = Colors.Red, Thickness = 2, Points = new Point3DCollection { new Point3D(minX, 0, 0), new Point3D(maxX, 0, 0) } });
        //    view1.Children.Add(new LinesVisual3D { Color = Colors.Green, Thickness = 2, Points = new Point3DCollection { new Point3D(0, minY, 0), new Point3D(0, maxY, 0) } });
        //    view1.Children.Add(new LinesVisual3D { Color = Colors.Blue, Thickness = 2, Points = new Point3DCollection { new Point3D(0, 0, minZ), new Point3D(0, 0, maxZ) } });

        //    // ---------------- AXIS LABELS ----------------
        //    view1.Children.Add(new BillboardTextVisual3D { Text = "X", Position = new Point3D(maxX + 0.5, 0, 0), Foreground = Brushes.Red });
        //    view1.Children.Add(new BillboardTextVisual3D { Text = "Y", Position = new Point3D(0, maxY + 0.5, 0), Foreground = Brushes.Green });
        //    view1.Children.Add(new BillboardTextVisual3D { Text = "Z", Position = new Point3D(0, 0, maxZ + 0.5), Foreground = Brushes.Blue });

        //    // ---------------- RULER TICKS ----------------
        //    for (double x = minX; x <= maxX; x += step)
        //    {
        //        view1.Children.Add(new LinesVisual3D { Color = Colors.Red, Points = new Point3DCollection { new Point3D(x, -0.1, 0), new Point3D(x, 0.1, 0) } });
        //        view1.Children.Add(new BillboardTextVisual3D { Text = x.ToString("0.###"), Position = new Point3D(x, -0.3, 0), Foreground = Brushes.Red });
        //    }
        //    for (double y = minY; y <= maxY; y += step)
        //    {
        //        view1.Children.Add(new LinesVisual3D { Color = Colors.Green, Points = new Point3DCollection { new Point3D(-0.1, y, 0), new Point3D(0.1, y, 0) } });
        //        view1.Children.Add(new BillboardTextVisual3D { Text = y.ToString("0.###"), Position = new Point3D(-0.3, y, 0), Foreground = Brushes.Green });
        //    }
        //    for (double z = minZ; z <= maxZ; z += step)
        //    {
        //        view1.Children.Add(new LinesVisual3D { Color = Colors.Blue, Points = new Point3DCollection { new Point3D(0, -0.1, z), new Point3D(0, 0.1, z) } });
        //        view1.Children.Add(new BillboardTextVisual3D { Text = z.ToString("0.###"), Position = new Point3D(0, -0.3, z), Foreground = Brushes.Blue });
        //    }

        //    // ---------------- Sphere & Yellow Guides ----------------
        //    if (pointSphere != null)
        //    {
        //        if (view1.Children.Contains(pointSphere))
        //            view1.Children.Remove(pointSphere);

        //        view1.Children.Add(pointSphere);

        //        Point3D p = pointSphere.Center; // ✅ correct way

        //        // Yellow guide lines to each axis
        //        view1.Children.Add(new LinesVisual3D { Color = Colors.Yellow, Points = new Point3DCollection { p, new Point3D(p.X, 0, 0) } });
        //        view1.Children.Add(new LinesVisual3D { Color = Colors.Yellow, Points = new Point3DCollection { p, new Point3D(0, p.Y, 0) } });
        //        view1.Children.Add(new LinesVisual3D { Color = Colors.Yellow, Points = new Point3DCollection { p, new Point3D(0, 0, p.Z) } });
        //    }
        //}

        private void DrawGrid()
        {
            view1.Children.Clear();
            view1.Children.Add(new DefaultLights());

            double.TryParse(txtStep.Text, out double step); // <-- set from txtStep

            // ---------------- GRID PLANES ----------------
            var gridXY = new LinesVisual3D { Color = Colors.LightGray, Thickness = 0.5 };
            for (double y = minY; y <= maxY; y += step)
            {
                gridXY.Points.Add(new Point3D(minX, y, 0));
                gridXY.Points.Add(new Point3D(maxX, y, 0));
            }
            for (double x = minX; x <= maxX; x += step)
            {
                gridXY.Points.Add(new Point3D(x, minY, 0));
                gridXY.Points.Add(new Point3D(x, maxY, 0));
            }
            view1.Children.Add(gridXY);

            var gridXZ = new LinesVisual3D { Color = Colors.LightGray, Thickness = 0.5 };
            for (double z = minZ; z <= maxZ; z += step)
            {
                gridXZ.Points.Add(new Point3D(minX, 0, z));
                gridXZ.Points.Add(new Point3D(maxX, 0, z));
            }
            for (double x = minX; x <= maxX; x += step)
            {
                gridXZ.Points.Add(new Point3D(x, 0, minZ));
                gridXZ.Points.Add(new Point3D(x, 0, maxZ));
            }
            view1.Children.Add(gridXZ);

            var gridYZ = new LinesVisual3D { Color = Colors.LightGray, Thickness = 0.5 };
            for (double z = minZ; z <= maxZ; z += step)
            {
                gridYZ.Points.Add(new Point3D(0, minY, z));
                gridYZ.Points.Add(new Point3D(0, maxY, z));
            }
            for (double y = minY; y <= maxY; y += step)
            {
                gridYZ.Points.Add(new Point3D(0, y, minZ));
                gridYZ.Points.Add(new Point3D(0, y, maxZ));
            }
            view1.Children.Add(gridYZ);

            // ---------------- AXES ----------------
            view1.Children.Add(new LinesVisual3D { Color = Colors.Red, Thickness = 2, Points = new Point3DCollection { new Point3D(minX, 0, 0), new Point3D(maxX, 0, 0) } });
            view1.Children.Add(new LinesVisual3D { Color = Colors.Green, Thickness = 2, Points = new Point3DCollection { new Point3D(0, minY, 0), new Point3D(0, maxY, 0) } });
            view1.Children.Add(new LinesVisual3D { Color = Colors.Blue, Thickness = 2, Points = new Point3DCollection { new Point3D(0, 0, minZ), new Point3D(0, 0, maxZ) } });

            // ---------------- AXIS LABELS ----------------
            view1.Children.Add(new BillboardTextVisual3D { Text = "X", Position = new Point3D(maxX + 0.5, 0, 0), Foreground = Brushes.Red });
            view1.Children.Add(new BillboardTextVisual3D { Text = "Y", Position = new Point3D(0, maxY + 0.5, 0), Foreground = Brushes.Green });
            view1.Children.Add(new BillboardTextVisual3D { Text = "Z", Position = new Point3D(0, 0, maxZ + 0.5), Foreground = Brushes.Blue });

            // ---------------- RULER TICKS ----------------
            for (double x = minX; x <= maxX; x += step)
            {
                view1.Children.Add(new LinesVisual3D { Color = Colors.Red, Points = new Point3DCollection { new Point3D(x, -0.1, 0), new Point3D(x, 0.1, 0) } });
                view1.Children.Add(new BillboardTextVisual3D { Text = x.ToString("0.###"), Position = new Point3D(x, -0.3, 0), Foreground = Brushes.Red });
            }
            for (double y = minY; y <= maxY; y += step)
            {
                view1.Children.Add(new LinesVisual3D { Color = Colors.Green, Points = new Point3DCollection { new Point3D(-0.1, y, 0), new Point3D(0.1, y, 0) } });
                view1.Children.Add(new BillboardTextVisual3D { Text = y.ToString("0.###"), Position = new Point3D(-0.3, y, 0), Foreground = Brushes.Green });
            }
            for (double z = minZ; z <= maxZ; z += step)
            {
                view1.Children.Add(new LinesVisual3D { Color = Colors.Blue, Points = new Point3DCollection { new Point3D(0, -0.1, z), new Point3D(0, 0.1, z) } });
                view1.Children.Add(new BillboardTextVisual3D { Text = z.ToString("0.###"), Position = new Point3D(0, -0.3, z), Foreground = Brushes.Blue });
            }

            // ---------------- Sphere & Yellow Guides ----------------
            if (pointSphere != null)
            {
                if (view1.Children.Contains(pointSphere))
                    view1.Children.Remove(pointSphere);

                view1.Children.Add(pointSphere);

                Point3D p = pointSphere.Center; // ✅ correct way

                // Yellow guide lines (thicker & more visible)
                var guideThickness = 2.0;

                // To X-axis
                view1.Children.Add(new LinesVisual3D
                {
                    Color = Colors.Yellow,
                    Thickness = guideThickness,
                    Points = new Point3DCollection { p, new Point3D(p.X, 0, 0) }
                });

                // To Y-axis
                view1.Children.Add(new LinesVisual3D
                {
                    Color = Colors.Yellow,
                    Thickness = guideThickness,
                    Points = new Point3DCollection { p, new Point3D(0, p.Y, 0) }
                });

                // To Z-axis
                view1.Children.Add(new LinesVisual3D
                {
                    Color = Colors.Yellow,
                    Thickness = guideThickness,
                    Points = new Point3DCollection { p, new Point3D(0, 0, p.Z) }
                });
            }
        }



        private void AddPoint(double x, double y, double z)
        {
            // Create the sphere once
            var sphere = new SphereVisual3D
            {
                Center = new Point3D(x, y, z),
                Radius = 0.2,
                Fill = Brushes.Red
            };

            pointSphere = sphere;
            view1.Children.Add(pointSphere); // only here
        }

        private void UpdatePoint(double x, double y, double z)
        {
            if (pointSphere != null)
            {
                ((SphereVisual3D)pointSphere).Center = new Point3D(x, y, z);
            }
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtX.Text, out double x) &&
                double.TryParse(txtY.Text, out double y) &&
                double.TryParse(txtZ.Text, out double z))
            {
                x = Math.Max(minX, Math.Min(maxX, x));
                y = Math.Max(minY, Math.Min(maxY, y));
                z = Math.Max(minZ, Math.Min(maxZ, z));

                UpdatePoint(x, y, z);
            }
        }

        private void btnSetRange_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtMinX.Text, out double newMinX) &&
                double.TryParse(txtMaxX.Text, out double newMaxX) &&
                double.TryParse(txtMinY.Text, out double newMinY) &&
                double.TryParse(txtMaxY.Text, out double newMaxY) &&
                double.TryParse(txtMinZ.Text, out double newMinZ) &&
                double.TryParse(txtMaxZ.Text, out double newMaxZ))
            {
                minX = newMinX; maxX = newMaxX;
                minY = newMinY; maxY = newMaxY;
                minZ = newMinZ; maxZ = newMaxZ;

                DrawGrid(); // this will re-add pointSphere automatically
            }
        }

        private void view1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isDragging = true;
                lastMousePos = e.GetPosition(view1);
                Mouse.Capture(view1);
            }
        }

        private void view1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && pointSphere != null)
            {
                Point pos = e.GetPosition(view1);

                var current = pointSphere.Center;
                double newX = current.X;
                double newY = current.Y;
                double newZ = current.Z;

                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    // Shift + Drag => control Z
                    double dz = -(pos.Y - lastMousePos.Y) * (maxZ - minZ) / 500;
                    newZ = Math.Max(minZ, Math.Min(maxZ, current.Z + dz));
                }
                else
                {
                    // Normal drag => control X/Y
                    double dx = (pos.X - lastMousePos.X) * (maxX - minX) / 500;
                    double dy = -(pos.Y - lastMousePos.Y) * (maxY - minY) / 500;

                    newX = Math.Max(minX, Math.Min(maxX, current.X + dx));
                    newY = Math.Max(minY, Math.Min(maxY, current.Y + dy));
                }

                UpdatePoint(newX, newY, newZ);

                txtX.Text = newX.ToString("F2");
                txtY.Text = newY.ToString("F2");
                txtZ.Text = newZ.ToString("F2");

                lastMousePos = pos;
            }
        }

        private void btnSetStep_Click(object sender, RoutedEventArgs e)
        {
            DrawGrid();
        }

        private void view1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (pointSphere != null)
            {
                var current = pointSphere.Center;

                // Move Z in steps, scaled by delta
                double dz = (e.Delta > 0 ? 0.2 : -0.2) * (maxZ - minZ) / 20;
                double newZ = Math.Max(minZ, Math.Min(maxZ, current.Z + dz));

                UpdatePoint(current.X, current.Y, newZ);

                txtX.Text = current.X.ToString("F2");
                txtY.Text = current.Y.ToString("F2");
                txtZ.Text = newZ.ToString("F2");
            }
        }

        private void view1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            Mouse.Capture(null);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (pointSphere != null)
            {
                UpdatePoint(sliderX.Value, sliderY.Value, sliderZ.Value);
            }
        }
    }
}
