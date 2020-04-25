using PXColle.Action;
using System;
using System.Collections.Generic;
using System.Text;

namespace PXColle.Master
{

    public class PXData
    {
        public string Name { get; set; }
        public string MD5 { get; set; }
        public string Url { get; set; }
        public List<PXDataDetail> Vers { get; set; }
    }

    public class PXDataDetail
    {
        public string Name { get; set; }
        public string Timestamp { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class PXManagedFile
    {
        public string Name { get; set; }
        public long Length { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string From { get; set; }
        public PXActionContext ActionContext { get; set; }
        public string MD5 { get; set; }
        public string Path { get; set; }

    }
}
