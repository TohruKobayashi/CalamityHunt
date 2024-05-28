using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Dyes
{
    public class GoopDyeShaderData : ArmorShaderData
    {
        public GoopDyeShaderData(Ref<Effect> shader, string passName) : base(shader, passName)
        {
            map = AssetDirectory.Textures.ColorMap[3].Value;
        }

        private Texture2D map;

        public override void Apply(Entity entity, DrawData? drawData = null)
        {
            if (drawData.HasValue) {
                UseColor(new Color(39, 31, 34));
                Shader.Parameters["uMap"].SetValue(map);
            }
            base.Apply(entity, drawData);
        }
    }
    public class GoopDye : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;

            if (!Main.dedServ) {
                Effect goopShader = AssetDirectory.Effects.Dyes.Goop.Value;

                GameShaders.Armor.BindShader(ModContent.ItemType<GoopDye>(), new GoopDyeShaderData(new Ref<Effect>(goopShader), "LiquidPass"));
            }
        }

        public override void SetDefaults()
        {
            int dye = Item.dye;
            Item.CloneDefaults(ItemID.BrownDye);
            Item.dye = dye;
            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                ModRarity r;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ChromaticMass>()
                .AddIngredient(ItemID.BottledWater)
                .AddTile(TileID.DyeVat)
                .Register();
        }
    }
}
