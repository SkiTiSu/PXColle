using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PXColle.Trigger
{
    public class PXConfigItem
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Type { get; set; } = "string";
        public string Pattern { get; set; }
        public string Description { get; set; }
        public string ErrorMessage { get; set; }
        public bool Required { get; set; } = false;

        public string Value { get; set; }
        public bool ValueBool
        {
            get
            {
                if (string.IsNullOrEmpty(Value))
                {
                    Value = "false";
                }
                return Value == "true" ? true : false;
            }
            set
            {
                Value = value ? "true" : "false";
            }
        }

        public bool IsValid
        {
            get
            {
                switch (Type)
                {
                    default:
                    case "string":
                        if (Value == null || Pattern == null)
                        {
                            return false;
                        }
                        else
                        {
                            return Regex.IsMatch(Value, Pattern);
                        }
                        break;
                    case "bool":
                        if (Value == "true" || Value == "false")
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                }

            }
        }

        public KeyValuePair<string, string> KeyValue
        {
            get => new KeyValuePair<string, string>(Key, Value);
        }
    }
}
