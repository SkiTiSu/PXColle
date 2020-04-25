using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace PXColle.Common
{
    public class LocalStorage : ILocalStorage
    {
        public string Path { get; private set; }
        protected ILocalStorage _file;
        protected object writeLock = new object();
        public bool AutoPersist { get; set; } = true;
        protected Dictionary<string, object> Storage = new Dictionary<string, object>();

        public LocalStorage(string path, ILocalStorage file)
        {
            Path = path;
            _file = file;
            Load();
        }

        public virtual void Load()
        {
            if (_file.Exists(Path))
            {
                Storage = _file.Get<Dictionary<string, object>>(Path);
            }
        }

        public void Set(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (Storage.ContainsKey(key))
            {
                Storage.Remove(key);
            }
            Storage.Add(key, value);
            if (AutoPersist)
            {
                Persist();
            }
        }

        public void Set(string key, string value)
            => Set(key, (object)value);

        public T Get<T>(string key)
        {
            if (Storage.TryGetValue(key, out object rawValue))
            {
                if (rawValue is JObject)
                {
                    //TODO: Maybe a better solution in LocalStorageRealFile?
                    return ((JObject)rawValue).ToObject<T>();
                }
                else if (rawValue is JArray)
                {
                    return ((JArray)rawValue).ToObject<T>();
                }
                else
                {
                    return (T)rawValue;
                }
            }
            else
            {
                return default;
            }
        }

        public string Get(string key)
        //=> Get<string>(key);
        {
            if (Storage.TryGetValue(key, out object rawValue))
            {
                return rawValue.ToString();
            }
            else
            {
                return default;
            }
        }

        public bool Exists(string key)
        {
            return Storage.ContainsKey(key);
        }

        public void Remove(string key)
        {
            if (Storage.ContainsKey(key))
            {
                Storage.Remove(key);
            }
        }

        public void Clear()
        {
            Storage.Clear();

            if (AutoPersist)
            {
                Persist();
            }
        }

        public virtual void Destroy()
        {
            _file.Remove(Path);
        }

        public virtual void Persist()
        {
            //if (!File.Exists(Path))
            //{
            //    File.Create(Path);
            //}
            lock (writeLock)
            {
                
                _file.Set(Path, Storage);
            }
        }
    }
}