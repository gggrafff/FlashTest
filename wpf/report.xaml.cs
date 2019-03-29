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

namespace wpf
{
    /// <summary>
    /// Логика взаимодействия для report.xaml
    /// </summary>
    public partial class report : Window
    {
        public report()
        {
            InitializeComponent();
        }

        private void butClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void setrepmb(string repmb)
        {
            this.repmb.Text = repmb;
        }
        public void seterrorsreport(string errorsreport)
        {
            this.errorsreport.Text += errorsreport;
        }
    }
}
