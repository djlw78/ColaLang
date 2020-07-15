﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColaLang.PluginFramework
{
    public interface IPlugin
    {
        string Name();

        Version Version();

        string Description();

        Guid PluginID();

        string[] PluginDependancies();

        void OnLoad();

        void Execute();

    }
}
