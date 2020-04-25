using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PXColle.Common
{
    public class LocalStorageRealFile : LocalStorage, ILocalStorage
    {
        public bool FormattingIndented { get; set; } = true;
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
        };
        public LocalStorageRealFile(string path)
            : base(path, null)
        {
        }

        public override void Load()
        {
            if (File.Exists(Path))
            {
                Storage = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(Path), settings);
            }
        }

        public override void Destroy()
        {
            File.Delete(Path);
        }

        public override void Persist()
        {
            lock (writeLock)
            {
                Formatting format = FormattingIndented ? Formatting.Indented : Formatting.None;
                File.WriteAllText(Path, JsonConvert.SerializeObject(Storage, format, settings));
            }
        }
    }
}
