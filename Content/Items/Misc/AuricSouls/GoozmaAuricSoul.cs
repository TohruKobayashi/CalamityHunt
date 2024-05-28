using CalamityHunt.Common.Players;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
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
using CalamityHunt.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace CalamityHunt.Content.Items.Misc.AuricSouls
{
    public class GoozmaAuricSoul : ModItem
    {
        public static Texture2D trailTexture;

        public override void Load()
        {
            trailTexture = ModContent.Request<Texture2D>(Texture + "Trail", AssetRequestMode.ImmediateLoad).Value;
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
            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind("Violet", out r);
                Item.rare = r.Type;
            }
        }

        public override bool OnPickup(Player player)
        {
            for (int i = 0; i < 150; i++)
            {
                if (Main.rand.NextBool(3))
                {
                    Vector2 off = Main.rand.NextVector2Circular(20, 20);
                    ParticleBehavior.NewParticle(ModContent.GetInstance<CrossSparkleParticleBehavior>(), Item.Center + off, Main.rand.NextVector2Circular(5, 5), GetAlpha(Color.White).Value * 0.2f, 1f + Main.rand.NextFloat());
                }

                if (Main.rand.NextBool(8))
                {
                    Vector2 off = Main.rand.NextVector2Circular(20, 20);
                    float scale = Main.rand.NextFloat() + Utils.GetLerpValue(50, 0, off.Length(), true);
                    ParticleBehavior.NewParticle(ModContent.GetInstance<PrettySparkleParticleBehavior>(), Item.Center + off, Main.rand.NextVector2Circular(7, 7), GetAlpha(Color.White).Value * 0.2f, scale * 0.6f);
                }

                Dust soul = Dust.NewDustPerfect(Item.Center, DustID.PortalBoltTrail, Main.rand.NextVector2Circular(10, 10), 0, GetAlpha(Color.White).Value, Main.rand.NextFloat(2f));
                soul.noGravity = true;
            }
            player.GetModPlayer<AuricSoulPlayer>().goozmaSoul = true;

            //SoundStyle jingle = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/AuricSoulJingle");
            //SoundEngine.PlaySound(jingle, player.Center); //fix? if implement at all?

            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.5f, 0.5f).ValueAt(Main.GlobalTimeWrappedHourly * 30 - 10);
            Color secColor = Color.SaddleBrown;
            Color final = Color.Lerp(glowColor, secColor, 0.3f + MathF.Sin(Main.GlobalTimeWrappedHourly * 0.1f) * 0.1f);
            return final;
        }

        public LoopingSound heartbeatSound;
        public LoopingSound droneSound;

        public int breathSoundCounter;

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (heartbeatSound == null)
                heartbeatSound = new LoopingSound(AssetDirectory.Sounds.GoozmaAuricSoulHeartbeat, new HuntOfTheOldGodUtils.ItemAudioTracker(Item).IsActiveAndInGame);
            heartbeatSound.Update(() => Item.position, () => 1f, () => 0f);

            if (droneSound == null)
                droneSound = new LoopingSound(AssetDirectory.Sounds.GoozmaAuricSoulDrone, new HuntOfTheOldGodUtils.ItemAudioTracker(Item).IsActiveAndInGame);
            droneSound.Update(() => Item.position, () => 1.5f, () => 0f);

            if (breathSoundCounter-- <= 0)
            {
                SoundEngine.PlaySound(AssetDirectory.Sounds.GoozmaAuricSoulBreathe, Item.Center);
                breathSoundCounter = Main.rand.Next(300, 500);
            }

            if (Main.rand.NextBool(10))
            {
                Vector2 off = Main.rand.NextVector2Circular(30, 30);
                float scale = Main.rand.NextFloat() + Utils.GetLerpValue(50, 0, off.Length(), true);
                ParticleBehavior.NewParticle(ModContent.GetInstance<CrossSparkleParticleBehavior>(), Item.Center + off, Main.rand.NextVector2Circular(1, 1), GetAlpha(Color.White).Value * 0.2f, scale);
            }

            if (Main.rand.NextBool(35))
            {
                Vector2 off = Main.rand.NextVector2Circular(20, 20);
                float scale = Main.rand.NextFloat() + Utils.GetLerpValue(50, 0, off.Length(), true);
                ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), Item.Center + off, Main.rand.NextVector2Circular(14, 14), GetAlpha(Color.White).Value * 0.2f, scale * 0.5f);
            }

            if (Main.rand.NextBool(5))
            {
                Dust soul = Dust.NewDustDirect(Item.Center - new Vector2(15), 30, 30, DustID.PortalBoltTrail, 0f, -Main.rand.NextFloat(1f, 2f), 0, GetAlpha(Color.White).Value, Main.rand.NextFloat(2f));
                soul.noGravity = true;
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D glowTexture = AssetDirectory.Textures.GlowBig.Value;
            Texture2D eyeTexture = AssetDirectory.Textures.Goozma.GodEye.Value;

            Color glowColor = GetAlpha(Color.White).Value;
            glowColor.A = 0;

            float soulScale = scale;
            float fastScale = 1f + MathF.Sin(Main.GlobalTimeWrappedHourly * 15f % MathHelper.TwoPi) * 0.1f;
            scale = 1f + MathF.Sin(Main.GlobalTimeWrappedHourly % MathHelper.TwoPi) * 0.1f;
           
            spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), Color.Black * 0.2f, 0, glowTexture.Size() * 0.5f, soulScale * 3f, 0, 0);

            VertexStrip strip = new VertexStrip();
            VertexStrip strip2 = new VertexStrip();

            int count = 500;
            Vector2[] offs = new Vector2[count];
            Vector2[] offs2 = new Vector2[count];
            float[] offRots = new float[count];
            float[] offRots2 = new float[count];

            float time = Main.GlobalTimeWrappedHourly * 0.5f;
            for (int i = 0; i < count; i++)
            {
                Vector2 x = new Vector2(60 * scale + MathF.Sin(time * 5f + i / (float)count * 76f) * 7f, 0).RotatedBy(MathHelper.TwoPi / (float)count * i - time);
                x.X *= 1f + MathF.Cos(time * 2f) * 0.1f;
                offs[i] = x.RotatedBy(time);                
                
                Vector2 y = new Vector2(30 + MathF.Sin(time * 8f + i / (float)count * 76f) * 3f, 0).RotatedBy(MathHelper.TwoPi / (float)count * i - time + 0.9f);
                y.X *= 1f + MathF.Cos(time * 3f) * 0.2f;
                offs2[i] = y.RotatedBy(time);
            }

            offRots[0] = offs[0].AngleTo(offs[1]);
            offRots2[0] = offs2[0].AngleTo(offs2[1]);

            for (int i = 1; i < count; i++)
            {
                offRots[i] = offs[i - 1].AngleTo(offs[i]);
                offRots2[i] = offs2[i - 1].AngleTo(offs[i]);
            }

            Color StripColor(float p)
            {
                Color final = new GradientColor(SlimeUtils.GoozColors, 0.5f, 0.5f).ValueAt(p * 20 - 10 + Main.GlobalTimeWrappedHourly * 30);
                return final * 0.8f;
            }
            float StripWidth(float p) => 15f * MathHelper.Clamp(MathF.Sin(p * 20) + 0.4f, 0f, 1f);

            strip.PrepareStrip(offs, offRots, StripColor, StripWidth, Item.Center - Main.screenPosition, offs.Length, true);
            strip2.PrepareStrip(offs2, offRots2, StripColor, StripWidth, Item.Center - Main.screenPosition, offs2.Length, true);
            
            Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/CrystalLightningEffect", AssetRequestMode.ImmediateLoad).Value;
            effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            effect.Parameters["uTexture"].SetValue(trailTexture);
            effect.Parameters["uGlow"].SetValue(TextureAssets.Extra[197].Value);
            effect.Parameters["uColor"].SetValue(Vector3.One);
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f % 1f);
            effect.CurrentTechnique.Passes[0].Apply();

            strip.DrawTrail();
            strip2.DrawTrail();

            for (int i = 0; i < 4; i++)
            {
                Color sparkleColor = new GradientColor(SlimeUtils.GoozColors, 0.5f, 0.5f).ValueAt(Main.GlobalTimeWrappedHourly * 30 + i) * 0.7f;
                sparkleColor.A = 0;
                float length = 4f;
                if (i % 2 == 1)
                    length *= 0.5f;
                DrawSideSparkle(spriteBatch, Item.Center + new Vector2(10 + length * 15, 0).RotatedBy(MathHelper.PiOver2 * i) - Main.screenPosition, MathHelper.PiOver2 * i, sparkleColor, length);
            }

            spriteBatch.Draw(eyeTexture, Item.Center - Main.screenPosition, eyeTexture.Frame(), GetAlpha(Color.White).Value * 0.3f, -MathHelper.PiOver4, eyeTexture.Size() * 0.5f, scale * 0.8f, 0, 0);
            spriteBatch.Draw(eyeTexture, Item.Center - Main.screenPosition, eyeTexture.Frame(), new Color(255, 255, 255, 0), -MathHelper.PiOver4, eyeTexture.Size() * 0.5f, scale * 0.8f, 0, 0);
            spriteBatch.Draw(eyeTexture, Item.Center - Main.screenPosition, eyeTexture.Frame(), glowColor, -MathHelper.PiOver4, eyeTexture.Size() * 0.5f, scale * 0.9f, 0, 0);

            spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), glowColor * 0.4f, 0, glowTexture.Size() * 0.5f, 0.2f + fastScale * 0.4f, 0, 0);
            spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), glowColor * 0.2f, 0, glowTexture.Size() * 0.5f, fastScale, 0, 0);
            spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), glowColor * 0.6f, 0, glowTexture.Size() * 0.5f, 0.1f + fastScale * 0.1f, 0, 0);

            return false;
        }

        private void DrawSideSparkle(SpriteBatch spriteBatch, Vector2 position, float rotation, Color color, float length)
        {
            Texture2D sparkTexture = AssetDirectory.Textures.Sparkle.Value;
            Vector2 t = new Vector2(0.2f, length * 0.7f);
            Vector2 l = new Vector2(0.6f, length + 0.2f);
            spriteBatch.Draw(sparkTexture, position, sparkTexture.Frame(), color * 0.1f, rotation + MathHelper.PiOver2, sparkTexture.Size() * 0.5f, l, 0, 0);
            spriteBatch.Draw(sparkTexture, position, sparkTexture.Frame(), color * 1.5f, rotation + MathHelper.PiOver2, sparkTexture.Size() * 0.5f, t, 0, 0);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {

        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            Texture2D glowTexture = AssetDirectory.Textures.GlowBig.Value;

            spriteBatch.Draw(texture, position, frame, GetAlpha(Color.White).Value, 0, frame.Size() * 0.5f, scale + 0.2f, 0, 0);
            spriteBatch.Draw(texture, position, frame, new Color(200, 200, 200, 0), 0, frame.Size() * 0.5f, scale + 0.2f, 0, 0);

            Color glowColor = GetAlpha(Color.White).Value;
            glowColor.A = 0;
            spriteBatch.Draw(glowTexture, position, glowTexture.Frame(), glowColor * 0.7f, 0, glowTexture.Size() * 0.5f, scale * 0.2f, 0, 0);

            return false;
        }
    }
}
