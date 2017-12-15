using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
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
        }

        private void LoadCanciones()
        {
            canciones = new ObservableCollection<Cancion>();
            canciones.Add(new Cancion { Nombre = "Soy Peor", Album = "Unknown", Genero = "Trap", ID = 1, Artista = "Bad Bunny" });
            canciones.Add(new Cancion { Nombre = "Soy Peor", Album = "Unknown", Genero = "Trap", ID = 2, Artista = "Bad Bunny" });
            canciones.Add(new Cancion { Nombre = "Soy Peor", Album = "Unknown", Genero = "Trap", ID = 3, Artista = "Bad Bunny" });
            canciones.Add(new Cancion { Nombre = "Soy Peor", Album = "Unknown", Genero = "Trap", ID = 4, Artista = "Bad Bunny" });
            this.RaisePropertyChanged(() => this.Canciones);
        }

        #region Command Methods
        #endregion
    }
}
