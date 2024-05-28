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
    public class GoopHairDyeShaderData : HairShaderData
    {
        public GoopHairDyeShaderData(Ref<Effect> shader, string passName) : base(shader, passName)
        {
            map = AssetDirectory.Textures.ColorMap[3].Value;
        }

        private Texture2D map;

        public override void Apply(Player player, DrawData? drawData = null)
        {
            if (drawData.HasValue) {
                UseColor(new Color(39, 31, 34));
                Shader.Parameters["uMap"].SetValue(map);
            }
            base.Apply(player, drawData);
        }
    }

    public class GoopHairDye : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;

            if (!Main.dedServ) {
                Effect goopShader = AssetDirectory.Effects.Dyes.Goop.Value;

                GameShaders.Hair.BindShader(ModContent.ItemType<GoopHairDye>(), new GoopHairDyeShaderData(new Ref<Effect>(goopShader), "LiquidPass"));
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                ModRarity r;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
        }

        public override bool ConsumeItem(Player player)
        {
            return true;
        }
    }
}
