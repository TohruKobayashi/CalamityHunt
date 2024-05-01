using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria;

namespace CalamityHunt.Content.Tiles.Autoloaded
{
    [Autoload(false)]
    public class AutoloadedBossTrophyTile : ModTile
    {
        public const int FrameWidth = 18 * 3;
        public const int FrameHeight = 18 * 4;
        public override string Texture => TexturePath + Name;
        public override string Name => InternalName != "" ? InternalName : base.Name;

        public string InternalName;
        protected readonly int ItemType;
        protected readonly string TexturePath;

        internal static Dictionary<int, Asset<Texture2D>> TrophyAssets;

        public AutoloadedBossTrophyTile(string NPCname, int dropType, string path = null)
        {
            InternalName = NPCname + "TrophyTile";
            ItemType = dropType;
            TexturePath = path;
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Trophy"));
        }

        public override bool CreateDust(int i, int j, ref int type) => false;

        public Asset<Texture2D> GetRelicTexture()
        {
            if (TrophyAssets == null)
                TrophyAssets = new Dictionary<int, Asset<Texture2D>>();

            if (TrophyAssets.TryGetValue(Type, out var asset))
                return asset;

            Asset<Texture2D> newAsset = ModContent.Request<Texture2D>(TexturePath + Name);
            TrophyAssets.Add(Type, newAsset);
            return newAsset;
        }
    }
}
