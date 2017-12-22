using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using NAudio.Wave;
using PlayerApp.Model;
using PlayerApp.Utils;
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
        private ObservableCollection<PlayList> listasDeReproduccion;
        private PlayList listaDeReproduccionSeleccionada;

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
        public ObservableCollection<PlayList> ListasDeReproduccion
        {
            get { return listasDeReproduccion; }
            set
            {
                listasDeReproduccion = value;
                RaisePropertyChanged("ListasDeReproduccion");
            }
        }
        public PlayList ListaDeReproduccionSeleccionada
        {
            get { return listaDeReproduccionSeleccionada; }
            set
            {
                listaDeReproduccionSeleccionada = value;
                ChangeMusicListMethod();
                RaisePropertyChanged("ListaDeReproduccionSeleccionada");
            }
        }
        #endregion

        #region Commands
        public ICommand PlayPauseMusicCommand { get; private set; }
        public ICommand NextSongCommand { get; private set; }
        public ICommand PrevSongCommand { get; private set; }
        public ICommand VolumeControlCommand { get; private set; }
        public ICommand ChangeMusicListCommand { get; private set; }
        #endregion

        public HomeViewModel()
        {
            SetupFiles();
            canciones = new ObservableCollection<Cancion>();
            listasDeReproduccion = new ObservableCollection<PlayList>();
            Canciones.LoadCanciones();
            ListasDeReproduccion.LoadPlayLists(ref listaDeReproduccionSeleccionada);
            
            _playbackState = PlaybackState.Stopped;
            CurrentVolume = 1f;

            PlayPauseMusicCommand = new RelayCommand(PlayPauseMusicMethod);
            NextSongCommand = new RelayCommand(NextSongMethod);
            PrevSongCommand = new RelayCommand(PrevSongMethod);
            VolumeControlCommand = new RelayCommand(VolumeControlValueChanged);
            ChangeMusicListCommand = new RelayCommand(ChangeMusicListMethod);
        }

        #region Command Methods
        private void PlayPauseMusicMethod()
        {
            TogglePlayPause();
        }

        private void NextSongMethod()
        {
            if (Canciones != null && Canciones.Count > 0 && cancionSonando != null)
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
            if (Canciones != null && Canciones.Count > 0 && cancionSonando != null)
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

        private void ChangeMusicListMethod()
        {
            if (listaDeReproduccionSeleccionada != null && listaDeReproduccionSeleccionada.Canciones.Count > 0)
            {
                Canciones.Clear();
                foreach (Cancion c in listaDeReproduccionSeleccionada.Canciones)
                {
                    if (File.Exists(c.Ruta))
                        Canciones.Add(c);
                }
            }
        }
        #endregion

        #region Class Methods
        private void NextSongEnding()
        {
            if (_audioPlayer.GetPositionInSeconds() == _audioPlayer.GetLenghtInSeconds())
                NextSongMethod();
        }

        private void SetupFiles()
        {
            string newDirectory = AppGenericDirectories.MusicDirectory;
            if(!Directory.Exists(newDirectory))
            {
                Directory.CreateDirectory(newDirectory);
            }
        }

        private void TogglePlayPause()
        {
            if (cancionSeleccionada != null)
            {
                if (_playbackState == PlaybackState.Playing && cancionSeleccionada == cancionSonando)
                {
                    _audioPlayer.Pause();
                    _playbackState = PlaybackState.Paused;
                }
                else if (cancionSeleccionada == cancionSonando)
                {
                    if (_playbackState == PlaybackState.Stopped || _playbackState == PlaybackState.Paused)
                    {
                        _audioPlayer.Play(CurrentVolume);
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
                    _audioPlayer.Play(CurrentVolume);
                    _playbackState = PlaybackState.Playing;
                    cancionSonando = cancionSeleccionada;
                    _audioPlayer.PlaybackStopped += NextSongEnding;
                }
            }
        }
        #endregion
    }
}
