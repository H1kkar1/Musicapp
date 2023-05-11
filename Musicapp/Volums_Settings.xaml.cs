using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NAudio.Gui;
 

namespace Musicapp
{
    /// <summary>
    /// Логика взаимодействия для Volums_Settings.xaml
    /// </summary>
    public partial class Volums_Settings : Window
    {
        public Volums_Settings()
        {
            InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider volume_value = (Slider)sender;
            if (Audio.outputDevice != null) { Audio.outputDevice.Volume = (float)volume_value.Value * 0.01F; }
        }

        
    }
}
