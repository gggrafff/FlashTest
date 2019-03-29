using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace flashtest
{
    public partial class Form1 : Form
    {
        DriveInfo[] drives;
        DriveInfo selected;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives) listBox1.Items.Add(drive.Name);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selected = drives[listBox1.SelectedIndex];
            descript(selected);
        }
        private void descript(DriveInfo selected)
        {
            textBox1.Text = String.Format("Название: {0}", selected.Name) + Environment.NewLine + String.Format("Тип диска: {0}", selected.DriveType) + Environment.NewLine;
            
            
            if (selected.IsReady)
            {
                textBox1.Text += String.Format("Объем диска: {0}", selected.TotalSize) + Environment.NewLine;
                textBox1.Text += String.Format("Общее свободное пространство: {0}", selected.TotalFreeSpace) + Environment.NewLine;
                textBox1.Text += String.Format("Доступное свободное пространство: {0}", selected.AvailableFreeSpace) + Environment.NewLine;
                textBox1.Text += String.Format("Метка: {0}", selected.VolumeLabel) + Environment.NewLine;
                textBox1.Text += String.Format("Файловая система: {0}", selected.DriveFormat) + Environment.NewLine;
            }
            
            

        }
        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            listBox1.Items.Clear();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string drv = folderBrowserDialog1.SelectedPath;
                drives = DriveInfo.GetDrives();
                foreach (DriveInfo drive in drives)
                {
                    if (drive.Name == drv)
                    {
                        selected = drive;
                        descript(selected);
                    }

                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Dialogdw.DialogResult
            if (DialogResult.Yes == MessageBox.Show("пиздец","всё сотрётся",MessageBoxButtons.YesNo))
            {
                label1.Text = "wait";
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
                label1.Text = "ok";
            }
            else return;
            
        }


    }
}
