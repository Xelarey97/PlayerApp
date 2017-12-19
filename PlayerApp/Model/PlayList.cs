using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerApp.Model
{
    public class PlayList : ObservableObject
    {
        #region Propiedades
        private string nombre;
        private List<Cancion> canciones;

        public string Nombre
        {
            get { return nombre; }
            set { Set<string>(() => this.Nombre, ref nombre, value); }
        }

        public List<Cancion> Canciones
        {
            get { return canciones; }
            set { Set<List<Cancion>>(() => this.Canciones, ref canciones, value); }
        }
        #endregion
    }
}
