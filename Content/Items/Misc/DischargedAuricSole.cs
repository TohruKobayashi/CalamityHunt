using CalamityHunt.Content.Items.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    public class DischargedAuricSole : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                ModRarity r;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }
        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact);
            bool favorited = Item.favorited;
            Item.SetDefaults(ModContent.ItemType<AuricSole>());
            Item.stack++;
            Item.favorited = favorited;
        }
    }
}
