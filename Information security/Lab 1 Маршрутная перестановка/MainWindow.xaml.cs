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

namespace Lab_1_Маршрутная_перестановка
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<int> RowsColumnsNames { get; set; }

        public Crypto crypto { get; set; }

        public MainWindow()
        {
            RowsColumnsNames = new List<int>(Enumerable.Range(0, 100));

            crypto = new Crypto();
            InitializeComponent();

            DataContext = this;

        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox) sender;
            string text = tb.Text;
            crypto.Encrypt(text);
        }
    }
}
