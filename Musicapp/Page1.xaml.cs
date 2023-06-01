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
using Microsoft.Win32;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.CoreAudioApi;
using System.Text.Json;
using System.IO;
using System.Windows.Threading;
using NAudio.Utils;

namespace Musicapp
{
    
    public partial class Page1 : Page
    {
        
        public string[] audioTracks;
        string ischanget = null;
        MediaFoundationReader media_url;
        WasapiOut apiOut;
        class AudioTrack
        {
            public string name { get; set; }
            public string path { get; set; }
            public string time { get; set; }
        }

        public Page1()
        {
            InitializeComponent();
        }

        private void Button_Play(object sender, RoutedEventArgs e)
        {
            try
            {
                apiOut = new WasapiOut();
                apiOut.Init(media_url);               
                apiOut.Play();
                stop.Visibility = Visibility.Visible;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Вы не выбрали ни 1 песню!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            apiOut.Pause();
            play.Visibility = Visibility.Visible;
            stop.Visibility = Visibility.Hidden;
        }

        private void pole_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                media_url = new MediaFoundationReader(pole.Text);
            }
            catch (Exception) {
                MessageBox.Show("Ссылка не правельно введена или не работает\nпопробуйте другую или убедитесь в корректности введённых данных");
            }
        }

        private void Volume_changet(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider volume_value = (Slider)sender;
            if (apiOut != null) apiOut.Volume = (float)volume_value.Value * 0.01F;
            
        }
    }
}
