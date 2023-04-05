using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using NAudio.Wave;
using System.Threading;

namespace Musicapp
{
    internal class ModifyAudioFileReader
    { /*
        public AudioFileReader AudioFileReader { get; set; }

        private readonly Timer timer;

        public long Position
        {
            get => AudioFileReader?.Position ?? 0;
            set => _AudioFileReader?.Position ?? 0;
        }
        private static readonly PropertyChangedEventArgs PositionChangedEventArgs = new(nameof(Position));

        public ModifyAudioFileReader()
        {
            timer = new(_ => PropertyChanged?.Invoke(this, PositionChangedEventArgs), 0, 0, 20);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        */
    }
}
