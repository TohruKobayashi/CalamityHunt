using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Placeable
{
    public class ChromaticTorch : ModItem
    {
        public Color rainbowGlow => new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 100f);


        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;

            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerTorch;
            ItemID.Sets.Torches[Item.type] = true;
            ItemID.Sets.WaterTorches[Item.type] = true;
        }

        public override void SetDefaults()
        {
            // DefaultToTorch sets various properties common to torch placing items. Hover over DefaultToTorch in Visual Studio to see the specific properties set.
            // Of particular note to torches are Item.holdStyle, Item.flame, and Item.noWet. 
            Item.DefaultToTorch(ModContent.TileType<Tiles.ChromaticTorchPlaced>(), 0, false);
            Item.noWet = false;
            Item.value = 50;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        { // Overrides the default sorting method of this Item.
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Torches; // Vanilla usually matches sorting methods with the right type of item, but sometimes, like with torches, it doesn't. Make sure to set whichever items manually if need be.
        }

        public override void HoldItem(Player player)
        {
            if (Main.rand.NextBool(player.itemAnimation > 0 ? 40 : 80)) {
                CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                    particle.position = new Vector2(player.itemLocation.X + 16f * player.direction, player.itemLocation.Y - 14f * player.gravDir);
                    particle.velocity = Vector2.UnitY;
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

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe(100)
                .AddIngredient(ItemID.Torch, 100)
                .AddIngredient<ChromaticMass>()
                //.AddTile<Tiles.Furniture.ExampleWorkbench>()
                .Register();
        }
    }
}
