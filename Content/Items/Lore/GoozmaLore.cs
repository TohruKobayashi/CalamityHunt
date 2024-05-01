using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Items.Rarities;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Lore
{
    public class GoozmaLore : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
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
            TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");
            if (!Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
            {
                if (line != null)
                    line.Text = Language.GetOrRegister($"Mods.{nameof(CalamityHunt)}.LoreGeneric").Value;
                return;
            }

            //stuff is in HuntOfTheoldGodUtils
            string tooltip = Language.GetOrRegister($"Mods.{nameof(CalamityHunt)}.LoreGoozma").Value;

            if (line != null)
                line.Text = tooltip;
        }

        public override bool CanUseItem(Player player) => false;

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void AddRecipes()
        {
                CreateRecipe()
                    .AddIngredient(Goozma.trophyTypes[0])
                    .AddTile(TileID.Bookcases)
                    .Register();
        }
    }
}
