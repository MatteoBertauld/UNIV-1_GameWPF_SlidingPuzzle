using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SlidingPuzzle
{
    /// <summary>
    /// Logique d'interaction pour Aide.xaml
    /// </summary>
    public partial class Aide : Window
    {
        public Aide()
        {
            InitializeComponent();
            ImageBrush canvaFond = new ImageBrush();
            canvaFond.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/oiseaux.png"));
            rectAide.Fill = canvaFond;
        }
    }
}
