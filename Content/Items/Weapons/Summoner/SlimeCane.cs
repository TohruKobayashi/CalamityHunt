using CalamityHunt.Content.Buffs;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Summoner;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Summoner
{
    public class SlimeCane : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 84;
            Item.useTime = 30;
            Item.mana = 20;
            Item.useAnimation = 30;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.reuseDelay = 2;
            Item.damage = 375;
            Item.noMelee = true;
            Item.knockBack = 10f;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.shoot = ModContent.ProjectileType<SlimeCaneGemCounter>();
            Item.shootSpeed = 10f;
            Item.buffType = ModContent.BuffType<SlimeCaneBuff>();
            Item.autoReuse = true;
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.value = Item.sellPrice(gold: 20);
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                ModRarity r;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
            Item.DamageType = DamageClass.Summon;
        }

        public override bool? UseItem(Player player)
        {
            player.AddBuff(Item.buffType, 5);
            return true;
        }
    }
}
