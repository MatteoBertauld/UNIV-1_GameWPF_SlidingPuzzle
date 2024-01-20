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
using System.Timers;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;


namespace SlidingPuzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImageBrush fondMenu = new ImageBrush();

        private System.Timers.Timer minuteur;
        private TimeSpan tempsRestant;

        private DispatcherTimer temps; // timer
        private Aide image = new Aide();
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private Defaite pageDefaite = new Defaite();
        private Menu fenetreMenu = new Menu();
        private Key keyTriche = Key.C;
        private int compteurTemps = 1;
        int[] valeurGrille;
        //Label[] grille;
        int difficulte;
        Button[] boutons;
        System.Windows.Controls.Image[] ListeImages;
        int minute;
        int mode;
        int taille = (int)Math.Pow(5, 2);
        int compteur = 0;
        bool voirImage = false;
        bool voirImageNouvelleFenetre = false;

        public MainWindow()
        {

            InitializeComponent();
            //fondMenu.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/fond.png"));
            //maGrille.Background = fondMenu;

            Console.WriteLine(fenetreMenu.ToucheTriche.ToString());

            fenetreMenu.ShowDialog();
            if (fenetreMenu.DialogResult == false)
            {
                Application.Current.Shutdown();
            }
            else
            {
                mode = fenetreMenu.Mode;
                difficulte = fenetreMenu.Niveau;
            }

            if (mode == 0)
            {
                //temps.Interval = TimeSpan.FromSeconds(1); //timer
                //temps.Tick += Timer_Tick;                 //timer
                //temps.Start();
            }

            taille = difficulte;

            InitialiseJeu();
        }

        public void ListButtons(DependencyObject parent)
        {

            // Parcourir tous les éléments enfants
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is Button button)
                {
                    // Si l'élément est un bouton, afficher son contenu
                    Console.WriteLine($"Bouton trouvé : {button.Content}");
                    compteur += 1;
                }

                // Récursivement appeler la fonction pour les enfants de cet élément
                ListButtons(child);
            }
        }

        public void ListButtonsInActiveWindow()
        {
            // Obtenir la fenêtre active
            var activeWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);

            if (activeWindow != null)
            {
                compteur = 0;
                // Lister tous les boutons dans la fenêtre active
                ListButtons(activeWindow);
                Console.WriteLine(compteur);

            }
            else
            {
                Console.WriteLine("Aucune fenêtre active trouvée.");
            }
        }


        private void InitialiseJeu()
        {
            valeurGrille = new int[taille];
            //grille = new Label[taille];
            boutons = new Button[taille];
            ListeImages = new System.Windows.Controls.Image[taille];
            //mode = fenetreMenu.Mode;
            CreationGrille();
            Generation_doubletableau();
            CreerBoutons();
            AffichageGrille();

            foreach (Button bout in boutons)
            {
                bout.Click += Clique;
            }
            ListButtonsInActiveWindow();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            labTemps.Content = "Temps : " + minute + "min" + (compteurTemps++) + "s";
            if ((double)compteurTemps % 60 == 0)
            {
                compteurTemps = 0;
                minute++;
            }
        }


        private void Triche()
        {
            string chaine = "";
            for (int i = 0; i < valeurGrille.Length; i++)
            {
                valeurGrille[i] = i;
                chaine += valeurGrille[i];
            }
            labelDebug.Content = chaine;
            AffichageGrille();
        }


        private void CreationGrille()
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

        private void DebugAffichageConsoleGrille()
        {
            for (int i = 0; i < taille; i++)
            {
                Console.Write(valeurGrille[i] + ",");
            }

            string chaine = "\n";

            int racineTaille = (int)Math.Sqrt(taille);

            for (int colonne = 0; colonne < racineTaille; colonne++)
            {
                for (int ligne = 0; ligne < racineTaille; ligne++)
                {
                    chaine += valeurGrille[colonne * racineTaille + ligne] + ",";
                }
                chaine += "\n";
            }
            Console.WriteLine(chaine);
        }

        private void Clique(object sender, EventArgs e)
        {
            Button bouton = sender as Button;

            int temp;
            int colonne = Grid.GetColumn(bouton);
            int ligne = Grid.GetRow(bouton);
            int numero = ligne * (int)Math.Sqrt(taille) + colonne;
            Console.WriteLine("Bouton cliquer : colonne " + colonne + " ligne " + ligne + "numero " + numero + " ValeurGrille " + valeurGrille[numero]);

            foreach (Button bout in boutons)
            {
                int c2 = Grid.GetColumn(bout);
                int l2 = Grid.GetRow(bout);
                int num2 = l2 * (int)Math.Sqrt(taille) + c2;
                Console.WriteLine("colonne " + c2 + " ligne " + l2 + " numero " + num2 + " ValeurGrille " + valeurGrille[num2]);

                if (bout.Tag == "zero")
                {
                    Console.WriteLine("zero");

                    if (((c2 == colonne - 1 || c2 == colonne + 1) && (l2 == ligne)) || ((l2 == ligne - 1 || l2 == ligne + 1) && (c2 == colonne)))
                    {

                        Console.WriteLine("\nValeurs de la grille AVANT changement");
                        DebugAffichageConsoleGrille();

                        temp = valeurGrille[numero];
                        valeurGrille[numero] = valeurGrille[num2];
                        valeurGrille[num2] = temp;

                        Console.WriteLine("\nValeurs de la grille APRES changement");
                        DebugAffichageConsoleGrille();

                        Console.WriteLine();
                        Console.WriteLine("numero : " + valeurGrille[numero] + " / num : " + valeurGrille[num2] + "\n");

                        bout.Tag = null;
                        bouton.Tag = "zero";
                    }
                }
            }
            AffichageGrille();
            Victoire();
        }

        private void CreerBoutons()
        {
            for (int i = 0; i < taille; i++)
            {
                Button test2 = new Button
                {
                    Name = "bouton" + i.ToString(),
                    Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)),
                    Tag = null,
                };
                Grid.SetRow(test2, i / (int)Math.Sqrt(taille));
                Grid.SetColumn(test2, i % (int)Math.Sqrt(taille));

                maGrille.Children.Add(test2);
                boutons[i] = test2;
                Panel.SetZIndex(boutons[i], 1);


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
            }
            boutons[taille - 1].Tag = "zero";
            AffichageGrille();
        }



        private void AffichageGrille()
        {
            for (int i = 0; i < taille; i++)
            {
                //Panel.SetZIndex(boutons[i], 1);
                Grid.SetRow(ListeImages[valeurGrille[i]], i / (int)Math.Sqrt(taille));
                Grid.SetColumn(ListeImages[valeurGrille[i]], i % (int)Math.Sqrt(taille));

                if (boutons[i].Tag == "zero")
                {
                    boutons[i].Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 255, 255));
                }
                else
                {
                    boutons[i].Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0));
                }
            }
        }

        private void Victoire()
        {
            bool testVictoire = false;
            for (int i = 0; i < taille; i++)
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
            foreach (int i in nombreDisponible) { Console.Write(i + ","); }
            Console.WriteLine("");

            for (int i = 0; i < valeurGrille.Length; i++)
            {
                if (nombreDisponible.Count > 0)
                {
                    indice = alea.Next(0, nombreDisponible.Count);
                }

                valeurGrille[i] = nombreDisponible[indice];
                nombreDisponible.RemoveAt(indice);
            }

            DebugAffichageConsoleGrille();
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
            image.Show();
        }

        private void maGrille_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine(keyTriche.ToString());
            if (e.Key == fenetreMenu.ToucheTriche)
            {
                Triche();
            }
            if (e.Key == Key.V)
            {
                voirImage = !voirImage;
            }

            if (e.Key == Key.I)
            {
                voirImageNouvelleFenetre = !voirImageNouvelleFenetre;
            }
        }

        private void butVoirImage_DragOver(object sender, DragEventArgs e)
        {
            image.Show();
        }
    }
}
