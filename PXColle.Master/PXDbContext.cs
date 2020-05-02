using LiteDB;
using PXColle.Action;
using System;
using System.Collections.Generic;
using System.Text;

namespace PXColle.Master
{
    public class PXDbContext : IDisposable
    {
        LiteDatabase db;
        public ILiteCollection<PXActionContext> Actions;
        public ILiteCollection<PXManagedFile> PXFiles;

        public PXDbContext(string connectionString)
        {
            db = new LiteDatabase(connectionString);
            if (db.UserVersion == 0)
            {
                //foreach (var doc in db.Engine.Find("MyCol"))
                //{
                //    doc["NewCol"] = Convert.ToInt32(doc["OldCol"].AsString);
                //    db.Engine.Update("MyCol", doc);
                //}
                //db.Engine.UserVersion = 1;
            }
            Actions = db.GetCollection<PXActionContext>("action");
            PXFiles = db.GetCollection<PXManagedFile>("pxfile");

            PXFiles.EnsureIndex(x => x.MD5, true);

            Actions.EnsureIndex(x => x.Id, true);
            Actions.EnsureIndex(x => x.Status);
            Actions.EnsureIndex(x => x.UpdatedAt);
            //Actions.EnsureIndex(x => x.CreatedAt);
        }

        public void Dispose()
        {
            ((IDisposable)db).Dispose();
        }
    }
}
