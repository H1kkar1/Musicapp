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
    public static class Audio
    {
        public static AudioFileReader audioFile;
        public static WaveOutEvent outputDevice;
    }
    public partial class MainWindow : Window
    {

        Volums_Settings vs;
        MMDevice device;
        AudioTrack track;
        public string jsonString;
        public List<string> jsonRecords = new List<string> { };
        public string[] audioTracks;
        public FileInfo fileInfo = new FileInfo("Traks.json");
        string ischanget = null;

        public MainWindow()
        {
            InitializeComponent();
            Volums_Settings vs;
            if (fileInfo.Exists) 
            { 
                audioTracks = File.ReadAllLines("Traks.json"); 
                foreach (string i in audioTracks)
                {   
                    track = JsonSerializer.Deserialize<AudioTrack>(i);
                    track_list.Items.Add(track);
                    Console.WriteLine(track.ToString());                 
                }
            }
            else
            {
                File.Create("Traks.json");
            }
               
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
                track_name.Content = name;
                track = new AudioTrack
                {
                    name = name,
                    path = @"" + path,
                    time = "00:00"
                };


                if (Audio.outputDevice == null)
                {
                    Audio.outputDevice = new WaveOutEvent();
                    Audio.outputDevice.PlaybackStopped += OnPlaybackStopped;
                }

                if (Audio.audioFile == null)
                {
                    ischanget = track.path;
                    Audio.audioFile = new AudioFileReader(track.path);
                    Audio.outputDevice.Init(Audio.audioFile);
                    track_time.Maximum = Audio.audioFile.Length;
                    MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
                    device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                    Audio.outputDevice.Volume = 1;
                }
                Audio.outputDevice.Pause();
                if (ischanget != null)
                {
                    if (track.path != ischanget)
                    {
                        Audio.outputDevice = null;
                        Audio.outputDevice = new WaveOutEvent();
                        Audio.audioFile = null;
                        Audio.audioFile = new AudioFileReader(track.path);
                        Audio.outputDevice.Init(Audio.audioFile);
                    }
                }

                TimeSpan time = Audio.audioFile.TotalTime;
                track.time = time.Minutes.ToString() + ":" + time.Seconds.ToString();
                jsonString = JsonSerializer.Serialize(track);
                File.AppendAllText("Traks.json", jsonString + '\n');
                track_list.Items.Add(track);

                //for (int i = 0; i < track_list.Items.Count; i++)
                //{
                //    AudioTrack q = (AudioTrack)track_list.Items[i];
                //    if (q.name == track.name)
                //    {
                //        track_list.Items.RemoveAt(i);
                //        break;
                //    }
                //}

            }
        }

        class AudioTrack
        {
            public string name { get; set; }
            public string path { get; set; }
            public string time { get; set; }
        }

        // Старт | Стоп
        //------------------------------------------------------------------------------------------------------------------------------
        private void Button_Play(object sender, RoutedEventArgs e)
        {
            try
            { 
                Audio.outputDevice.Play();
                stop.Visibility = Visibility.Visible;
            }
            catch(NullReferenceException)
            {
                MessageBox.Show("Вы не выбрали ни 1 песню!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            Audio.outputDevice.Pause();
            
            play.Visibility = Visibility.Visible;
            stop.Visibility = Visibility.Hidden;
        }
        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            Audio.outputDevice.Dispose();
            Audio.outputDevice = null;
            Audio.audioFile.Dispose();
            Audio.audioFile = null;
        }


     //Измение положения проигрывания
     //----------------------------------------------------------------------------------------------------------------------------

        private void StartTrackChanget_tack_time(object sender, MouseButtonEventArgs e)
        {
            if(Audio.outputDevice != null) Audio.outputDevice.Pause();
        }

        private void EndTrackChanget_tack_time(object sender, MouseButtonEventArgs e)
        {
            if (Audio.outputDevice != null)
            {
                long newPos = (long)track_time.Value;
                if ((newPos % Audio.audioFile.WaveFormat.BlockAlign) != 0)
                {
                    newPos -= newPos % Audio.audioFile.WaveFormat.BlockAlign;
                }
                newPos = Math.Max(0, Math.Min(Audio.audioFile.Length, newPos));
                Audio.audioFile.Position = newPos;
                Audio.outputDevice.Play();
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
            if (Audio.outputDevice != null) { Audio.outputDevice.Volume = (float)volume_value.Value * 0.01F; }
        }

        // Работа с треками в ListBox
        //----------------------------------------------------------------------------------------------------------------------------
        private void Button_DelTrack(object sender, RoutedEventArgs e)
        {
            track_list.Items.Remove(track_list.SelectedIndex);
        }

        private void track_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AudioTrack ischanget = new AudioTrack();
            try
            {
                if (Audio.outputDevice != null)
                {
                    Audio.outputDevice.Pause();
                    ischanget = (AudioTrack)e.AddedItems[0];
                    if (track.path != ischanget.path)
                    {
                        track = (AudioTrack)e.AddedItems[0];
                        Audio.audioFile = null;
                        Audio.outputDevice = null;
                        Audio.outputDevice = new WaveOutEvent();
                        Audio.audioFile = new AudioFileReader(track.path);
                        Audio.outputDevice.Init(Audio.audioFile);
                        track_name.Content = ischanget.name;
                        stop.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Audio.outputDevice.Play();
                        stop.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    track = (AudioTrack)e.AddedItems[0];
                    Audio.outputDevice = new WaveOutEvent();
                    Audio.outputDevice.PlaybackStopped += OnPlaybackStopped;
                    Audio.audioFile = new AudioFileReader(track.path);
                    Audio.outputDevice.Init(Audio.audioFile);
                    track_time.Maximum = Audio.audioFile.Length;
                    MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
                    device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                    Audio.outputDevice.Volume = 1;
                    track_name.Content = track.name;
                    stop.Visibility = Visibility.Visible;
                }
                Audio.outputDevice.Play();
                stop.Visibility = Visibility.Visible;
            }
            catch(Exception ex) {
                MessageBox.Show("Давай по новой Миша, всё хуйня");
            }
        }

        private void App_Close(object sender, EventArgs e)
        {
            //foreach(var i in jsonRecords)
            //{
            //    File.AppendAllText("Traks.json", i + '\n');
            //}
        }

        private void Options(object sender, RoutedEventArgs e)
        {

        }
    }
}