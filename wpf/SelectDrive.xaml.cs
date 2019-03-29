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
using System.IO;

namespace wpf
{
    /// <summary>
    /// Логика взаимодействия для SelectDrive.xaml
    /// </summary>
    public partial class SelectDrive : Window
    {
        DriveInfo[] drives;
        DriveInfo selected;
        bool good;

        public SelectDrive()
        {
            good = false;
            InitializeComponent();
            drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives) ListDrive.Items.Add(drive.Name);
            ListDrive.SelectedIndex = 0;
            ListDrive.Focus();
        }

        public DriveInfo getselected(){return selected;}
        public bool getsgood() { return good; }
        private void butCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ListDrive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected = drives[ListDrive.SelectedIndex];
            DriveDescr.Text = String.Format("Название: {0}", selected.Name) + Environment.NewLine + String.Format("Тип диска: {0}", selected.DriveType) + Environment.NewLine;


            if (selected.IsReady)
            {
                DriveDescr.Text += String.Format("Объем диска: {0:f2}", (float)selected.TotalSize/1024/1024/1024) +" Гб"+ Environment.NewLine;
                DriveDescr.Text += String.Format("Общее свободное пространство: {0:f2}", (float)selected.TotalFreeSpace / 1024 / 1024 / 1024) + " Гб" + Environment.NewLine;
                DriveDescr.Text += String.Format("Доступное свободное пространство: {0:f2}", (float)selected.AvailableFreeSpace / 1024 / 1024 / 1024) + " Гб" + Environment.NewLine;
                DriveDescr.Text += String.Format("Метка: {0}", selected.VolumeLabel) + Environment.NewLine;
                DriveDescr.Text += String.Format("Файловая система: {0}", selected.DriveFormat) + Environment.NewLine;
            }
            
        }

        private void butOK_Click(object sender, RoutedEventArgs e)
        {
            if (selected != null) good = true;
            this.Close();
        }
    }
}
