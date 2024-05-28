using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Mounts;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Mounts
{
    public class PaladinPalanquin : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToMount(ModContent.MountType<PaladinPalanquinMount>());
            Item.width = 54;
            Item.height = 50;
            Item.value = Item.sellPrice(0, 30, 0, 0);
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = 1;
            Item.noMelee = true;
            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                ModRarity r;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }
    }
}
