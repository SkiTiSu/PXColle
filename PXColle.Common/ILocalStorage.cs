namespace PXColle.Common
{
    public interface ILocalStorage
    {
        bool AutoPersist { get; set; }

        void Clear();
        void Destroy();
        string Get(string key);
        T Get<T>(string key);
        bool Exists(string key);
        void Remove(string key);
        void Load();
        void Persist();
        void Set(string key, object value);
        void Set(string key, string value);
    }
}