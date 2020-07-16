using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColaLang.PluginFramework
{
    public class PluginData : Attribute
    {
        private string name = "";
        private string author = "";
        private string description = "";
        private Version version = null;
        private string[] dependencies = new string[0];

        public PluginData(string name, Version version)
        {
            this.name = name;
            this.version = version;
        }

        public PluginData(string name, Version version, string description)
        {
            this.name = name;
            this.version = version;
            this.description = description;

        }

        public string Name
        {
            get => name;
            set
            {
                if (value == null || value == "")
                    name = "";
                else
                    name = value;
            }
        }

        public string Description
        {
            get => description;
            set
            {
                if (value == "" || value == null)
                    description = "";
                else
                    description = value;
            }
        }
        public Version Version
        {
            get => version;
            set
            {
                if (value == null)
                    version = new Version(0, 1);
                else
                    version = value;
            }
        }
        public string[] Dependencies
        {
            get => dependencies;
            set
            {
                if (value.Length == 0 || value == null)
                    dependencies = new string[0];
                else
                    dependencies = value;
            }
        }

        public static PluginData GetPluginData(object obj)
        {
            if (!(obj == null))
            {
                object[] attributes = obj.GetType().GetCustomAttributes(typeof(PluginData), true);
                PluginData plugData = (PluginData)attributes.FirstOrDefault();
                return plugData;
            }
            return null;
        }
    }
}
