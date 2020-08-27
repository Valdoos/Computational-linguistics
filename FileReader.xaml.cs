using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for FileReader.xaml
    /// </summary>
    public partial class FileReader : Window
    {
        String filepath;
        string[] help;
        int firstIndex = 0;
        int secondIndex = 0;
        public FileReader(String filepath)
        {
            if (System.IO.File.Exists(filepath) && filepath != null)
            {
                this.filepath = filepath;
                InitializeComponent();
                Upload();
            }
        }
        public void Upload()
        {
            lbox1.ItemsSource = System.IO.File.ReadAllLines("min_help.txt");
            string s = System.IO.File.ReadAllText(filepath);
            List<string> result = new List<string>();
            Regex regular = new Regex(".{0,50}(?=\\s|$)", RegexOptions.Singleline);
            MatchCollection matches = regular.Matches(s);
            foreach (Match match in matches)
            {
                string a = match.Value.Trim();
                a = a.Replace("\n"," ");
                a = a.Replace("\r", " ");
                result.Add(a);
            }
            foreach (string i in result)
            {
                lbox2.Items.Add(i);
            }
            help = System.IO.File.ReadAllLines("min_help.txt");
            for (int i = 0; i < help.Length; i++)
            {
                help[i] = help[i].Split('\t')[0];
            }
            lbox3.Visibility = Visibility.Hidden;
            lbox4.Visibility = Visibility.Hidden;
            tb1.Visibility = Visibility.Hidden;
            bt1.Visibility = Visibility.Hidden;
        }

        private void Lbox2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            lbox3.Items.Clear();
            string str = ((sender as ListBox).SelectedItem as string);
            firstIndex = (sender as ListBox).SelectedIndex;
            foreach (string i in str.Split(' '))
            {
                if(i.Trim().Length>2)
                lbox3.Items.Add(i.Trim());
            }
           
            lbox3.Visibility = Visibility.Visible;
        }

        private void Lbox3_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            string str = ((sender as ListBox).SelectedItem as string);
            secondIndex = (sender as ListBox).SelectedIndex;
            MatchCollection mc = Regex.Matches(str, @"_[A-Z]+");
            bool flag = false;
            foreach (Match match in mc)
            {

                String c = match.ToString().Substring(1, match.ToString().Length-1);
                List<string> choice = new List<string>();
                foreach (string i in help)
                {
                    choice.Add(str.Replace(c, i));
                }
                lbox4.ItemsSource = choice;
                flag = true;
                break;
            }
            if (!flag)
            {
                List<String> m = new List<String>();
                m.Add(",");
                m.Add("\"");
                m.Add("\'");
                m.Add(".");
                m.Add("!");
                m.Add("?");
                int diff = 0;
                if (m.Contains(str[str.Length - 1].ToString())) diff = 1;
                List<string> choice = new List<string>();
                foreach (string i in help)
                {
                    choice.Add(str.Substring(0,str.Length-diff)+"_"+i+ str.Substring(str.Length-diff,diff));
                }
                lbox4.ItemsSource = choice;
            }
            tb1.Visibility = Visibility.Visible;
            bt1.Visibility = Visibility.Visible;
            lbox4.Visibility = Visibility.Visible;
            tb1.Text = str;
        }

        private void Lbox4_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string cs = ((sender as ListBox).SelectedItem as string);
            lbox3.Items.RemoveAt(secondIndex);
            lbox3.Items.Insert(secondIndex,cs);
            string str = "";
            foreach (var item in lbox3.Items)
            {
                str += item.ToString() + " "; 
            }
            lbox2.Items.RemoveAt(firstIndex);
            lbox2.Items.Insert(firstIndex, str);
            lbox3.Visibility = Visibility.Hidden;
            lbox4.Visibility = Visibility.Hidden;
            bt1.Visibility = Visibility.Hidden;
            tb1.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> str = new List<string>();
            str.AddRange(lbox2.Items.Cast<string>());
            System.IO.File.WriteAllLines(filepath, str);
            MessageBox.Show("The file has been saved");
        }

        private void Bt1_Click(object sender, RoutedEventArgs e)
        {
            string cs = tb1.Text;
            lbox3.Items.RemoveAt(secondIndex);
            lbox3.Items.Insert(secondIndex, cs);
            string str = "";
            foreach (var item in lbox3.Items)
            {
                str += item.ToString() + " ";
            }
            lbox2.Items.RemoveAt(firstIndex);
            lbox2.Items.Insert(firstIndex, str);
            lbox3.Visibility = Visibility.Hidden;
            lbox4.Visibility = Visibility.Hidden;
            tb1.Visibility = Visibility.Hidden;
            bt1.Visibility = Visibility.Hidden;
        }
    }
}
