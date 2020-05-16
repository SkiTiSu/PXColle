using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Security.Cryptography;

namespace PXColle.Master.Base
{
    public class PXBTypeManager
    {
        const string CONNECT_RELATIONSHIP_INHERITANCE = "inheritance";
        IPXBNodeManager nodeManager;

        public PXBTypeManager(IPXBNodeManager manager)
        {
            nodeManager = manager;
        }

        public Dictionary<string, string> GetTemplateParams(string key)
        {
            var res = new Dictionary<string, string>();
            GetTemplateParamsInner(key, ref res);
            return res;
        }

        public void GetTemplateParamsInner(string key, ref Dictionary<string, string> res)
        {
            var node = nodeManager.GetType(key);
            foreach (var kv in node.Templates)
            {
                // 下级可以覆盖上级同名成员的类型
                if (!res.ContainsKey(kv.Key))
                {
                    res.Add(kv.Key, kv.Value);
                }
            }
            if ((node.Connects?.Count ?? 0) == 0)
            {
                return;
            }
            else
            {
                foreach (var con in node.Connects)
                {
                    if (con.Value == CONNECT_RELATIONSHIP_INHERITANCE)
                    {
                        GetTemplateParamsInner(con.Key, ref res);
                    }
                }
            }
        }
    }
}
