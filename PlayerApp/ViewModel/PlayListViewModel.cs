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

namespace PlayerApp.ViewModel
{
    public class PlayListViewModel : ViewModelBase
    {
        #region Propiedades
        ObservableCollection<Cancion> allSongsList;
        ObservableCollection<Cancion> addSongsList;
        #endregion
    }
}
