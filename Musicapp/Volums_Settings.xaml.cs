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

namespace Musicapp
{
    /// <summary>
    /// Логика взаимодействия для Volums_Settings.xaml
    /// </summary>
    public partial class Volums_Settings : Window
    {
       /* public Volums_Settings()
        {
            InitializeComponent();
        }
        public EventHandler<VolumeChengetEventArgs> PropertyChanged;

        public void OnPropertyChanged(double value)
        {
            if(PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(value.ToString())); }
        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        { 
            Slider volume_value = (Slider)sender;
            OnPropertyChanged(volume_value.Value);          
        }

        public class VolumeChengetEventArgs : EventArgs
        {
            public double volume { get; set; }
        }
       */
    }
}
