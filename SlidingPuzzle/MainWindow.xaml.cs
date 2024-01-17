using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;

namespace SlidingPuzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImageBrush fondMenu = new ImageBrush();


        private DispatcherTimer temps;
        private Menu fenetreMenu = new Menu();
        private string toucheTriche;
        private int compteurTemps = 1;
        // crée une nouvelle instance de la classe dispatch timer
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        int[] valeurGrille;
        Label[] grille;
        int difficulte;
        Button[] boutons;
        //ImageBrush[] boutonSkin;
        System.Windows.Controls.Image[] ListeImages;
        System.Windows.Controls.Image[] ListeImagesTrier;
        int minute;
        string mode;
        int taille = (int)Math.Pow(5, 2);
        int taille = (int)Math.Pow(5, 2);

        bool voirImage = false;
        bool voirImageNouvelleFenetre = false;
        bool voirImageNouvelleFenetre = false;

        public MainWindow()
        {
            fondMenu.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/fond.png"));
            maGrille.Background = fondMenu;

            toucheTriche = fenetreMenu.ToucheTriche;

            Menu fenetreMenu = new Menu();
            Menu fenetreMenu = new Menu();
            fenetreMenu.ShowDialog();
            if (fenetreMenu.DialogResult == false)
            {
                System.Windows.Application.Current.Shutdown();

            }

            else
            {
                //mode = fenetreMenu.Mode;
                difficulte = fenetreMenu.Niveau;
            }

            taille = difficulte;
            InitialiseJeu();
            temps = new DispatcherTimer();            //timer
            temps.Interval = TimeSpan.FromSeconds(1); //timer
            temps.Tick += Timer_Tick;                 //timer
            temps.Start();                            //timer
            foreach (Button bout in boutons)
            {
                bout.Click += Clique;
            }
        }
            ListeImagesTrier = new System.Windows.Controls.Image[taille];
            //mode = fenetreMenu.Mode;
            CreationGrille(taille);
            Generation_doubletableau();
            CreerBoutons(taille);
            AffichageGrille();
            //boutonSkin = new ImageBrush[taille];
            ListeImages = new System.Windows.Controls.Image[taille];
            ListeImagesTrier = new System.Windows.Controls.Image[taille];
            //mode = fenetreMenu.Mode;
        {
            labelDebug.Content = "Temps : " + minute + "min" + (compteurTemps++) + "s";
            if ((double)compteurTemps % 60 == 0)
            {
                compteurTemps = 0;
                minute++;

            labTemps.Content = "Temps : " + minute + "min" + (compteurTemps++) + "s";
            if ((double)compteurTemps % 60 == 0)

        private void TouchePresser(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C)
            {
                Cheat();

        private void TouchePresser(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C)
            {
                Cheat();
            }
            {
                voirImage = !voirImage;
            }

            if (e.Key == Key.I)
            {
                voirImageNouvelleFenetre = !voirImageNouvelleFenetre;
            }
        }

        private void Cheat()
        {
            string chaine = "";
            for(int i = 0;i< valeurGrille.Length;i++)
            {
                valeurGrille[i] = i;
                chaine += valeurGrille[i];
            }
            labelDebug.Content = chaine;
            AffichageGrille();
        }


        private void CreationGrille(int taille)
        {
            //maGrille.ShowGridLines = true;
            maGrille.ColumnDefinitions.Clear();
            maGrille.RowDefinitions.Clear();

            for (int i = 0; i < Math.Sqrt(taille); i++)
            {
                ColumnDefinition colone = new ColumnDefinition();
                maGrille.ColumnDefinitions.Add(colone);

                RowDefinition ligne = new RowDefinition();
                maGrille.RowDefinitions.Add(ligne);
            }
        }

        private void Clique(object sender, EventArgs e)
        {
            Button bouton = sender as Button;

            int temp;
            int colonne = Grid.GetColumn(bouton);
            int ligne = Grid.GetRow(bouton);
            int numero = ligne * (int)Math.Sqrt(taille) + colonne;
            //labelDebug.Content = "colonne bouton " + colonne + " ligne " + ligne + "numero " + numero;

            foreach (Button bout in boutons)
            {
                int c2 = Grid.GetColumn(bout);
                int l2 = Grid.GetRow(bout);
                int num2 = l2 * (int)Math.Sqrt(taille) + c2;

                if (bout.Tag == "zero")
                {
                    //labelDebug.Content = "colonne zero " + colonne + " ligne " + ligne + "numero " + numero;
                    if (((c2 == colonne - 1 || c2 == colonne + 1) && (l2 == ligne)) || ((l2 == ligne - 1 || l2 == ligne + 1) && (c2 == colonne)))
                    {
                        temp = valeurGrille[numero];
                        valeurGrille[numero] = valeurGrille[num2];
                        valeurGrille[num2] = temp;

                        Grid.SetRow(ListeImages[valeurGrille[num2]], l2);
                        Grid.SetColumn(ListeImages[valeurGrille[num2]], c2);

                        bout.Tag = "";
                        bout.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 255, 255, 255));
                        Panel.SetZIndex(bout, 1);

                        bouton.Tag = "zero";
                        bouton.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 255, 255));
                        Panel.SetZIndex(bouton, 1);
                    }
                }
            }
            Victoire();

        }

        private void CreerBoutons(int taille)
        {

            for (int i = 0; i < taille; i++)
            {
                Button test2 = new Button
                {
                    Name = "bouton" + i.ToString(),
                };
                Grid.SetRow(test2, i / (int)Math.Sqrt(taille));
                Grid.SetColumn(test2, i % (int)Math.Sqrt(taille));

                maGrille.Children.Add(test2);
                boutons[i] = test2;
                boutons[i].Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0));

                boutons[i].Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0));


                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/oiseaux.png");
                bitmapImage.EndInit();

                System.Windows.Controls.Image croppedImage = new System.Windows.Controls.Image();
 
                // Définir les coordonnées de découpe (x, y, largeur, hauteur)
                int x = (i % (int)Math.Sqrt(taille)) * (int)(bitmapImage.Width / (int)Math.Sqrt(taille));
                int y = (i / (int)Math.Sqrt(taille)) * (int)(bitmapImage.Height / (int)Math.Sqrt(taille));
                int largeur = (int)(bitmapImage.Width / (int)Math.Sqrt(taille));
                int hauteur = (int)(bitmapImage.Height / (int)Math.Sqrt(taille));

                CroppedBitmap croppedBitmap = new CroppedBitmap(bitmapImage, new Int32Rect(x, y, largeur, hauteur));
                croppedImage.Source = croppedBitmap;
               
                maGrille.Children.Add(croppedImage);
                croppedImage.Stretch = Stretch.Fill;

                ListeImages[i] = croppedImage;
                ListeImagesTrier[i] = croppedImage;

                Grid.SetRow(croppedImage, i / (int)Math.Sqrt(taille));
                Grid.SetColumn(croppedImage, i % (int)Math.Sqrt(taille));
                bitmapImage.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/oiseaux.png");
                bitmapImage.EndInit();

                System.Windows.Controls.Image croppedImage = new System.Windows.Controls.Image();
 
                // Définir les coordonnées de découpe (x, y, largeur, hauteur)
                int x = (i % (int)Math.Sqrt(taille)) * (int)(bitmapImage.Width / (int)Math.Sqrt(taille));
                int y = (i / (int)Math.Sqrt(taille)) * (int)(bitmapImage.Height / (int)Math.Sqrt(taille));
                int largeur = (int)(bitmapImage.Width / (int)Math.Sqrt(taille));
                int hauteur = (int)(bitmapImage.Height / (int)Math.Sqrt(taille));

                CroppedBitmap croppedBitmap = new CroppedBitmap(bitmapImage, new Int32Rect(x, y, largeur, hauteur));
                croppedImage.Source = croppedBitmap;
               
                maGrille.Children.Add(croppedImage);
                croppedImage.Stretch = Stretch.Fill;

                ListeImages[i] = croppedImage;
                ListeImagesTrier[i] = croppedImage;

                Grid.SetRow(croppedImage, i / (int)Math.Sqrt(taille));
                Grid.SetColumn(croppedImage, i % (int)Math.Sqrt(taille));
            }
            boutons[taille-1].Tag = "zero";
            boutons[taille-1].Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 255, 255));
        }



        private void AffichageGrille()
        {
            System.Windows.Controls.Image imageTemp;

            for (int i = 0; i < grille.Length; i++)
            {
                if (boutons[i].Tag != "zero")
                {
                    boutons[i].Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 255, 255, 255));
                    Panel.SetZIndex(boutons[i], 1);
                    Grid.SetRow(ListeImages[i], valeurGrille[i] / (int)Math.Sqrt(taille));
                    Grid.SetColumn(ListeImages[i], valeurGrille[i] % (int)Math.Sqrt(taille));
                }
                else
                {
                    boutons[i].Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 255, 255));
                    Panel.SetZIndex(boutons[i], 1);
                }
            };
        }

        private void Victoire()
        {
            bool testVictoire = false;
            for (int i = 0; i < grille.Length; i++)
            {
                if (valeurGrille[i] != i)
                {
                    testVictoire = false;
                }
            }

            if (testVictoire == true)
            {
                Victoire page = new Victoire();
                page.Show();
            }
        }
        

        private void Generation_doubletableau()
        {
            int indice = 0;
            Random alea = new Random();
            List<int> nombreDisponible = new List<int>();
            for (int i = 0; i < taille; i++)
            {
                nombreDisponible.Add(i);
            }

            for (int i = 0; i < valeurGrille.Length; i++)
            {
                if (nombreDisponible.Count > 0)
                {
                    indice = alea.Next(0, nombreDisponible.Count);
                }

                //debug.Content = "\nvaleur = " + nombreDisponible[indice] + "\nindice " + indice + "\nlongueur " + nombreDisponible.Count;

                valeurGrille[i] = nombreDisponible[indice];
                nombreDisponible.RemoveAt(indice);
            }
        }

        public void GenererNouvelleGrille()
        {
            Generation_doubletableau();
            AffichageGrille();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Menu retour = new Menu();
            retour.ShowDialog();
            if (retour.DialogResult == false)
                System.Windows.Application.Current.Shutdown();
            else
                difficulte = retour.Niveau;
            taille = difficulte;
            InitialiseJeu();
        }


        private void BoutonVoirImage(object sender, RoutedEventArgs e)
        {
            Aide image = new Aide();
            image.ShowDialog();
        }
        private void Triche()
        {
            string chaine = "";
            for (int i = 0; i< valeurGrille.Length; i++)
            {
                valeurGrille[i] = i;
                chaine += valeurGrille[i];
            }
            labelDebug.Content= chaine;
            AffichageGrille();
        }
        private void maGrille_KeyDown(object sender, KeyEventArgs e)
        {
            /*if (e.Key.ToString() == toucheTriche || e.Key == Key.C)
                Triche();*/
            if (e.Key == Key.C)
            {
                Triche();
            }
        }
        
    }
}
