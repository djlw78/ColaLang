using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ColaLang.PluginFramework
{
    public sealed class PluginLoader : IPluginLoader
    {
        private List<Plugin> plugins = null;

        public PluginLoader(int maxPlugins = 5000)
        {
            if (maxPlugins <= 0)
                plugins = new List<Plugin>();
            else
                plugins = new List<Plugin>(maxPlugins);
        }

        public IReadOnlyList<Plugin> GetPlugins() => plugins;

        public void LoadPlugins()
        {
            //Check if plugin directory exists.
            if (!(PluginDirectory.Exists))
                PluginDirectory.Create();
            else
            {
                //Load plugins.
                FileInfo[] files = PluginDirectory.GetFiles(".dll", SearchOption.TopDirectoryOnly);
                foreach (FileInfo file in files)
                {
                    LoadPluginFile(file);
                }

                foreach (Plugin plugin in plugins)
                {
                    if (RequiresDependencies(plugin))
                    {
                        foreach (string s in plugin.GetPluginDependencies())
                        {
                            var plug = GetPluginByName(s);
                            if (plug == null) throw new DependencyNotFoundException(plugin);
                            else
                                continue;
                        }
                        //Load Reguardless.
                        plugin.OnPluginLoaded(this);
                        plugin.Enable();
                        //Create plugin data directory.
                        if (!(plugin.GetPluginDataDirectory().Exists))
                            plugin.GetPluginDataDirectory().Create();
                    }
                    else
                    {
                        plugin.OnPluginLoaded(this);
                        plugin.Enable();
                        //Create plugin data directory.
                        if (!(plugin.GetPluginDataDirectory().Exists))
                            plugin.GetPluginDataDirectory().Create();
                    }
                }
            }
        }

        public void LoadPlugin(FileInfo file)
        {
            Type pType = typeof(Plugin);
            Plugin plugin = null;
            Assembly assembly = Assembly.LoadFrom(file.FullName);
            if (assembly == null) return;
            else
            {
                Type[] types = assembly.GetExportedTypes();
                foreach (Type t in types)
                {
                    if (!(t.IsClass) && t.IsNotPublic)
                        continue;

                    if (t.IsAssignableFrom(pType))
                    {
                        plugin = Activator.CreateInstance(t) as Plugin;
                        plugin.SetFile(file);
                        plugin.SetLoader(this);
                    }
                }
            }

            if (RequiresDependencies(plugin))
            {
                foreach (string s in plugin.GetPluginDependencies())
                {
                    var plug = GetPluginByName(s);
                    if (plug == null) throw new DependencyNotFoundException(plugin);
                    else
                        continue;
                }
                //Load Reguardless.
                plugin.OnPluginLoaded(this);
                plugin.Enable();
                //Create plugin data directory.
                if (!(plugin.GetPluginDataDirectory().Exists))
                    plugin.GetPluginDataDirectory().Create();
            }
            else
            {
                plugin.OnPluginLoaded(this);
                plugin.Enable();
                //Create plugin data directory.
                if (!(plugin.GetPluginDataDirectory().Exists))
                    plugin.GetPluginDataDirectory().Create();
            }
        }

        private void LoadPluginFile(FileInfo f)
        {
            Type pType = typeof(Plugin);
            Assembly assembly = Assembly.LoadFrom(f.FullName);
            if (assembly == null)
                return;
            else
            {
                Type[] types = assembly.GetExportedTypes();
                foreach (Type t in types)
                {
                    if (!(t.IsClass) && t.IsNotPublic)
                        continue;

                    if (t.IsAssignableFrom(pType))
                    {
                        Plugin plugin = Activator.CreateInstance(t) as Plugin;
                        plugin.SetFile(f);
                        plugin.SetLoader(this);
                        if (plugin != null)
                            plugins.Add(plugin);
                    }
                }
            }
        }

        public void UnloadPlugin(Plugin plugin)
        {
            if (plugins.Contains(plugin))
            {
                plugin.OnPluginUnloaded(this);
                plugin.Disable();
                plugins.Remove(plugin);
            }
        }

        public void UnloadPlugins()
        {
            foreach (Plugin p in plugins)
            {
                p.OnPluginUnloaded(this);
                p.Disable();
                plugins.Remove(p);
            }
        }

        public void ReloadPlugins()
        {
            UnloadPlugins();
            LoadPlugins();
        }

        public Plugin GetPluginByName(string name)
        {
            foreach (Plugin p in plugins)
                if (p.Name().Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return p;
            return null;
        }

        public Plugin GetPluginByID(Guid ID)
        {
            foreach (Plugin p in plugins)
                if (p.PluginID() == ID)
                    return p;
            return null;

        }

        public Plugin GetPluginByID(string ID) => GetPluginByID(Guid.Parse(ID));

        public Plugin GetPlugin(string value)
        {
            var pname = GetPluginByName(value);
            var pid = GetPluginByID(value);
            if (pname != null && pid == null)
                return pname;
            else if (pname == null && pid != null)
                return pid;
            else if (pname != null && pid != null)
                return pid;
            else
                return null;
        }

        public bool RequiresDependencies(Plugin p)
        {
            if (p.PluginDependancies().Length <= 0 || (p.PluginDependancies() == null))
                return false;
            return true;
        }

        static readonly DirectoryInfo WorkingDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
        public readonly DirectoryInfo PluginDirectory = new DirectoryInfo(WorkingDirectory.FullName + "\\Plugins");
    }
}
