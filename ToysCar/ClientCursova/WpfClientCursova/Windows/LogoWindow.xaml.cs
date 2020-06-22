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

namespace WpfClientCursova.Windows
{
    /// <summary>
    /// Interaction logic for LogoWindow.xaml
    /// </summary>
    public partial class LogoWindow : Window
    {
        public LogoWindow(double mainLeft, double mainTop, double mainHeight, double mainWidth)
        {
            InitializeComponent();

            this.Left = mainLeft + mainWidth / 2 - this.Width / 2;
            this.Top = mainTop + mainHeight / 2 - this.Height / 2;
        }
    }
}
