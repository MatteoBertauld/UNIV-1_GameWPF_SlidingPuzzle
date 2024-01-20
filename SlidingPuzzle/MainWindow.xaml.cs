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
using System.Threading;


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
        //private Defaite pageDefaite = new Defaite();
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
        int taille;
        int DebugNombreBouton;
        int DebugNombreImage;
        int DebugNombreBoutonSupprimer;
        int DebugNombreImageSupprimer;
        int TestPrecedent = 0;
        bool voirImage = false;
        bool voirImageNouvelleFenetre = false;

        string[] listesSourceimages = new string[4] { "oiseaux.png", "artAbstrait.jpg", "info.jpg", "lac.jpg" };
        private static string sourceImage = "oiseaux.png";


        public static string SourceImage
        {
            get { return sourceImage; }
        }


        public MainWindow()
        {
            
            InitializeComponent();

            ImageBrush SkinMaison = new ImageBrush();
            SkinMaison.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/hut.png"));
            Maison.Background = SkinMaison;

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
                    DebugNombreBouton += 1;
                }
                if (child is Image img)
                {
                    DebugNombreImage += 1;
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
                DebugNombreImage = 0;
                DebugNombreBouton = 0;
                // Lister tous les boutons dans la fenêtre active
                ListButtons(activeWindow);
                Console.WriteLine("Nombre d'image trouvé :" + DebugNombreImage);
                Console.WriteLine("Nombre de Bouton trouvé :" + DebugNombreBouton);
            }
            else
            {
                Console.WriteLine("Aucune fenêtre active trouvée.");
            }
        }

        private void SupprimeObjectDeLaGrille()
        {
            DebugNombreBoutonSupprimer = 0;
            foreach (Button bout in boutons)
            {
                if (maGrille.Children.Contains(bout))
                {
                    DebugNombreBoutonSupprimer += 1;
                    maGrille.Children.Remove(bout);
                }
            }
            Console.WriteLine("Nombre de bouton supprimer de la grille " + DebugNombreBoutonSupprimer);


            DebugNombreImageSupprimer = 0;
            foreach (Image img in ListeImages)
            {
                if (maGrille.Children.Contains(img))
                {
                    DebugNombreBoutonSupprimer += 1;
                    maGrille.Children.Remove(img);
                }

            }
            Console.WriteLine("Nombre d'image supprimer de la grille " + DebugNombreImageSupprimer);
        }



        private void InitialiseJeu()
        {
            valeurGrille = new int[taille];
            boutons = new Button[taille];
            ListeImages = new System.Windows.Controls.Image[taille];
            Random alea = new Random();
            sourceImage = listesSourceimages[alea.Next(0, 4)];

            //mode = fenetreMenu.Mode;
            CreationGrille();
            Generation_doubletableau();
            CreerBoutons();
            ListButtonsInActiveWindow();
            MelangerGrille();
            DebugAffichageConsoleGrille();
            AffichageGrille();

            foreach (Button bout in boutons)
            {
                bout.Click += Clique;
            }
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
            for(int i = 0;i< valeurGrille.Length;i++)
            {
                boutons[i].Tag = null;
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

            Console.WriteLine("Bouton cliquer : colonne " + colonne + " ligne " + ligne + " numero " + numero + " ValeurGrille " + valeurGrille[numero]);

            
            foreach (Button bout in boutons)
            {
                int c2 = Grid.GetColumn(bout);
                int l2 = Grid.GetRow(bout);
                int num2 = l2 * (int)Math.Sqrt(taille) + c2;

                if (bout.Tag == "zero")
                {
                    Console.WriteLine("colonne " + c2 + " ligne " + l2 + " numero " + num2 + " ValeurGrille " + valeurGrille[num2]);

                    if (((c2 == colonne - 1 || c2 == colonne + 1) && (l2 == ligne)) || ((l2 == ligne - 1 || l2 == ligne + 1) && (c2 == colonne)))
                    {

                        temp = valeurGrille[numero];
                        valeurGrille[numero] = valeurGrille[num2];
                        valeurGrille[num2] = temp;

                        Console.WriteLine("\nValeurs de la grille");
                        DebugAffichageConsoleGrille();

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
                bitmapImage.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/FondPuzzle/" + sourceImage);
                bitmapImage.EndInit();

                Image croppedImage = new();
                //bitmapImage.Width
                //Console.WriteLine("taille" + bitmapImage.PixelHeight);
                // Définir les coordonnées de découpe (x, y, largeur, hauteur)
                int x = (i % (int)Math.Sqrt(taille)) * (int)(bitmapImage.PixelWidth / (int)Math.Sqrt(taille));
                int y = (i / (int)Math.Sqrt(taille)) * (int)(bitmapImage.PixelHeight / (int)Math.Sqrt(taille));
                int largeur = (int)(bitmapImage.PixelWidth / (int)Math.Sqrt(taille));
                int hauteur = (int)(bitmapImage.PixelHeight / (int)Math.Sqrt(taille));
                CroppedBitmap croppedBitmap = new CroppedBitmap(bitmapImage, new Int32Rect(x, y, largeur, hauteur));
                croppedImage.Source = croppedBitmap;


                maGrille.Children.Add(croppedImage);
                croppedImage.Stretch = Stretch.Fill;
                ListeImages[i] = croppedImage;
            }
            boutons[taille-1].Tag = "zero";
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
                    boutons[i].Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255)); 
                }
                else
                {
                    boutons[i].Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0));
                }
            }
        }

        private void Victoire()
        {
            bool testVictoire = true;
            for (int i = 0; i < taille; i++)
            {
                if (valeurGrille[i] != i)
                {
                    testVictoire = false;
                }
            }

            if (testVictoire == true)
            {
                //Victoire page = new Victoire();
                //page.Show();
            }
        }
        
        private int coupPossible(int numero,int numeroCoupPrecedent)
        {
            int colonne = numero % (int)Math.Sqrt(taille);
            int ligne = numero / (int)Math.Sqrt(taille);

            List<int> ListeCoupPossible = new List<int>();

            if ( colonne > 0 )
            {
                int numCoup = ligne * (int)Math.Sqrt(taille) + (colonne - 1);
                if (numCoup != numeroCoupPrecedent)
                {
                    ListeCoupPossible.Add(numCoup);
                }
            }
            if (ligne > 0)
            {
                int numCoup = (ligne - 1) * (int)Math.Sqrt(taille) + colonne;
                if (numCoup != numeroCoupPrecedent)
                {
                    ListeCoupPossible.Add(numCoup);
                }
            }

            if (colonne < (int)Math.Sqrt(taille)-1)
            {
                int numCoup = ligne * (int)Math.Sqrt(taille) + (colonne + 1);
                if (numCoup != numeroCoupPrecedent)
                {
                    ListeCoupPossible.Add(numCoup);
                }
            }

            if (ligne < (int)Math.Sqrt(taille)-1)
            {
                int numCoup = (ligne +1) * (int)Math.Sqrt(taille) + colonne;
                if (numCoup != numeroCoupPrecedent)
                {
                    ListeCoupPossible.Add(numCoup);
                }
            }
            Random alea = new Random();

            int indice = alea.Next(0, ListeCoupPossible.Count);
            //Console.WriteLine("coup Jouer " + ListeCoupPossible[indice]);
            return ListeCoupPossible[indice];
        }

        private void MelangerGrille()
        {
            int numCaseZero = taille-1;
            int numCoupPrecedent = taille - 1;
            for (int i = 0; i < taille; i++)
            {
                if (boutons[i].Tag == "zero")
                {
                    numCaseZero = i;
                    numCoupPrecedent = numCaseZero;
                }
            }
            int temp;

            Random alea = new Random();
            int nombreCoup = alea.Next(81, 100);


            for (int i = 0;i < nombreCoup; i++)
            {
                temp = numCaseZero;
                numCaseZero = coupPossible(numCaseZero, numCoupPrecedent);
                numCoupPrecedent = temp;
                boutons[numCaseZero].Tag = "zero";
                boutons[numCoupPrecedent].Tag = null;


                temp = valeurGrille[numCaseZero];
                valeurGrille[numCaseZero] = valeurGrille[numCoupPrecedent];
                valeurGrille[numCoupPrecedent] = temp;
            }
            AffichageGrille();
        }

        private void Generation_doubletableau()
        {
            for (int i = 0; i < taille; i++)
            {
                valeurGrille[i] = i;
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SupprimeObjectDeLaGrille();
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
            if (e.Key == Key.R)
            {
                MelangerGrille();
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
