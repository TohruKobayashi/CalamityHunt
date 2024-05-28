using System;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged
{
    public class CometKunaiCritProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 100;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 100;
            Projectile.manualDirectionChange = true;
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                DamageClass d;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind("RogueDamageClass", out d);
                Projectile.DamageType = d;
            }
        }

        private Vector2 oldVelocity;

        private bool isAttached = false;

        private float sineStart = Main.rand.NextFloat(10f);

        public override void AI()
        {
            for (int i = 0; i < 5; i++) {
                Color randomColor = Color.Lerp(Color.Blue, Color.RoyalBlue, Main.rand.NextFloat());
                randomColor.A = 0;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity / 5f * i, DustID.SparkForLightDisc, Projectile.velocity * 0.1f, 0, randomColor, 1.1f);
                d.noGravity = true;
            }

            if (Projectile.ai[0] == 0 && Main.myPlayer == Projectile.owner) {
                Projectile.ai[0]++;
                oldVelocity = Projectile.velocity;
                Projectile.rotation = Main.rand.NextFloat();
                Projectile.netUpdate = true;
            }


            if (!isAttached) {
                Projectile.direction = oldVelocity.X > 0 ? 1 : -1;

                Projectile.rotation += Projectile.direction * 0.2f;

                Projectile.velocity = oldVelocity.RotatedBy(MathF.Sin(Projectile.ai[1] * 0.1f + sineStart) * 0.2f);
            }
            else {
                NPC target = Main.npc[(int)Projectile.ai[2]];
                {
                    if (target.active) {
                        Projectile.Center += (target.position - target.oldPosition) / (Projectile.extraUpdates + 1);
                        Projectile.netUpdate = true;
                    }
                    else {
                        Projectile.Kill();
                    }
                }
            }

            Projectile.ai[1]++;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++) {
                Color randomColor = Color.Lerp(Color.Blue, Color.RoyalBlue, Main.rand.NextFloat());
                randomColor.A = 0;
                Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(10), 20, 20, DustID.SparkForLightDisc, 0, 0, 0, randomColor);
                d.noGravity = true;
                d.velocity += Main.rand.NextVector2Circular(4, 4);
            }

            //if (Main.myPlayer == Projectile.owner) {
            //    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -oldVelocity * 0.6f, ModContent.ProjectileType<CometKunaiGhostProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, -1);
            //    Projectile.netUpdate = true;
            //}
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[2] = target.whoAmI;
            SoundStyle attachSound = AssetDirectory.Sounds.GoozmaMinions.StellarConstellationWave with { MaxInstances = 0, Pitch = 0.8f, PitchVariance = 0.1f, Volume = 0.4f };
            SoundEngine.PlaySound(attachSound, Projectile.Center);
            for (int i = 0; i < 9; i++) {
                Color randomColor = Color.Lerp(Color.Blue, Color.RoyalBlue, Main.rand.NextFloat());
                randomColor.A = 0;
                Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(5), 10, 10, DustID.SparkForLightDisc, 0, 0, 0, randomColor, 2f);
                d.noGravity = true;
                d.velocity += Main.rand.NextVector2Circular(7, 7);
            }
            Projectile.timeLeft = 70;
            Projectile.Center += Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            isAttached = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = AssetDirectory.Textures.Glow[0].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), new Color(0, 70, 200, 0), Projectile.rotation, texture.Size() * new Vector2(0.5f, 0.6f), Projectile.scale * 1.3f, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.White, Projectile.rotation, texture.Size() * new Vector2(0.5f, 0.6f), Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), new Color(0, 10, 90, 0), Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 0.7f, 0, 0);

            return false;
        }
    }
}
