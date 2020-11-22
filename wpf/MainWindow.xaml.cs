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
using System.Windows.Media.Animation;
using System.IO;
using System.ComponentModel;
using System.Timers;
using System.IO.Compression;

namespace wpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DriveInfo selected;
        SelectDrive SW;
        ReadyWindow RW;
        ReportWindow Rep;
        int wrfulltime;
        int rdfulltime;
        long writesummb;
        long readsummb;
        long uspread;
        long[] razmery;
        int coeftimer;
        string adr;
        bool res;
        List<string> errors;
        BackgroundWorker BW;
        bool stop = false;
        public MainWindow()
        {
            wrfulltime = 0;
            writesummb = 0;
            rdfulltime=0;
            readsummb = 0;
            coeftimer = 5;
            errors = new List<string>();
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SW = new SelectDrive();
            SW.Closed += SW_Closed;
            SW.Show();
            this.IsEnabled = false;
        }


        void SW_Closed(object sender, EventArgs e)
        {
            if (SW.getsgood())
            {
                selected = SW.getselected();
                ButDrv.Content = selected.Name;
                ButDrv.Foreground = Brushes.Black;
            }
            this.IsEnabled = true;
        }

        private void Label_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://logoprime.ru");
        }

        private void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
            if (selected != null)
            {
                DirectoryInfo dir;
                FileInfo[] files; 
                DirectoryInfo[] dires;
                try
                {
                    dir = new DirectoryInfo(selected.Name);
                    files = dir.GetFiles();
                    dires = dir.GetDirectories();
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        errors.Add("Флеш-носитель повреждён или не читается. ");
                        errlab.Content = "Обнаружено " + errors.Count + " ошибок.";
                        errlab.Cursor = Cursors.Hand;
                        if (errors.Count() == 1) errlab.MouseLeftButtonDown += errlab_MouseLeftButtonDown;
                    }));
                    return;
                }
                if (files.Count()!=0 | dires.Count()!=0)
                {
                    RW = new ReadyWindow();
                    RW.Closed += RW_Closed;
                    RW.Loaded += RW_Loaded;
                    RW.Show();
                    this.IsEnabled = false;
                }
                else
                {
                    Process();
                }
            }
            else {
                ColorAnimation ca = new ColorAnimation();
                ca.AutoReverse = true;
                ca.To = Color.FromRgb(240,100,120);
                ca.SpeedRatio=7;
                ((Rectangle)ButDrv.Template.FindName("RectDrv", ButDrv)).Fill.BeginAnimation(SolidColorBrush.ColorProperty,ca);
            }
        }

        void RW_Loaded(object sender, EventArgs e)
        {
            RW.settext("Вы хотите очистить носитель?"+Environment.NewLine+"В случае отказа будет проверено "+(selected.AvailableFreeSpace/1024/1024).ToString()+" Мб из "+(selected.TotalSize/1024/1024).ToString()+" Мб. ");
            RW.setDrv(selected);
        }

        void RW_Closed(object sender, EventArgs e)
        {
            if (RW.getready())
            {
                if (RW.res)
                {
                    this.res = RW.res;
                    this.adr = RW.adr+RW.zipname;
                }
                Process();
            }
            this.IsEnabled = true;
            
        }
        void Process() {
            butStop.Visibility = Visibility.Visible;
            ButtonRun.Visibility = Visibility.Hidden;
            ButDrv.Visibility = Visibility.Hidden;
            progress.Visibility = Visibility.Visible;
            progress.Maximum = selected.AvailableFreeSpace/1024/1024;
            stop = false;
            BW = new BackgroundWorker();
            BW.DoWork += BW_DoWork;
            BW.RunWorkerCompleted += BW_RunWorkerCompleted;
            BW.WorkerSupportsCancellation = true;
            BW.RunWorkerAsync();
        }

        void BW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            butStop.Visibility = Visibility.Hidden;
            ButtonRun.Visibility = Visibility.Visible;
        }

        


























        void BW_DoWork(object sender, DoWorkEventArgs e)
        {
            wrfulltime = 0;
            writesummb = 0;
            rdfulltime = 0;
            readsummb = 0;
            errors.Clear();
            Dispatcher.Invoke(new Action(() =>
            {
                errlab.Content = "";
                errlab.Cursor = Cursors.Arrow;
                errlab.MouseLeftButtonDown -= errlab_MouseLeftButtonDown ;
                wrtime.Content = "";
                wrsum.Content = "";
                wrspeed.Content = "";
                rdtime.Content = "";
                rdsum.Content = "";
                rdspeed.Content = "";
                progress.Value = 0;
            }));
            
            Timer timer = new Timer();
            timer.Interval = coeftimer * 1000;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
            byte[] bts = new byte[1024 * 1024];
            for (int i = 0; i < bts.Count(); i++) bts[i] = (byte)(i % 123);
            long sr = selected.AvailableFreeSpace / 1024 / 1024 / 1024;
            razmery = new long[sr + 1];
            
            for (long j = 0; j <= sr; j++)
            {
                long lim = 1024;
                if (j == sr) lim = selected.AvailableFreeSpace / 1024 / 1024 - 1;
                razmery[j] = lim;

                try
                { FileStream fs = File.Create(selected.Name + j.ToString() + ".mosf");
                for (long i = 0; i < lim; i++)
                {
                    fs.Write(bts, 0, bts.Count());
                    writesummb++;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        progress.Value = writesummb / 2;
                    }));
                    if (stop) break;
                }
                fs.Close();
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        //errors.Add(ex.Message);
                        errors.Add("Запись невозможна или носитель повреждён.");
                        errlab.Content = "Обнаружено " + errors.Count + " ошибок.";
                        errlab.Cursor = Cursors.Hand;
                        if (errors.Count() == 1) errlab.MouseLeftButtonDown += errlab_MouseLeftButtonDown;
                    }));
                    return;
                }
                
                
                if (stop) break;
            }
            timer.Stop();
            Dispatcher.Invoke(new Action(() =>
            {
                wrfulltime += coeftimer;
                int s = wrfulltime % 60;
                int m = (wrfulltime / 60);
                wrtime.Content = String.Format("{0:00}:{1:00}", m, s);
                wrsum.Content = writesummb.ToString() + " Мб";
                wrspeed.Content = String.Format("{0:f1}", (float)writesummb / (float)wrfulltime) + " Мб/c";
            }));
            bool err;
            int errsch;
            uspread = 0;
            DirectoryInfo dir;
            FileInfo[] files;

            try
            {
                dir = new DirectoryInfo(selected.Name);
                files = dir.GetFiles("*.mosf");
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    //errors.Add(ex.Message);
                    errors.Add("Носитель повреждён или чтение с него невозможно. ");
                    errlab.Content = "Обнаружено " + errors.Count + " ошибок.";
                    errlab.Cursor = Cursors.Hand;
                    if (errors.Count() == 1) errlab.MouseLeftButtonDown += errlab_MouseLeftButtonDown;
                }));
                return;
            }


            

            try
            {
                if (stop)
                {
                    foreach (FileInfo file in files) file.Delete();
                    return;
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    //errors.Add(ex.Message);
                    errors.Add("Нельзя удалить файлы с диска или носитель повреждён. ");
                    errlab.Content = "Обнаружено " + errors.Count + " ошибок.";
                    errlab.Cursor = Cursors.Hand;
                    if (errors.Count() == 1) errlab.MouseLeftButtonDown += errlab_MouseLeftButtonDown;
                }));
                return;
            }

            byte[] bts2 = new byte[1024*1024];
            timer = new Timer();
            timer.Interval = coeftimer * 1000;
            timer.Elapsed += timer_ElapsedRead;
            timer.Start();
            for (int j = 0; j < files.Count(); j++)
            {
                
                
                
                long lim = files[j].Length / 1024 / 1024;
                
                if (razmery[j] != lim)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        errors.Add("Файл " + files[j].Name + " имеет неверный размер. ");
                        errlab.Content = "Обнаружено " + errors.Count + " ошибок.";
                        errlab.Cursor = Cursors.Hand;
                        if (errors.Count() == 1) errlab.MouseLeftButtonDown += errlab_MouseLeftButtonDown;
                    }));
                }
                uspread += lim;
                errsch = 0;
                try
                {
                    FileStream fs = File.OpenRead(files[j].FullName);
                    for (long i = 0; i < lim; i++)
                    {
                        fs.Read(bts2, 0, bts2.Count());
                        err = false;

                        for (int z = 0; z < bts.Count(); z++)
                        {
                            if (bts[z] != bts2[z])
                            {
                                err = true;
                            }
                        }
                        if (err) errsch++;
                        if ((!err && errsch > 0) | (err && i == lim - 1))
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                errors.Add("Ошибка при чтении c " + (j * 1024 + i - errsch).ToString() + " Мб до " + (i + j * 1024).ToString() + " Мб.");
                                uspread -= errsch;
                                errlab.Content = "Обнаружено " + errors.Count + " ошибок.";
                                errlab.Cursor = Cursors.Hand;
                                if (errors.Count() == 1) errlab.MouseLeftButtonDown += errlab_MouseLeftButtonDown;
                            }));
                            errsch = 0;
                        }
                        readsummb++;
                        Dispatcher.Invoke(new Action(() =>
                        {
                            progress.Value = writesummb / 2 + readsummb / 2;
                        }));
                        if (stop) break;
                    }
                    fs.Close();
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        //errors.Add(ex.Message);
                        errors.Add("Нельзя считать файлы с диска или носитель повреждён. ");
                        errlab.Content = "Обнаружено " + errors.Count + " ошибок.";
                        errlab.Cursor = Cursors.Hand;
                        if (errors.Count() == 1) errlab.MouseLeftButtonDown += errlab_MouseLeftButtonDown;
                    }));
                    return;
                }
                
                if (stop) break;
            }
            timer.Stop();
            
            try
            {
                foreach (FileInfo file in files) file.Delete();
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    //errors.Add(ex.Message);
                    errors.Add("Нельзя удалить файлы с диска или носитель повреждён. ");
                    errlab.Content = "Обнаружено " + errors.Count + " ошибок.";
                    errlab.Cursor = Cursors.Hand;
                    if (errors.Count() == 1) errlab.MouseLeftButtonDown += errlab_MouseLeftButtonDown;
                }));
                return;
            }
            Dispatcher.Invoke(new Action(() =>
            {
                rdfulltime += coeftimer;
                int s = rdfulltime % 60;
                int m = (rdfulltime / 60);
                rdtime.Content = String.Format("{0:00}:{1:00}",m,s);
                rdsum.Content = readsummb.ToString() + " Мб";
                rdspeed.Content = String.Format("{0:f1}", (float)readsummb / (float)rdfulltime) + " Мб/c";
            }));
            if (errors.Count == 0) Dispatcher.Invoke(new Action(() =>
            {
                errlab.Content = "Ошибок не обнаружено. ";
            }));
            Dispatcher.Invoke(new Action(() =>
            {
            ButDrv.Visibility = Visibility.Visible;
            progress.Visibility = Visibility.Hidden;
            }));
            if (res)
            {
                if (errors.Count == 0)
                {
                    Dispatcher.Invoke(new Action(() =>
                {
                    ButDrv.Content = "Идёт восстановление. Подождите, пожалуйста.";
                }));
                    timer = new Timer();
                    timer.Interval = 1000;
                    timer.Elapsed += timer_ElapsedMove;
                    timer.Start();

                    try
                    {
                        ZipFile.ExtractToDirectory(adr, selected.RootDirectory.FullName);
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            //errors.Add(ex.Message);
                            errors.Add("Не удаётся восстановить резервную копию. Она находится по адресу " + adr);
                            errlab.Content = "Обнаружено " + errors.Count + " ошибок.";
                            errlab.Cursor = Cursors.Hand;
                            if (errors.Count() == 1) errlab.MouseLeftButtonDown += errlab_MouseLeftButtonDown;
                        }));
                        return;
                    }



                    try
                    {
                        File.Delete(adr);
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            //errors.Add(ex.Message);
                            errors.Add("Не удаётся удалить резервную копию. Она находится по адресу " + adr);
                            errlab.Content = "Обнаружено " + errors.Count + " ошибок.";
                            errlab.Cursor = Cursors.Hand;
                            if (errors.Count() == 1) errlab.MouseLeftButtonDown += errlab_MouseLeftButtonDown;
                        }));
                        return;
                    }

                    Dispatcher.Invoke(new Action(() =>
                {
                    ButDrv.Content = selected.Name;
                }));
                    timer.Stop();


                }
            }
            else
            {
                errors[errors.Count - 1] += Environment.NewLine+"Резервная копия Ваших данных находится по адресу " + adr;
            }

        }





















        int z = 0;
        void timer_ElapsedMove(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ButDrv.Content = "Идёт восстановление. Подождите, пожалуйста.";

                for (int j = 0; j < z % 3; j++) ButDrv.Content += ".";
                z++;
            }));
        }
        void errlab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rep = new ReportWindow();
            Rep.Loaded += Rep_Loaded;
            Rep.Closed += Rep_Closed;
            Rep.Show();
            this.IsEnabled = false;
        }

        void Rep_Closed(object sender, EventArgs e)
        {
            this.IsEnabled = true;
        }

        void Rep_Loaded(object sender, RoutedEventArgs e)
        {
            if (razmery != null) Rep.setrepmb("Было успешно считано " + uspread.ToString() + " Мб из " + razmery.Sum().ToString() + " Мб записанной информации.");
            else
            {
                Rep.setrepmb("Обнаруженные ошибки: ");
                Rep.errorsreport.Margin = new Thickness(10, 50, 0, 0);
                Rep.errorsreport.Height = 300;
            }
            foreach(string errortext in errors)
            Rep.seterrorsreport(errortext+Environment.NewLine);
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => {
                wrfulltime+=coeftimer;
                int s = wrfulltime % 60;
                int m = (wrfulltime / 60);
                wrtime.Content = String.Format("{0:00}:{1:00}", m, s);
                wrsum.Content = writesummb.ToString()+ " Мб";
                wrspeed.Content = String.Format("{0:f1}",(float)writesummb / (float)wrfulltime) + " Мб/c";
            }));
        }
        void timer_ElapsedRead(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                rdfulltime += coeftimer;
                int s = rdfulltime % 60;
                int m = (rdfulltime / 60);
                rdtime.Content = String.Format("{0:00}:{1:00}", m, s);
                rdsum.Content = readsummb.ToString() + " Мб";
                rdspeed.Content = String.Format("{0:f1}", (float)readsummb / (float)rdfulltime) + " Мб/c";
            }));
        }
        private void butStop_Click(object sender, RoutedEventArgs e)
        {
            stop = true;
            BW.CancelAsync();
            DirectoryInfo dir = new DirectoryInfo(selected.Name);
            butStop.Visibility = Visibility.Hidden;
            ButtonRun.Visibility = Visibility.Visible;
            ButDrv.Visibility = Visibility.Visible;
            progress.Visibility = Visibility.Hidden;
        }



    }
}
