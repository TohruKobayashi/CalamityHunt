using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Ranged;
using CalamityHunt.Content.Tiles;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using System;
using CalamityHunt.Common.Utilities;
using ReLogic.Content;

namespace CalamityHunt.Content.Items.Weapons.Ranged
{
    public class Trailblazer : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SkipsInitialUseSound[Type] = true;
        }

        public override void SetDefaults()
		{
			Item.width = 90;
			Item.height = 38;
			Item.damage = 750;
			Item.noMelee = true;
			Item.useAnimation = 15;
			Item.useTime = 3;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 10f;
			Item.UseSound = AssetDirectory.Sounds.Weapon.TrailBlazerFireStart;
			Item.channel = true;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<TrailblazerFlame>();
			Item.shootSpeed = 9.5f;
			Item.rare = ModContent.RarityType<VioletRarity>();
			Item.useAmmo = AmmoID.Gel;
            Item.value = Item.sellPrice(gold: 20);
            Item.ArmorPenetration = 15;
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
            Item.DamageType = DamageClass.Ranged;
            Item.consumeAmmoOnFirstShotOnly = true;
		}

        public override Vector2? HoldoutOffset() => new Vector2(-16f, 0f);

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (Main.rand.Next(0, 100) < 80)
                return false;
            return true;
        }

        public static Asset<Texture2D> backTexture;
        public static Asset<Texture2D> backSwirlTexture;
        public static Asset<Texture2D> backAntennaTexture;
        public static Asset<Texture2D> strapTexture;
        public static Asset<Texture2D> goggleTexture;

        public override void Load()
        {
            backTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "_Back");
            backSwirlTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "_BackSwirl");
            backAntennaTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "_BackAntenna");
            strapTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "_Strap");
            goggleTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "_Goggles");
        }

        public override void AddRecipes()
        {
            if (ModLoader.HasMod("CalamityMod"))
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");
                CreateRecipe()
                    .AddIngredient<ChromaticMass>(15)
                    .AddIngredient(calamity.Find<ModItem>("GodsBellows").Type)
                    .AddIngredient(calamity.Find<ModItem>("PristineFury").Type)
                    .AddIngredient(calamity.Find<ModItem>("OverloadedBlaster").Type)
                    .AddIngredient(calamity.Find<ModItem>("AuroraBlazer").Type)
                    .AddTile(calamity.Find<ModTile>("DraedonsForge").Type)
                    .Register();
            }
            else 
            {
                CreateRecipe()
                    .AddIngredient(ItemID.ElfMelter)
                    .AddIngredient<ChromaticMass>(15)
                    .AddTile<SlimeNinjaStatueTile>()
                    .Register();
            }
        }
    }

    public class TrailblazerBackpackLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Backpacks);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<Trailblazer>() && VanityUtils.NoBackpackOn(ref drawInfo);

        private int frame;

        private int frameCounter;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D texture = Trailblazer.backTexture.Value;
            Texture2D swirlTexture = Trailblazer.backSwirlTexture.Value;
            Texture2D antennaTexture = Trailblazer.backAntennaTexture.Value;

            Vector2 vec5 = drawInfo.BodyPosition() + new Vector2(-16 * drawInfo.drawPlayer.direction, -1 * drawInfo.drawPlayer.gravDir);
            vec5 = vec5.Floor();
            vec5.ApplyVerticalOffset(drawInfo);

            Vector2 aPos = vec5 + new Vector2(9 * drawInfo.drawPlayer.direction, -18 * drawInfo.drawPlayer.gravDir);
            for (int i = 0; i < 9; i++)
            {
                Color aColor = Lighting.GetColor(drawInfo.drawPlayer.MountedCenter.ToTileCoordinates());
                Rectangle aFrame = antennaTexture.Frame(1, 3, 0, 2);
                if (i == 8)
                {
                    aColor = Color.White;
                    aFrame = antennaTexture.Frame(1, 3, 0, 0);
                }
                else if (i > 4)
                    aFrame = antennaTexture.Frame(1, 3, 0, 1);

                float rot = -drawInfo.drawPlayer.velocity.X * (0.03f * (i + 1) / 12f) - Math.Abs(drawInfo.drawPlayer.velocity.Y) * drawInfo.drawPlayer.velocity.X * (0.01f * i / 12f);
                DrawData antenna = new DrawData(antennaTexture, aPos, aFrame, aColor * (1f - drawInfo.shadow), drawInfo.drawPlayer.bodyRotation + rot, aFrame.Size() * new Vector2(0.5f, 1f) - Vector2.UnitY, 1f, drawInfo.playerEffect);
                drawInfo.DrawDataCache.Add(antenna);
                aPos += new Vector2(0, -aFrame.Height).RotatedBy(rot);
            }

            if (drawInfo.shadow == 0f)
            {
                if (frameCounter++ > 5)
                {
                    frame = (frame + 1) % 5;
                    frameCounter = 0;
                }
            }

            DrawData swirl = new DrawData(swirlTexture, vec5, swirlTexture.Frame(1, 5, 0, frame), Color.White * (1f - drawInfo.shadow), drawInfo.drawPlayer.bodyRotation, swirlTexture.Frame(1, 5, 0, frame).Size() * 0.5f, 1f, drawInfo.playerEffect);
            drawInfo.DrawDataCache.Add(swirl);

            Rectangle itemFrame = texture.Frame(1, 20, 0, (int)(drawInfo.drawPlayer.legFrame.Y / drawInfo.drawPlayer.legFrame.Height));

            DrawData item = new DrawData(texture, vec5, itemFrame, Lighting.GetColor(drawInfo.drawPlayer.MountedCenter.ToTileCoordinates()) * (1f - drawInfo.shadow), drawInfo.drawPlayer.bodyRotation, itemFrame.Size() * 0.5f, 1f, drawInfo.playerEffect);
            drawInfo.DrawDataCache.Add(item);  
        }
    }        
    
    public class TrailblazerStrapLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.FrontAccFront);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<Trailblazer>() && VanityUtils.NoBackpackOn(ref drawInfo);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D texture = Trailblazer.strapTexture.Value;

            Vector2 vec5 = drawInfo.BodyPosition();//drawInfo.Position - Main.screenPosition + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.width / 2, drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height / 2) + new Vector2(0f, -4f);
            vec5 = vec5.Floor();

            DrawData item = new DrawData(texture, vec5, drawInfo.drawPlayer.bodyFrame, drawInfo.colorArmorBody * (1f - drawInfo.shadow), drawInfo.drawPlayer.bodyRotation, new Vector2(texture.Width * 0.5f, drawInfo.bodyVect.Y), 1f, drawInfo.playerEffect);
            drawInfo.DrawDataCache.Add(item);
        }
    }    
    
    public class TrailblazerGogglesLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.FaceAcc);

        public override bool IsHeadLayer => true;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<Trailblazer>();

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D goggleTexture = Trailblazer.goggleTexture.Value;

            Vector2 vec5 = drawInfo.HeadPosition();
            vec5 = vec5.Floor();
            vec5.ApplyVerticalOffset(drawInfo);

            DrawData item = new DrawData(goggleTexture, vec5, goggleTexture.Frame(), drawInfo.colorArmorHead * (1f - drawInfo.shadow), drawInfo.drawPlayer.headRotation, new Vector2((int)drawInfo.headVect.X, (int)drawInfo.headVect.Y), 1f, drawInfo.playerEffect);
            drawInfo.DrawDataCache.Add(item);
        }
    }
}
