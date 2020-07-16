using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColaLang.PluginFramework
{
    public class Plugin : IPlugin
    {
        private string name = "";
        private Version version = null;
        private string description = "";
        private Guid id = Guid.Empty;
        private string[] dependancies = new string[0];
        private bool toggled = false;
        private FileInfo pluginFile = null;
        private PluginLoader loader = null;

        public Plugin()
        {
            id = new Guid();
            //bind from attribute.
            PluginData data = PluginData.PluginData(this);
            name = data.Name;
            version = data.Version;
            description = data.Description;
            dependancies = data.Dependencies;
        }

        public string Description()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }

        public string Name()
        {
            throw new NotImplementedException();
        }

        public void OnLoad()
        {
            throw new NotImplementedException();
        }

        public string[] PluginDependancies()
        {
            throw new NotImplementedException();
        }

        public Guid PluginID()
        {
            throw new NotImplementedException();
        }

        public Version Version()
        {
            throw new NotImplementedException();
        }
    }
}
