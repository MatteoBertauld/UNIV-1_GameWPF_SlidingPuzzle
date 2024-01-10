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

        int[] valeurGrille;
        Label[] grille;
        int taille = (int)Math.Pow(15,2);
        int[] valeurGrille = new int[9];
        Label[] grille = new Label[9];
        int difficulte;


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
            else
                difficulte = fenetreMenu.Niveau;


            valeurGrille = new int[taille];
            grille = new Label[taille];

            CreationGrille(taille);
            Generation_doubletableau();
            CreerCase(taille);
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

        private void CreerCase(int taille)
        {
            for (int i = 0; i < taille; i++)
            {
                Label test = new Label
                {
                    Name = "case" + Convert.ToString(i),
                    Content = "x",
                    FontSize = 18,
                };
                test.VerticalAlignment = VerticalAlignment.Center;
                test.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(test, i/((int)Math.Sqrt(taille)));
                Grid.SetColumn(test, i%((int)Math.Sqrt(taille)));
                //Canvas.SetTop(test, 80 + i/((int)Math.Sqrt(taille))*100);
                //Canvas.SetLeft(test, 250 + i%((int)Math.Sqrt(taille))*100);

                grille[i] = test;
                maGrille.Children.Add(test);
            }
            debug.Content = "debug:\n" + difficulte;
        }

        private void AffichageGrille()
        {
            for(int i = 0 ; i < grille.Length ; i++)
            {
                grille[i].Content = valeurGrille[i];
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

        private void Generer_Click(object sender, RoutedEventArgs e)
        {
            Generation_doubletableau();
            AffichageGrille();
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
    }
}
