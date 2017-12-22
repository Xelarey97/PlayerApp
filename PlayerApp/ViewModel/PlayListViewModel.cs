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
        private ObservableCollection<Cancion> SongsList;
        private ObservableCollection<Cancion> allSongsList;
        private ObservableCollection<Cancion> addSongsList;
        private Cancion cancionSeleccionada;
        private Cancion cancionSeleccionadaQuit;
        private string newPlayListName;
        private string searchTextBoxText;

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

        public string SearchTextBoxText
        {
            get { return searchTextBoxText; }
            set
            {
                searchTextBoxText = value;
                RaisePropertyChanged("SearchTextBoxText");
            }
        }
        #endregion

        #region Commands
        public ICommand AddCommand { get; private set; }
        public ICommand QuitCommand { get; private set; }
        public ICommand AddPlayListCommand { get; private set; }
        public ICommand SearchBoxCommand { get; private set; }
        #endregion

        public PlayListViewModel()
        {
            AllSongsList = new ObservableCollection<Cancion>();
            SongsList = new ObservableCollection<Cancion>();

            SongsList.LoadCanciones();
            AllSongsList.AddRange(SongsList);

            AddCommand = new RelayCommand(AddMethod);
            QuitCommand = new RelayCommand(QuitMethod);
            AddSongList = new ObservableCollection<Cancion>();
            AddPlayListCommand = new RelayCommand(AddPlayListMethod);
            SearchBoxCommand = new RelayCommand(SearchBoxMethod);
            SearchTextBoxText = "";
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
                //Load Object PlayList
                PlayList pl = new PlayList() { 
                    Canciones = new List<Cancion>(),
                    Nombre = NewPlayListName,
                };
                pl.Canciones.AddRange(AddSongList);

                //Save JSON
                LoadSavePLFromJSON<PlayList> rw = new LoadSavePLFromJSON<PlayList>(true);
                rw.SaveJSON(pl, pl.Nombre);

                //Reset UI and Add item to CB PlayLists at HomeView
                AllSongsList.Clear();
                AllSongsList.AddRange(SongsList);
                AddSongList.Clear();
                NewPlayListName = "";
                (App.Current.Resources["Locator"] as ViewModelLocator).HomeViewModel.ListasDeReproduccion.Add(pl);
            }
        }

        private void SearchBoxMethod()
        {
            if (!string.IsNullOrWhiteSpace(SearchTextBoxText))
            {
                AllSongsList.Clear();
                var auxList = SongsList.Select(x => x).Where(x => x.Nombre.Contains(SearchTextBoxText)).ToList();
                AllSongsList.AddRange(auxList);
            }
            else
            {
                AllSongsList.AddRange(SongsList);
            }
            RaisePropertyChanged("AllSongsList");
        }
        #endregion

        #region Private Methods
        
        #endregion
    }
}
