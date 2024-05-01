using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Buffs;
using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Summoner;
using CalamityHunt.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityHunt.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Summoner
{
    public class Gobflogger : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Gobbed.TagDamage);

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<GobfloggerProj>(), 300, 12f, 4f, 100);
            Item.width = 56;
            Item.height = 48;
            Item.shootSpeed = 5;
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.channel = true;
            Item.value = Item.sellPrice(gold: 20);
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (ModLoader.HasMod("CalamityMod"))
            {
                Item.autoReuse = true;
            }
            return base.CanUseItem(player);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "Glow").Value;
            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.3f, 0.3f).Value;
            glowColor.A /= 2;
            spriteBatch.Draw(glow, position, frame, glowColor, 0, origin, scale, 0, 0);
            spriteBatch.Draw(glow, position, frame, glowColor, 0, origin, scale, 0, 0);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "Glow").Value;
            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.3f, 0.3f).Value;
            glowColor.A /= 2;
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, glow.Frame(), glowColor, rotation, Item.Size * 0.5f, scale, 0, 0);
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, glow.Frame(), glowColor, rotation, Item.Size * 0.5f, scale, 0, 0);
        }

        public override bool MeleePrefix() => true;

        public override void AddRecipes()
        {
            if (ModLoader.HasMod("CatalystMod"))
            {
                Mod calamity = ModLoader.GetMod("CatalystMod");
                Mod calamityfr = ModLoader.GetMod("CalamityMod");
                CreateRecipe()
                    .AddIngredient<ChromaticMass>(15)
                    .AddIngredient(calamity.Find<ModItem>("UnrelentingTorment").Type)
                    .AddIngredient(calamity.Find<ModItem>("UnderBite").Type)
                    .AddIngredient(calamity.Find<ModItem>("CongeledDuoWhip").Type)
                    .AddIngredient(calamity.Find<ModItem>("BlossomsBlessing").Type)
                    .AddTile(calamityfr.Find<ModTile>("DraedonsForge").Type)
                    .Register();
            }
            else if (ModLoader.HasMod("CalamityMod"))
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");
                CreateRecipe()
                    .AddIngredient<ChromaticMass>(15)
                    .AddIngredient(calamity.Find<ModItem>("CosmicDischarge").Type)
                    .AddIngredient(calamity.Find<ModItem>("Mourningstar").Type)
                    .AddIngredient(calamity.Find<ModItem>("CrescentMoon").Type)
                    .AddIngredient(calamity.Find<ModItem>("Nebulash").Type)
                    .AddTile(calamity.Find<ModTile>("DraedonsForge").Type)
                    .Register();
            }
            else 
            {
                CreateRecipe()
                    .AddIngredient(ItemID.RopeCoil, 5)
                    .AddIngredient<ChromaticMass>(15)
                    .AddTile<SlimeNinjaStatueTile>()
                    .Register();
            }
	}
    }
}
