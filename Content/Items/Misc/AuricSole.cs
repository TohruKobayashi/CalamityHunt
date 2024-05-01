using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    public class AuricSole : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }
        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact);
            bool favorited = Item.favorited;
            Item.SetDefaults(ModContent.ItemType<DischargedAuricSole>());
            Item.stack++;
            Item.favorited = favorited;
        }

        public override void UpdateInventory(Player player)
        {
            player.runSlowdown = 0;
            player.moveSpeed *= 1.5f;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            Color glowColor = Color.DodgerBlue;
            glowColor.A = 0;
            spriteBatch.Draw(glow.Value, position, glow.Frame(), glowColor, 0, glow.Size() * 0.48f, scale, 0, 0);
            return true;
        }

        public override void AddRecipes()
        {
            if (ModLoader.HasMod("CalamityMod"))
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");
                CreateRecipe()
                    .AddIngredient(calamity.Find<ModItem>("AuricBar").Type, 10)
                    .AddIngredient<ChromaticMass>(5)
                    .AddTile(calamity.Find<ModTile>("DraedonsForge").Type)
                    .Register();
            }
            else 
            {
                CreateRecipe()
                    .AddIngredient(ItemID.GoldBar, 10)
                    .AddIngredient<ChromaticMass>(5)
                    .AddTile<SlimeNinjaStatueTile>()
                    .Register();
            }
        }
    }
}
