using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Timers;

namespace GetPixelApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern int BitBlt(IntPtr hDc, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);
        private System.Drawing.Color GetColorAt(System.Drawing.Point location)
        {
            var screenPixel = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var gdest = Graphics.FromImage(screenPixel))
            {
                using (var gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDc = gsrc.GetHdc();
                    IntPtr hDc = gdest.GetHdc();
                    BitBlt(hDc, 0, 0, 1, 1, hSrcDc, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }

            return screenPixel.GetPixel(0, 0);
        }

        private Timer timerColor = new Timer();
        private Timer timerBomb = new Timer();
        private SolidColorBrush brush = new SolidColorBrush();

        private int time = 46;
        public MainWindow()
        {
            Topmost = true;
            InitializeComponent();
            timerColor.Interval = 100;
            timerColor.Elapsed += TimerColor_Elapsed;
            timerBomb.Interval = 1000;
            timerBomb.Elapsed += TimerBomb_Elapsed;
        }

        private void TimerBomb_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(time == 0)
            {
                timerBomb.Stop();
                return;
            }
            this.Dispatcher.Invoke(() =>
            {
                lbTimer.Foreground = brush;
                lbTimer.Content = --time;
            });
        }

        private void TimerColor_Elapsed(object sender, ElapsedEventArgs e)
        {
            System.Drawing.Color color = GetColorAt(new System.Drawing.Point(960, 50));
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    brush.Color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
                    imgPixelColor.Fill = brush;
                });
            }
            catch
            {
                this.Close();
            }

            if (color.ToArgb() == System.Drawing.Color.FromArgb(0, 177, 42, 38).ToArgb())
            {
                timerBomb.Start();
                timerColor.Stop();
                return;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            timerColor.Start();
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            timerColor.Start();
            timerBomb.Stop();
            time = 46;
            lbTimer.Content = 45;
            lbTimer.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
        }
    }
}
