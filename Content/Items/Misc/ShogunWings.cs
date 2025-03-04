using CalamityHunt;
using CalamityHunt.Content.Items.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    [AutoloadEquip(EquipType.Wings)]
    public class ShogunWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.wingSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Wings);
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(10, -1, 0.2f);
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 32;
            Item.accessory = true;
            Item.vanity = true;
            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                ModRarity r;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind("Violet", out r);
                Item.rare = r.Type;
            }
        }
    }
}
