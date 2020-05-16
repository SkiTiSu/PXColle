using System;
using System.Collections.Generic;
using System.Text;

namespace PXColle.Master.Base
{
    public interface IPXBTypeManager
    {
        Dictionary<string, string> GetTemplateParams(string key);
        
    }
}
