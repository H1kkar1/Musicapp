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
                track_name.Content = PATH["name"];
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
                track_time.Maximum = audioFile.Length;
            }
            outputDevice.Play();
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            outputDevice.Pause();
            pole.Text = audioFile.Position.ToString();
            pole.Text += "\n" + track_time.Maximum.ToString();
        }
        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            outputDevice.Dispose();
            outputDevice = null;
            audioFile.Dispose();
            audioFile = null;
        }

     

        private void StartTrackChanget_tack_time(object sender, MouseButtonEventArgs e)
        {
            outputDevice.Pause();
        }

        private void EndTrackChanget_tack_time(object sender, MouseButtonEventArgs e)
        {
            // Calculate new position
            long newPos = (long)track_time.Value;
            // Force it to align to a block boundary
            if ((newPos % audioFile.WaveFormat.BlockAlign) != 0)
                newPos -= newPos % audioFile.WaveFormat.BlockAlign;
            // Force new position into valid range
            newPos = Math.Max(0, Math.Min(audioFile.Length, newPos));
            // set position
            audioFile.Position = newPos;
            outputDevice.Play();
        }
    }
}
