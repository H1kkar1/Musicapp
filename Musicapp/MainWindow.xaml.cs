using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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


namespace Musicapp
{ 
    public partial class MainWindow : Window
    {
        Dictionary<string,string> PATH = new Dictionary<string, string>();
        AudioFileReader audioFile;
        WaveOutEvent outputDevice;
        //TimeSpan duration = outputDevice.TotalTime;
        public MainWindow()
        {
            InitializeComponent();
        }

        public string GetPath()
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "MP3|*.mp3|WAV|*.wav"; // Filter files by extension
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    return dlg.FileName;
                }
                return null;
            } catch (Exception f) { return null; }
        }
        private void Button_Change(object sender, RoutedEventArgs e)
        {
            PATH.Clear();
            string a = GetPath();
            if (a != null)
            {
                string[] name = a.Split(new char[] { (char)92 });
                string[] expansion = name[name.Length - 1].Split(new char[] { '.' });
                PATH.Add("path",a);
                PATH.Add("name", name[name.Length - 1].Substring(0, name[name.Length - 1].Length - 4));
                PATH.Add("expansion", expansion[1]);
                pole.Text = PATH["path"] + "\n\n" + PATH["name"] + "\n\n" + PATH["expansion"];
                treck_name.Content = PATH["name"];
            }
        }
        private void Button_Play(object sender, RoutedEventArgs e)
        {
            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped +=OnPlaybackStopped;
            }
            if (audioFile == null)
            {
                audioFile = new AudioFileReader(@PATH["path"]);
                outputDevice.Init(audioFile);
            }
            outputDevice.Play();
           
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            outputDevice.Pause();
           
        }
        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            outputDevice.Dispose();
            outputDevice = null;
            audioFile.Dispose();
            audioFile = null;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
