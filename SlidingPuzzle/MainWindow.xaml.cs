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
using static System.Net.Mime.MediaTypeNames;


namespace SlidingPuzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        const int ZINDEXBOUTON = 2;
        const int ZINDEXLABEL = 3;
        const int POLICECHIFFREGRILLE = 25;

        private MediaPlayer musiqueFond = new MediaPlayer();

        private ImageBrush fondMenu = new ImageBrush();
        private ImageBrush skinMaison = new ImageBrush();
        private ImageBrush contourBouton = new ImageBrush();

        private TimeSpan tempsMaximum;
        private TimeSpan tempsdeJeu;

        private bool statutPause = false;
        private bool statutMelange;
        private bool contreLaMontreActiver;
        private bool voirImage = false;
        private bool voirImageNouvelleFenetre = false;
        private bool debutPartie = true;
        private bool grilleChiffre = false;

        private DispatcherTimer temps; 
        private Aide image = new Aide();

        //private Defaite pageDefaite = new Defaite();
        private Key toucheTriche = Key.C;
        private Key toucheRecommencer = Key.R;
        private Key touchePause = Key.P;
        private Key toucheAfficherImage = Key.C;

        private int compteurTemps = 1;
        private int difficulte;
        private int taille;
        private int debugNombreBouton;
        private int debugNombreImage;
        private int debugNombreBoutonSupprimer;
        private int debugNombreImageSupprimer;
        private int testPrecedent = 0;
        private int choixVolume = 50;

        private int[] valeurGrille;
        private Button[] boutons;

        private static string[] tableauSourceImages = new string[4] { "oiseaux.png", "artAbstrait.jpg", "info.jpg", "lac.jpg" };
        private static int indiceSourceImage = 0;

        System.Windows.Controls.Image[] ListeImages;


        public static string[] TableauSourceImages
        {
            get { return tableauSourceImages; }
        }


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

            skinMaison.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/hut.png"));
            boutRetourMenu.Background = skinMaison;

            contourBouton.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/toucheClavier.png"));
            boutVoirImage.Background = contourBouton;

            AfficheMenu();
            InitialiseJeu();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (!statutPause)
            {
                if (contreLaMontreActiver)
                {
                    tempsdeJeu -= TimeSpan.FromSeconds(1);
                }
                else
                {
                    tempsdeJeu += TimeSpan.FromSeconds(1);
                }

                labTemps.Content = "Temps restant : " + tempsdeJeu.Minutes.ToString() + " min et " + tempsdeJeu.Seconds.ToString() + " s";
            }
        }

        private void SupprimeObjectDeLaGrille()
        {
#if DEBUG
            debugNombreBoutonSupprimer = 0;
#endif
            foreach (Button bout in boutons)
            {
                if (maGrille.Children.Contains(bout))
                {
#if DEBUG
                    debugNombreBoutonSupprimer += 1;
#endif
                    maGrille.Children.Remove(bout);
                }
            }
#if DEBUG
            Console.WriteLine("Nombre de bouton supprimer de la grille " + debugNombreBoutonSupprimer);


            debugNombreImageSupprimer = 0;
#endif
            foreach (System.Windows.Controls.Image img in ListeImages)
            {
                if (maGrille.Children.Contains(img))
                {
#if DEBUG
                    debugNombreBoutonSupprimer += 1;
#endif
                    maGrille.Children.Remove(img);
                }

            }
#if DEBUG
            Console.WriteLine("Nombre d'image supprimer de la grille " + debugNombreImageSupprimer);
#endif
        }

        private void AfficheMenu()
        {
            Menu fenetreMenu = new Menu();
            fenetreMenu.ShowDialog();
            if (fenetreMenu.DialogResult == false)
            {
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                taille = fenetreMenu.Niveau;
                indiceSourceImage = fenetreMenu.IndiceSourceImagePuzzle;
                contreLaMontreActiver = fenetreMenu.ContreLaMontreActiver;
                grilleChiffre = fenetreMenu.GrilleChiffre;
                toucheTriche = fenetreMenu.ToucheTriche;
                toucheAfficherImage = fenetreMenu.ToucheAfficherImage;
                touchePause = fenetreMenu.TouchePause;
                toucheRecommencer = fenetreMenu.ToucheRecommencer;
                choixVolume = fenetreMenu.ChoixVolume;
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
            
            labMelangerGrille.Visibility = Visibility.Visible;
            debutPartie = true;
            valeurGrille = new int[taille];
            boutons = new Button[taille];
            ListeImages = new System.Windows.Controls.Image[taille];
            Random alea = new Random();

            CreationGrille();
            Generation_doubletableau();
            CreerBoutons();
            Panel.SetZIndex(labMelangerGrille, ZINDEXLABEL);

            musiqueFond.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "son/musique.wav"));
            musiqueFond.Play();
            musiqueFond.MediaEnded += (sender, e) => musiqueFond.Position = TimeSpan.Zero;
            musiqueFond.Volume = choixVolume / 100;
#if DEBUG
            DebugAfficheTousLesBoutonsActif();
            DebugAffichageConsoleGrille();
#endif


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
                labMelangerGrille.Visibility = Visibility.Hidden;
                return;
            }
            Button boutonCliquer = sender as Button;

            int valeurTemporaire;
            int colonne = Grid.GetColumn(boutonCliquer);
            int ligne = Grid.GetRow(boutonCliquer);
            int numero = ligne * (int)Math.Sqrt(taille) + colonne;
#if DEBUG
            Console.WriteLine("Bouton cliquer : colonne " + colonne + " ligne " + ligne + " numero " + numero + " ValeurGrille " + valeurGrille[numero]);
#endif

            foreach (Button bout in boutons)
            {
                int c2 = Grid.GetColumn(bout);
                int l2 = Grid.GetRow(bout);
                int num2 = l2 * (int)Math.Sqrt(taille) + c2;

                if (bout.Tag == "zero")
                {
#if DEBUG
                    Console.WriteLine("colonne " + c2 + " ligne " + l2 + " numero " + num2 + " ValeurGrille " + valeurGrille[num2]);
#endif

                    if (((c2 == colonne - 1 || c2 == colonne + 1) && (l2 == ligne)) || ((l2 == ligne - 1 || l2 == ligne + 1) && (c2 == colonne)))
                    {
                        valeurTemporaire = valeurGrille[numero];
                        valeurGrille[numero] = valeurGrille[num2];
                        valeurGrille[num2] = valeurTemporaire;

                        if (grilleChiffre)
                        {
                            bout.Content = valeurGrille[num2];
                            boutonCliquer.Content = valeurGrille[numero];
                        }
#if DEBUG
                        Console.WriteLine("\nValeurs de la grille");
                        DebugAffichageConsoleGrille();
                        Console.WriteLine("numero : " + valeurGrille[numero] + " / num : " + valeurGrille[num2] + "\n");
#endif

                        bout.Tag = null;
                        boutonCliquer.Tag = "zero";
                    }
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
                Panel.SetZIndex(boutons[i], ZINDEXBOUTON);

                if (grilleChiffre) 
                {
                    test2.Content = i+1;   
                    test2.FontSize = POLICECHIFFREGRILLE;
                }
                else 
                {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/FondPuzzle/" + tableauSourceImages[indiceSourceImage]);
                bitmapImage.EndInit();

                System.Windows.Controls.Image croppedImage = new();
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
            }
            boutons[taille-1].Tag = "zero";
            AffichageGrille(false);
        }


        private void AffichageGrille(bool AfficheZero)
        {
            for (int i = 0; i < taille; i++)
            {
                if (grilleChiffre)
                {
                    boutons[i].Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 115, 115, 115));
                    boutons[i].Content = valeurGrille[i]+1;
                    if (AfficheZero)
                    {
                        if (boutons[i].Tag == "zero")
                        {
                            boutons[i].Content = "";
                        }
                    }
                }
                else
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
            return ListeCoupPossible[indice];
        }
        private void AfficherImage()
        {
            Aide image = new Aide();

            if (image.IsEnabled == false)
            {
                image.Show();
            }
            
            else if (image.IsActive == false)
            {
                image.Show();
            }
            else if (image.IsActive == true) 
            {
                image.Hide();
            }

        }

        private static void PauseEtRaffraichissementInterfaceGraphique() // fonction trouvé sur stackoverflow : https://stackoverflow.com/questions/37787388/how-to-force-a-ui-update-during-a-lengthy-task-on-the-ui-thread
        {
            DispatcherFrame frame = new();
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Input, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                Thread.Sleep(10); 
                return null;
            }), null);
            Dispatcher.PushFrame(frame);
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, new Action(delegate { }));
        }

        private void MelangerGrille()
        {
            statutMelange = true;
            labMelangerGrille.Visibility = Visibility.Hidden;
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
                PauseEtRaffraichissementInterfaceGraphique();
#if DEBUG
                DebugAffichageConsoleGrille();
#endif
            }
            AffichageGrille(true);
            statutMelange = false;
        }

        private void Generation_doubletableau()
        {
            for (int i = 0; i < taille; i++)
            {
                valeurGrille[i] = i;
            }
        }




        private void maGrille_KeyDown(object sender, KeyEventArgs e)
        {
            if (debutPartie == true)
            {
                MelangerGrille();
                labMelangerGrille.Visibility = Visibility.Hidden;
                debutPartie = false;
            }

            if (e.Key == toucheTriche)
            {
                Triche();
            }
            if (e.Key == touchePause)
            {
                statutPause = !statutPause;
            }

            if (e.Key == toucheAfficherImage)
            {
                AfficherImage();
            }

            if (e.Key == toucheRecommencer)
            {
                if (statutMelange == false)
                {
                    MelangerGrille();
                }
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



        public void DebugCompteNombreObjet(DependencyObject parent)
        {
            // Parcourir tous les éléments enfants
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is Button button)
                {
                    debugNombreBouton += 1;
                }
                if (child is System.Windows.Controls.Image img)
                {
                    debugNombreImage += 1;
                }

                // Récursivement appeler la fonction pour les enfants de cet élément
                #if DEBUG
                DebugCompteNombreObjet(child);
                #endif
            }
        }

        public void DebugAfficheTousLesBoutonsActif()
        {
            // Obtenir la fenêtre active
            var activeWindow = System.Windows.Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);

            if (activeWindow != null)
            {
                debugNombreImage = 0;
                debugNombreBouton = 0;
                // Lister tous les boutons dans la fenêtre active
                DebugCompteNombreObjet(activeWindow);
                Console.WriteLine("Nombre d'image trouvé :" + debugNombreImage);
                Console.WriteLine("Nombre de Bouton trouvé :" + debugNombreBouton);
            }
            else
            {
                Console.WriteLine("Aucune fenêtre active trouvée.");
            }
        }

        private void BoutVoirImage_Click(object sender, RoutedEventArgs e)
        {
            AfficherImage();
        }

        private void BoutMaison_click(object sender, RoutedEventArgs e)
        {
            SupprimeObjectDeLaGrille();
            AfficheMenu();
            InitialiseJeu();
        }
    }
}
