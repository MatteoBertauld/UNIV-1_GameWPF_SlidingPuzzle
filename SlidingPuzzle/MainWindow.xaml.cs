using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace SlidingPuzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer temps;
        private int compteurTemps = 1;
        // booléens pour aller à gauche et à droite
        // crée une nouvelle instance de la classe dispatch timer
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        int[] valeurGrille;
        Label[] grille;
        int difficulte;
        Button[] boutons;
        ImageBrush[] boutonSkin;
        int minute;
        string mode;
        int taille = (int)Math.Pow(5, 2);



        public MainWindow()
        {
            InitializeComponent();

            Menu fenetreMenu = new Menu();
            fenetreMenu.ShowDialog();
            if (fenetreMenu.DialogResult == false)
                System.Windows.Application.Current.Shutdown();
            else
                difficulte = fenetreMenu.Niveau;

            taille = difficulte;
            InitialiseJeu();

            // configure le Timer et les événements
            // lie le timer du répartiteur à un événement appelé moteur de jeu gameengine
            dispatcherTimer.Tick += GameEngine;
            // rafraissement toutes les 16 milliseconds
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(17);
            // lancement du timer
            dispatcherTimer.Start();


            temps = new DispatcherTimer();            //timer
            temps.Interval = TimeSpan.FromSeconds(1); //timer
            temps.Tick += Timer_Tick;                 //timer
            temps.Start();                            //timer
        }

        private void InitialiseJeu()
        {
            valeurGrille = new int[taille];
            grille = new Label[taille];
            boutons = new Button[taille];
            boutonSkin = new ImageBrush[taille];
            //mode = fenetreMenu.Mode;
            CreationGrille(taille);
            Generation_doubletableau();
            CreerBoutons(taille);
            AffichageGrille();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            labelDebug.Content = "Temps : " + minute + "min" + (compteurTemps++) + "s";
            if ((double)compteurTemps % 60 == 0)
            {
                compteurTemps = 0;
                minute++;

            }
        }
        private void GameEngine(object sender, EventArgs e)
        {
            foreach (Button bout in boutons)
            {
                bout.Click += Clique;
            }
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
                    labelDebug.Content = "colonne zero " + colonne + " ligne " + ligne + "numero " + numero;
                    if (((c2 == colonne - 1 || c2 == colonne + 1) && (l2 == ligne)) || ((l2 == ligne - 1 || l2 == ligne + 1) && (c2 == colonne)))
                    {
                        temp = valeurGrille[numero];
                        valeurGrille[numero] = valeurGrille[num2];
                        valeurGrille[num2] = temp;


                        bout.Tag = "";
                        bout.Visibility = Visibility.Visible;
                        boutons[num2].Background = boutonSkin[valeurGrille[num2]];

                        bouton.Tag = "zero";
                        bouton.Visibility = Visibility.Hidden;
                        boutons[numero].Background = boutonSkin[valeurGrille[numero]];
                    }
                }
            }
        }

        private void CreerBoutons(int taille)
        {
            for (int i = 0; i < taille-1; i++)
            {
                Button test2 = new Button
                {
                    Name = "bouton" + i.ToString(),
                };
                Grid.SetRow(test2, i / (int)Math.Sqrt(taille));
                Grid.SetColumn(test2, i % (int)Math.Sqrt(taille));

                maGrille.Children.Add(test2);
                boutons[i] = test2;
                boutonSkin[i] = new ImageBrush();

                boutonSkin[i].ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/Triopiqueur/5x5/" + i + ".jpg"));
                boutons[i].Background = boutonSkin[i];

                /*
                System.Windows.Controls.Image clipImage = new System.Windows.Controls.Image();
                //Create & Set source
                BitmapImage bi = new BitmapImage();

                bi.BeginInit();
                bi.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/oiseaux.png");
                bi.EndInit();
                clipImage.Source = bi;

                double posx = i / (int)Math.Sqrt(taille) * (int)(clipImage.ActualWidth / Math.Sqrt(taille));
                double posy = i % (int)Math.Sqrt(taille) * (clipImage.ActualHeight / Math.Sqrt(taille));
                double largeur = clipImage.ActualWidth / Math.Sqrt(taille);
                double hauteur = clipImage.ActualHeight / Math.Sqrt(taille);

                //Clip using a rect 
                RectangleGeometry clipRect = new RectangleGeometry { Rect = new Rect(posx, posy, largeur, hauteur) };
                clipImage.Clip = clipRect;
                */
            }

            Button changernom = new Button
            {
                Name = "bouton" + (taille-1).ToString()
            };
            Grid.SetRow(changernom, (taille - 1) / (int)Math.Sqrt(taille));
            Grid.SetColumn(changernom, (taille - 1) % (int)Math.Sqrt(taille));

            maGrille.Children.Add(changernom);
            boutons[(taille - 1)] = changernom;
            boutonSkin[(taille - 1)] = new ImageBrush();

            boutonSkin[(taille - 1)].ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/Triopiqueur/5x5/" + (taille - 1) + ".jpg"));
            boutons[(taille - 1)].Background = boutonSkin[(taille - 1)];

            boutons[(taille - 1)].Tag = "zero";
            boutons[(taille - 1)].Visibility = Visibility.Hidden;
        }



        private void AffichageGrille()
        {
            for (int i = 0; i < grille.Length; i++)
            {
                if (boutons[i].Tag != "zero")
                {
                    boutons[i].Tag = "";
                    //boutons[i].Content = valeurGrille[i].ToString();
                    boutons[i].Background = boutonSkin[valeurGrille[i]];

                }
                else
                {
                    boutons[i].Background = boutonSkin[valeurGrille[i]];
                    //boutons[i].Content = "";
                    boutons[i].Tag = "zero";
                    boutons[i].Visibility = Visibility.Hidden;
                }
            };
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


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Aide image = new Aide();
            image.ShowDialog();
        }
    }
}
