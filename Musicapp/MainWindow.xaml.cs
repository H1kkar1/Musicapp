﻿using System;
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
using System.Windows.Threading;
using NAudio.Utils;

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
        public string[] playLists;
        public string playList = @"Play_Lists\Tracks.json";
        public FileInfo fileInfotrack = new FileInfo(@"Play_Lists\Tracks.json");
        public DirectoryInfo directotyplaylists = new DirectoryInfo("Play_Lists");
        string ischanget = null;
        DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();
            if (fileInfotrack.Exists) 
            { 
                audioTracks = File.ReadAllLines(playList); 
                foreach (string i in audioTracks)
                {   
                    track = JsonSerializer.Deserialize<AudioTrack>(i);
                    track_list.Items.Add(track);
                    Console.WriteLine(track.ToString());                 
                }
                
            }
            else
            {
                File.Create(@"Play_Lists\Tracks.json");
            }

            if (directotyplaylists.Exists)
            {
                playLists = Directory.GetFiles("Play_Lists");
                foreach (string i in playLists)
                {
                    string[] pl = i.Split('.');
                    play_lists.Items.Add(pl[0].Substring(11));
                }
            }
            else
            {
                Directory.CreateDirectory("Play_Lists");
            }

        }
        // Делегат который вызываеться DispatcherTimer и меняет положение слайдера
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if(Audio.audioFile != null)
            {
                track_time.Maximum = Audio.audioFile.Length;
                track_time.Value = Audio.audioFile.Position;
            }
                
            else
            {
                dispatcherTimer.Stop();
                track_time.Value = 0.0;
                play.Visibility = Visibility.Visible;
                stop.Visibility = Visibility.Hidden;
            }
                
        }
        // Запуск изменения положения слвйдера относительно положения песни
        private void Track_Time_Value()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }
        //Получаем путь до выбранной песни
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
            } catch (Exception) { return null; }
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
                File.AppendAllText($"{playList}", jsonString + '\n');
                track_list.Items.Add(track);

                Audio.outputDevice.Play();
                stop.Visibility = Visibility.Visible;
                Track_Time_Value();

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
                        Audio.outputDevice.Play();
                        stop.Visibility = Visibility.Visible;
                        Track_Time_Value();
                    }
                    else
                    {
                        Audio.outputDevice.Play();
                        stop.Visibility = Visibility.Visible;
                        Track_Time_Value();
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
                    Audio.outputDevice.Play();
                    Track_Time_Value();
                }
               
            }
            catch(Exception) {}
        }

        private void App_Close(object sender, EventArgs e)
        {
            //foreach(var i in jsonRecords)
            //{
            //    File.AppendAllText("Tracks.json", i + '\n');
            //}
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Full_Screen(object sender, RoutedEventArgs e)
        {
            if(this.WindowState != WindowState.Normal)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        

        private void Closing_W(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PlayList_Select(object sender, SelectionChangedEventArgs e)
        {
            string play_list_name = play_lists.SelectedItem.ToString();
            audioTracks = File.ReadAllLines($"Play_Lists\\{play_list_name}.json");
            track_list.Items.Clear();
            foreach (string i in audioTracks)
            {
                track = JsonSerializer.Deserialize<AudioTrack>(i);
                track_list.Items.Add(track);
               
            }
            playList = play_list_name;
        }

        private void Play_with_links(object sender, RoutedEventArgs e)
        {
            if (Page.Content == null)
            {
                Page.Content = new Page1();
                volume.Visibility = Visibility.Hidden;
                play_with_links.Content = "Back to audio";
            }
            else 
            {
                Page.Content = null;
                volume.Visibility = Visibility.Visible;
                play_with_links.Content = "Play with links";
            }
               
        }

        private void Create_PalyList(object sender, RoutedEventArgs e)
        {
            FileInfo fileInfo = new FileInfo($"Play_Lists\\{pole.Text}.json");
            if (fileInfo.Exists)
            {
                MessageBox.Show("Такой плейлист уже существует");
            }
            else
            {
                File.Create($"Play_Lists\\{pole.Text}.json").Close();
                         
                File.Copy("Tracks.json", $"Play_Lists\\{pole.Text}.json",true);
                MessageBox.Show("Плейлист успешно создан!)");
                play_lists.Items.Add(pole.Text);
            }
        }
    }
}