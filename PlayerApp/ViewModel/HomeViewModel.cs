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
            LoadPlayLists();
            
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
            string path = Directory.GetCurrentDirectory();
            string newDirectory = string.Format("{0}\\Musica", path);
            if(!Directory.Exists(newDirectory))
            {
                Directory.CreateDirectory(newDirectory);
            }
            LoadCanciones(newDirectory);
        }

        public void LoadCanciones(string sDirectory)
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

        public void LoadPlayLists()
        {
            ListasDeReproduccion = new ObservableCollection<PlayList>();
            using (ReadWriteJSON<PlayList> rw = new ReadWriteJSON<PlayList>(true))
            {
                ListasDeReproduccion.Add(new PlayList() { Nombre = "Todas las canciones...", Canciones = this.Canciones.ToList() });
                foreach (string sFile in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "PlayLists")))
                {
                    FileInfo fi = new FileInfo(sFile);
                    ListasDeReproduccion.Add(rw.ReadJSON(fi.Name));
                }
            }

            if (ListasDeReproduccion != null)
            {
                listaDeReproduccionSeleccionada = ListasDeReproduccion.FirstOrDefault();
                RaisePropertyChanged("ListaDeReproduccionSeleccionada");
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
