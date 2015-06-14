using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Windows.Threading;
using System.Threading;

namespace Lab11
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> foundfiles = new List<string>();
        IAsyncResult cbResult;

        public MainWindow()
        {
            InitializeComponent();
        }

        void addFile(string file)
        {
            listbox1.Items.Add(file);
        }
        void ShowFile(string fileData)
        {
            textbox3.Text = fileData;
            tabItem2.IsSelected = true;
        }
        void search(string path, string pattern)
        {
           
            string[] files = System.IO.Directory.GetFiles(path,pattern);
            foreach (string file in files)
            {
                if (Dispatcher.CheckAccess())
                    addFile(file);
                else
                    Dispatcher.Invoke(
                      new Action<string>(addFile),
                      System.Windows.Threading.DispatcherPriority.Background,
                      new string[] { file }
                    );
            }
            string[] dirs = System.IO.Directory.GetDirectories(path);
            foreach (string dir in dirs)
                search(dir, pattern);

           
        }
        public void displayContent(string fileName)
        {
            string fileData = "";
            using (StreamReader reader = File.OpenText(fileName))
            {
               fileData = reader.ReadToEnd();
            }
            Dispatcher.Invoke(
                     new Action<string>(ShowFile),
                     System.Windows.Threading.DispatcherPriority.Background,
                     new string[] { fileData }
                   );
           
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            listbox1.Items.Clear();
            FolderBrowserDialog fldDialog = new FolderBrowserDialog();
            string path = "C:\\Sem 2\\IP\\Resources";
            fldDialog.RootFolder = Environment.SpecialFolder.Desktop;
            fldDialog.SelectedPath = path;
            DialogResult result = fldDialog.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                path = fldDialog.SelectedPath;
                string pattern = "*.cs";
                Action<string, string> showProc = this.search;
                cbResult = showProc.BeginInvoke(path, pattern, null, null);
            }
        }
        private void List_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
               
                    if (listbox1.SelectedItem != null)
                    {
                        displayContent(listbox1.SelectedItem.ToString());
                      
                    }

                
            }
));
        }

    }
}
