using GalaSoft.MvvmLight;
using PlayerApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Models;
using YoutubeExplode.Models.MediaStreams;

namespace PlayerApp.Model
{
    public class CancionesDescargas : ObservableObject
    {
        private string titulo;
        private string url;
        private string autor;
        private EstadoDescarga estado;
        private Video videoInfo;

        public string Titulo
        {
            get { return titulo; }
            set { Set<string>(() => this.Titulo, ref titulo, value); }
        }

        public string Url
        {
            get { return url; }
            set { Set<string>(() => this.Url, ref url, value); }
        }

        public string Autor
        {
            get { return autor; }
            set { Set<string>(() => this.Autor, ref autor, value); }
        }

        public EstadoDescarga Estado
        {
            get { return estado; }
            set { Set<EstadoDescarga>(() => this.Estado, ref estado, value); }
        }

        public Video VideoInfo
        {
            get { return videoInfo; }
            set { Set<Video>(() => this.VideoInfo, ref videoInfo, value); }
        }
    }
}
