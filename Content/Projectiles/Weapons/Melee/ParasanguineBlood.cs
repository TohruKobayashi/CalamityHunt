using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Melee
{
    public class ParasanguineBlood : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.DamageType = DamageClass.Melee;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Mode => ref Projectile.ai[1];
        public ref float StickHost => ref Projectile.ai[2];

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[0] = Main.rand.Next(0, 2);
            Projectile.localAI[1] = Main.rand.NextFloat(0.8f, 1.2f);
            StickHost = -1;
            Projectile.spriteDirection = Main.rand.NextBool() ? 1 : -1;
        }

        public override void AI()
        {
            Projectile.scale = MathF.Sqrt(Utils.GetLerpValue(-5, 8, Time, true) * Utils.GetLerpValue(250, 200, Time, true)) * 1.1f;
            if (Mode == 0)
                Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.velocity *= 0.999f;
            if (Projectile.velocity.Length() > 15f)
                Projectile.velocity *= 0.98f;

            Projectile.velocity.Y += Time < 25 ? 0.15f : 0.7f;

            if (Time > 250)
                Projectile.Kill();

            if (Main.rand.NextBool(25) || (Main.rand.NextBool(7) && Mode == 0))
            {
                Dust blood = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 10), DustID.Blood, -Projectile.velocity * 0.5f + Main.rand.NextVector2Circular(5, 5), 0, Color.DarkRed, 2f + Main.rand.NextFloat());
                blood.noGravity = true;
            }

            if (StickHost > -1)
            {
                if (Time < 200)
                    Time = 200;

                if (!Main.npc[(int)StickHost].active)
                    Projectile.Kill();

                Projectile.velocity = Vector2.Zero;
                Projectile.rotation = Projectile.AngleTo(Main.npc[(int)StickHost].Center);
                Projectile.Center += Main.npc[(int)StickHost].position - Main.npc[(int)StickHost].oldPosition;
            }

            if (Mode == 2)
                Projectile.rotation = Projectile.rotation.AngleLerp(MathHelper.PiOver2, 0.5f);

            Projectile.frame = (Mode > 0 ? 2 : 0) + (int)Projectile.localAI[0];

            Time++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Mode = 1;
            Projectile.velocity *= 0.9f;
            StickHost = target.whoAmI;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity.X *= 0.8f;

            if (Mode == 0 && Projectile.velocity.Y >= 0)
            {
                Mode = 2;
                if (Time < 200)
                    Time = 200;

                SoundEngine.PlaySound(SoundID.NPCDeath9, Projectile.Center);

            }

            if (Mode == 0 && MathF.Abs(Projectile.velocity.Y - oldVelocity.Y) > 0)
                Projectile.velocity.Y *= -1;

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Projectile.frame = (Mode > 0 ? 2 : 0) + (int)Projectile.localAI[0];
            Rectangle frame = texture.Frame(4, 1, Projectile.frame, 0);
            Vector2 squish = new Vector2(1.05f - Projectile.velocity.Length() * 0.005f, 1.05f + Projectile.velocity.Length() * 0.01f);
            if (Mode == 2)
                squish = new Vector2(1.2f, 0.8f);

            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, Projectile.height * 0.5f * (Mode == 2 ? 1 : 0)) - Main.screenPosition, frame, new Color(180, 10, 30).MultiplyRGBA(lightColor), Projectile.rotation - MathHelper.PiOver2 * Projectile.spriteDirection, frame.Size() * new Vector2(0.5f, 0.5f + 0.4f * Projectile.spriteDirection), Projectile.scale * Projectile.localAI[1] * squish, spriteEffects, 0);

            return false;
        }
    }
}
