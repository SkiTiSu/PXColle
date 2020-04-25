using System;
using System.Collections.Generic;
using System.Text;

namespace PXColle.Common
{
    public class LocalStorageProperty : LocalStorage, ILocalStorage
    {
        (Func<Dictionary<string, object>> get, Action<Dictionary<string, object>> set) storageOut;
        public LocalStorageProperty((Func<Dictionary<string, object>> get, Action<Dictionary<string, object>> set) storage)
            : base(null, null)
        {
            storageOut = storage;
            this.Storage = storage.get();
        }

        public override void Load()
        {
            //Nothing to do
        }

        public override void Destroy()
        {
            Clear();
            if (AutoPersist)
            {
                Persist();
            }
        }

        public override void Persist()
        {
            lock (writeLock)
            {
                storageOut.set(Storage);
            }
        }
    }
}
