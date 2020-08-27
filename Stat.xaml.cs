using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyApp
{
    /// <summary>
    /// Interaction logic for Stat.xaml
    /// </summary>
    public partial class Stat : Window
    {
        string f = "";
        public class S
        {
            public string CS { get; set; }
            public int Frequency { get; set; }
        }
        public class S2
        {
            public string CS1 { get; set; }
            public string CS2 { get; set; }
            public int Frequency { get; set; }
        }
        [Serializable]
        public class k
        {
            public string CS1 { get; set; }
            public string CS2 { get; set; }

            public override bool Equals(object obj)
            {
                if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    k p = (k)obj;
                    return (CS1 == p.CS1) && (CS2 == p.CS2);
                }
            }
            public override int GetHashCode()
            {
                return CS1.GetHashCode()*2 + CS2.GetHashCode();
            }
        }

        public Stat()
        {
            InitializeComponent();
            ShowGrid();
        }
        SortedDictionary<String, int> dictionary = new SortedDictionary<string, int>();
        Dictionary<k, int> d = new Dictionary<k, int>();
        Dictionary<k, int> d1 = new Dictionary<k, int>();
        List<String> files = new List<String>();
        private void Bt1_Click(object sender, RoutedEventArgs e)
        {
            String filepath = tb1.Text;
            filepath = filepath.Substring(0, filepath.Length - 4) + "_painted.txt";
            string text = File.ReadAllText(filepath);
            char[] c = { ',', '?', ';', '!', '\"', '\'', '.', ':', '-' };
            string[] cs = File.ReadAllLines("min_help.txt");
            for (int i = 0; i < c.Length; i++)
            {

                int res = text.Count(x => x == c[i]) / 2;
                Add(ref dictionary, c[i].ToString(), res);
            }
            for (int i = 0; i < cs.Length; i++)
            {
                string k = cs[i].Split('\t')[0];
                MatchCollection mc = Regex.Matches(text, cs[i].Split('\t')[0]);
                foreach (Match match in mc)
                {
                    String s = match.ToString();
                    if (s != "") Add(ref dictionary, s, 1);
                }
            }
            for (int i = 0; i < cs.Length; i++)
            {
                for (int j = 0; j < cs.Length; j++)
                {
                    string k = cs[i].Split('\t')[0];
                    MatchCollection mc = Regex.Matches(text, cs[i].Split('\t')[0] + ".+" + cs[j].Split('\t')[0]);
                    foreach (Match match in mc)
                    {
                        String s = match.ToString();
                        k a = new k();
                        a.CS1 = cs[i].Split('\t')[0];
                        a.CS2 = cs[j].Split('\t')[0];
                        if (s != "") Add1(ref d, a, 1);
                    }
                }
            }
            run_cmd("chunk.py", tb1.Text);
            string[] text1 = System.IO.File.ReadAllLines("output.txt");
            for (int i = 0; i < text1.Length; i++)
            {
                string[] t = text1[i].Split(' ');
                t[0] = t[0].ToLower();
                k p = new k();
                p.CS1 = t[0];
                p.CS2 = t[1];
                Add1(ref d1, p, 1);
            }
            files.Add(tb1.Text);
            ShowGrid();
        }
        public void Add1(ref Dictionary<k, int> l, k c, int v)
        {
            if (l.TryGetValue(c, out int h))
            {
                l[c] = h + v;
            }
            else
            {
                l.Add(c, v);
            }
        }
        public void Add(ref SortedDictionary<String, int> l, string c, int v)
        {
            if (l.TryGetValue(c, out int h))
            {
                l[c] = h + v;
            }
            else
            {
                l.Add(c, v);
            }
        }

        public string run_cmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python.exe";
            start.Arguments = string.Format("\"{0}\" \"{1}\"", cmd, args);
            start.UseShellExecute = false;// Do not use OS shell
            start.CreateNoWindow = true; // We don't need new window
            start.RedirectStandardOutput = true;// Any output, generated by application will be redirected back
            start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                    string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
                    return result;
                }
            }
        }

        public void ShowGrid()
        {
            DataGridXAML.ItemsSource = null;
            DataGridXAML1.ItemsSource = null;
            DataGridXAML2.ItemsSource = null;
            DataGridXAML.Items.Clear();
            DataGridXAML1.Items.Clear();
            DataGridXAML2.Items.Clear();
            lb1.ItemsSource = null;
            lb1.Items.Clear();
            List<S> l = new List<S>();
            foreach (string i in dictionary.Keys)
            {
                l.Add(new S()
                {
                    CS = i,
                    Frequency = dictionary[i]
                });
            }
            DataGridXAML.ItemsSource = l;
            List<S2> F = new List<S2>();
            foreach (k i in d.Keys)
            {
                F.Add(new S2()
                {
                    CS1 = i.CS1,
                    CS2 = i.CS2,
                    Frequency = d[i]
                });
            }
            List<S2> F1 = new List<S2>();
            foreach (k i in d1.Keys)
            {
                F1.Add(new S2()
                {
                    CS1 = i.CS1,
                    CS2 = i.CS2,
                    Frequency = d1[i]
                });
            }
            DataGridXAML1.ItemsSource = F;
            DataGridXAML2.ItemsSource = F1;
            lb1.ItemsSource = files;
        }
        private void Bt4_Click(object sender, RoutedEventArgs e)
        {
            Serialize();
            MessageBox.Show("The statistics has been saved");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            dictionary = new SortedDictionary<String, int>();
            files = new List<String>();
            d = new Dictionary<k, int>();
            ShowGrid();
        }
        private void Bt5_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.RedirectStandardInput = false;
            start.FileName = "help.txt";
            Process.Start(start);
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            DeSerialize();
        }
        public void Serialize()
        {
            Stream s = File.Open("stat.dat", FileMode.Create);
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(s, dictionary);
            s.Close();
            s = File.Open("lb1.dat", FileMode.Create);
            b.Serialize(s, files);
            s.Close();
            s = File.Open("d.dat", FileMode.Create);
            b.Serialize(s, d);
            s.Close();
            s = File.Open("d1.dat", FileMode.Create);
            b.Serialize(s, d1);
            s.Close();
        }
        public void DeSerialize()
        {
            Stream s = File.Open("stat.dat", FileMode.Open);
            BinaryFormatter b = new BinaryFormatter();
            dictionary = (SortedDictionary<String, int>)b.Deserialize(s);
            s.Close();
            s = File.Open("lb1.dat", FileMode.Open);
            files = (List<String>)b.Deserialize(s); 
            s.Close();
            s = File.Open("d.dat", FileMode.Open);
            d = (Dictionary<k,int>)b.Deserialize(s);
            s.Close();
            s = File.Open("d1.dat", FileMode.Open);
            d1 = (Dictionary<k, int>)b.Deserialize(s);
            s.Close();
            
           
        }
    }
}
