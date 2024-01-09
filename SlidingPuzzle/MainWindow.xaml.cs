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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

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

        int[] valeurGrille = new int[9];
        Label[] grille = new Label[9];


        public MainWindow()
        {
            
            InitializeComponent();
           
            Menu FenetreMenu = new Menu();
            FenetreMenu.ShowDialog();
            if (FenetreMenu.DialogResult == false)            
                Application.Current.Shutdown();
            Generation_doubletableau(ref valeurGrille);


            CreerCase(ref grille);
            AffichageGrille(ref grille, valeurGrille);
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

        private void CreerCase(ref Label[] grille)
        {
            int compteur = 0;
            for (int i = 0; i < 9; i++)
            { 
                Label test = new Label
                {
                    Name = "case" + Convert.ToString(i),
                    Content = "x",
                };
                Canvas.SetTop(test, 80 + i/3*100);
                Canvas.SetLeft(test, 250 + i%3*100);

                grille[i] = test;
                compteur += 1;

                myCanvas.Children.Add(test);
            }
            debug.Content = "debug:\n" + compteur;
        }

        private void AffichageGrille(ref Label[] grille, int[] valeurGrille)
        {
            for(int i = 0 ; i < grille.Length ; i++)
            {
                grille[i].Content = valeurGrille[i];
            };
        }

        private void Generer_Click(object sender, RoutedEventArgs e)
        {
            Generation_doubletableau(ref valeurGrille);
            AffichageGrille(ref grille, valeurGrille);
        }

    
        private void CanvasKeyIsDown(object sender, KeyEventArgs e)
        {
            // on gère les booléens gauche et droite en fonction de l’appui de la touche
            if (e.Key == Key.Left)
            {
                goLeft = true;
            }
            if (e.Key == Key.Right)
            {
                goRight = true;
            }

            if (e.Key == Key.Space)
            {
                jump = true;
                jumpPhase1 = true;
            }
        }

        private void CanvasKeyIsUp(object sender, KeyEventArgs e)
        {
            // on gère les booléens gauche et droite en fonction du relâchement de la touche
            if (e.Key == Key.Left)
            {
                goLeft = false;
            }
            if (e.Key == Key.Right)
            {
                goRight = false;
            }
        }


        public static void  Generation_doubletableau(ref int[] grille)
        {
            int indice = 0;
            Random alea = new Random();
            List<int> nombreDisponible = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            int longueur = nombreDisponible.Count;

            for (int i = 0; i < grille.Length; i++)
            {
                if (longueur > 0)
                {
                    indice = alea.Next(0, longueur);
                }

                Console.WriteLine("valeur = " + nombreDisponible[indice] + " indice " + indice + " longueur " + longueur);

                grille[i] = nombreDisponible[indice];
                nombreDisponible.RemoveAt(indice);
                longueur -= 1;
            }
        }
    }
}
