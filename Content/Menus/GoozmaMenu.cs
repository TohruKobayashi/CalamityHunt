using CalamityHunt.Common.Graphics.Skies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Events;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Menus
{
    public class GoozmaMenu : ModMenu
    {
        public override Asset<Texture2D> Logo => base.Logo;

        public override Asset<Texture2D> SunTexture => AssetDirectory.Textures.Empty;

        public override Asset<Texture2D> MoonTexture => AssetDirectory.Textures.Empty;

        public override int Music => AssetDirectory.Music.GoozmaSoul;

        public override string DisplayName => "Slime Monsoon";

        public bool IsSelected => MenuLoader.CurrentMenu == this;

        public override void OnSelected()
        {
            SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.Awaken);
        }
        //public override void OnDeselected()
        //{
        //    SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.Pop);
        //}

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            drawColor = Main.DiscoColor; // Changes the draw color of the logo
            return true;
        }
    }
}
