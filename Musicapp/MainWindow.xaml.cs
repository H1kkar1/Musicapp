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
        string PATH_to_mf = @"";
        AudioFileReader audioFile;
        WaveOutEvent outputDevice;
        public MainWindow()
        {
            InitializeComponent();
        }

        public string GetPath()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "MP3|*.mp3|WAV|*.wav"; // Filter files by extension
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                return dlg.FileName;
            }
            return null;
        }
        private void Button_Change(object sender, RoutedEventArgs e)
        {
            PATH_to_mf = @"";
            PATH_to_mf += GetPath();
            pole.Text = PATH_to_mf;
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
                audioFile = new AudioFileReader(PATH_to_mf);
                outputDevice.Init(audioFile);
            }
            outputDevice.Play();
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            outputDevice.Stop();
        }
        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            outputDevice.Dispose();
            outputDevice = null;
            audioFile.Dispose();
            audioFile = null;
        }
    }
}
