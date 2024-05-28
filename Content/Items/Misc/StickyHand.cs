using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using CalamityHunt.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    public class StickyHand : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.GrapplingHook);
            Item.width = 42;
            Item.height = 40;
            Item.shoot = ModContent.ProjectileType<StickyHandProj>();
            Item.shootSpeed = 25f;
            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tooltip = new(Mod, "CalamityHunt:HookStats", Language.GetOrRegister($"Mods.{nameof(CalamityHunt)}.StickyHandStats").Value);
            int check = tooltips.IndexOf(tooltips.Find(t => t.Text.Equals("\'It won't break, I promise\'")));
            if (ModLoader.HasMod("CalamityMod"))
                tooltips.Insert(check, tooltip);
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.6f, 0.6f).Value;
            glowColor.A = 150;
            spriteBatch.Draw(TextureAssets.Item[Type].Value, Item.Center - Main.screenPosition, TextureAssets.Item[Type].Frame(), glowColor, rotation, TextureAssets.Item[Type].Size() * 0.5f, scale, 0, 0);
            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.6f, 0.6f).Value;
            glowColor.A = 150;
            spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, glowColor, 0, origin, scale, 0, 0);
            return false;
        }
    }
}
