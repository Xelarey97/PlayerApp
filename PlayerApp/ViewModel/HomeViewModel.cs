using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using NAudio.Wave;
using PlayerApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ObservableCollection<Cancion> Canciones
        {
            get => canciones;
            set
            {
                canciones = value;
                RaisePropertyChanged("Canciones");
            }
        }

        public Cancion CancionSeleccionada
        {
            get => cancionSeleccionada;
            set
            {
                cancionSeleccionada = value;
                RaisePropertyChanged("CancionSeleccionada");
            }
        }
        #endregion

        #region Commands
        public ICommand PlayPauseMusicCommand { get; private set; }
        public ICommand NextSongCommand { get; private set; }
        public ICommand PrevSongCommand { get; private set; }
        #endregion

        public HomeViewModel()
        {
            LoadCanciones();
            _playbackState = PlaybackState.Stopped;

            PlayPauseMusicCommand = new RelayCommand(StartPlayback);
        }

        private void LoadCanciones()
        {
            canciones = new ObservableCollection<Cancion>();
            canciones.Add(new Cancion { Nombre = "Soy Peor", Album = "Unknown", Genero = "Trap", ID = 1, Artista = "Bad Bunny", Ruta = @"C:\Users\alexreyros\Downloads\Bad Bunny - Soy Peor.mp3" });
            canciones.Add(new Cancion { Nombre = "Chambea", Album = "Unknown", Genero = "Trap", ID = 2, Artista = "Bad Bunny", Ruta = @"C:\Users\alexreyros\Downloads\Chambea - Bad Bunny  Video Oficial.mp3" });
            canciones.Add(new Cancion { Nombre = "Soy Peor", Album = "Unknown", Genero = "Trap", ID = 3, Artista = "Bad Bunny" });
            canciones.Add(new Cancion { Nombre = "Soy Peor", Album = "Unknown", Genero = "Trap", ID = 4, Artista = "Bad Bunny" });
            this.RaisePropertyChanged(() => this.Canciones);
        }

        #region Command Methods
        private void StartPlayback()
        {
            if (cancionSeleccionada != null)
            {
                if(_playbackState == PlaybackState.Playing)
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
                else if (_playbackState == PlaybackState.Stopped || _playbackState == PlaybackState.Paused)
                {
                    _audioPlayer = new AudioPlayer(cancionSeleccionada.Ruta, 0.5f);
                    _audioPlayer.PlaybackStopType = AudioPlayer.PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;
                    _audioPlayer.Play(0.5);
                    _playbackState = PlaybackState.Playing;
                    cancionSonando = cancionSeleccionada;
                }
            }
        }
        #endregion
    }
}
