using System;
using System.IO;
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
                    _modDirectory = GetModDirectory();
                return _modDirectory;
            }
        }

        private static string GetModDirectory()
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(basePath, @"Colossal Order\Cities_Skylines\Addons\Mods\NetworkHighlightOverlay");
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

        public static string LoadTextFile(string fileName)
        {
            string fullPath = Path.Combine(ResourcesPath, fileName);
            return File.Exists(fullPath) ? File.ReadAllText(fullPath) : null;
        }

        public static byte[] LoadRawFile(string fileName)
        {
            string fullPath = Path.Combine(ResourcesPath, fileName);
            return File.Exists(fullPath) ? File.ReadAllBytes(fullPath) : null;
        }
    }
}