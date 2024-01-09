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
    /// Logique d'interaction pour test.xaml
    /// </summary>
    public partial class test : Window
    {
        public test()
        {
            InitializeComponent();

            maGrille.ShowGridLines = true;

            // Define the Columns
            ColumnDefinition col1 = new ColumnDefinition();
            ColumnDefinition col2 = new ColumnDefinition();
            ColumnDefinition col3 = new ColumnDefinition();
            maGrille.ColumnDefinitions.Add(col1);
            maGrille.ColumnDefinitions.Add(col2);
            maGrille.ColumnDefinitions.Add(col3);

            // Define the Rows
            RowDefinition ligne1 = new RowDefinition();
            RowDefinition ligne2 = new RowDefinition();
            RowDefinition ligne3 = new RowDefinition();
            maGrille.RowDefinitions.Add(ligne1);
            maGrille.RowDefinitions.Add(ligne2);
            maGrille.RowDefinitions.Add(ligne3);
        }
    }
}
