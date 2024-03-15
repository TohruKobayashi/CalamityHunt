using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using CalamityHunt.Content.Particles;
using CalamityHunt.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    public class ChromaticGooztick : ModItem
    {
        public Color rainbowGlow => new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 100f);

        public override bool IsLoadingEnabled(Mod mod) => false;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
            ItemID.Sets.Glowsticks[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.holdStyle = ItemHoldStyleID.HoldFront;
            Item.useTime = Item.useAnimation = 40;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<ChromaticGooztickProjectile>();
            Item.autoReuse = true;
            Item.width = 16;
            Item.height = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                ModRarity r;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        { // Overrides the default sorting method of this Item.
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Glowsticks; // Vanilla usually matches sorting methods with the right type of item, but sometimes, like with torches, it doesn't. Make sure to set whichever items manually if need be.
        }

        public override void HoldItem(Player player)
        {
            if (Main.rand.NextBool(player.itemAnimation > 0 ? 40 : 80)) {
                CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                    particle.position = new Vector2(player.itemLocation.X + 16f * player.direction, player.itemLocation.Y - 14f * player.gravDir);
                    particle.velocity = -Vector2.UnitY;
                    particle.scale = 1f;
                    particle.color = Color.White;
                    particle.colorData = new ColorOffsetData(true, Main.GlobalTimeWrappedHourly);
                }));
            }

            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);

            Lighting.AddLight(position, rainbowGlow.ToVector3());
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, rainbowGlow.ToVector3());
        }

        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddIngredient(ItemID.Glowstick, 100).
                AddIngredient<ChromaticMass>().
                Register();
        }
    }
}
