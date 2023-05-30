using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Threading;

namespace Musicapp
{
    /// <summary>
    /// Логика взаимодействия для CrealePlayList.xaml
    /// </summary>
    public partial class CrealePlayList : Window
    {
        public string playlist_name;
        public CrealePlayList()
        {
            InitializeComponent();
        }
        private void OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void pole_TextChanged(object sender, TextChangedEventArgs e)
        {
            playlist_name = pole.Text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {   
            FileInfo fileInfo = new FileInfo($"Play_Lists\\{playlist_name}.json");
            if (fileInfo.Exists) {
                MessageBox.Show("Такой плейлист уже существует");
            }
            else
            {
                File.Create($"Play_Lists\\{playlist_name}.json");
                MessageBox.Show("Плейлист успешно создан!)");
                
                this.Hide();
                pole.Text = "";
            }
        }
    }
}
