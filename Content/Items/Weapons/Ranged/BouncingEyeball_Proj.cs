using System;
using System.Collections.Generic;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Ranged
{
    public class BouncingEyeball_Proj : ModProjectile
    {
        public enum Variant
        {
            Standard = 0,
            Cataract = 1,
            Sleepy = 2,
            Dialated = 3,
            Green = 4,
            Purple = 5,
            Retro = 6
        }

        public enum Costume
        {
            None = 0,
            Owl = 1,
            Spaceship = 2
        }

        public ref float Timer => ref Projectile.ai[0];
        public ref float BounceCooldown => ref Projectile.ai[1];

        public Variant CurrentVariant
        {
            get => (Variant)Projectile.localAI[0];
            set => Projectile.localAI[0] = (float)value;
        }

        public Costume CurrentCostume
        {
            get => (Costume)Projectile.localAI[1];
            set => Projectile.localAI[1] = (float)value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 240;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            Projectile.penetrate = 3;
        }

        public override void AI()
        {
            if (Timer == 0)
            {
                if (Main.rand.NextBool(6))
                    CurrentVariant = (Variant)Main.rand.Next(1, 6);
                if (Main.rand.NextBool(1000))
                    CurrentVariant = Variant.Retro;

                if ((Main.halloween && Main.rand.NextBool(20)) || (Main.rand.NextBool(5000))) 
                    CurrentCostume = (Costume)Main.rand.Next(1, 3);
            }

            if (Projectile.velocity.X < 0)
                Projectile.direction = -1;

            // collision handled in modsystem for optimization
            
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 0.98f;

            Timer++;
            BounceCooldown--;
        }

        public override void OnKill(int timeLeft)
        {
            SoundStyle killSound = SoundID.NPCDeath22 with { MaxInstances = 0, Pitch = 0.6f, PitchVariance = 0.4f, Volume = 0.4f };
            SoundEngine.PlaySound(killSound, Projectile.Center);
            for (int i = 0; i < 9; i++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood);
            for (int i = 0; i < 5; i++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DynastyWall, Scale: 0.8f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (BounceCooldown <= 0)
            {
                SoundEngine.PlaySound(SoundID.NPCHit19 with { MaxInstances = 0, PitchVariance = 0.4f, Volume = 0.3f }, Projectile.Center);
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                    Projectile.velocity.X = -oldVelocity.X;
                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                    Projectile.velocity.Y = -oldVelocity.Y;

                BounceCooldown = 12;
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (BounceCooldown <= 0)
            {
                SoundEngine.PlaySound(SoundID.NPCHit19 with { MaxInstances = 0, PitchVariance = 0.4f, Volume = 0.3f }, Projectile.Center);
                for (int i = 0; i < 4; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Scale: 0.8f);

                Vector2 velocToConserve = new Vector2(3, 0);
                Projectile.velocity += velocToConserve.RotatedBy(Projectile.DirectionFrom(target.Center).ToRotation());
                Projectile.velocity += target.velocity * 0.25f;
                Projectile.timeLeft = 240;

                BounceCooldown = 12;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 DrawCenter = Projectile.Center;
            Rectangle EyeFrame = new Rectangle(0, (int)CurrentVariant * 20, 24, 20);

            Main.EntitySpriteDraw(texture, DrawCenter - Main.screenPosition, EyeFrame, lightColor, Projectile.rotation, EyeFrame.Size() * 0.5f, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None);

            if (CurrentCostume == Costume.Owl)
            {
                Rectangle OwlFrame = new Rectangle(24, 0, 32, 22);
                Vector2 offset = new Vector2(-2, -3 * Projectile.direction).RotatedBy(Projectile.rotation);

                Main.EntitySpriteDraw(texture, DrawCenter + offset - Main.screenPosition, OwlFrame, lightColor, Projectile.rotation, OwlFrame.Size() * 0.5f, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None);
            }
            else if (CurrentCostume == Costume.Spaceship)
            {
                Rectangle SpaceshipFrame = new Rectangle(24, 22, 28, 26);
                Vector2 offset = new Vector2(2, 1 * Projectile.direction).RotatedBy(Projectile.rotation);

                if (Timer % 12 > 6)
                    SpaceshipFrame.Y += SpaceshipFrame.Height;

                Main.EntitySpriteDraw(texture, DrawCenter + offset - Main.screenPosition, SpaceshipFrame, lightColor, Projectile.rotation, SpaceshipFrame.Size() * 0.5f, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None);
            }

            return false;
        }
    }

    public class BouncingEyeball_HandleCollisions : ModSystem
    {
        public override void PostUpdateProjectiles()
        {
            foreach (Projectile proj1 in Main.ActiveProjectiles)
            {
                if (proj1.type == ModContent.ProjectileType<BouncingEyeball_Proj>())
                {
                    foreach (Projectile proj2 in Main.ActiveProjectiles)
                    {
                        if (proj2.type == ModContent.ProjectileType<BouncingEyeball_Proj>() && proj2.whoAmI != proj1.whoAmI)
                        {
                            BouncingEyeball_Proj eye1 = proj1.ModProjectile as BouncingEyeball_Proj;
                            BouncingEyeball_Proj eye2 = proj2.ModProjectile as BouncingEyeball_Proj;

                            if (proj2.Hitbox.Intersects(proj1.Hitbox) && eye1.BounceCooldown <= 0 && eye2.BounceCooldown <= 0)
                            {
                                Vector2 velocToConserve = new Vector2(3, 0);

                                proj2.velocity += velocToConserve.RotatedBy(proj2.DirectionFrom(proj1.Center).ToRotation());
                                eye2.BounceCooldown = 2;
                                proj2.timeLeft = 240;

                                proj1.velocity += velocToConserve.RotatedBy(proj1.DirectionFrom(proj2.Center).ToRotation());
                                eye1.BounceCooldown = 2;
                                proj1.timeLeft = 240;

                                SoundEngine.PlaySound(SoundID.NPCHit19 with { MaxInstances = 0, PitchVariance = 0.4f, Volume = 0.3f }, proj1.Center);
                                Vector2 roomInBetween = Vector2.Lerp(proj1.Center, proj2.Center, 0.5f);
                                for (int i = 0; i < 4; i++)
                                    Dust.NewDustDirect(roomInBetween - new Vector2(8, 8), 8, 8, DustID.Blood, Scale: 0.8f);
                            }
                        }
                    }
                }
            }
            
            
        }
    }
}
