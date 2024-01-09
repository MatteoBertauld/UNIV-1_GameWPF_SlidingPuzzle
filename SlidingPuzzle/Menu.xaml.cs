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
using System.Windows.Shapes;

namespace SlidingPuzzle
{
    /// <summary>
    /// Logique d'interaction pour Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        private int choix;
        private int niveau;
        public Menu()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            choix = comboBoxChoix.SelectedIndex;
            this.DialogResult = true;
        }        

        public int Niveau
        {
            get { if (choix == 0) niveau = 3; else if (choix == 1) niveau = 4; else niveau = 5;  return niveau; }
        }

        private void quitter_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
