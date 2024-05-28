using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.Bestiary;
using Terraria.UI;

namespace CalamityHunt.Common.Graphics
{
    public class SlimeMonsoonPortraitBackground : IBestiaryInfoElement, IBestiaryBackgroundImagePathAndColorProvider
    {
        public Asset<Texture2D> GetBackgroundImage() => AssetDirectory.Textures.SlimeMonsoon.MapBG;
        public Color? GetBackgroundColor() => Color.Purple;
        public UIElement ProvideUIElement(BestiaryUICollectionInfo info) => null;
    }
}
