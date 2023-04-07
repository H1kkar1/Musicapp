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
using NAudio.CoreAudioApi;
using System.Text.Json;
using System.IO;

namespace Musicapp
{ 
    public partial class MainWindow : Window
    {
        AudioFileReader audioFile;
        WaveOutEvent outputDevice;
        Volums_Settings vs;
        MMDevice device;
        AudioTrack track;
        FileInfo fileInfo = new FileInfo("Traks.json");

        public MainWindow()
        {
            InitializeComponent();
            Volums_Settings vs;
            fileInfo.Exists ? : File.Create("Traks.json");
            
        }

        public string GetPath()
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "MP3|*.mp3|WAV|*.wav"; // Фильтр файлов в проводнике
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    return dlg.FileName;
                }
                return null;
            } catch (Exception f) { return null; }
        }


        //Всё о выбранном треке
        //------------------------------------------------------------------------------------------------------------------------------
        private void Button_Change(object sender, RoutedEventArgs e)
        {
            string path = GetPath();
            if (path != null)
            {
                string[] names = path.Split(new char[] { (char)92 });
                string[] expansion = names[names.Length - 1].Split(new char[] { '.' });
                string name = names[names.Length - 1].Substring(0, names[names.Length - 1].Length - 4);
                string ex = expansion[0];
                track_name.Content = name;
                pole.Text = name;
                track = new AudioTrack
                {
                    name = name,
                    path = @"" + path,
                    id = 1,
                    expansion = ex,
                    time = "00:00"
                };
                string jsonString = JsonSerializer.Serialize(track);
                Console.WriteLine(jsonString);
            }
        }

        class AudioTrack
        {
            public string name { get; set; }
            public string path { get; set; }
            public int id { get; set; }
            public string expansion { get; set; }
            public string time { get; set; }
        }

        // Старт | Стоп
        //------------------------------------------------------------------------------------------------------------------------------
        private void Button_Play(object sender, RoutedEventArgs e)
        {
            try
            {
                if (outputDevice == null)
                {
                    outputDevice = new WaveOutEvent();
                    outputDevice.PlaybackStopped += OnPlaybackStopped;
                }
                if (audioFile == null)
                {
                    audioFile = new AudioFileReader(track.path);
                    outputDevice.Init(audioFile);
                    track_time.Maximum = audioFile.Length;
                    MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
                    device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                    outputDevice.Volume = 1;
                }
                outputDevice.Play();
                stop.Visibility = Visibility.Visible;
            }
            catch (KeyNotFoundException)
            {
                MessageBox.Show("Вы не выбрали ни 1 песню!","Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            outputDevice.Pause();
            pole.Text = audioFile.Position.ToString();
            pole.Text += "\n";
            play.Visibility = Visibility.Visible;
            stop.Visibility = Visibility.Hidden;
        }
        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            outputDevice.Dispose();
            outputDevice = null;
            audioFile.Dispose();
            audioFile = null;
        }


     //Измение положения проигрывания
     //----------------------------------------------------------------------------------------------------------------------------

        private void StartTrackChanget_tack_time(object sender, MouseButtonEventArgs e)
        {
            if(outputDevice!=null) outputDevice.Pause();
        }

        private void EndTrackChanget_tack_time(object sender, MouseButtonEventArgs e)
        {
            if (outputDevice != null)
            {
                long newPos = (long)track_time.Value;
                if ((newPos % audioFile.WaveFormat.BlockAlign) != 0)
                {
                    newPos -= newPos % audioFile.WaveFormat.BlockAlign;
                }
                newPos = Math.Max(0, Math.Min(audioFile.Length, newPos));
                audioFile.Position = newPos;
                outputDevice.Play();
            }
        }

        // Всё связанное со звуком
        //----------------------------------------------------------------------------------------------------------------------------
        private void Button_volume(object sender, RoutedEventArgs e)
        {
            if (vs == null) 
            {
                vs = new Volums_Settings();
            }
            vs.Owner = this;
            vs.Show();
        }
     
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider volume_value = (Slider)sender;
            if (outputDevice != null) { outputDevice.Volume = (float)volume_value.Value * 0.01F; }
        }
        public void Volume_Value()
        {
            Master_value.Value = device.AudioMeterInformation.MasterPeakValue;
        }
    }
}
