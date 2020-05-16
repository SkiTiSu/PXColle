using System;
using System.Collections.Generic;
using System.Text;

namespace PXColle.Master.Base
{
    public interface IPXBNodeManager
    {
        //IEnumerable<PXNode> GetNodesConnectsTo(string id);
        //PXNode GetNode(string id);
        PXNodeType GetType(string key);
    }
}
