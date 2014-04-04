using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace winVDictionary
{
    public partial class Form1 : Form
    {
 
        List<string> l = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }


        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        
        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            string f = l[e.ItemIndex];
            ListViewItem lvi = new ListViewItem(f);
            long size = (new System.IO.FileInfo(f)).Length / 1024;
            lvi.SubItems.Add(size.ToString() + " kb");
            e.Item = lvi;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            start();

            listView1.FullRowSelect = true;

            /*
            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\Rich\Desktop\dictionary.txt");
            string line;
            int i = 0;

            while ((line = file.ReadLine()) != null)
            {
                ListViewItem lvi = new ListViewItem((++i).ToString());
                lvi.SubItems.Add(line);
                l.Add(lvi);
            }
            */
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            c = 0;
            long now = Environment.TickCount;
            Debug.WriteLine("Started..");
            ParsePath(@"G:\", l);
            Debug.WriteLine("Complete: " + (Environment.TickCount - now));
            Debug.WriteLine("Count: " + l.Count);
            listView1.VirtualListSize = l.Count;
        }

        int c = 0;
        private void ParsePath(string path, List<string> lst)
        {
            string[] d = DirectoryManager.GetDirectories(path, "*").ToArray();
            string[] f = DirectoryManager.GetFiles(path, "*").ToArray();
            /*
            for (int i = 0; i < d.Length; i++)
            {
                lst.Add(d[i]);
                //lst.Add((++c).ToString());
                //ListViewItem lvi = new ListViewItem((++c).ToString());
                ///lvi.SubItems.Add("");//d[i]);
                //lst.Add(lvi);
            }*/

            for (int i = 0; i < f.Length; i++)
            {
                lst.Add(f[i]);
                //lst.Add((++c).ToString());
               // ListViewItem lvi = new ListViewItem((++c).ToString());
                //lvi.SubItems.Add("");//f[i]);
                //lst.Add(lvi);
            }
            foreach (string subdir in d) ParsePath(subdir, lst);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
            listBox1.BeginUpdate();
            listBox1.DataSource = MyDir.find(@"C:\Users\Rich\Desktop", "*");
            listBox1.EndUpdate();*/

            long now = Environment.TickCount;
            Debug.Write("Time: " + now);

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (obj, en) => MyDir.find(@"G:\", "*");
            worker.RunWorkerCompleted += (obj, en) => Debug.WriteLine(" Complete: " + (Environment.TickCount - now));
            worker.RunWorkerAsync();
        }

        private void start()
        {
            List<FileSystemWatcher> lFSW = new List<FileSystemWatcher>();
            foreach (DriveInfo di in DriveInfo.GetDrives())
            {
                if (di.DriveType == DriveType.Fixed)
                {
                    FileSystemWatcher _watcher = new FileSystemWatcher();
                    _watcher.Changed += new FileSystemEventHandler(FolderWatcherTest_Changed);
                    _watcher.Created += new FileSystemEventHandler(FolderWatcherTest_Created);
                    _watcher.Deleted += new FileSystemEventHandler(FolderWatcherTest_Deleted);
                    _watcher.Renamed += new RenamedEventHandler(FolderWatcherTest_Renamed);
                    _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;// | NotifyFilters.LastWrite;
                    _watcher.Path = di.Name;
                    _watcher.IncludeSubdirectories = true;
                    _watcher.SynchronizingObject = this;
                    lFSW.Add(_watcher);
                    _watcher.EnableRaisingEvents = true;
                }
            }
        }

        private void FolderWatcherTest_Changed(object sender, FileSystemEventArgs e)
        {
            Invoke(new Action(() =>
            {
                listBox2.Items.Add("Changed " + e.FullPath);
            }));

        }
        private void FolderWatcherTest_Created(object sender, FileSystemEventArgs e)
        {
            Invoke(new Action(() =>
            {
                listBox2.Items.Add("Created " + e.FullPath);
            }));
        }
        private void FolderWatcherTest_Deleted(object sender, FileSystemEventArgs e)
        {
            Invoke(new Action(() =>
            {
                listBox2.Items.Add("Deleted " + e.FullPath);
            }));
        }
        private void FolderWatcherTest_Renamed(object sender, RenamedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                listBox2.Items.Add("Renamed " + e.OldFullPath + " => " + e.FullPath + " " + sender.ToString());
            }));
        }
    }
}
