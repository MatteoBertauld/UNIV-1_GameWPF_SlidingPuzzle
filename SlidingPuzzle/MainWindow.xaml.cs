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
        // booléens pour aller à gauche et à droite
        private bool goLeft, goRight, jump, jumpPhase1, boolScore = false;
        // crée une nouvelle instance de la classe dispatch timer
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();

        int[] valeurGrille;
        Label[] grille;
        int difficulte;
        Button[] boutons;
        ImageBrush[] boutonSkin;

        int taille = (int)Math.Pow(5,2);



        public MainWindow()
        {
            InitializeComponent();
            Menu FenetreMenu = new Menu();
            FenetreMenu.ShowDialog();
            if (FenetreMenu.DialogResult == false)
           
            Menu fenetreMenu = new Menu();
            fenetreMenu.ShowDialog();
            if (fenetreMenu.DialogResult == false)
                Application.Current.Shutdown();
                System.Windows.Application.Current.Shutdown();
            else
                difficulte = fenetreMenu.Niveau;


            valeurGrille = new int[taille];
            grille = new Label[taille];
            boutons = new Button[taille];
            boutonSkin = new ImageBrush[taille];

            CreationGrille(taille);
            Generation_doubletableau();
            CreerBoutons(taille);

            AffichageGrille();
            // configure le Timer et les événements
            // lie le timer du répartiteur à un événement appelé moteur de jeu gameengine
            dispatcherTimer.Tick += GameEngine;
            // rafraissement toutes les 16 milliseconds
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(17);
            // lancement du timer
            dispatcherTimer.Start();
        }

        private void GameEngine(object sender, EventArgs e)
        {
        }

        private void CreationGrille(int taille)
        {
            //maGrille.ShowGridLines = true;

            for(int i = 0; i < Math.Sqrt(taille); i++)
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

            int colonne = Grid.GetColumn(bouton);
            int ligne = Grid.GetRow(bouton);
            int numero = colonne * (int)Math.Sqrt(taille) + ligne;

            foreach (Button bout in boutons)
            {
                int c2 = Grid.GetColumn(bout);
                int l2 = Grid.GetRow(bout);
                int num2 = c2 * (int)Math.Sqrt(taille) + l2;

                if (bout.Tag == "zero")
                {
                    if (( (c2 == colonne - 1 || c2 == colonne + 1) && (l2 == ligne) ) || ( (l2 == ligne - 1 || l2 == ligne + 1) && (c2 == colonne) ) )
                    {
                        bout.Tag = "";
                        //bout.Content = bouton.Content;
                        bout.Visibility = Visibility.Visible;
                        boutons[num2].Background = boutonSkin[numero];

                        bouton.Visibility = Visibility.Hidden;
                        //bouton.Content = "";
                        bouton.Tag = "zero";
                        boutons[numero].Background = boutonSkin[num2];
                    }
                }
            }






        }

        private void CreerBoutons(int taille)
        {
            for (int i = 0; i < taille; i++)
            {
                Button test2 = new Button
                {
                    Name = "bouton" + i.ToString(),
                    Height = 60,
                    Width = 60,
                };
                test2.VerticalAlignment = VerticalAlignment.Center;
                test2.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(test2, i / (int)Math.Sqrt(taille));
                Grid.SetColumn(test2, i % (int)Math.Sqrt(taille));

                test2.Click += Clique;

                maGrille.Children.Add(test2);
                boutons[i] = test2;
                boutonSkin[i] = new ImageBrush();
                boutonSkin[i].ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/Triopiqueur/5x5/" + i + ".jpg"));
                boutons[i].Background = boutonSkin[i];
            }
            debug.Content = "debug:\n" + difficulte;
        }



        private void AffichageGrille()
        {
            for(int i = 0 ; i < grille.Length ; i++)
            {
                if (valeurGrille[i] != 0) 
                {
                    boutons[i].Tag = "";
                    //boutons[i].Content = valeurGrille[i].ToString();
                    boutons[i].Background = boutonSkin[i];

                } else 
                {
                    boutons[i].Background = boutonSkin[i];
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
            for (int i = 0 ;i< taille; i++)
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
    
        private void CanvasKeyIsDown(object sender, KeyEventArgs e)
        {
        }

        private void CanvasKeyIsUp(object sender, KeyEventArgs e)
        {
        }
    }
}
