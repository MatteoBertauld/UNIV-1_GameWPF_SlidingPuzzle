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

        private TimeSpan tempsMaximum;
        private TimeSpan tempsdeJeu;

        bool contreLaMontreActiver;

        private DispatcherTimer temps; // timer
        private Aide image = new Aide();
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        //private Defaite pageDefaite = new Defaite();
        private Key keyTriche = Key.C;
        private int compteurTemps = 1;
        int[] valeurGrille;
        //Label[] grille;
        int difficulte;
        Button[] boutons;
        System.Windows.Controls.Image[] ListeImages;
        int taille;
        int DebugNombreBouton;
        int DebugNombreImage;
        int DebugNombreBoutonSupprimer;
        int DebugNombreImageSupprimer;
        int TestPrecedent = 0;
        bool voirImage = false;
        bool voirImageNouvelleFenetre = false;

        bool debutPartie = true;

        private static string[] tableauSourceImages = new string[4] { "oiseaux.png", "artAbstrait.jpg", "info.jpg", "lac.jpg" };

        public static string[] TableauSourceImages
        {
            get { return tableauSourceImages; }
        }

        private static int indiceSourceImage = 0;

        public static int IndiceSourceImage
        {
            get { return indiceSourceImage; }
        }


        public MainWindow()
        {
            
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            ImageBrush SkinMaison = new ImageBrush();
            SkinMaison.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/hut.png"));
            Maison.Background = SkinMaison;

            AfficheMenu();
            InitialiseJeu();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (contreLaMontreActiver)
            {
                tempsdeJeu -= TimeSpan.FromSeconds(1);
            } else
            {
                tempsdeJeu += TimeSpan.FromSeconds(1);
            }
            
            labelTemps.Content = tempsdeJeu.Minutes.ToString() + " min et " + tempsdeJeu.Seconds.ToString() + " s";
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

        private void AfficheMenu()
        {
            Menu fenetreMenu = new Menu();
            fenetreMenu.ShowDialog();
            if (fenetreMenu.DialogResult == false)
            {
                Application.Current.Shutdown();
            }
            else
            {
                taille = fenetreMenu.Niveau;
                indiceSourceImage = fenetreMenu.IndiceSourceImagePuzzle;
                contreLaMontreActiver = fenetreMenu.ContreLaMontreActiver;
                keyTriche = fenetreMenu.ToucheTriche;
                if (contreLaMontreActiver)
                {
                    tempsMaximum = fenetreMenu.TempsLimite;
                }
            }
        }

        private void InitialiseJeu()
        {
            if (contreLaMontreActiver)
            {
                tempsdeJeu = tempsMaximum;
            } else
            {
                tempsdeJeu = TimeSpan.Zero;
            }
            
            labelMelangerGrille.Visibility = Visibility.Visible;
            debutPartie = true;
            valeurGrille = new int[taille];
            boutons = new Button[taille];
            ListeImages = new System.Windows.Controls.Image[taille];
            Random alea = new Random();

            CreationGrille();
            Generation_doubletableau();
            CreerBoutons();
            DebugAfficheTousLesBoutonsActif();
            DebugAffichageConsoleGrille();
            Panel.SetZIndex(labelMelangerGrille, 3);

            foreach (Button bout in boutons)
            {
                bout.Click += Clique;
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
            AffichageGrille(true);
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
            if (debutPartie == true)
            {
                MelangerGrille();
                debutPartie = false;
                labelMelangerGrille.Visibility = Visibility.Hidden;
                return;
            }
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
                    musiqueFond.Play();
                }
            }
            AffichageGrille(true);
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
                Panel.SetZIndex(boutons[i], 2);


                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/FondPuzzle/" + tableauSourceImages[indiceSourceImage]);
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
            AffichageGrille(false);
        }


        private void AffichageGrille(bool AfficheZero)
        {
            for (int i = 0; i < taille; i++)
            {
                Grid.SetRow(ListeImages[valeurGrille[i]], i / (int)Math.Sqrt(taille));
                Grid.SetColumn(ListeImages[valeurGrille[i]], i % (int)Math.Sqrt(taille));

                if (AfficheZero)
                {
                    if (boutons[i].Tag == "zero")
                    {
                        boutons[i].Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));
                    }
                    else
                    {
                        boutons[i].Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0));
                    }
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

        private static void PauseEtRaffraichissementInterfaceGraphique() // fonction trouvé sur stackoverflow : https://stackoverflow.com/questions/37787388/how-to-force-a-ui-update-during-a-lengthy-task-on-the-ui-thread
        {
            DispatcherFrame frame = new();
            // DispatcherPriority set to Input, the highest priority
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Input, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                Thread.Sleep(10); // Stop all processes to make sure the UI update is perform
                return null;
            }), null);
            Dispatcher.PushFrame(frame);
            // DispatcherPriority set to Input, the highest priority
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, new Action(delegate { }));
        }

        private void MelangerGrille()
        {
            labelMelangerGrille.Visibility = Visibility.Hidden;
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
            int nombreCoup = alea.Next(taille*3, taille * 3+15);


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
                AffichageGrille(false);
                DebugAffichageConsoleGrille();
                PauseEtRaffraichissementInterfaceGraphique();
            }
            AffichageGrille(true);
        }

        private void Generation_doubletableau()
        {
            for (int i = 0; i < taille; i++)
            {
                valeurGrille[i] = i;
            }
        }


        private void BoutonMaison_click(object sender, RoutedEventArgs e)
        {
            SupprimeObjectDeLaGrille();
            AfficheMenu();
            InitialiseJeu();
        }

        private void maGrille_KeyDown(object sender, KeyEventArgs e)
        {
            if (debutPartie == true)
            {
                MelangerGrille();
                labelMelangerGrille.Visibility = Visibility.Hidden;
                debutPartie = false;
            }

            if (e.Key == keyTriche)
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


        private void butVoirImage_Click(object sender, RoutedEventArgs e)
        {
            if (!image.IsLoaded)
            {
                image = new Aide(); 
            }

        private void butVoirImage_DragOver(object sender, DragEventArgs e)
        {
            image.Show();
        }


        public void DebugCompteNombreObjet(DependencyObject parent)
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
                DebugCompteNombreObjet(child);
            }
        }

        public void DebugAfficheTousLesBoutonsActif()
        {
            // Obtenir la fenêtre active
            var activeWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);

            if (activeWindow != null)
            {
                DebugNombreImage = 0;
                DebugNombreBouton = 0;
                // Lister tous les boutons dans la fenêtre active
                DebugCompteNombreObjet(activeWindow);
                Console.WriteLine("Nombre d'image trouvé :" + DebugNombreImage);
                Console.WriteLine("Nombre de Bouton trouvé :" + DebugNombreBouton);
            }
            else
            {
                Console.WriteLine("Aucune fenêtre active trouvée.");
            }
        }

    }
}
