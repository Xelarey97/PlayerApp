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
using MvvmDialogs;
using System.Reflection;
using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using MvvmDialogs.FrameworkDialogs.SaveFile;
using System.Text.RegularExpressions;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;
using YoutubeExplode.Models;
using CliWrap;

namespace PlayerApp.ViewModel
{
    public class DownloadViewModel : ViewModelBase
    {
        #region Propiedades
        private ObservableCollection<CancionesDescargas> listaCancionesDescargar;
        private string downloadFileListPath;

        public ObservableCollection<CancionesDescargas> ListaCancionesDescargar
        {
            get { return listaCancionesDescargar; }
            set
            {
                listaCancionesDescargar = value;
                RaisePropertyChanged("ListaCancionesDescargar");
            }
        }

        public string DownloadFileListPath
        {
            get { return downloadFileListPath; }
            set
            {
                downloadFileListPath = value;
                RaisePropertyChanged("DownloadFileListPath");
            }
        }
        #endregion

        #region Commands
        public ICommand OpenFileDialogCommand { get; private set; }
        public ICommand DownloadFilesCommand { get; private set; }
        #endregion

        private readonly IDialogService dialogService;
        private ViewModelLocator loc = HelperMethods.GetLocator();

        public DownloadViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            listaCancionesDescargar = new ObservableCollection<CancionesDescargas>();

            OpenFileDialogCommand = new RelayCommand(OpenFileDialogMethod);
            DownloadFilesCommand = new RelayCommand(DownloadFilesMethod);

            if (!Directory.Exists(AppGenericDirectories.DownloadsDirectory))
            {
                Directory.CreateDirectory(AppGenericDirectories.DownloadsDirectory);
            }
        }

        #region Command methods
        private async void OpenFileDialogMethod()
        {
            var settings = new SaveFileDialogSettings
            {
                Title = "Seleccionar lista",
                InitialDirectory = AppGenericDirectories.DownloadsDirectory,
                Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*",
                CheckFileExists = false
            };

            bool? success = dialogService.ShowSaveFileDialog(this, settings);
            if (success == true)
            {
                DownloadFileListPath = settings.FileName;

                string[] ytLinks = File.ReadAllLines(DownloadFileListPath);

                foreach (string url in ytLinks)
                {
                    if (IsYouTubeUrl(url))
                    {
                        listaCancionesDescargar.Add(new CancionesDescargas()
                        {
                            Url = url,
                        });
                    }
                }

                foreach (CancionesDescargas cd in listaCancionesDescargar)
                {
                    var msi = await LoadMediaInfo(cd.Url);
                    if (msi != null)
                    {
                        cd.Titulo = msi.Title;
                        cd.Autor = msi.Author;
                        cd.VideoInfo = msi;
                    }
                }
                RaisePropertyChanged("ListaCancionesDescargar");
            }            
        }

        private async void DownloadFilesMethod()
        {
            if (listaCancionesDescargar != null)
            {
                foreach (CancionesDescargas cd in listaCancionesDescargar)
                {
                    cd.Estado = EstadoDescarga.Iniciada;
                    var client = new YoutubeClient();
                    var id = YoutubeClient.ParseVideoId(cd.Url);
                    var streamInfoSet = await client.GetVideoMediaStreamInfosAsync(id);

                    string pattern = "[\\~#%&*{}/:<>?|\"-]";
                    string replacement = " ";

                    Regex regEx = new Regex(pattern);
                    string title = Regex.Replace(regEx.Replace(cd.Titulo, replacement), @"\s+", " ");

                    var streamInfo = streamInfoSet.Audio.OrderByDescending(x => x.AudioEncoding).FirstOrDefault();
                    string ext = ".webm";
                    string fileName = string.Format("{0}\\{1}{2}", AppGenericDirectories.DownloadsDirectory, title, ext);
                    await client.DownloadMediaStreamAsync(streamInfo, fileName);

                    cd.Estado = EstadoDescarga.Convirtiendo;

                    var outputFilePath = Path.Combine(AppGenericDirectories.MusicDirectory, title + ".mp3");
                    if (!File.Exists(outputFilePath))
                    {
                        await AppGenericDirectories.FfmpegCli.ExecuteAsync(string.Format("-i \"{0}\" -q:a 0 -map a \"{1}\" -y", fileName, outputFilePath));
                    }
                    File.Delete(fileName);
                    
                    var file = TagLib.File.Create(outputFilePath);

                    string[] artistas = new string[]{ cd.Autor };
                    file.Tag.AlbumArtists = artistas;
                    file.Save();

                    cd.Estado = EstadoDescarga.Finalizada;
                }
                listaCancionesDescargar.Clear();
                listaCancionesDescargar = null;

                loc.HomeViewModel.Canciones.LoadCanciones();
            }
        }
        #endregion

        #region Private Methods
        private static bool IsYouTubeUrl(string testUrl)
        {
            return TestUrl(@"^(http://youtu\.be/([a-zA-Z0-9]|_)+($|\?.*)|https?://www\.youtube\.com/watch\?v=([a-zA-Z0-9]|_)+($|&).*)", testUrl);
        }

        private static bool TestUrl(string pattern, string testUrl)
        {
            Regex l_expression = new Regex(pattern, RegexOptions.IgnoreCase);

            return l_expression.Matches(testUrl).Count > 0;
        }

        private async static Task<Video> LoadMediaInfo(string url)
        {
            var id = YoutubeClient.ParseVideoId(url);
            var client = new YoutubeClient();
            var videoInfo = await client.GetVideoAsync(id);
            if (videoInfo != null)
                return videoInfo;
            return null;
        }
        #endregion
    }
}
