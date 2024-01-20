using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Numerics;
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
    /// Logique d'interaction pour Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        private MediaPlayer musiqueFond = new MediaPlayer();
        private int choixDiff;
        private int choixMode;
        private int choixVolume;
        private int niveau;
        private Key toucheTriche = Key.C;


        public Menu()
        {
            InitializeComponent();
            musiqueFond.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "son/musique.wav"));
            musiqueFond.Play();
            musiqueFond.MediaEnded += (sender, e) => musiqueFond.Position = TimeSpan.Zero;
            musiqueFond.Volume = choixVolume / 100;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            choixDiff = comboBoxChoix.SelectedIndex;
            choixMode = comboBoxMode.SelectedIndex;
            choixVolume = (int)sliderSon.Value;
            this.DialogResult = true;
        }

        public int Niveau
        {
            get { if (choixDiff == 0) niveau = 9; else if (choixDiff == 1) niveau = 16; else niveau = 25; return niveau; }
        }

        public int Mode
        {
            get { return choixMode; }
        }


        private void quitter_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            canvaParam.Visibility = Visibility.Visible;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            canvaParam.Visibility = Visibility.Hidden;

        }


        public Key ToucheTriche
        {
            get { return toucheTriche; }
        }

        private void butTriche_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            butTriche.Content = e.Key.ToString();
            toucheTriche = e.Key;
        }

        private void sliderSon_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sliderSon.Value = choixVolume;
        }

    }
}
