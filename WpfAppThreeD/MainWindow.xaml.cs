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

        private BillboardTextVisual3D coordLabel;

        public MainWindow()
        {
            InitializeComponent();
            DrawGrid();
            AddPoint(0, 0, 0);
        }


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
        }


        private void DrawGuides(Point3D p)
        {
            // remove old guide lines (yellow + black)
            for (int i = view1.Children.Count - 1; i >= 0; i--)
            {
                if (view1.Children[i] is LinesVisual3D line &&
                    (line.Color == Colors.Goldenrod || line.Color == Colors.Black))
                {
                    view1.Children.RemoveAt(i);
                }
            }

            var guideThickness = 2.0;

            // === Yellow Guides (corner style) ===
            view1.Children.Add(new LinesVisual3D
            {
                Color = Colors.Goldenrod,   // darker yellow
                Thickness = guideThickness,
                Points = new Point3DCollection
            {
                new Point3D(p.X, 0, 0),
                new Point3D(p.X, p.Y, 0)
            }
            });

            view1.Children.Add(new LinesVisual3D
            {
                Color = Colors.Goldenrod,
                Thickness = guideThickness,
                Points = new Point3DCollection
            {
                new Point3D(0, p.Y, 0),
                new Point3D(p.X, p.Y, 0)
            }
            });

            view1.Children.Add(new LinesVisual3D
            {
                Color = Colors.Goldenrod,
                Thickness = guideThickness,
                Points = new Point3DCollection
            {
                new Point3D(p.X, p.Y, 0),
                new Point3D(p.X, p.Y, p.Z)
            }
            });

            // ✅ New guide from Z axis to pointer
            view1.Children.Add(new LinesVisual3D
            {
                Color = Colors.Goldenrod,
                Thickness = guideThickness,
                Points = new Point3DCollection
        {
            new Point3D(0, 0, p.Z),
            new Point3D(p.X, p.Y, p.Z)
        }
            });

            // === Black Bounding Box ===
            // bottom rectangle (z=0)
            //    view1.Children.Add(new LinesVisual3D
            //    {
            //        Color = Colors.Black,
            //        Thickness = guideThickness,
            //        Points = new Point3DCollection
            //{
            //    new Point3D(0, 0, 0),
            //    new Point3D(p.X, 0, 0),

            //    new Point3D(p.X, 0, 0),
            //    new Point3D(p.X, p.Y, 0),

            //    new Point3D(p.X, p.Y, 0),
            //    new Point3D(0, p.Y, 0),

            //    new Point3D(0, p.Y, 0),
            //    new Point3D(0, 0, 0)
            //}
            //    });

            //    // top rectangle (z = p.Z)
            //    view1.Children.Add(new LinesVisual3D
            //    {
            //        Color = Colors.Black,
            //        Thickness = guideThickness,
            //        Points = new Point3DCollection
            //{
            //    new Point3D(0, 0, p.Z),
            //    new Point3D(p.X, 0, p.Z),

            //    new Point3D(p.X, 0, p.Z),
            //    new Point3D(p.X, p.Y, p.Z),

            //    new Point3D(p.X, p.Y, p.Z),
            //    new Point3D(0, p.Y, p.Z),

            //    new Point3D(0, p.Y, p.Z),
            //    new Point3D(0, 0, p.Z)
            //}
            //    });

            //    // vertical edges
            //    view1.Children.Add(new LinesVisual3D
            //    {
            //        Color = Colors.Black,
            //        Thickness = guideThickness,
            //        Points = new Point3DCollection
            //{
            //    new Point3D(0, 0, 0), new Point3D(0, 0, p.Z),
            //    new Point3D(p.X, 0, 0), new Point3D(p.X, 0, p.Z),
            //    new Point3D(p.X, p.Y, 0), new Point3D(p.X, p.Y, p.Z),
            //    new Point3D(0, p.Y, 0), new Point3D(0, p.Y, p.Z)
            //}
            //    });
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

        private void UpdateSphereLabel(Point3D p)
        {
            // remove old label if exists
            if (coordLabel != null)
                view1.Children.Remove(coordLabel);

            // format coordinates nicely
            string text = $"({p.X:0.0}, {p.Y:0.0}, {p.Z:0.0})";

            // create label just offset from the sphere
            coordLabel = new BillboardTextVisual3D
            {
                Text = text,
                Position = new Point3D(p.X + 2, p.Y + 2, p.Z + 2), // offset a bit so it’s visible
                Foreground = Brushes.DarkBlue,
                FontWeight = FontWeights.Bold,
                FontSize = 16
            };

            view1.Children.Add(coordLabel);
        }

        private void UpdatePoint(double x, double y, double z)
        {
            //if (pointSphere != null)
            //{
            //    ((SphereVisual3D)pointSphere).Center = new Point3D(x, y, z);
            //}

            if (pointSphere != null)
            {
                ((SphereVisual3D)pointSphere).Center = new Point3D(x, y, z);

                // ✅ draw guides right after update
                DrawGuides(new Point3D(x, y, z));
                
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

                // ✅ Sync sliders
                sliderX.Value = x;
                sliderY.Value = y;
                sliderZ.Value = z;
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
                // update ranges
                minX = newMinX; maxX = newMaxX;
                minY = newMinY; maxY = newMaxY;
                minZ = newMinZ; maxZ = newMaxZ;

                // redraw grid
                DrawGrid();
                
                // reset sliders
                sliderX.Minimum = minX; sliderX.Maximum = maxX; sliderX.Value = 0;
                sliderY.Minimum = minY; sliderY.Maximum = maxY; sliderY.Value = 0;
                sliderZ.Minimum = minZ; sliderZ.Maximum = maxZ; sliderZ.Value = 0;

                // reset sphere and guides to (0,0,0)
                UpdatePoint(0, 0, 0);
                AddPoint(0, 0, 0);
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

                UpdateSphereLabel(new Point3D(newX, newY, newZ));

                txtX.Text = newX.ToString("F2");
                txtY.Text = newY.ToString("F2");
                txtZ.Text = newZ.ToString("F2");

                // ✅ Update sliders too
                sliderX.Value = newX;
                sliderY.Value = newY;
                sliderZ.Value = newZ;

                lastMousePos = pos;
            }

        }

        private void btnSetStep_Click(object sender, RoutedEventArgs e)
        {
            DrawGrid();

            // ✅ Restore the pointer sphere
            if (pointSphere != null)
                view1.Children.Add(pointSphere);

            // ✅ Restore the coordinate label
            if (coordLabel != null)
                view1.Children.Add(coordLabel);

            if (pointSphere != null)
                DrawGuides(pointSphere.Center);

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
