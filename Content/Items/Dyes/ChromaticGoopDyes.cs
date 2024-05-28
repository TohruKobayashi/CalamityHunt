using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
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
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityHunt.Content.Bosses.Goozma.GoozmaColorUtils;
using CalamityHunt.Content.Bosses.Goozma;


namespace CalamityHunt.Content.Items.Dyes
{
    public class ChromaticDyeLoader : ModSystem
    {
        public static Mod Hunt;

        //Use this list to get obtention methods or whatever
        public static List<int> LoadedDyes;

        public override void Load()
        {
            LoadedDyes = new List<int>();
            loadedDyeCount = 0;
            Hunt = ModLoader.GetMod("CalamityHunt");

            while (loadedDyeCount < Palettes.Count)
                LoadNextChromaDye();
        }

        public static int loadedDyeCount;

        public static int LoadNextChromaDye()
        {
            AutoloadedChromaticDye autoloadedDye = new AutoloadedChromaticDye(loadedDyeCount++);
            Hunt.AddContent(autoloadedDye);

            //Cache the item's type
            LoadedDyes.Add(autoloadedDye.Type);
            return autoloadedDye.Type;
        }

        public static List<Vector3[]> Palettes = new List<Vector3[]>()
        {
            Oil,
            Nuclear,
            Gold,
            Grayscale,
            Honey,
            Masterful,
            DarkEnergy,
            Frigid,
            Tritanopic,
            Caliginous,
            Spectrum,
            Evil,
            Shadowflame,
            Dogmatic,
            FIREBLU,
            Distorted,
            Electric,
            Polterplasmic,
            Exhumed,
            Raptured,
            Ibanical,
            AcidRainbow,
            Rubicon,
            DoubleRainbow,
            Unpleasant,
            OceanJasper,
            Seashell,
            JackOLantern,
            CottonCandy,
            MeanGreen,
            ColorCalibration,
            Enchanted,
            Sisyphan,
            Babil,
            Doomsday,
            Rugamarian,
            Poozma,
            Festive,
            MoonCarpet,
            Hein,
            Zeromus,
            Water,
            Stellar,
            Crimson,
            Corruption,
            Hallow,
            TheDragon,
            Speevil,
            HyperrealisticBloody,
            AuricTesla,
            Pikmin,
            Flame,
            Poland,
            Kindergarten,
            Daniel,
            Autumnal,
            Subworld
        };
    }

    [Autoload(false)]
    public class AutoloadedChromaticDye : ModItem
    {
        public override string Texture => "CalamityHunt/Content/Items/Dyes/ChromaticGoopDye";
        public static Asset<Texture2D> Overlay;
        public static Asset<Texture2D> Highlight;

        #region Autoloading
        public override string Name => InternalName != "" ? InternalName : base.Name;
        //Necessary so new instances get cloned from our autoloaded one
        protected override bool CloneNewInstances => true;

        public string InternalName;
        public int Index;

        public AutoloadedChromaticDye(int index)
        {
            InternalName = "ChromaticDye" + index.ToString();
            Index = index;
        }
        #endregion

        public override LocalizedText DisplayName => Language.GetText("Mods.CalamityHunt.Items.ChromaticDye.Name").WithFormatArgs(Language.GetText("Mods.CalamityHunt.Items.ChromaticDye.Variant" + Index.ToString()));
        public override LocalizedText Tooltip => Language.GetText("Mods.CalamityHunt.Items.ChromaticDye.Tooltip");

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            if (!Main.dedServ)
            {
                Effect goopShader = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/HolographDyeEffect", AssetRequestMode.ImmediateLoad).Value;
                GameShaders.Armor.BindShader(Type, new ChromaticDyeShaderData(new Ref<Effect>(goopShader), "LiquidPass", Index));
            }
        }

        public override void SetDefaults()
        {
            int dye = Item.dye;
            Item.CloneDefaults(ItemID.BrownDye);
            Item.dye = dye;
            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GoopDye>()
                .AddIngredient<ChromaticMass>()
                .AddTile(TileID.DyeVat)
                .Register();
        }

        #region Drawing in world
        public Effect GetShader()
        {
            Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/HolographDyeEffect").Value;
            Goozma.GetGradientMapValues(ChromaticDyeLoader.Palettes[Index], out float[] brightnesses, out Vector3[] colors);

            effect.Parameters["uColor"].SetValue(new Color(39, 31, 34).ToVector3());
            effect.Parameters["uOpacity"].SetValue(1f);
            effect.Parameters["colors"].SetValue(colors);
            effect.Parameters["baseToScreenPercent"].SetValue(1.05f);
            effect.Parameters["baseToMapPercent"].SetValue(-0.05f);
            effect.Parameters["brightnesses"].SetValue(brightnesses);

            return effect;
        }


        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Overlay ??= ModContent.Request<Texture2D>(Texture + "Overlay");
            Highlight ??= ModContent.Request<Texture2D>(Texture + "Highlight");

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, GetShader(), Main.UIScaleMatrix);

            spriteBatch.Draw(Overlay.Value, position, frame, drawColor, 0f, origin, scale, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);

            spriteBatch.Draw(Highlight.Value, position, frame, drawColor, 0f, origin, scale, 0, 0);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Overlay ??= ModContent.Request<Texture2D>(Texture + "Overlay");
            Highlight ??= ModContent.Request<Texture2D>(Texture + "Highlight");

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, GetShader(), Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(Overlay.Value, Item.Center - Main.screenPosition, null, lightColor, rotation, Overlay.Size() / 2f, scale, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(Highlight.Value, Item.Center - Main.screenPosition, null, lightColor, rotation, Overlay.Size() / 2f, scale, 0, 0);
        }
        #endregion
    }


    public class ChromaticDyeShaderData : ArmorShaderData
    {
        public int index;

        public ChromaticDyeShaderData(Ref<Effect> shader, string passName, int index) : base(shader, passName) 
        {
            this.index = index;
        }

        public override void Apply(Entity entity, DrawData? drawData)
        {
            UseColor(new Color(239, 231, 234));
            UseOpacity(0.6f); //Opacity will darken the original color

            Goozma.GetGradientMapValues(ChromaticDyeLoader.Palettes[index], out float[] brightnesses, out Vector3[] colors);

            Shader.Parameters["colors"].SetValue(colors);
            Shader.Parameters["baseToScreenPercent"].SetValue(1f);
            Shader.Parameters["baseToMapPercent"].SetValue(0.6f);
            Shader.Parameters["brightnesses"].SetValue(brightnesses);
            base.Apply(entity, drawData);
        }
    }
}
