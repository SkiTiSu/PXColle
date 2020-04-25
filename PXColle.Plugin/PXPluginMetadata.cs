using System;
using System.Collections.Generic;
using System.Text;

namespace PXColle.Plugin
{
    public class PXPluginMetadata
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public string Website { get; set; }
        public string ProgramLang { get; set; }
        public string ExecuteFileName { get; set; }
        public List<ConfigItem> ConfigItems { get; set; }
    }

    public class ConfigItem
    {
        public string Name { get; set; }
        public bool Required { get; set; }
        public string Pattern { get; set; }
        public string Description { get; set; }
    }
}
