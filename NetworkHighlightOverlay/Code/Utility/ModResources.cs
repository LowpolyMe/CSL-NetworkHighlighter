using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ColossalFramework.Plugins;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.Utility
{
    public static class ModResources
    {
        private static string _modDirectory;

        private static string ModDirectory
        {
            get
            {
                if (_modDirectory == null)
                {
                    _modDirectory = ResolveModDirectory();
                    Debug.Log($"[NetworkHighlightOverlay] ModDirectory resolved to: {_modDirectory}");
                }

                return _modDirectory;
            }
        }

        private static string ResolveModDirectory()
        {
            try
            {
                PluginManager pluginManager = PluginManager.instance;
                Assembly thisAssembly = typeof(ModResources).Assembly;

                if (pluginManager != null)
                {
                    foreach (PluginManager.PluginInfo plugin in pluginManager.GetPluginsInfo())
                    {
                        if (plugin == null)
                            continue;

                        // ✔ THIS is the correct API:
                        List<Assembly> assemblies = plugin.GetAssemblies();
                        if (assemblies == null)
                            continue;

                        foreach (Assembly asm in assemblies)
                        {
                            if (asm == thisAssembly)
                            {
                                if (!string.IsNullOrEmpty(plugin.modPath)) return plugin.modPath;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[NetworkHighlightOverlay] Failed to resolve mod path via PluginManager: {ex}");
            }

            // Fallback 1: assembly location
            string asmLocation = typeof(ModResources).Assembly.Location;
            if (!string.IsNullOrEmpty(asmLocation)) return Path.GetDirectoryName(asmLocation);

            // Fallback 2: current directory (last resort)
            Debug.LogWarning("[NetworkHighlightOverlay] Assembly location is empty, using Environment.CurrentDirectory as fallback.");
            return Environment.CurrentDirectory;
        }



        private static string ResourcesPath => Path.Combine(ModDirectory, "Resources");

        public static Texture2D LoadTexture(string fileName)
        {
            string fullPath = Path.Combine(ResourcesPath, fileName);
            if (!File.Exists(fullPath))
            {
                Debug.LogError($"[NetworkHighlightOverlay] Resource not found: {fullPath}");
                return null;
            }

            byte[] data = File.ReadAllBytes(fullPath);
            Texture2D tex = new Texture2D(2, 2); // Will resize automatically
            tex.LoadImage(data);
            tex.wrapMode = TextureWrapMode.Clamp;
            return tex;
        }
    }
}
