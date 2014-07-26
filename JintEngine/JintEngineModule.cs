﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace JintEngine
{
    using Fougerite;

    public class JintEngineModule : Module
    {
        public override string Name
        {
            get { return "JintEngine"; }
        }

        public override string Author
        {
            get { return "Riketta, mikec, EquiFox & xEnt"; }
        }

        public override string Description
        {
            get { return "Jint Javascript Plugin Engine"; }
        }

        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        private static JintEngineModule instance;
        private DirectoryInfo pluginDirectory;
        private Dictionary<string, JavascriptPlugin> plugins;

        public static JintEngineModule Instance()
        {
            if (instance == null)
                instance = new JintEngineModule();
            return instance;
        }

        private JintEngineModule()
        {
            pluginDirectory = new DirectoryInfo(Util.GetFougeriteFolder());
            plugins = new Dictionary<string, JavascriptPlugin>();
            ReloadPlugins();
        }        

        private IEnumerable<String> GetPluginNames()
        {
            foreach (var dirInfo in pluginDirectory.GetDirectories())
            {
                var path = Path.Combine(dirInfo.FullName, dirInfo.Name + ".js");
                if (File.Exists(path)) yield return dirInfo.Name;
            }
        }
        private String GetPluginDirectoryPath(String name)
        {
            return Path.Combine(pluginDirectory.FullName, name);
        }
        private String GetPluginScriptPath(String name)
        {
            return Path.Combine(GetPluginDirectoryPath(name), name + ".js");
        }

        private string GetPluginScriptText(string name)
        {
            string path = GetPluginScriptPath(name);
            string[] strArray = File.ReadAllLines(path);
            string scriptHeader = @"
                var Datastore = DataStore, IsNull = Util.IsNull, toLowerCase = Data.ToLower, Time = Plugin;
                var GetStaticField = Util.GetStaticField, SetStaticField = Util.SetStaticField, InvokeStatic = Util.InvokeStatic;
                var Rust = importNamespace('Rust'), Facepunch = importNamespace('Facepunch'), Magma = importNamespace('Fougerite');
                var UnityEngine = importNamespace('UnityEngine'), uLink = importNamespace('uLink'), RustPP = importNamespace('RustPP');  
                var ParamsList = function ParamsList() {
                    var list = System.Collections.Generic.List(System.Object);
                    this.objs = new list();
                };
                ParamsList.prototype = {
                    Remove: function Remove(o) { this.objs.Remove(o); },
                    Get: function Get(i) { return this.objs[parseInt(i, 10)]; },
                    Add: function Add(o) { this.objs.Add(o); },
                    ToArray: function ToArray() { return this.objs.ToArray(); },
                    get Length () { return this.objs.Count; }
                };
                ";
            if (strArray[0].Contains("Fougerite") || strArray[0].Contains("fougerite") || strArray[0].Contains("FOUGERITE"))
                return String.Join("\r\n", strArray);
            return scriptHeader + String.Join("\r\n", strArray);
        }

        public void UnloadPlugin(string name, bool removeFromDict = true)
        {
            Logger.Log("Unloading " + name + " plugin.");

            if (plugins.ContainsKey(name))
            {
                var plugin = plugins[name];
                plugin.RemoveHooks();
                plugin.KillTimers();
                if (removeFromDict) plugins.Remove(name);

                Logger.Log(name + " plugin was unloaded successfuly.");
            }
            else
            {
                Logger.LogError("Can't unload " + name + ". Plugin is not loaded.");
                throw new InvalidOperationException("Can't unload " + name + ". Plugin is not loaded.");
            }
        }

        public void UnloadPlugins()
        {
            foreach (var name in plugins.Keys)
                UnloadPlugin(name, false);
            plugins.Clear();
        }

        private void LoadPlugin(string name)
        {
            Logger.Log("Loading plugin " + name + ".");

            if (plugins.ContainsKey(name))
            {
                Logger.LogError(name + " plugin is already loaded.");
                throw new InvalidOperationException(name + " plugin is already loaded.");
            }

            try
            {
                String text = GetPluginScriptText(name);
                DirectoryInfo dir = new DirectoryInfo(Path.Combine(pluginDirectory.FullName, name));
                JavascriptPlugin plugin = new JavascriptPlugin(dir, name, text);
                plugin.InstallHooks();
                plugins[name] = plugin;

                Logger.Log(name + " plugin was loaded successfuly.");
            }
            catch (Exception ex)
            {
                string arg = name + " plugin could not be loaded.";
                Server.GetServer().Broadcast(arg);
                Logger.LogException(ex);
            }
        }

        public void ReloadPlugin(string name)
        {
            UnloadPlugin(name);
            LoadPlugin(name);
        }

        public void ReloadPlugins()
        {
            UnloadPlugins();

            foreach (var name in GetPluginNames())
                LoadPlugin(name);

            Data.GetData().Load();
        }
    }
}