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

        private void DrawGrid()
        {
            // remove previous grid
            view1.Children.Clear();
            view1.Children.Add(new DefaultLights());

            double step = 1.0;

            // Draw X axis lines in YZ plane
            for (double x = minX; x <= maxX; x += step)
            {
                var line1 = new LinesVisual3D
                {
                    Points = new Point3DCollection
            {
                new Point3D(x, minY, 0),
                new Point3D(x, maxY, 0)
            },
                    Color = Colors.LightGray,
                    Thickness = 1
                };
                view1.Children.Add(line1);

                var line2 = new LinesVisual3D
                {
                    Points = new Point3DCollection
            {
                new Point3D(x, 0, minZ),
                new Point3D(x, 0, maxZ)
            },
                    Color = Colors.LightGray,
                    Thickness = 1
                };
                view1.Children.Add(line2);
            }

            // Draw Y axis lines in XZ plane
            for (double y = minY; y <= maxY; y += step)
            {
                var line1 = new LinesVisual3D
                {
                    Points = new Point3DCollection
            {
                new Point3D(minX, y, 0),
                new Point3D(maxX, y, 0)
            },
                    Color = Colors.LightGray,
                    Thickness = 1
                };
                view1.Children.Add(line1);

                var line2 = new LinesVisual3D
                {
                    Points = new Point3DCollection
            {
                new Point3D(0, y, minZ),
                new Point3D(0, y, maxZ)
            },
                    Color = Colors.LightGray,
                    Thickness = 1
                };
                view1.Children.Add(line2);
            }

            // Draw Z axis lines in XY plane
            for (double z = minZ; z <= maxZ; z += step)
            {
                var line1 = new LinesVisual3D
                {
                    Points = new Point3DCollection
            {
                new Point3D(minX, 0, z),
                new Point3D(maxX, 0, z)
            },
                    Color = Colors.LightGray,
                    Thickness = 1
                };
                view1.Children.Add(line1);

                var line2 = new LinesVisual3D
                {
                    Points = new Point3DCollection
            {
                new Point3D(0, minY, z),
                new Point3D(0, maxY, z)
            },
                    Color = Colors.LightGray,
                    Thickness = 1
                };
                view1.Children.Add(line2);
            }

            // Axes (Red = X, Green = Y, Blue = Z)
            view1.Children.Add(new LinesVisual3D
            {
                Points = new Point3DCollection { new Point3D(minX, 0, 0), new Point3D(maxX, 0, 0) },
                Color = Colors.Red,
                Thickness = 2
            });
            view1.Children.Add(new LinesVisual3D
            {
                Points = new Point3DCollection { new Point3D(0, minY, 0), new Point3D(0, maxY, 0) },
                Color = Colors.Green,
                Thickness = 2
            });
            view1.Children.Add(new LinesVisual3D
            {
                Points = new Point3DCollection { new Point3D(0, 0, minZ), new Point3D(0, 0, maxZ) },
                Color = Colors.Blue,
                Thickness = 2
            });

            // Re-add point sphere if exists
            if (pointSphere != null)
            {
                if (view1.Children.Contains(pointSphere))
                    view1.Children.Remove(pointSphere);

                view1.Children.Add(pointSphere);
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

                view1.Children.Clear();
                view1.Children.Add(new DefaultLights());

                DrawGrid();
                AddPoint(pointSphere.Center.X, pointSphere.Center.Y, pointSphere.Center.Z);
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
