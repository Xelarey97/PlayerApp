using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerApp.Model
{
    public class Cancion : ObservableObject
    {
        #region Propiedades
        private int id;
        private string nombre;
        private string artista;
        private string album;
        private float duracion;
        private string genero;
        private string ruta;

        public int ID
        {
            get => id;
            set => Set<int>(() => this.ID, ref id, value);
        }

        public string Nombre
        {
            get => nombre;
            set => Set<string>(() => this.Nombre, ref nombre, value);
        }

        public string Artista
        {
            get => artista;
            set => Set<string>(() => this.Artista, ref artista, value);
        }

        public string Album
        {
            get => album;
            set => Set<string>(() => this.Album, ref album, value);
        }

        public float Duracion
        {
            get => duracion;
            set => Set<float>(() => this.Duracion, ref duracion, value);
        }

        public string Genero
        {
            get => genero;
            set => Set<string>(() => this.Genero, ref genero, value);
        }

        public string Ruta
        {
            get => ruta;
            set => Set<string>(() => this.Ruta, ref ruta, value);
        }
        #endregion
    }
}
