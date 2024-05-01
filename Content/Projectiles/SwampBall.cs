using CalamityHunt.Content.Buffs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Humanizer.In;

namespace CalamityHunt.Content.Projectiles
{
    public class SwampBall : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.timeLeft = 10000;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
        }

        public ref float Time => ref Projectile.ai[0];

        public override bool? CanDamage() => true;

        public override void AI()
        {
            if (Time < 20)
                Projectile.scale = 1f + Projectile.ai[1];
            else
                Projectile.scale *= 0.99f;

            if (Projectile.scale < 0.3f)
                Projectile.Kill();

            if (Projectile.owner == Main.myPlayer)
            {
                Rectangle rectangle4 = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile. height);
                for (int npc = 0; npc < Main.maxNPCs; npc++)
                {
                    if (Main.npc[npc].active && !Main.npc[npc].dontTakeDamage && Main.npc[npc].lifeMax > 1)
                    {
                        Rectangle value4 = new Rectangle((int)Main.npc[npc].position.X, (int)Main.npc[npc].position.Y, Main.npc[npc].width, Main.npc[npc].height);
                        if (rectangle4.Intersects(value4))
                        {
                            Main.npc[npc].AddBuff(ModContent.BuffType<Swamped>(), 1500);
                            Projectile.Kill();
                        }
                    }
                }

                for (int player = 0; player < Main.maxPlayers; player++)
                {
                    if (player != Projectile.owner && Main.player[player].active && !Main.player[player].dead)
                    {
                        Rectangle value5 = new Rectangle((int)Main.player[player].position.X, (int)Main.player[player].position.Y, Main.player[player].width, Main.player[player].height);
                        if (rectangle4.Intersects(value5))
                        {
                            Main.player[player].AddBuff(ModContent.BuffType<Swamped>(), 1500);
                            Projectile.Kill();
                        }
                    }
                }
            }

            Projectile.velocity.Y += 0.05f + Utils.GetLerpValue(30, 120, Time, true);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            for (int i = 0; i < 3; i++)
            {
                Dust slime = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(7, 7), DustID.SlimeBunny, Projectile.velocity.RotatedByRandom(0.1f) * 0.33f * Main.rand.NextFloat(-1f, 1f), 210, new Color(0, 235, 90, 100), 1f + Main.rand.NextFloat());
                slime.noGravity = !Main.rand.NextBool(20);
            }

            Time++;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < Main.rand.Next(10, 20); i++)
            {
                Dust slime = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(7, 7), DustID.SlimeBunny, -Projectile.velocity * 0.1f + Main.rand.NextVector2Circular(4, 4), 210, new Color(0, 235, 90, 100), 1f + Main.rand.NextFloat());
                slime.noGravity = Main.rand.NextBool();
            }

            SoundEngine.PlaySound(SoundID.NPCDeath9, Projectile.Center);
        }
    }
}
