using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColaLang.PluginFramework
{
    public interface IPluginLoader
    {
        IReadOnlyList<Plugin> GetPlugins();

        void LoadPlugins();

        void UnloadPlugins();
    }
}
