using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Melee
{
    public class SacredArmsHeld : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            //TODO make this not suck
            Projectile.width = 88;
            Projectile.height = 72;
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
            // so this is based on blockarozs swing code used on parasanguine, scythe, pump action
            // pretty lazy but id rather kill myself than look at examplecustomswinganim + this code is nice n simple
            // unfortunately, this Also Sucks bcuz nothing is commented 
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed) {
                Projectile.Kill();
                return;
            }

            // mode 0 dictates swinging the weapon backwards
            if (Mode == 0) {
                if (Time == 0) {
                    Projectile.velocity = Owner.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * 5f;
                    Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
                }

                if (Time > 5) {
                    recoil += 1.5f / MathF.Pow(Time * 0.5f, 1.5f);
                }

                if (Time > 25) {
                    Mode = 1;
                    Time = -1;
                }
            }
            // mode not 0 dictates swinging the weapon forwards
            else {
                if (Time < 19) {
                    recoil -= (float)Math.Pow((Time - 1) / 18f, 3f) * 0.5f;
                }
                else {
                    recoil = MathHelper.Lerp(recoil, 0f, 0.2f);
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
            Vector2 origin = Projectile.direction < 0 ?  new Vector2(10f, 8f + 0.25f * Projectile.direction) : new Vector2(10f, (-8f + Projectile.height) + 0.25f * Projectile.direction);
            SpriteEffects spriteEffects = Projectile.direction < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), lightColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
        }
    }
}
