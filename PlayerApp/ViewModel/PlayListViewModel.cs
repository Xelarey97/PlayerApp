using GalaSoft.MvvmLight;
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
using PlayerApp.Utils;

namespace PlayerApp.ViewModel
{
    public class PlayListViewModel : ViewModelBase
    {
        #region Propiedades
        private ObservableCollection<Cancion> allSongsList;
        private  ObservableCollection<Cancion> addSongsList;
        private Cancion cancionSeleccionada;
        private Cancion cancionSeleccionadaQuit;
        private string newPlayListName;

        public ObservableCollection<Cancion> AllSongsList
        {
            get { return allSongsList; }
            set
            {
                allSongsList = value;
                RaisePropertyChanged("AllSongsList");
            }
        }

        public ObservableCollection<Cancion> AddSongList
        {
            get { return addSongsList; }
            set
            {
                addSongsList = value;
                RaisePropertyChanged("AddSongList");
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

        public Cancion CancionSeleccionadaQuit
        {
            get { return cancionSeleccionadaQuit; }
            set
            {
                cancionSeleccionadaQuit = value;
                RaisePropertyChanged("CancionSeleccionada");
            }
        }

        public string NewPlayListName
        {
            get { return newPlayListName; }
            set
            {
                newPlayListName = value;
                RaisePropertyChanged("NewPlayListName");
            }
        }
        #endregion

        #region Commands
        public ICommand AddCommand { get; private set; }
        public ICommand QuitCommand { get; private set; }
        public ICommand AddPlayListCommand { get; private set; }
        #endregion

        public PlayListViewModel()
        {
            LoadCanciones();
            AddCommand = new RelayCommand(AddMethod);
            QuitCommand = new RelayCommand(QuitMethod);
            AddSongList = new ObservableCollection<Cancion>();
            AddPlayListCommand = new RelayCommand(AddPlayListMethod);
        }

        #region Command Methods
        private void AddMethod()
        {
            if (cancionSeleccionada != null)
            {
                int? index = AllSongsList.IndexOf(cancionSeleccionada);
                if (index.HasValue)
                {
                    AddSongList.Add(cancionSeleccionada);
                    AllSongsList.RemoveAt((int)index);
                    RaisePropertyChanged("AddSongList");
                }
            }
        }

        private void QuitMethod()
        {
            if (cancionSeleccionadaQuit != null)
            {
                int? index = AddSongList.IndexOf(cancionSeleccionadaQuit);
                if (index.HasValue)
                {
                    AllSongsList.Add(cancionSeleccionadaQuit);
                    AddSongList.RemoveAt((int)index);
                    RaisePropertyChanged("AllSongsList");
                }
            }
        }

        private void AddPlayListMethod()
        {
            if (AddSongList != null && AddSongList.Count > 0 && !string.IsNullOrEmpty(NewPlayListName))
            {
                PlayList pl = new PlayList();
                pl.Canciones = new List<Cancion>();
                pl.Nombre = NewPlayListName;
                pl.Canciones.AddRange(AddSongList);
                ReadWriteJSON<PlayList> rw = new ReadWriteJSON<PlayList>(true);
                rw.SaveJSON(pl, pl.Nombre);
                LoadCanciones();
                AddSongList.Clear();
                NewPlayListName = "";
            }
        }
        #endregion

        #region Private Methods
        private void LoadCanciones()
        {
            string path = Directory.GetCurrentDirectory();
            string sDirectory = string.Format("{0}\\Musica", path);
            allSongsList = new ObservableCollection<Cancion>();
            int i = 0;
            foreach (var filename in Directory.GetFiles(sDirectory))
            {
                FileInfo fi = new FileInfo(filename);
                TagLib.File tagFile = TagLib.File.Create(filename);
                Cancion cancion = new Cancion()
                {
                    ID = i,
                    Nombre = Path.GetFileNameWithoutExtension(fi.Name),
                    Artista = tagFile.Tag.FirstAlbumArtist,
                    Genero = tagFile.Tag.FirstGenre,
                    Album = tagFile.Tag.Album,
                    Ruta = filename
                };
                AllSongsList.Add(cancion);
                this.RaisePropertyChanged(() => this.AllSongsList);
                i++;
            }
        }
        #endregion
    }
}
