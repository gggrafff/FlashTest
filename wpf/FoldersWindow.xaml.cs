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
using System.Windows.Media.Animation;
using System.Timers;
namespace wpf
{
    /// <summary>
    /// Логика взаимодействия для Folders.xaml
    /// </summary>
    public partial class FoldersWindow : Window
    {
        DriveInfo[] drives;
        DirectoryInfo dir;
        List<DirectoryInfo> olddir;
        public bool uspeh = false;
        DirectoryInfo[] dires;
        public FoldersWindow()
        {
            InitializeComponent();
            try
            {
                olddir = new List<DirectoryInfo>();
                drives = DriveInfo.GetDrives();
                foreach (DriveInfo drv in drives) spisok.Items.Add(drv.RootDirectory.FullName);
                butUp.IsEnabled = false;
            }
            catch(Exception ex){
                title.Content = ex.Message;
                return;
            }
            spisok.SelectedIndex = 0;
            spisok.Focus();
        }

        private void spisok_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try{
            if (dir!=null) olddir.Add(dir);
            result.Content += spisok.SelectedItem.ToString();
            if (olddir.Count > 0) result.Content +="\\";
            dir = new DirectoryInfo(result.Content.ToString());
            spisok.Items.Clear();
            dires = dir.GetDirectories();
            foreach (DirectoryInfo dr in dires) spisok.Items.Add(dr.Name);
            butUp.IsEnabled = true;
            }
            catch(Exception ex){
                timer = new Timer();
                timer.Interval = 3000;
                timer.Elapsed += timer_Elapsed;
                timer.Start();
                title.Content = ex.Message;
                butUp_Click(sender, e);
                return;
            }
        }
        Timer timer;
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                title.Content = "Выберите директорию для сохранения резервной копии:";
            }));
            
            timer.Stop();
        }

        private void butUp_Click(object sender, RoutedEventArgs e)
        {
            try{
            if (olddir.Count > 0)
            {
                dir = olddir.Last();
                olddir.Remove(olddir.Last());
                result.Content = dir.FullName;
                spisok.Items.Clear();
                dires = dir.GetDirectories();
                foreach (DirectoryInfo dr in dires) spisok.Items.Add(dr.Name);
            }
            else
            {
                result.Content = "";
                dir = null;
                spisok.Items.Clear();
                drives = DriveInfo.GetDrives();
                foreach (DriveInfo drv in drives) spisok.Items.Add(drv.RootDirectory.FullName);
                butUp.IsEnabled = false;
            }
            }
            catch (Exception ex)
            {
                title.Content = ex.Message;
                return;
            }
        }

        private void butNew_Click(object sender, RoutedEventArgs e)
        {
            try{
            pole.Visibility = Visibility.Visible;
            ThicknessAnimation da=new ThicknessAnimation();
            da.To = new Thickness(pole.Margin.Left + 150, pole.Margin.Top, pole.Margin.Right-150, pole.Margin.Bottom);
            da.SpeedRatio = 5;
            pole.BeginAnimation(Grid.MarginProperty,da);
            butNew.Visibility = Visibility.Hidden;
            butCreate.Visibility = Visibility.Visible;
            namefolder.Focus();
            }
            catch (Exception ex)
            {
                title.Content = ex.Message;
                return;
            }
        }

        private void butCreate_Click(object sender, RoutedEventArgs e)
        {
            try {dir.CreateSubdirectory(namefolder.Text);
            spisok.Items.Clear();
            dires = dir.GetDirectories();
            foreach (DirectoryInfo dr in dires) spisok.Items.Add(dr.Name);
            butNew.Visibility = Visibility.Visible;
            butCreate.Visibility = Visibility.Hidden;
            ThicknessAnimation da = new ThicknessAnimation();
            da.To = new Thickness(pole.Margin.Left - 150, pole.Margin.Top, pole.Margin.Right + 150, pole.Margin.Bottom);
            da.SpeedRatio = 5;
            da.Completed += da_Completed;
            pole.BeginAnimation(Grid.MarginProperty, da);
            }
            catch (Exception ex)
            {
                title.Content = ex.Message;
                return;
            }
        }

        void da_Completed(object sender, EventArgs e)
        {
            pole.Visibility = Visibility.Hidden;
        }

        private void butCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void butOk_Click(object sender, RoutedEventArgs e)
        {
            if (result.Content.ToString() != "")
            {
                uspeh = true;
                //dir.CreateSubdirectory("MosFlashBackUp");
            //result.Content += "MosFlashBackUp\\";
            }

            this.Close();
        }
        public string getadr()
        {
            return result.Content.ToString();
        }
    }
}
