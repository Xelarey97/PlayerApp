using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerApp.Utils
{
    public class LoadSavePLFromJSON<T> : IDisposable
    {
        private string PLAYLIST_PATH = string.Format("{0}\\PlayLists", Directory.GetCurrentDirectory());

        public LoadSavePLFromJSON(){}

        public LoadSavePLFromJSON(bool CreateDir)
        {
            if (!Directory.Exists(PLAYLIST_PATH) && CreateDir)
            {
                Directory.CreateDirectory(PLAYLIST_PATH);
            }
        }

        public bool SaveJSON(T saveObject, string Filename)
        {
            JsonSerializer serializer = new JsonSerializer();
            string FileRoute = string.Format("{0}\\{1}.json", PLAYLIST_PATH, Filename);
            var objectTyped = (T)saveObject;
            using (StreamWriter sw = new StreamWriter(FileRoute))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, objectTyped);
            }

            if (File.Exists(FileRoute))
            {
                return true;
            }
            return false;            
        }

        public T ReadJSON(string Filename)
        {
            string EndFile = string.Format("{0}\\{1}", PLAYLIST_PATH, Filename);
            using (StreamReader file = File.OpenText(EndFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                var obj = (T)serializer.Deserialize(file, typeof(T));
                return obj;
            }
        }

        public void Dispose()
        {
            
        }
    }
}
