using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using NetworkHighlightOverlay.Code.Utility;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.GUI
{
    internal static class ToggleButtonAtlas
    {
        private const string TextureFileName = "ToggleButton.png";
        private const string AtlasName = "NHO_ToggleButtonAtlas";

        internal const string InactiveSpriteName = "NHO_ToggleInactive";
        internal const string ActiveSpriteName = "NHO_ToggleActive";
        internal const string HoveredSpriteName = "NHO_ToggleHovered";
        internal const string PressedSpriteName = "NHO_TogglePressed";

        private static UITextureAtlas _atlas;

        internal static UITextureAtlas GetOrCreate()
        {
            if (_atlas != null)
                return _atlas;

            Texture2D texture = ModResources.LoadTexture(TextureFileName);
            if (texture == null)
                throw new InvalidOperationException("Missing required toggle atlas texture: Resources/" + TextureFileName);

            float spriteWidth = texture.width / 4f;
            float spriteHeight = texture.height;

            IDictionary<string, Rect> spritePixels = new Dictionary<string, Rect>(4);
            spritePixels.Add(InactiveSpriteName, new Rect(0f, 0f, spriteWidth, spriteHeight));
            spritePixels.Add(ActiveSpriteName, new Rect(spriteWidth, 0f, spriteWidth, spriteHeight));
            spritePixels.Add(HoveredSpriteName, new Rect(spriteWidth * 2f, 0f, spriteWidth, spriteHeight));
            spritePixels.Add(PressedSpriteName, new Rect(spriteWidth * 3f, 0f, spriteWidth, spriteHeight));

            _atlas = SpriteAtlasExtractor.CreateAtlas(AtlasName, texture, spritePixels);
            return _atlas;
        }

        internal static void Clear()
        {
            UITextureAtlas atlas = _atlas;
            _atlas = null;

            if (atlas == null)
                return;

            if (atlas.material != null)
            {
                Texture texture = atlas.material.mainTexture;
                UnityEngine.Object.Destroy(atlas.material);
                if (texture != null)
                {
                    UnityEngine.Object.Destroy(texture);
                }
            }

            UnityEngine.Object.Destroy(atlas);
        }
    }
}
