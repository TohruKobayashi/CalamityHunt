using CalamityHunt.Common.Players;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityHunt.Common;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityHunt.Common.Utilities;

namespace CalamityHunt.Content.Items.Misc.AuricSouls
{
    public class AshenSoul : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod) => ModLoader.HasMod(HUtils.CommunityRemix);

        public static Texture2D chainTexture;
        public static Texture2D iconTexture;

        public override void Load()
        {
            chainTexture = AssetDirectory.Textures.AuricSouls.YharonSoulChain.Value;
            iconTexture = AssetDirectory.Textures.AshenSoulEye.Value;
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Type] = true;
            ItemID.Sets.ItemsThatShouldNotBeInInventory[Type] = true;
            ItemID.Sets.IgnoresEncumberingStone[Type] = true;

            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            ItemID.Sets.ItemIconPulse[Type] = true;
            ItemID.Sets.IsLavaImmuneRegardlessOfRarity[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Purple;
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind("Turquoise", out r);
                Item.rare = r.Type;
            }
        }

        public override bool OnPickup(Player player)
        {
            for (int i = 0; i < 150; i++)
            {
                Color glowColor = Color.Lerp(GetAlpha(Color.White).Value, Color.OrangeRed, Main.rand.NextFloat(0.5f) + 0.3f);
                glowColor.A = 0;

                Dust soul = Dust.NewDustPerfect(Item.Center, DustID.PortalBoltTrail, Main.rand.NextVector2Circular(10, 10), 0, glowColor, Main.rand.NextFloat(2f));
                soul.noGravity = true;
            }
            player.GetModPlayer<AuricSoulPlayer>().pyrogenSoul = true;

            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color[] array = new Color[]
            {
                new Color(255, 180, 51),
                new Color(230, 159, 39),
                new Color(184, 93, 24),
                new Color(150, 76, 20),
                new Color(181, 96, 31),
                new Color(217, 121, 48)
            };
            Color final = new GradientColor(array, 0.15f, 0.5f).ValueAt(Main.GlobalTimeWrappedHourly * 25f);
            return final;
        }

        public LoopingSound heartbeatSound;
        public LoopingSound droneSound;

        public int breathSoundCounter;

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (heartbeatSound == null)
                heartbeatSound = new LoopingSound(AssetDirectory.Sounds.Souls.YharonSoulHeartbeat, new HUtils.ItemAudioTracker(Item).IsActiveAndInGame);
            heartbeatSound.PlaySound(() => Item.position, () => 1f, () => 0f);

            if (droneSound == null)
                droneSound = new LoopingSound(AssetDirectory.Sounds.Souls.YharonSoulDrone, new HUtils.ItemAudioTracker(Item).IsActiveAndInGame);
            droneSound.PlaySound(() => Item.position, () => 1.5f, () => 0f);

            if (breathSoundCounter-- <= 0)
            {
                SoundEngine.PlaySound(AssetDirectory.Sounds.Souls.YharonSoulBreathe, Item.Center);
                breathSoundCounter = Main.rand.Next(500, 800);
            }

            if (Main.rand.NextBool(5))
            {
                Dust soul = Dust.NewDustDirect(Item.Center - new Vector2(15), 30, 30, DustID.PortalBoltTrail, 0f, -Main.rand.NextFloat(1f, 2f), 0, GetAlpha(Color.White).Value, Main.rand.NextFloat(2f));
                soul.noGravity = true;
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            bool includeChains = true;
            bool includeLensFlare = true;

            Texture2D texture = TextureAssets.Item[Type].Value;
            Texture2D sparkTexture = AssetDirectory.Textures.Sparkle.Value;
            Texture2D glowTexture = AssetDirectory.Textures.Glow[1].Value;

            Color glowColor = GetAlpha(Color.White).Value;
            glowColor.A = 0;
            Color darkColor = Color.Lerp(glowColor, Color.IndianRed, 0.7f);
            darkColor.A = 0;

            float soulScale = scale;
            scale = 1f + MathF.Sin(Main.GlobalTimeWrappedHourly * 3f % MathHelper.TwoPi) * 0.01f;

            if (includeChains)
            {
                VertexStrip strip1 = new VertexStrip();

                int count = 180;
                Vector2[] offs1 = new Vector2[count];
                float[] offRots1 = new float[count];

                float time = Main.GlobalTimeWrappedHourly * 0.5f;
                for (int i = 0; i < count; i++)
                {
                    Vector2 x = new Vector2(140 + MathF.Sin(time - i / (float)count * MathHelper.TwoPi * 2) * 10f, 0).RotatedBy(MathHelper.TwoPi / (count - 1f) * i - time);
                    offs1[i] = x.RotatedBy(time * 1f) * 0.8f;
                }

                for (int i = 1; i < count; i++)
                {
                    offRots1[i] = offs1[i - 1].AngleTo(offs1[i]);
                }
                offRots1[0] = offRots1[1];

                Color StripColor(float p)
                {
                    Color[] array = new Color[]
                    {
                        new Color(255, 215, 13),
                        new Color(230, 195, 23),
                        new Color(207, 176, 25),
                        new Color(171, 146, 26),
                        new Color(196, 167, 26),
                        new Color(237, 200, 17)
                    };
                    Color final = new GradientColor(array, 0.15f, 0.5f).ValueAt((p + Main.GlobalTimeWrappedHourly) * 25f);
                    return final * 0.6f;
                }
                float StripWidth(float p) => 12;

                strip1.PrepareStrip(offs1, offRots1, StripColor, StripWidth, Item.Center - Main.screenPosition, offs1.Length, true);

                Effect effect = AssetDirectory.Effects.CrystalLightning.Value;
                effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
                effect.Parameters["uTexture"].SetValue(chainTexture);
                effect.Parameters["uGlow"].SetValue(TextureAssets.Extra[197].Value);
                effect.Parameters["uColor"].SetValue(Vector3.One);
                effect.Parameters["uTime"].SetValue(-time * 1.1f % 1f);
                effect.CurrentTechnique.Passes[0].Apply();

                strip1.DrawTrail();

                Main.pixelShader.CurrentTechnique.Passes[0].Apply();

                spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), darkColor * 0.1f, 0, glowTexture.Size() * 0.5f, scale, 0, 0);
            }

            if (includeLensFlare)
            {
                for (int i = 0; i < 2; i++) {
                    float lensScale = scale + MathF.Sin(Main.GlobalTimeWrappedHourly * 24) * 0.2f;
                    float lensScaleSlow = scale + MathF.Sin(Main.GlobalTimeWrappedHourly * 24 + 3) * 0.15f;
                    float time = Main.GlobalTimeWrappedHourly * 0.1f;
                    float exRot = i == 0 ? MathHelper.TwoPi / 16 : 0;
                    float lensRotation = time % MathHelper.TwoPi + exRot;
                    float lensRotationSlow = (time + MathF.Sin(time * 2) * 0.3f) % MathHelper.TwoPi + exRot;

                    spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), darkColor, lensRotation - MathHelper.PiOver4, sparkTexture.Size() * 0.5f, new Vector2(0.5f, 1f + lensScale * 5f), 0, 0);
                    spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), darkColor, lensRotation + MathHelper.PiOver4, sparkTexture.Size() * 0.5f, new Vector2(0.5f, 1f + lensScale * 5f), 0, 0);
                    spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), glowColor, lensRotation - MathHelper.PiOver4, sparkTexture.Size() * 0.5f, new Vector2(0.5f, 1f + lensScale), 0, 0);
                    spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), glowColor, lensRotation + MathHelper.PiOver4, sparkTexture.Size() * 0.5f, new Vector2(0.5f, 1f + lensScale), 0, 0);

                    spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), darkColor, lensRotationSlow, sparkTexture.Size() * 0.5f, new Vector2(0.08f, 2f + lensScaleSlow * 5f), 0, 0);
                    spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), darkColor, lensRotationSlow + MathHelper.PiOver2, sparkTexture.Size() * 0.5f, new Vector2(0.08f, 2f + lensScaleSlow * 5f), 0, 0);
                    spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), glowColor, lensRotationSlow, sparkTexture.Size() * 0.5f, new Vector2(0.2f, 2f + lensScaleSlow), 0, 0);
                    spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), glowColor, lensRotationSlow + MathHelper.PiOver2, sparkTexture.Size() * 0.5f, new Vector2(0.2f, 2f + lensScaleSlow), 0, 0);

                    spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), glowColor * 0.05f, 0, glowTexture.Size() * 0.5f, lensScale * 1.5f, 0, 0);
                }
            }

            spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), darkColor * 0.1f, 0, glowTexture.Size() * 0.5f, scale * 1.4f, 0, 0);

            spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), darkColor * 0.7f, 0, glowTexture.Size() * 0.5f, scale * 0.8f, 0, 0);

            spriteBatch.Draw(iconTexture, Item.Center - Main.screenPosition, iconTexture.Frame(), GetAlpha(Color.White).Value * 0.3f, 0, iconTexture.Size() * 0.5f, scale * 3.2f, 0, 0);
            spriteBatch.Draw(iconTexture, Item.Center - Main.screenPosition, iconTexture.Frame(), new Color(255, 255, 255, 0),0, iconTexture.Size() * 0.5f, scale * 3.2f, 0, 0);
            spriteBatch.Draw(iconTexture, Item.Center - Main.screenPosition, iconTexture.Frame(), glowColor,0, iconTexture.Size() * 0.5f, scale * 3.4f, 0, 0);

            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            Texture2D glowTexture = AssetDirectory.Textures.Glow[1].Value;

            Color glowColor = GetAlpha(Color.White).Value;
            glowColor.A = 0;

            spriteBatch.Draw(texture, position, frame, GetAlpha(Color.White).Value, 0, frame.Size() * 0.5f, scale + 0.2f, 0, 0);
            spriteBatch.Draw(texture, position, frame, new Color(255, 255, 255, 0), 0, frame.Size() * 0.5f, scale + 0.2f, 0, 0);

            spriteBatch.Draw(glowTexture, position, glowTexture.Frame(), glowColor * 0.7f, 0, glowTexture.Size() * 0.5f, scale * 0.2f, 0, 0);

            return false;
        }
    }
}
