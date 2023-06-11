using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfTerce
{
    public partial class MainWindow : Window
    {
        private Random random;
        private List<Terč> terče;
        private int skóre;

        public MainWindow()
        {
            InitializeComponent();

            random = new Random();
            terče = new List<Terč>();
            skóre = 0;

            AktualizovatSkóre();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double velikost = random.Next(20, 100);
            double x = random.Next((int)(velikost / 2), (int)(formulář.ActualWidth - velikost / 2));
            double y = random.Next((int)(velikost / 2), (int)(formulář.ActualHeight - velikost / 2));
            Point umístění = new Point(x, y);

            Terč terč = new Terč(umístění, velikost);
            terč.GrafickýPrvek.MouseLeftButtonDown += Terč_MouseLeftButtonDown;

            formulář.Children.Add(terč.GrafickýPrvek);
            terče.Add(terč);

            DispatcherTimer ztracenýTerčTimer = new DispatcherTimer();
            ztracenýTerčTimer.Interval = TimeSpan.FromSeconds(3); // Časový limit pro zásah terče
            ztracenýTerčTimer.Tick += (s, args) =>
            {
                formulář.Children.Remove(terč.GrafickýPrvek);
                terče.Remove(terč);
                skóre -= 5; // Snížení skóre při "minutí" terče
                AktualizovatSkóre();
                ztracenýTerčTimer.Stop();
            };
            ztracenýTerčTimer.Start();
        }

        private void Terč_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse terč = (Ellipse)sender;
            terč.Fill = Brushes.Gray;

            Terč zasaženýTerč = terče.Find(t => t.GrafickýPrvek == terč);
            zasaženýTerč.Zasažen = true;

            skóre += 10; // Zvýšení skóre při zásahu
            AktualizovatSkóre();

            formulář.Children.Remove(zasaženýTerč.GrafickýPrvek);
            terče.Remove(zasaženýTerč);
        }

        private void AktualizovatSkóre()
        {
            skóreTextBlock.Text = $"Skóre: {skóre}";
        }
    }

    public class Terč
    {
        private Ellipse terč;
        private bool zasažen;

        public Terč(Point umístění, double velikost)
        {
            terč = new Ellipse
            {
                Width = velikost,
                Height = velikost,
                Fill = Brushes.Red,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Margin = new Thickness(umístění.X - velikost / 2, umístění.Y - velikost / 2, 0, 0)
            };

            zasažen = false;
        }

        public UIElement GrafickýPrvek => terč;

        public bool Zasažen
        {
            get { return zasažen; }
            set
            {
                zasažen = value;
                terč.Fill = zasažen ? Brushes.Gray : Brushes.Red;
            }
        }
    }
}
