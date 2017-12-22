using CliWrap;
using PlayerApp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerApp.Utils
{
    public class AppGenericDirectories
    {
        public static readonly string MusicDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Musica");
        public static readonly string DownloadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Downloads");
        public static readonly string PlayListsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "PlayLists");
        public static readonly Cli FfmpegCli = new Cli(Directory.GetCurrentDirectory() + "\\Resources\\ffmpeg.exe");
    }
}
