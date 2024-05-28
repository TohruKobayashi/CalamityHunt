using CalamityHunt.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Placeable;

public class StrangeManPortrait : ModItem
{
    public override bool IsLoadingEnabled(Mod mod) => Main.rand.NextBool(1000); //Good luck.

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.StrangeManPortraitTile>());

        Item.width = 30;
        Item.height = 20;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(0, 50);
    }

    public override void AddRecipes()
    {
        if (!ModLoader.TryGetMod(HUtils.CalamityMod, out Mod Calamity)) {
            CreateRecipe()
                .AddCondition(Condition.InEvilBiome)
                .AddCondition(Condition.InSpace)
                .AddCondition(Condition.NearShimmer)
                .AddIngredient(ItemID.DrillContainmentUnit, 2)
                .AddIngredient(ItemID.LesionBlock, 100)
                .AddIngredient(ItemID.LogicGate_NAND)
                .AddIngredient<ChromaticMass>(20)
                .AddTile(TileID.GlowingSnailCage)
                .Register();

            CreateRecipe()
                .AddCondition(Condition.InEvilBiome)
                .AddCondition(Condition.InSpace)
                .AddCondition(Condition.NearShimmer)
                .AddIngredient(ItemID.DrillContainmentUnit, 2)
                .AddIngredient(ItemID.FleshBlock, 100)
                .AddIngredient(ItemID.LogicGate_NXOR)
                .AddIngredient<ChromaticMass>(20)
                .AddTile(TileID.GlowingSnailCage)
                .Register();
        }
    }
}
