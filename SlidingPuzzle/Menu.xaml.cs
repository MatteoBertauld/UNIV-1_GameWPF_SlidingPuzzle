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
        private Key toucheAfficherImage = Key.I;
        private Key toucheRecommencer = Key.R;
        private Key touchePause = Key.P;

        string sourceGrilleChiffre = "grilleChiffre.png";
        private bool grilleChiffre = false;

        public bool GrilleChiffre
        {
            get { return grilleChiffre; }
            set { grilleChiffre = value; }
        }

        private TimeSpan tempsLimite = TimeSpan.Zero;
        private int indiceSourceImagePuzzle = 0;
        private bool contreLaMontreActiver = false;

        public TimeSpan TempsLimite
        {
            get { return tempsLimite; }
            set { tempsLimite = value; }
        }

        private bool contreLaMontreActiver = false;

        public bool ContreLaMontreActiver
        {
            get { return contreLaMontreActiver; }
            set { contreLaMontreActiver = value; }
        }

        public int IndiceSourceImagePuzzle
        {
            get { return indiceSourceImagePuzzle; }
            set { indiceSourceImagePuzzle = value; }
        }


        public Menu()
        {
            InitializeComponent();
            musiqueFond.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "son/musique.wav"));
            musiqueFond.Play();
            musiqueFond.MediaEnded += (sender, e) => musiqueFond.Position = TimeSpan.Zero;
            musiqueFond.Volume = choixVolume / 100;

            ImageBrush SkinBoutonQuitter = new ImageBrush();
            SkinBoutonQuitter.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/close-window.png"));
            boutQuitter.Background = SkinBoutonQuitter;

            ImageBrush SkinboutontParametre = new ImageBrush();
            SkinboutontParametre.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/settings.png"));
            boutParam.Background = SkinboutontParametre;

            ImageBrush Skinfond = new ImageBrush();
            Skinfond.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/fond.png"));
            grillePageParametre.Background = Skinfond;
            pagePrincipal.Background = Skinfond;

            ImageBrush SkinbouttonRetour = new ImageBrush();
            SkinbouttonRetour.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/back.png"));
            boutRetour.Background = SkinbouttonRetour;

            ImageBrush SkinbouttonTriche = new ImageBrush();
            SkinbouttonTriche.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/toucheClavier.png"));
            boutTriche.Background = SkinbouttonTriche;
            boutRecommencer.Background = SkinbouttonTriche;
            boutPause.Background = SkinbouttonTriche;
            boutAfficherImage.Background = SkinbouttonTriche;

            ImageBrush skinBoutonJouer = new ImageBrush();
            skinBoutonJouer.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/boutonJouer.png"));
            boutJouer.Background = skinBoutonJouer;

            ImageBrush skinFlecheGauche = new ImageBrush();
            skinFlecheGauche.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/flecheGauche.png"));
            boutChangerImagePuzzleGauche.Background = skinFlecheGauche;

            ImageBrush skinFlecheDroite = new ImageBrush();
            skinFlecheDroite.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/flecheDroite.png"));
            boutChangerImagePuzzleDroit.Background = skinFlecheDroite;


            ChangerImagePuzzle();
        }

        private void ChangerImagePuzzle()
        {
            string source = "";
            ImageBrush SkinImagePuzzle = new ImageBrush();
            if (grilleChiffre) 
            { 
                source = "grilleChiffre.png";
            }
            else
            {
                source = MainWindow.TableauSourceImages[IndiceSourceImagePuzzle];
            }
            
            SkinImagePuzzle.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/FondPuzzle/" + source));
            RectangleImagePuzzle.Fill = SkinImagePuzzle;
        }

        public int Niveau
        {
            get { if (choixDiff == 0) niveau = 9; else if (choixDiff == 1) niveau = 16; else niveau = 25; return niveau; }
        }

        public int Mode
        {
            get { return choixMode; }
        }

        public Key ToucheTriche
        {
            get { return toucheTriche; }
        }

        public Key ToucheAfficherImage
        {
            get { return toucheAfficherImage; }
        }

        public Key ToucheRecommencer
        {
            get { return toucheRecommencer;}
        }
        
        public Key TouchePause
        {
            get { return touchePause; }
        }

        private void BoutJouer_Click(object sender, RoutedEventArgs e)
        {
            choixDiff = comboBoxChoix.SelectedIndex;
            choixMode = 1;
            choixVolume = (int)sliderSon.Value;
            this.DialogResult = true;
        }

        private void BoutQuitter_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BoutParam_Click(object sender, RoutedEventArgs e)
        {
            grillePageParametre.Visibility = Visibility.Visible;
        }

        private void BoutRetour_Clique(object sender, RoutedEventArgs e)
        {
            grillePageParametre.Visibility = Visibility.Hidden;
        }

        private void BoutTriche_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            boutTriche.Content = e.Key.ToString();
            toucheTriche = e.Key;
        }

        private void BoutAfficherImage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            boutAfficherImage.Content = e.Key.ToString();
            toucheAfficherImage = e.Key;
        }

        private void BoutRecommencer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            boutRecommencer.Content = e.Key.ToString();
            toucheRecommencer = e.Key;
        }

        private void BoutPause_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            boutPause.Content = e.Key.ToString();
            touchePause = e.Key;
            choixVolume = (int)sliderSon.Value;
        }

        private void BoutChangerImagePuzzleDroit_Click(object sender, RoutedEventArgs e)
        {
            if (IndiceSourceImagePuzzle == 0)
            {
                IndiceSourceImagePuzzle = 3;
            }
            else
                if (GrilleChiffre)
                {
                    IndiceSourceImagePuzzle = 3;
                }
                GrilleChiffre = !GrilleChiffre;
            } 
            else
            {
                IndiceSourceImagePuzzle -= 1;
            }
            ChangerImagePuzzle();
        }

        private void BoutChangerImagePuzzleGauche_Click(object sender, RoutedEventArgs e)
        {
            if (IndiceSourceImagePuzzle == 3)
            {
                if (GrilleChiffre)
                {
                    IndiceSourceImagePuzzle = 0;
                }
                GrilleChiffre = !GrilleChiffre;
            }
            else
            {
                IndiceSourceImagePuzzle += 1;
            }
            ChangerImagePuzzle();
        }

        private void CheckBoxContreLaMontre_Click(object sender, RoutedEventArgs e)
        {
            if (ContreLaMontreActiver)
            {
                ContreLaMontreActiver = false;
                sliderTemps.Visibility = Visibility.Collapsed;
                textBoxTemps.Visibility = Visibility.Collapsed;
                labelMinutes.Visibility = Visibility.Collapsed;
            }
            else
            {
                ContreLaMontreActiver = true;
                sliderTemps.Visibility = Visibility.Visible;
                textBoxTemps.Visibility = Visibility.Visible;
                labelMinutes.Visibility = Visibility.Visible;
            }
        }
        private void sliderSon_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sliderSon.Value = choixVolume;
        }

        private void sliderTemps_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TempsLimite = TimeSpan.FromMinutes((int)sliderTemps.Value);
        }
    }
}