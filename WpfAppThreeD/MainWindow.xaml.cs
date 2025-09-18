using HelixToolkit.Wpf;
using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using NCalc;

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

        // Keep references to axis labels for font size updates
        private BillboardTextVisual3D xLabel, yLabel, zLabel;

        private bool isPerspective = true;

        private bool isUpdatingUI = false;

        private bool txtUpdating = false;
        private bool sliderUpdating = false;
        RangeVM rangeX = new RangeVM();
        RangeVM rangeY=new RangeVM();
        RangeVM rangeZ=new RangeVM();
        public MainWindow()
        {
            
            InitializeComponent();
            DrawGrid();
            AddPoint(0, 0, 0);           

            var cam = view1.Camera as PerspectiveCamera;
            if (cam != null)
            {
                //cam.Position = new Point3D(20, 20, 20);      // now at +X +Y +Z corner
                //cam.LookDirection = new Vector3D(-20, -20, -20); // looking back at origin
                //cam.UpDirection = new Vector3D(0, 0, 1);     // Z axis is up

                cam.Position = new Point3D(0, 0, 86.71035032622612);      // now at +X +Y +Z corner
                cam.LookDirection = new Vector3D(-0, -0, -86.71035032622612); // looking back at origin
                cam.UpDirection = new Vector3D(0, 1, 0);
            }

            txtX.TextChanged += txtPointerInput_TextChanged;
            txtY.TextChanged += txtPointerInput_TextChanged;
            txtZ.TextChanged += txtPointerInput_TextChanged;
            //txtStep.TextChanged += txtStep_TextChanged;
            txtMinX.TextChanged += txtRange_TextChanged;
            txtMinY.TextChanged += txtRange_TextChanged;
            txtMinZ.TextChanged += txtRange_TextChanged;
            txtMaxX.TextChanged += txtRange_TextChanged;
            txtMaxY.TextChanged += txtRange_TextChanged;
            txtMaxZ.TextChanged += txtRange_TextChanged;
            sliderPointSize.ValueChanged += sliderPointSize_ValueChanged;
            sliderFontSize.ValueChanged += sliderFontSize_ValueChanged;
            txtFontMin.TextChanged += txtFont_TextChanged;
            txtFontMax.TextChanged += txtFont_TextChanged;
            txtPointMin.TextChanged += txtPoint_TextChanged;
            txtPointMax.TextChanged += txtPoint_TextChanged;
        }


        private List<BillboardTextVisual3D> xTicks = new();
        private List<BillboardTextVisual3D> yTicks = new();
        private List<BillboardTextVisual3D> zTicks = new();


        private void DrawGrid()
        {
            view1.Children.Clear();
            view1.Children.Add(new DefaultLights());

            // Parse step values for each axis, fallback to 1 if invalid
            double stepX = 1, stepY = 1, stepZ = 1;
            double.TryParse(txtStepX.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out stepX);
            double.TryParse(txtStepY.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out stepY);
            double.TryParse(txtStepZ.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out stepZ);

            // reset tick references
            xTicks.Clear();
            yTicks.Clear();
            zTicks.Clear();

            // ---------------- BOX ----------------
            var box = new LinesVisual3D { Color = Colors.Black, Thickness = 1.5 };
            Point3D p000 = new Point3D(minX, minY, minZ);
            Point3D p100 = new Point3D(maxX, minY, minZ);
            Point3D p010 = new Point3D(minX, maxY, minZ);
            Point3D p110 = new Point3D(maxX, maxY, minZ);
            Point3D p001 = new Point3D(minX, minY, maxZ);
            Point3D p101 = new Point3D(maxX, minY, maxZ);
            Point3D p011 = new Point3D(minX, maxY, maxZ);
            Point3D p111 = new Point3D(maxX, maxY, maxZ);

            // bottom square
            box.Points.Add(p000); box.Points.Add(p100);
            box.Points.Add(p100); box.Points.Add(p110);
            box.Points.Add(p110); box.Points.Add(p010);
            box.Points.Add(p010); box.Points.Add(p000);

            // top square
            box.Points.Add(p001); box.Points.Add(p101);
            box.Points.Add(p101); box.Points.Add(p111);
            box.Points.Add(p111); box.Points.Add(p011);
            box.Points.Add(p011); box.Points.Add(p001);

            // vertical edges
            box.Points.Add(p000); box.Points.Add(p001);
            box.Points.Add(p100); box.Points.Add(p101);
            box.Points.Add(p110); box.Points.Add(p111);
            box.Points.Add(p010); box.Points.Add(p011);

            view1.Children.Add(box);

            // ---------------- GRID PLANES ----------------
            var gridXY = new LinesVisual3D { Color = Colors.LightGray, Thickness = 0.5 };
            int stepsY = (int)Math.Round((maxY - minY) / stepY);
            int stepsX = (int)Math.Round((maxX - minX) / stepX);
            for (int i = 0; i <= stepsY; i++)
            {
                double y = Math.Round(minY + i * stepY, 6);
                gridXY.Points.Add(new Point3D(minX, y, 0));
                gridXY.Points.Add(new Point3D(maxX, y, 0));
            }
            for (int i = 0; i <= stepsX; i++)
            {
                double x = Math.Round(minX + i * stepX, 6);
                gridXY.Points.Add(new Point3D(x, minY, 0));
                gridXY.Points.Add(new Point3D(x, maxY, 0));
            }
            view1.Children.Add(gridXY);

            var gridXZ = new LinesVisual3D { Color = Colors.LightGray, Thickness = 0.5 };
            int stepsZ = (int)Math.Round((maxZ - minZ) / stepZ);
            for (int i = 0; i <= stepsZ; i++)
            {
                double z = Math.Round(minZ + i * stepZ, 6);
                gridXZ.Points.Add(new Point3D(minX, 0, z));
                gridXZ.Points.Add(new Point3D(maxX, 0, z));
            }
            for (int i = 0; i <= stepsX; i++)
            {
                double x = Math.Round(minX + i * stepX, 6);
                gridXZ.Points.Add(new Point3D(x, 0, minZ));
                gridXZ.Points.Add(new Point3D(x, 0, maxZ));
            }
            view1.Children.Add(gridXZ);

            var gridYZ = new LinesVisual3D { Color = Colors.LightGray, Thickness = 0.5 };
            for (int i = 0; i <= stepsZ; i++)
            {
                double z = Math.Round(minZ + i * stepZ, 6);
                gridYZ.Points.Add(new Point3D(0, minY, z));
                gridYZ.Points.Add(new Point3D(0, maxY, z));
            }
            for (int i = 0; i <= stepsY; i++)
            {
                double y = Math.Round(minY + i * stepY, 6);
                gridYZ.Points.Add(new Point3D(0, y, minZ));
                gridYZ.Points.Add(new Point3D(0, y, maxZ));
            }
            view1.Children.Add(gridYZ);

            // ---------------- AXES ----------------
            view1.Children.Add(new LinesVisual3D { Color = Colors.Red, Thickness = 2, Points = new Point3DCollection { new Point3D(minX, 0, 0), new Point3D(maxX, 0, 0) } });
            view1.Children.Add(new LinesVisual3D { Color = Colors.Green, Thickness = 2, Points = new Point3DCollection { new Point3D(0, minY, 0), new Point3D(0, maxY, 0) } });
            view1.Children.Add(new LinesVisual3D { Color = Colors.Blue, Thickness = 2, Points = new Point3DCollection { new Point3D(0, 0, minZ), new Point3D(0, 0, maxZ) } });

            // ---------------- AXIS LABELS ----------------
            xLabel = new BillboardTextVisual3D { Text = "X", Position = new Point3D(maxX + 0.5, 0, 0), Foreground = Brushes.Red, FontSize = sliderFontSize.Value, FontWeight = FontWeights.Bold };
            yLabel = new BillboardTextVisual3D { Text = "Y", Position = new Point3D(0, maxY + 0.5, 0), Foreground = Brushes.Green, FontSize = sliderFontSize.Value, FontWeight = FontWeights.Bold };
            zLabel = new BillboardTextVisual3D { Text = "Z", Position = new Point3D(0, 0, maxZ + 0.5), Foreground = Brushes.Blue, FontSize = sliderFontSize.Value, FontWeight = FontWeights.Bold };
            view1.Children.Add(xLabel); view1.Children.Add(yLabel); view1.Children.Add(zLabel);

            // ---------------- RULER TICKS ----------------
            for (int i = 0; i <= stepsX; i++)
            {
                double x = Math.Round(minX + i * stepX, 6);
                view1.Children.Add(new LinesVisual3D { Color = Colors.Red, Points = new Point3DCollection { new Point3D(x, -0.1, 0), new Point3D(x, 0.1, 0) } });
                var tick = new BillboardTextVisual3D { Text = x.ToString("0.###"), Position = new Point3D(x, -0.3, 0), Foreground = Brushes.Red, FontSize = sliderFontSize.Value };
                view1.Children.Add(tick); xTicks.Add(tick);
            }
            for (int i = 0; i <= stepsY; i++)
            {
                double y = Math.Round(minY + i * stepY, 6);
                view1.Children.Add(new LinesVisual3D { Color = Colors.Green, Points = new Point3DCollection { new Point3D(-0.1, y, 0), new Point3D(0.1, y, 0) } });
                var tick = new BillboardTextVisual3D { Text = y.ToString("0.###"), Position = new Point3D(-0.3, y, 0), Foreground = Brushes.Green, FontSize = sliderFontSize.Value };
                view1.Children.Add(tick); yTicks.Add(tick);
            }
            for (int i = 0; i <= stepsZ; i++)
            {
                double z = Math.Round(minZ + i * stepZ, 6);
                view1.Children.Add(new LinesVisual3D { Color = Colors.Blue, Points = new Point3DCollection { new Point3D(0, -0.1, z), new Point3D(0, 0.1, z) } });
                var tick = new BillboardTextVisual3D { Text = z.ToString("0.###"), Position = new Point3D(0, -0.3, z), Foreground = Brushes.Blue, FontSize = sliderFontSize.Value };
                view1.Children.Add(tick); zTicks.Add(tick);
            }

            // ---------------- RE-ADD POINTER + LABEL ----------------
            if (pointSphere != null) view1.Children.Add(pointSphere);
            if (coordLabel != null) view1.Children.Add(coordLabel);
            if (pointSphere != null) DrawGuides(pointSphere.Center);
        }



        private void DrawGuides(Point3D p)
        {
            // remove old guide lines
            for (int i = view1.Children.Count - 1; i >= 0; i--)
            {
                if (view1.Children[i] is LinesVisual3D line &&
                    (line.Color == Colors.Goldenrod))
                {
                    view1.Children.RemoveAt(i);
                }
            }

            var guideThickness = 2.0;

            // === Yellow Guides===
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

            // New guide from Z axis to pointer
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

            // format coordinates
            string text = $"({p.X:0.0}, {p.Y:0.0}, {p.Z:0.0})";

            // create label
            coordLabel = new BillboardTextVisual3D
            {
                Text = text,
                Position = new Point3D(p.X + 2, p.Y + 2, p.Z + 2),
                Foreground = Brushes.DarkBlue,
                FontWeight = FontWeights.Bold,
                FontSize = 16
            };

            view1.Children.Add(coordLabel);
        }

        private void UpdatePoint(double x, double y, double z)
        {
            if (pointSphere != null)
            {
                pointSphere.Center = new Point3D(x, y, z);
                DrawGuides(new Point3D(x, y, z));

                isUpdatingUI = true; // prevent recursive TextChanged
                
                //txtX.Text = x.ToString("0.###");
                txtCoordinateX.Text = x.ToString("0.###");
                txtCoordinateX.CaretIndex = txtCoordinateX.Text.Length;

                txtCoordinateY.Text = y.ToString("0.###");
                txtCoordinateY.CaretIndex = txtCoordinateY.Text.Length;

                txtCoordinateZ.Text = z.ToString("0.###");
                txtCoordinateZ.CaretIndex = txtCoordinateZ.Text.Length;

                isUpdatingUI = false;
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
                    // Control Z movement
                    double dz = -(pos.Y - lastMousePos.Y) * (maxZ - minZ) / 500;
                    newZ = Math.Max(minZ, Math.Min(maxZ, current.Z + dz));
                }
                else
                {
                    // Control X/Y movement
                    double dx = (pos.X - lastMousePos.X) * (maxX - minX) / 500;
                    double dy = -(pos.Y - lastMousePos.Y) * (maxY - minY) / 500;

                    newX = Math.Max(minX, Math.Min(maxX, current.X + dx));
                    newY = Math.Max(minY, Math.Min(maxY, current.Y + dy));
                }

                //  Snap-to-Grid logic with separate steps
                double stepX = Convert.ToDouble(txtStepX.Text), stepY = Convert.ToDouble(txtStepY.Text), stepZ = Convert.ToDouble(txtStepZ.Text);
                double.TryParse(txtStepX.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out stepX);
                double.TryParse(txtStepY.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out stepY);
                double.TryParse(txtStepZ.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out stepZ);

                if (chkSnapToGrid.IsChecked == true)
                {
                    newX = GetSnappedValue(newX, stepX);
                    newY = GetSnappedValue(newY, stepY);
                    newZ = GetSnappedValue(newZ, stepZ);
                }

                // Apply updates
                UpdatePoint(newX, newY, newZ);
                UpdateSphereLabel(new Point3D(newX, newY, newZ));

                txtX.Text = newX.ToString("F2");
                txtY.Text = newY.ToString("F2");
                txtZ.Text = newZ.ToString("F2");

                // Update sliders too
                sliderX.Value = newX;
                sliderY.Value = newY;
                sliderZ.Value = newZ;

                lastMousePos = pos;
            }
        }



        private void btnSetStep_Click(object sender, RoutedEventArgs e)
        {
           

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

        private void view1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (pointSphere != null)
            {
                isDragging = true;
                lastMousePos = e.GetPosition(view1);
                view1.CaptureMouse(); // lock mouse to viewport
            }
        }

        private void view1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                view1.ReleaseMouseCapture();

                // Final snap-to-grid adjustment
                if (pointSphere != null && chkSnapToGrid.IsChecked == true)
                {
                    double stepX = 1, stepY = 1, stepZ = 1;
                    double.TryParse(txtStepX.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out stepX);
                    double.TryParse(txtStepY.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out stepY);
                    double.TryParse(txtStepZ.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out stepZ);

                    double newX = GetSnappedValue(pointSphere.Center.X, stepX);
                    double newY = GetSnappedValue(pointSphere.Center.Y, stepY);
                    double newZ = GetSnappedValue(pointSphere.Center.Z, stepZ);

                    UpdatePoint(newX, newY, newZ);
                    UpdateSphereLabel(new Point3D(newX, newY, newZ));

                    txtX.Text = newX.ToString("F2");
                    txtY.Text = newY.ToString("F2");
                    txtZ.Text = newZ.ToString("F2");

                    sliderX.Value = newX;
                    sliderY.Value = newY;
                    sliderZ.Value = newZ;
                }
            }
        }



        //private void txtPointerInput_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (isUpdatingUI) return; // ignore programmatic changes

        //    if (double.TryParse(txtX.Text, out double x) &&
        //        double.TryParse(txtY.Text, out double y) &&
        //        double.TryParse(txtZ.Text, out double z))
        //    {
        //        x = Math.Max(minX, Math.Min(maxX, x));
        //        y = Math.Max(minY, Math.Min(maxY, y));
        //        z = Math.Max(minZ, Math.Min(maxZ, z));

        //        UpdatePoint(x, y, z);

        //        sliderX.Value = x;
        //        sliderY.Value = y;
        //        sliderZ.Value = z;
        //    }
        //}

        private void txtPointerInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!sliderUpdating)
            {
                txtUpdating = true;
                if (isUpdatingUI)
                    return; // ignore programmatic changes

                TextBox tb = sender as TextBox;
                if (tb == null) return;

                string text = tb.Text;

                // ✅ If empty or invalid intermediate state → reset to 0
                if (string.IsNullOrEmpty(text) || text == "-" || text == "." || text == "-.")
                {
                    tb.Text = "0";
                    tb.CaretIndex = tb.Text.Length;
                    return;
                }

                if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                {
                    double x = sliderX.Value;
                    double y = sliderY.Value;
                    double z = sliderZ.Value;

                    if (tb == txtX) x = value;
                    if (tb == txtY) y = value;
                    if (tb == txtZ) z = value;

                    // clamp
                    x = Math.Max(minX, Math.Min(maxX, x));
                    y = Math.Max(minY, Math.Min(maxY, y));
                    z = Math.Max(minZ, Math.Min(maxZ, z));

                    // ✅ keep clamped values in textboxes
                    txtX.Text = x.ToString(CultureInfo.InvariantCulture);
                    txtY.Text = y.ToString(CultureInfo.InvariantCulture);
                    txtZ.Text = z.ToString(CultureInfo.InvariantCulture);

                    txtX.CaretIndex = txtX.Text.Length;
                    txtY.CaretIndex = txtY.Text.Length;
                    txtZ.CaretIndex = txtZ.Text.Length;

                    ApplyExpressionToX();
                    ApplyExpressionToY();
                    ApplyExpressionToZ();

                    x = Convert.ToDouble(txtCoordinateX.Text, CultureInfo.InvariantCulture);
                    y = Convert.ToDouble(txtCoordinateY.Text, CultureInfo.InvariantCulture);
                    z = Convert.ToDouble(txtCoordinateZ.Text, CultureInfo.InvariantCulture);

                    UpdatePoint(x, y, z); // Apply updates
                    UpdateSphereLabel(new Point3D(x, y, z));

                    sliderX.Value = Convert.ToDouble(txtX.Text, CultureInfo.InvariantCulture);
                    sliderY.Value = Convert.ToDouble(txtY.Text, CultureInfo.InvariantCulture);
                    sliderZ.Value = Convert.ToDouble(txtZ.Text, CultureInfo.InvariantCulture);

                    txtUpdating = false;
                }
            }
        }








        //private void txtStep_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        //{
        //    if (double.TryParse(txtStep.Text, out double step) && step > 0)
        //    {
        //        DrawGrid();

        //        // Restore the pointer sphere
        //        if (pointSphere != null && !view1.Children.Contains(pointSphere))
        //            view1.Children.Add(pointSphere);

        //        // Restore the coordinate label
        //        if (coordLabel != null && !view1.Children.Contains(coordLabel))
        //            view1.Children.Add(coordLabel);

        //        if (pointSphere != null)
        //            DrawGuides(pointSphere.Center);
        //    }
        //}


        private void txtRange_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
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
                txtX.Text = "0";
                txtY.Text = "0";
                txtZ.Text = "0";
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!txtUpdating)
            {
                sliderUpdating = true;
                if (pointSphere == null) return;

                double newX = sliderX.Value;
                double newY = sliderY.Value;
                double newZ = sliderZ.Value;

                // Snap-to-Grid logic
                double stepX = ParseStep(txtStepX.Text, 1);
                double stepY = ParseStep(txtStepY.Text, 1);
                double stepZ = ParseStep(txtStepZ.Text, 1);

                if (chkSnapToGrid.IsChecked == true)
                {
                    newX = GetSnappedValue(newX, stepX);
                    newY = GetSnappedValue(newY, stepY);
                    newZ = GetSnappedValue(newZ, stepZ);

                    // Prevent feedback loops
                    if (Math.Abs(sliderX.Value - newX) > double.Epsilon) sliderX.Value = newX;
                    if (Math.Abs(sliderY.Value - newY) > double.Epsilon) sliderY.Value = newY;
                    if (Math.Abs(sliderZ.Value - newZ) > double.Epsilon) sliderZ.Value = newZ;
                }

                // Apply expressions here
                txtX.Text = newX.ToString(CultureInfo.InvariantCulture);
                txtY.Text = newY.ToString(CultureInfo.InvariantCulture);
                txtZ.Text = newZ.ToString(CultureInfo.InvariantCulture);

                ApplyExpressionToX();
                ApplyExpressionToY();
                ApplyExpressionToZ();

                // Read expression results back
                if (!double.TryParse(txtCoordinateX.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out newX))
                    newX = 0;
                if (!double.TryParse(txtCoordinateY.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out newY))
                    newY = 0;
                if (!double.TryParse(txtCoordinateZ.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out newZ))
                    newZ = 0;

                // Apply updates
                UpdatePoint(newX, newY, newZ);
                UpdateSphereLabel(new Point3D(newX, newY, newZ));

                sliderUpdating = false;
            }
            
        }

        private double ParseStep(string input, double fallback)
        {
            if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double step) && step > 0)
                return step;
            return fallback;
        }

        private void sliderFontSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int size = Math.Clamp((int)e.NewValue, int.Parse(txtFontMin.Text), int.Parse(txtFontMax.Text));

            if (xLabel != null) xLabel.FontSize = size;
            if (yLabel != null) yLabel.FontSize = size;
            if (zLabel != null) zLabel.FontSize = size;
            if (coordLabel != null) coordLabel.FontSize = size;

            foreach (var tick in xTicks) tick.FontSize = size;
            foreach (var tick in yTicks) tick.FontSize = size;
            foreach (var tick in zTicks) tick.FontSize = size;

            //DrawGrid();
        }

        private void txtFont_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(txtFontMin.Text, out double min) &&
                double.TryParse(txtFontMax.Text, out double max) &&
                min < max)
            {
                sliderFontSize.Minimum = min;
                sliderFontSize.Maximum = max;

                // keep slider value within new bounds
                if (sliderFontSize.Value < min) sliderFontSize.Value = min;
                if (sliderFontSize.Value > max) sliderFontSize.Value = max;
            }
        }

        private void txtPoint_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(txtPointMin.Text, out double min) &&
               double.TryParse(txtPointMax.Text, out double max) &&
               min < max)
            {
                sliderPointSize.Minimum = min;
                sliderPointSize.Maximum = max;

                if (sliderPointSize.Value < min) sliderPointSize.Value = min;
                if (sliderPointSize.Value > max) sliderPointSize.Value = max;
            }
        }

        private void SwitchCamera_Click(object sender, RoutedEventArgs e)
        {
            // Get current camera (works for both Perspective and Orthographic)
            var currentCam = view1.Camera;

            Point3D position = currentCam.Position;
            Vector3D lookDirection = currentCam.LookDirection;
            Vector3D upDirection = currentCam.UpDirection;

            // Calculate distance from camera to target (scene center = 0,0,0 here)
            Point3D target = new Point3D(0, 0, 0);
            double distance = (position - target).Length;

            if (isPerspective)
            {
                // Switch to Orthographic while preserving view
                view1.Camera = new OrthographicCamera
                {
                    Position = position,
                    LookDirection = lookDirection,
                    UpDirection = upDirection,
                    Width = distance // width scales with distance
                };

                btnSwitchCamera.Content = "Switch to Perspective View";
            }
            else
            {
                // Switch to Perspective while preserving view
                view1.Camera = new PerspectiveCamera
                {
                    Position = position,
                    LookDirection = lookDirection,
                    UpDirection = upDirection,
                    FieldOfView = 45
                };

                btnSwitchCamera.Content = "Switch to Orthographic View";
            }

            isPerspective = !isPerspective;
        }

        private void sliderPointSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double size = Math.Clamp(e.NewValue, double.Parse(txtPointMin.Text), double.Parse(txtPointMax.Text));
            if (pointSphere != null) pointSphere.Radius = size;
        }

        private double GetSnappedValue(double value, double step)
        {
            return Math.Round(value / step) * step;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //// Allow only numbers
            //e.Handled = !int.TryParse(e.Text, out _);

            TextBox textBox = sender as TextBox;

            // simulate what the text will look like after input
            string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);

            Regex regex = new Regex(@"^-?\d*\.?\d*$");
            e.Handled = !regex.IsMatch(newText);
        }

        private void txtMinMax_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && int.TryParse(tb.Text, out int value))
            {
                if (value < 5) tb.Text = "5"; // Enforce min size = 5
            }
        }

        private void BtnSetXRange_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtMinX.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double min) &&
                double.TryParse(txtMaxX.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double max))
            {
                if (rangeX == null)
                {
                    rangeX = new RangeVM();
                }
                
                rangeX.axis = "X";
                rangeX.min = Convert.ToDouble(txtMinX.Text);
                rangeX.max= Convert.ToDouble(txtMaxX.Text);
                rangeX.expression = txtXExp.Text;
                rangeX.step = Convert.ToDouble(txtStepX.Text);
                

                var dlg = new RangeDialog(rangeX) { Owner = this };
                if (dlg.ShowDialog() == true)
                {
                    // Update min/max values
                    txtMinX.Text = dlg.MinValue.ToString(CultureInfo.InvariantCulture);
                    txtMaxX.Text = dlg.MaxValue.ToString(CultureInfo.InvariantCulture);
                    sliderX.Minimum = dlg.MinValue;
                    sliderX.Maximum = dlg.MaxValue;
                    txtXExp.Text = dlg.Expression;
                    txtStepX.Text =  dlg.step.ToString(CultureInfo.InvariantCulture);

                    rangeX.axis = "X";
                    rangeX.min = Convert.ToDouble(txtMinX.Text);
                    rangeX.max = Convert.ToDouble(txtMaxX.Text);
                    rangeX.expression = txtXExp.Text;
                    rangeX.step = Convert.ToDouble(txtStepX.Text);
                    rangeX.roundingDigits = dlg.Digits;
                    rangeX.rounding=dlg.rounding;


                    DrawGrid();
                    // Save rounding / digits / expression
                    ApplyRangeSettings("X", dlg);

                    // Get current coordinates
                    double x = pointSphere.Center.X;
                    double y = pointSphere.Center.Y;
                    double z = pointSphere.Center.Z;


                    // Apply the expression immediately
                    ApplyExpressionToX();
                    x = Convert.ToDouble(txtCoordinateX.Text);
                    // Update the pointer
                    UpdatePoint(x, y, z);
                    UpdateSphereLabel(new Point3D(x, y, z));
                }
            }
        }

        private void BtnSetYRange_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtMinY.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double min) &&
                double.TryParse(txtMaxY.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double max))
            {
                if (rangeY == null)
                {
                    rangeY = new RangeVM();
                }

                rangeY.axis = "Y";
                rangeY.min = Convert.ToDouble(txtMinY.Text);
                rangeY.max = Convert.ToDouble(txtMaxY.Text);
                rangeY.expression = txtYExp.Text;
                rangeY.step = Convert.ToDouble(txtStepY.Text);


                var dlg = new RangeDialog(rangeY) { Owner = this };
                if (dlg.ShowDialog() == true)
                {
                    // Update min/max values
                    txtMinY.Text = dlg.MinValue.ToString(CultureInfo.InvariantCulture);
                    txtMaxY.Text = dlg.MaxValue.ToString(CultureInfo.InvariantCulture);
                    sliderY.Minimum = dlg.MinValue;
                    sliderY.Maximum = dlg.MaxValue;
                    txtYExp.Text = dlg.Expression;
                    txtStepY.Text = dlg.step.ToString(CultureInfo.InvariantCulture);

                    rangeY.axis = "Y";
                    rangeY.min = Convert.ToDouble(txtMinY.Text);
                    rangeY.max = Convert.ToDouble(txtMaxY.Text);
                    rangeY.expression = txtYExp.Text;
                    rangeY.step = Convert.ToDouble(txtStepY.Text);
                    rangeY.roundingDigits = dlg.Digits;
                    rangeY.rounding = dlg.rounding;

                    DrawGrid();
                    // Save rounding / digits / expression
                    ApplyRangeSettings("Y", dlg);

                    // Get current coordinates
                    double x = pointSphere.Center.X;
                    double y = pointSphere.Center.Y;
                    double z = pointSphere.Center.Z;


                    // Apply the expression immediately
                    ApplyExpressionToY();
                    y = Convert.ToDouble(txtCoordinateY.Text);
                    // Update the pointer
                    UpdatePoint(x, y, z);
                    UpdateSphereLabel(new Point3D(x, y, z));
                }
            }
        }

        private void BtnSetZRange_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtMinZ.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double min) &&
                double.TryParse(txtMaxZ.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double max))
            {
                if (rangeZ == null)
                {
                    rangeZ = new RangeVM();
                }

                rangeZ.axis = "Z";
                rangeZ.min = Convert.ToDouble(txtMinZ.Text);
                rangeZ.max = Convert.ToDouble(txtMaxZ.Text);
                rangeZ.expression = txtZExp.Text;
                rangeZ.step = Convert.ToDouble(txtStepZ.Text);

                var dlg = new RangeDialog(rangeZ) { Owner = this };
                if (dlg.ShowDialog() == true)
                {
                    // Update min/max values
                    txtMinZ.Text = dlg.MinValue.ToString(CultureInfo.InvariantCulture);
                    txtMaxZ.Text = dlg.MaxValue.ToString(CultureInfo.InvariantCulture);
                    sliderZ.Minimum = dlg.MinValue;
                    sliderZ.Maximum = dlg.MaxValue;
                    txtZExp.Text = dlg.Expression;
                    txtStepZ.Text = dlg.step.ToString(CultureInfo.InvariantCulture);

                    rangeZ.axis = "Z";
                    rangeZ.min = Convert.ToDouble(txtMinZ.Text);
                    rangeZ.max = Convert.ToDouble(txtMaxZ.Text);
                    rangeZ.expression = txtZExp.Text;
                    rangeZ.step = Convert.ToDouble(txtStepZ.Text);
                    rangeZ.roundingDigits = dlg.Digits;
                    rangeZ.rounding = dlg.rounding;

                    DrawGrid();
                    // Save rounding / digits / expression
                    ApplyRangeSettings("Z", dlg);

                    // Get current coordinates
                    double x = pointSphere.Center.X;
                    double y = pointSphere.Center.Y;
                    double z = pointSphere.Center.Z;
                    

                    // Apply the expression immediately
                    ApplyExpressionToZ();
                    z=Convert.ToDouble(txtCoordinateZ.Text);
                    // Update the pointer
                    UpdatePoint(x, y, z);
                    UpdateSphereLabel(new Point3D(x, y, z));
                }
            }
        }

        /// <summary>
        /// Store additional settings returned from RangeDialog
        /// </summary>
        private void ApplyRangeSettings(string axis, RangeDialog dlg)
        {
            // Example: You could store them in dictionaries if you want per-axis configs
            // For now, just showing how to capture them
            var rounding = dlg.SelectedMode;
            var digits = dlg.Digits;
            var expr = dlg.Expression;

            // Debug or apply logic later
            Console.WriteLine($"Axis={axis}, Mode={rounding}, Digits={digits}, Expression={expr}");
        }

        private void ApplyExpressionToX()
        {
            if (double.TryParse(txtX.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double originalX))
            {
                string expression = txtXExp.Text;

                if (!string.IsNullOrWhiteSpace(expression))
                {
                    try
                    {
                        var exp = new NCalc.Expression(expression);
                        exp.Parameters["X"] = originalX;
                        exp.Parameters["Y"] = pointSphere.Center.Y;
                        exp.Parameters["Z"] = pointSphere.Center.Z;

                        var result = exp.Evaluate();

                        if (result is double d)
                            txtCoordinateX.Text = d.ToString("0.###", CultureInfo.InvariantCulture);
                        else
                            txtCoordinateX.Text = result.ToString();
                    }
                    catch (Exception ex)
                    {
                        txtCoordinateX.Text = "ERR";
                        Console.WriteLine($"Expression error in X: {ex.Message}");
                    }
                }
                else
                {
                    txtCoordinateX.Text = originalX.ToString("0.###", CultureInfo.InvariantCulture);
                }
            }
        }

        private void ApplyExpressionToY()
        {
            if (double.TryParse(txtY.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double originalY))
            {
                string expression = txtYExp.Text;

                if (!string.IsNullOrWhiteSpace(expression))
                {
                    try
                    {
                        var exp = new NCalc.Expression(expression);
                        exp.Parameters["X"] = pointSphere.Center.X;
                        exp.Parameters["Y"] = originalY;
                        exp.Parameters["Z"] = pointSphere.Center.Z;

                        var result = exp.Evaluate();

                        if (result is double d)
                            txtCoordinateY.Text = d.ToString("0.###", CultureInfo.InvariantCulture);
                        else
                            txtCoordinateY.Text = result.ToString();
                    }
                    catch (Exception ex)
                    {
                        txtCoordinateY.Text = "ERR";
                        Console.WriteLine($"Expression error in Y: {ex.Message}");
                    }
                }
                else
                {
                    txtCoordinateY.Text = originalY.ToString("0.###", CultureInfo.InvariantCulture);
                }
            }
        }

        private void ApplyExpressionToZ()
        {
            if (double.TryParse(txtZ.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double originalZ))
            {
                string expression = txtZExp.Text;

                if (!string.IsNullOrWhiteSpace(expression))
                {
                    try
                    {
                        var exp = new NCalc.Expression(expression);
                        exp.Parameters["X"] = pointSphere.Center.X;
                        exp.Parameters["Y"] = pointSphere.Center.Y;
                        exp.Parameters["Z"] = originalZ;

                        var result = exp.Evaluate();

                        if (result is double d)
                            txtCoordinateZ.Text = d.ToString("0.###", CultureInfo.InvariantCulture);
                        else
                            txtCoordinateZ.Text = result.ToString();
                    }
                    catch (Exception ex)
                    {
                        txtCoordinateZ.Text = "ERR";
                        Console.WriteLine($"Expression error in Z: {ex.Message}");
                    }
                }
                else
                {
                    txtCoordinateZ.Text = originalZ.ToString("0.###", CultureInfo.InvariantCulture);
                }
            }
        }


    }
}
