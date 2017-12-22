using PlayerApp.Model;
using PlayerApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerApp.Utils
{
    public static class HelperMethods
    {

        /// <summary>
        /// Cargar todas las canciones en una lista.
        /// </summary>
        /// <param name="SongsList"></param>
        public static void LoadCanciones(this ObservableCollection<Cancion> SongsList)
        {
            string sDirectory = AppGenericDirectories.MusicDirectory;
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
                SongsList.Add(cancion);
                i++;
            }
        }

        /// <summary>
        /// AddRange para ObservableCollection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lista"></param>
        /// <param name="items">Lista que queremos añadir en la lista principal.</param>
        public static void AddRange<T>(this ObservableCollection<T> lista, IEnumerable<T> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    lista.Add(item);
                }
            }
        }

        /// <summary>
        /// Carga las listas de reproducción y selecciona la lista de reproducción que se ha escogido.
        /// </summary>
        /// <param name="ListasDeReproduccion"></param>
        /// <param name="listaDeReproduccionSeleccionada"></param>
        public static void LoadPlayLists(this ObservableCollection<PlayList> ListasDeReproduccion, ref PlayList listaDeReproduccionSeleccionada)
        {
            ObservableCollection<Cancion> auxCancion = new ObservableCollection<Cancion>();
            auxCancion.LoadCanciones();

            using (LoadSavePLFromJSON<PlayList> rw = new LoadSavePLFromJSON<PlayList>(true))
            {
                ListasDeReproduccion.Add(new PlayList() { Nombre = "Todas las canciones...", Canciones = auxCancion.ToList() });
                foreach (string sFile in Directory.GetFiles(AppGenericDirectories.PlayListsDirectory))
                {
                    FileInfo fi = new FileInfo(sFile);
                    ListasDeReproduccion.Add(rw.ReadJSON(fi.Name));
                }
            }

            if (ListasDeReproduccion != null)
            {
                listaDeReproduccionSeleccionada = ListasDeReproduccion.FirstOrDefault();
            }
        }

        /// <summary>
        /// Obtener el ViewModelLocator.
        /// </summary>
        /// <returns>Retorna un objeto de tipo ViewModelLocator.</returns>
        public static ViewModelLocator GetLocator()
        {
            ViewModelLocator loc = (App.Current.Resources["Locator"] as ViewModelLocator);
            return loc;
        }
    }
}
