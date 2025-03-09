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
    public class CrimulanGougeDyeShaderData : ArmorShaderData
    {
        public CrimulanGougeDyeShaderData(Ref<Effect> shader, string passName) : base(shader, passName)
        {
            map = AssetDirectory.Textures.Noise[6].Value;
            mapSize = map.Size();
        }

        private Texture2D map;
        private Vector2 mapSize;

        public override void Apply(Entity entity, DrawData? drawData = null)
        {
            if (drawData.HasValue) {
                UseColor(new Color(147, 38, 31));
                Shader.Parameters["noisemap"].SetValue(map);
                Shader.Parameters["noisemapSize"].SetValue(mapSize);
            }
            base.Apply(entity, drawData);
        }
    }
    public class CrimulanGougeDye : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;

            if (!Main.dedServ) {
                Effect crimShader = AssetDirectory.Effects.Dyes.CrimulanGouge.Value;
                GameShaders.Armor.BindShader(ModContent.ItemType<CrimulanGougeDye>(), new CrimulanGougeDyeShaderData(new Ref<Effect>(crimShader), "DyePass"));
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
