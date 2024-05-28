using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using CalamityHunt.Common.Systems.Particles;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Consumable
{
    public class SupremeRestorationPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 30;
        }

        public override void SetDefaults()
        {
			Item.width = 20;
			Item.height = 26;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useAnimation = 17;
			Item.useTime = 17;
			Item.useTurn = true;
			Item.UseSound = AssetDirectory.Sounds.SupremeRestorationBigGulp;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.rare = ModContent.RarityType<VioletRarity>();
			if (ModLoader.HasMod("CalamityMod"))
			{
			   ModRarity r;
			    Mod calamity = ModLoader.GetMod("CalamityMod");
			    calamity.TryFind<ModRarity>("Violet", out r);
			    Item.rare = r.Type;
			}
			Item.value = Item.buyPrice(gold: 1);

			Item.healLife = 225; // While we change the actual healing value in GetHealLife, Item.healLife still needs to be higher than 0 for the item to be considered a healing item
			Item.potion = true; // Makes it so this item applies potion sickness on use and allows it to be used with quick heal
		}

        public override void Load()
        {
            On_Player.ApplyPotionDelay += PotionDelay_SupremeRestoration;
        }

        private void PotionDelay_SupremeRestoration(On_Player.orig_ApplyPotionDelay orig, Player self, Item sItem)
        {
            if (sItem.type == ModContent.ItemType<SupremeRestorationPotion>())
            {
                self.potionDelay = self.restorationDelayTime; 
                self.AddBuff(21, self.potionDelay);
            }
            else
                orig(self, sItem);
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient(ItemID.RestorationPotion, 4)
                .AddIngredient<ChromaticMass>()
                .AddTile(TileID.Bottles)
                .Register()
                .DisableDecraft();
        }
    }
}
