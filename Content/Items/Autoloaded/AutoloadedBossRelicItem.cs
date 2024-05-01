using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace CalamityHunt.Content.Items.Autoloaded
{
    [Autoload(false)]
    public class AutoloadedBossRelicItem : ModItem
    {
        public string InternalName = "";
        public int TileType;

        private readonly string TexturePath;

        protected override bool CloneNewInstances => true;

        public override string Name => InternalName != "" ? InternalName : base.Name;
        public override string Texture => TexturePath + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType, 0);

            Item.width = 30;
            Item.height = 40;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Master;
            Item.master = true; // This makes sure that "Master" displays in the tooltip, as the rarity only changes the item name color
            Item.value = Item.buyPrice(0, 5);
            Item.ResearchUnlockCount = 1;
        }

        public AutoloadedBossRelicItem(string NPCName, string texturePath)
        {
            InternalName = NPCName + "RelicItem";
            TexturePath = texturePath;
        }
    }
}
