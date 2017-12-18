﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using NAudio.Wave;
using PlayerApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PlayerApp.ViewModel
{
    public class HomeViewModel : ViewModelBase
    {
        #region Propiedades
        private ObservableCollection<Cancion> canciones;
        private Cancion cancionSeleccionada;
        private Cancion cancionSonando;
        private AudioPlayer _audioPlayer;
        private PlaybackState _playbackState;
        private float currentVolume;

        public ObservableCollection<Cancion> Canciones
        {
            get { return canciones; }
            set
            {
                canciones = value;
                RaisePropertyChanged("Canciones");
            }
        }
        public Cancion CancionSeleccionada
        {
            get { return cancionSeleccionada; }
            set
            {
                cancionSeleccionada = value;
                RaisePropertyChanged("CancionSeleccionada");
            }
        }
        public float CurrentVolume
        {
            get { return currentVolume; }
            set
            {
                currentVolume = value;
                RaisePropertyChanged("CurrentVolume");
            }
        }
        #endregion

        #region Commands
        public ICommand PlayPauseMusicCommand { get; private set; }
        public ICommand NextSongCommand { get; private set; }
        public ICommand PrevSongCommand { get; private set; }
        public ICommand VolumeControlCommand { get; private set; }
        #endregion

        public HomeViewModel()
        {
            SetupFiles();
            
            _playbackState = PlaybackState.Stopped;
            CurrentVolume = 0.5f;

            PlayPauseMusicCommand = new RelayCommand(PlayPauseMusicMethod);
            NextSongCommand = new RelayCommand(NextSongMethod);
            PrevSongCommand = new RelayCommand(PrevSongMethod);
            VolumeControlCommand = new RelayCommand(VolumeControlValueChanged);
        }

        #region Command Methods
        private void PlayPauseMusicMethod()
        {
            if (cancionSeleccionada != null)
            {
                if(_playbackState == PlaybackState.Playing && cancionSeleccionada == cancionSonando)
                {
                    _audioPlayer.Pause();
                    _playbackState = PlaybackState.Paused;
                }
                else if (cancionSeleccionada == cancionSonando)
                {
                    if (_playbackState == PlaybackState.Stopped || _playbackState == PlaybackState.Paused)
                    {
                        _audioPlayer.Play(0.5);
                        _playbackState = PlaybackState.Playing;
                    }
                    else
                    {
                        _audioPlayer.Pause();
                        _playbackState = PlaybackState.Paused;
                    }
                }
                else if ((_playbackState == PlaybackState.Stopped || _playbackState == PlaybackState.Paused) || cancionSeleccionada != cancionSonando)
                {
                    if (_audioPlayer != null)
                    {
                        _audioPlayer.Dispose();
                        _audioPlayer = null;
                    }
                    _audioPlayer = new AudioPlayer(cancionSeleccionada.Ruta, CurrentVolume);
                    _audioPlayer.PlaybackStopType = AudioPlayer.PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;
                    _audioPlayer.Play(CurrentVolume);
                    _playbackState = PlaybackState.Playing;
                    cancionSonando = cancionSeleccionada;
                }
            }
        }

        private void NextSongMethod()
        {
            if (Canciones != null && Canciones.Count > 0)
            {
                int? currentIndex = Canciones.IndexOf(Canciones.Where(x => x.ID == cancionSonando.ID).FirstOrDefault());
                if (currentIndex.HasValue)
                {
                    int currentListSize = Canciones.Count - 1;
                    if (currentIndex < currentListSize)
                    {
                        CancionSeleccionada = Canciones[(int)(currentIndex + 1)];
                        PlayPauseMusicMethod();
                    }
                    else
                    {
                        CancionSeleccionada = Canciones[0];
                        PlayPauseMusicMethod();
                    }
                }
            }
        }

        private void PrevSongMethod()
        {
            if (Canciones != null && Canciones.Count > 0)
            {
                int? currentIndex = Canciones.IndexOf(Canciones.Where(x => x.ID == cancionSonando.ID).FirstOrDefault());
                if (currentIndex.HasValue)
                {
                    int currentListSize = Canciones.Count - 1;
                    if (currentIndex > 0)
                    {
                        CancionSeleccionada = Canciones[(int)(currentIndex - 1)];
                        PlayPauseMusicMethod();
                    }
                    else
                    {
                        CancionSeleccionada = Canciones[currentListSize];
                        PlayPauseMusicMethod();
                    }
                }
            }
        }

        private void VolumeControlValueChanged()
        {
            if (_audioPlayer != null)
            {
                _audioPlayer.SetVolume(CurrentVolume);
            }
        }
        #endregion

        #region Class Methods
        private void SetupFiles()
        {
            string path = Directory.GetCurrentDirectory();
            string newDirectory = string.Format("{0}\\Musica", path);
            if(!Directory.Exists(newDirectory))
            {
                Directory.CreateDirectory(newDirectory);
            }
            LoadCanciones(newDirectory);
        }

        private void LoadCanciones(string sDirectory)
        {
            canciones = new ObservableCollection<Cancion>();
            int i = 0;
            foreach (var filename in Directory.GetFiles(sDirectory))
            {
                FileInfo fi = new FileInfo(filename);
                TagLib.File tagFile = TagLib.File.Create(filename);
                Cancion cancion = new Cancion() {
                    ID = i,
                    Nombre = Path.GetFileNameWithoutExtension(fi.Name),
                    Artista = tagFile.Tag.FirstAlbumArtist,
                    Genero = tagFile.Tag.FirstGenre,
                    Album = tagFile.Tag.Album,
                    Ruta = filename
                };
                Canciones.Add(cancion);
                this.RaisePropertyChanged(() => this.Canciones);
                i++;
            }
        }
        #endregion
    }
}
