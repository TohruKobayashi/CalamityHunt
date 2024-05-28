using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Dyes
{
    public class GoopHairDyeShaderData : HairShaderData
    {
        public GoopHairDyeShaderData(Ref<Effect> shader, string passName) : base(shader, passName)
        {
            map = AssetDirectory.Textures.Extras.RainbowMap.Value;
        }

        private Texture2D map;

        public override void Apply(Player player, DrawData? drawData = null)
        {
            if (drawData.HasValue)
            {
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

            if (!Main.dedServ)
            {
                Effect goopShader = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/GoopDye", AssetRequestMode.ImmediateLoad).Value;

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
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
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
