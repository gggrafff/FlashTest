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
using System.ComponentModel;
using System.Timers;
using System.IO.Compression;

namespace wpf
{
    /// <summary>
    /// Логика взаимодействия для ReadyWindow.xaml
    /// </summary>
    public partial class ReadyWindow : Window
    {
        DriveInfo selected;
        FoldersWindow fd;
        bool ready = false;
        bool delete = false;
        public bool res = false;
        public string adr;
        public string zipname;
        int i = 0;
        public ReadyWindow()
        {
            InitializeComponent();
        }
        public void settext(string text){
            warning.Text = text;
        }
        
        public void setDrv(DriveInfo Drv)
        {
            selected = Drv;
        }
        private void butYes_Click(object sender, RoutedEventArgs e)
        {
            
            delete = true;
            bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.RunWorkerAsync();
        }
        BackgroundWorker bw;
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ready = true;
            this.Close();
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Timer timer=new Timer();
                Dispatcher.Invoke(new Action(() =>
                {
                    butYes.IsEnabled = false;
                    butNo.IsEnabled = false;
                }));
               try
            { if (res)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        warning.Text = "Идёт сохранение. Подождите, пожалуйста.";
                    }));

                    timer = new Timer();
                    timer.Interval = 1000;
                    timer.Elapsed += timer_ElapsedMove;
                    timer.Start();
                    zipname = "MosFlashBackUp" + DateTime.Now.Millisecond.ToString() + ".zip";
                    ZipFile.CreateFromDirectory(selected.RootDirectory.FullName, adr + zipname);

                    timer.Stop();
                }

            }
               catch (Exception ex)
               {
                   Dispatcher.Invoke(new Action(() =>
                   {
                       warning.Text = "Не удаётся создать резервную копию. Продолжить без сохранения данных?";
                       //warning.Text = ex.Message+"Продолжить без сохранения данных?";
                       butYes.IsEnabled = true;
                       butNo.IsEnabled = true;
                       reserv.IsChecked = false;
                       reserv.Visibility = Visibility.Hidden;
                       butYes.Click -= butYes_Click;
                       butNo.Click -= butNo_Click;
                       butYes.Click+=butYes2_Click;
                       butNo.Click+=butNo2_Click;
                       bw.RunWorkerCompleted -= bw_RunWorkerCompleted;
                       timer.Stop();
                       return;
                   }));
                   return;
               }
            try
            { 

                if (delete)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        warning.Text = "Идёт очистка. Подождите, пожалуйста.";
                    }));

                    timer = new Timer();
                    timer.Interval = 1000;
                    timer.Elapsed += timer_ElapsedDel;
                    timer.Start();

                    DirectoryInfo dir = new DirectoryInfo(selected.Name);
                    FileInfo[] files = dir.GetFiles();
                    DirectoryInfo[] dires = dir.GetDirectories();
                    foreach (FileInfo file in files)
                    {
                        file.Delete();

                    }
                    foreach (DirectoryInfo dire in dires)
                    {
                        dire.Delete(true);
                    }
                    timer.Stop();
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    warning.Text = "Не удаётся выполнить очистку или носитель повреждён.";
                    bw.RunWorkerCompleted -= bw_RunWorkerCompleted;
                    timer.Stop();
                    return;
                }));
                return;
            }

        }

        private void butNo2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void butYes2_Click(object sender, RoutedEventArgs e)
        {
            res = false;
            bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.RunWorkerAsync();
        }

        void timer_ElapsedDel(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                warning.Text = "Идёт очистка. Подождите, пожалуйста.";
                
                for (int j = 0; j < i % 3;j++ ) warning.Text += ".";
                    i++;
            }));
        }
        void timer_ElapsedMove(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                warning.Text = "Идёт сохранение. Подождите, пожалуйста.";
                
                for (int j = 0; j < i % 3; j++) warning.Text += ".";
                i++;
            }));
        }
        private void butNo_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.RunWorkerAsync();
        }
        public bool getready() { return ready; }

        private void reserv_Checked(object sender, RoutedEventArgs e)
        {
            fd = new FoldersWindow();
            fd.Closed += fd_Closed;
            fd.Show();
            this.IsEnabled = false;
        }

        void fd_Closed(object sender, EventArgs e)
        {
            if (fd.uspeh)
            {
                res = true;
                adr = fd.getadr();
            }
            else reserv.IsChecked = false;
            this.IsEnabled = true;
        }

    }
}
