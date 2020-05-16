using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PXColle.Master.Base
{
    public class PXBNodeManagerInMemory : IPXBNodeManager
    {
        List<PXNode> db;
        public PXBNodeManagerInMemory(IEnumerable<PXNode> nodes)
        {
            db = nodes.ToList();
        }

        public PXNodeType GetType(string key)
        {
            return (PXNodeType)db.Where(x => x.Key == key).FirstOrDefault();
        }
    }
}
