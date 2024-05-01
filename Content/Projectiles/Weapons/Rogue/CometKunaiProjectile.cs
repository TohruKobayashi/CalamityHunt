using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Rogue
{
    public class CometKunaiProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Throwing;
            if (ModLoader.HasMod("CalamityMod"))
            {
                DamageClass d;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<DamageClass>("RogueDamageClass", out d);
                Projectile.DamageType = d;
            }
            Projectile.extraUpdates = 11;
            Projectile.timeLeft = 80;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1] = -1;

                Color randomColor = Color.Lerp(Color.Blue, Color.RoyalBlue, Main.rand.NextFloat());
                randomColor.A = 0;
                Dust d = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * Main.rand.NextFloat(), DustID.SparkForLightDisc, Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(), 0, randomColor, 0.9f);
                d.noGravity = true;

                if (Projectile.timeLeft < 40)
                {
                    int t = Projectile.FindTargetWithLineOfSight(400);
                    if (t > -1 && Main.myPlayer == Projectile.owner)
                    {
                        if (Main.npc[t].Distance(Main.MouseWorld) < 600)
                        {
                            Projectile.velocity += Projectile.DirectionTo(Main.npc[t].Center).SafeNormalize(Vector2.Zero) * 1.1f;
                            Projectile.velocity *= 0.95f;
                            Projectile.netUpdate = true;
                        }
                    }
                }

                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.localAI[0] += Projectile.velocity.Length() * 0.04f;

                if (Projectile.timeLeft < 2)
                    SetCollided(false);
            }
            else
            {
                if (Projectile.ai[0] == -2)
                {
                    Projectile.localAI[1] = MathHelper.Clamp(Projectile.localAI[1] - 0.015f, 0f, 1f);
                    Projectile.rotation -= MathF.Sin(Projectile.localAI[0] * MathHelper.TwoPi * 2f) * 0.4f * Projectile.localAI[1] * Projectile.direction;
                }

                if (Projectile.ai[1] > -1)
                {
                    NPC target = Main.npc[(int)Projectile.ai[1]];
                    if (target.active)
                        Projectile.Center += (target.position - target.oldPosition) / (Projectile.extraUpdates + 1);
                    else
                        Projectile.Kill();
                }

                Projectile.velocity *= 0.92f;
                Projectile.localAI[0] -= 0.6f;

                if (Projectile.timeLeft == 10)
                {
                    ParticleBehavior.NewParticle(ModContent.GetInstance<CrossSparkleParticleBehavior>(), Projectile.Center, MathHelper.PiOver4.ToRotationVector2(), new Color(50, 180, 255, 0), 1f);
                    ParticleBehavior.NewParticle(ModContent.GetInstance<CrossSparkleParticleBehavior>(), Projectile.Center, Vector2.Zero, new Color(50, 180, 255, 0), 0.5f);
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundStyle killSound = SoundID.MaxMana with { MaxInstances = 0, Pitch = 1f, PitchVariance = 0.4f };
            SoundEngine.PlaySound(killSound, Projectile.Center);
            for (int i = 0; i < 9; i++)
            {
                Color randomColor = Color.Lerp(Color.Blue, Color.RoyalBlue, Main.rand.NextFloat());
                randomColor.A = 0;
                Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(5), 10, 10, DustID.SparkForLightDisc, 0, 0, 0, randomColor, 2f);
                d.noGravity = true;
            }
        }

        private void SetCollided(bool stick)
        {
            Projectile.extraUpdates = 1;
            Projectile.ai[0] = stick ? -2 : -1;
            Projectile.localAI[1] = 1f;
            Projectile.timeLeft = stick ? 80 : 50;
            if (stick)
            {
                Projectile.tileCollide = false;
                SoundStyle attachSound = SoundID.Item108 with { MaxInstances = 0, Pitch = 1f, PitchVariance = 0.2f, Volume = 0.2f };
                SoundEngine.PlaySound(attachSound, Projectile.Center);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 0)
                SetCollided(true);

            Projectile.velocity *= 0.01f;
            Projectile.Center += oldVelocity * 2f;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SetCollided(true);
            Projectile.Center += Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            Projectile.ai[1] = target.whoAmI;

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.player[Projectile.owner].Center, Main.rand.NextVector2CircularEdge(3, 3), ModContent.ProjectileType<CometKunaiGhostProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, target.whoAmI);
        }

        public override bool? CanDamage() => Projectile.ai[0] == 0;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.ai[1] < 0)
                behindNPCsAndTiles.Add(index);
            else
                Projectile.hide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glowTexture = TextureAssets.Projectile[ModContent.ProjectileType<CometKunaiGhostProjectile>()].Value;
            Vector2 direction = Projectile.rotation.ToRotationVector2() * 10;

            VertexStrip strip = new VertexStrip();

            if (Projectile.localAI[0] > 2)
            {
                Vector2[] oldPos = new Vector2[Projectile.oldPos.Length * 2];
                float[] oldRot = new float[Projectile.oldPos.Length * 2];
                for (int i = 0; i < oldPos.Length; i++)
                {
                    Vector2 first = Projectile.oldPos[Math.Clamp(i / 2, 0, Projectile.oldPos.Length - 1)];
                    Vector2 second = Projectile.oldPos[Math.Clamp(i /2 + 1, 0, Projectile.oldPos.Length - 1)];
                    oldPos[i] = Vector2.Lerp(first, second, (i % 2f) / 2f);
                }
                for (int i = 1; i < oldPos.Length; i++)
                    oldRot[i] = oldPos[i].AngleTo(oldPos[i - 1]);
                oldRot[0] = Projectile.rotation;

                strip.PrepareStrip(oldPos, oldRot, StripColor, StripWidth, -Main.screenPosition + Projectile.Size * 0.5f + direction, oldPos.Length);

                //god i need to start loading these on load time
                Effect effect = ModContent.Request<Effect>(Mod.Name + "/Assets/Effects/ThrowingStarsTrailEffect", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                effect.Parameters["uColor"].SetValue(new Color(0, 80, 255, 0).ToVector4());
                effect.Parameters["uInnerColor"].SetValue(new Color(140, 255, 255, 50).ToVector4());
                effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
                effect.Parameters["uTexture"].SetValue(AssetDirectory.Textures.SlimeMonsoon.LightningGlow.Value);
                effect.Parameters["uTexture2"].SetValue(TextureAssets.Extra[189].Value);
                effect.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 2);
                effect.CurrentTechnique.Passes[0].Apply();

                strip.DrawTrail();

                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = new Vector2(1).RotatedBy(MathHelper.TwoPi / 4f * i + Projectile.rotation);
                Main.EntitySpriteDraw(glowTexture, Projectile.Center + offset * 2 + direction - Main.screenPosition, glowTexture.Frame(), new Color(20, 20, 200, 0), Projectile.rotation, glowTexture.Size() * new Vector2(1f, 0.5f), Projectile.scale, 0, 0);
                Main.EntitySpriteDraw(glowTexture, Projectile.Center + offset + direction - Main.screenPosition, glowTexture.Frame(), new Color(100, 200, 255, 0), Projectile.rotation, glowTexture.Size() * new Vector2(1f, 0.5f), Projectile.scale, 0, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center + direction - Main.screenPosition, texture.Frame(), lightColor, Projectile.rotation, texture.Size() * new Vector2(1f, 0.5f), Projectile.scale, 0, 0);

            return false;
        }

        public float StripWidth(float x) => 20f * (1 - x) * Utils.GetLerpValue(2, 8, Projectile.localAI[0], true) * Utils.GetLerpValue(-0.02f, 0.1f, x, true);

        public Color StripColor(float x) => Color.Lerp(new Color(10, 140, 255, 0), new Color(10, 170, 255, 128), Utils.GetLerpValue(0.9f, 0.7f, x, true));
    }
}
