﻿using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerApp.Model
{
    public class AudioPlayer
    {
        public enum PlaybackStopTypes
        {
            PlaybackStoppedByUser, PlaybackStoppedReachingEndOfFile
        }
        public PlaybackStopTypes PlaybackStopType { get; set; }
        private AudioFileReader _audioFileReader;
        private DirectSoundOut _output;

        public event Action PlaybackResumed;
        public event Action PlaybackStopped;
        public event Action PlaybackPaused;

        public AudioPlayer(string filepath, float volume)
        {
            _audioFileReader = new AudioFileReader(filepath) { Volume = volume };

            _output = new DirectSoundOut(200);
            _output.PlaybackStopped += _output_PlaybackStopped;
            var wc = new WaveChannel32(_audioFileReader);
            wc.PadWithZeroes = false;

            _output.Init(wc);
        }

        public void Play(double currentVolumeLevel)
        {
            _output.Play();
            _audioFileReader.Volume = (float)currentVolumeLevel;
        }

        private void _output_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (PlaybackStopped != null)
            {
                PlaybackStopped();
            }
        }

        public void Stop()
        {
            if (_output != null)
            {
                _output.Stop();
            }
        }

        public void Pause()
        {
             _output.Pause();
        }

        public void Dispose()
        {
            if (_output != null)
            {
                if (_output.PlaybackState == PlaybackState.Playing)
                {
                    _output.Pause();
                }
                _output.Dispose();
                _output = null;
            }
            if (_audioFileReader != null)
            {
                _audioFileReader.Dispose();
                _audioFileReader = null;
            }
        }

        public double GetLenghtInSeconds()
        {
            if (_audioFileReader != null)
            {
                return _audioFileReader.TotalTime.TotalSeconds;
            }
            else
            {
                return 0;
            }
        }

        public double GetPositionInSeconds()
        {
            return _audioFileReader != null ? _audioFileReader.CurrentTime.TotalSeconds : 0;
        }

        public float GetVolume()
        {
            if (_audioFileReader != null)
            {
                return _audioFileReader.Volume;
            }
            return 1;
        }

        public void SetPosition(double value)
        {
            if (_audioFileReader != null)
            {
                _audioFileReader.CurrentTime = TimeSpan.FromSeconds(value);
            }
        }

        public void SetVolume(float value)
        {
            if (_output != null)
            {
                _audioFileReader.Volume = value;
            }
        }
    }
}
