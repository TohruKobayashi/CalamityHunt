using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles
{
    public class PumpActionSwampgunHeld : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.manualDirectionChange = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Mode => ref Projectile.ai[1];

        public ref Player Owner => ref Main.player[Projectile.owner];

        public float recoil;
        public float pump;

        public override void AI()
        {
            Projectile.damage = 0;

            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed) {
                Projectile.Kill();
                return;
            }

            if (Mode == 0) {
                if (Time == 0) {
                    Projectile.velocity = Owner.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * 5f;
                    Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
                }

                if (Time == 2) {
                    Projectile.frame = 2;
                }

                if (Time == 8) {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 30f, Projectile.velocity.SafeNormalize(Vector2.Zero) * 20f, ModContent.ProjectileType<SwampBall>(), 0, 0);

                    for (int i = 0; i < Main.rand.Next(3, 5); i++) {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 30f, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f) * Main.rand.Next(5, 20), ModContent.ProjectileType<SwampBall>(), 0, 0, ai1: -Main.rand.NextFloat(0.3f, 0.6f));
                    }

                    Projectile.frame = 1;
                }

                if (Time == 5) {
                    SoundEngine.PlaySound(SoundID.Item38, Projectile.Center);
                }

                if (Time > 5) {
                    recoil += 1.5f / MathF.Pow(Time * 0.5f, 1.5f);
                }

                if (Time > 25) {
                    Mode = 1;
                    Time = -1;
                }
            }
            else {
                if (Time == 15) {
                    SoundStyle cock = AssetDirectory.Sounds.GunCocking;
                    SoundEngine.PlaySound(cock, Projectile.Center);
                }

                if (Time < 19) {
                    recoil -= (float)Math.Pow((Time - 1) / 18f, 3f) * 0.5f;
                }
                else {
                    recoil = MathHelper.Lerp(recoil, 0f, 0.2f);
                }

                if (Time > 13) {
                    Projectile.frame = Time < 30 ? 2 : 0;
                }

                if (Time > 35) {
                    if (Owner.channel && Owner.controlUseItem) {
                        Time = -1;
                        Mode = 0;
                    }

                    if (Time > 40) {
                        Projectile.Kill();
                    }
                }
            }

            float rotation = Projectile.velocity.ToRotation();
            Projectile.rotation = rotation - recoil * Projectile.direction;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation - MathHelper.PiOver2 + (-recoil * 0.3f + pump) * Owner.direction);
            Vector2 position = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rotation - MathHelper.PiOver2 - recoil * 0.5f * Owner.direction);
            position.Y += Owner.gfxOffY;
            Projectile.Center = position;

            Owner.itemAnimation = 2;
            Owner.itemTime = 2;
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;

            recoil = MathHelper.Lerp(recoil, 0f, 0.02f);
            pump = MathHelper.Lerp(pump, 0f, 0.02f);

            Time++;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, 3, 0, Projectile.frame);
            Vector2 origin = frame.Size() * new Vector2(0.5f, 0.5f + 0.25f * Projectile.direction);
            SpriteEffects spriteEffects = Projectile.direction < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
        }
    }
}
