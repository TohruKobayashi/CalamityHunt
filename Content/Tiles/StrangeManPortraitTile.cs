using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityHunt.Content.Tiles
{
    public class StrangeManPortraitTile : ModTile
    {
        public const int FrameWidth = 18 * 3;
        public const int FrameHeight = 18 * 4;

        internal static Dictionary<int, Asset<Texture2D>> TrophyAssets;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Width = 10;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16 };
            TileObjectData.newTile.Origin = new Point16(5, 2);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Painting"));
        }
    }
}
