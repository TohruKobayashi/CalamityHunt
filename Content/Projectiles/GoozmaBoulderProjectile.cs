using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles
{
    public class GoozmaBoulderProjectile : ModProjectile
    {
        public int TimesHit
        {
            get => (int)Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        public bool InsaneMode
        {
            get => Projectile.localAI[1] == 1;
            set => Projectile.localAI[1] = value == true ? 1 : 0;
        }

        public bool InitialVelocityCheck
        {
            get => Projectile.localAI[2] == 1;
            set => Projectile.localAI[2] = value == true ? 1 : 0;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.ForcePlateDetection[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 92;
            Projectile.height = 88;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.trap = true;
            Projectile.timeLeft = 36000;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(TimesHit);
            writer.Write(InsaneMode);
            writer.Write(InitialVelocityCheck);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            TimesHit = reader.ReadInt32();
            InsaneMode = reader.ReadBoolean();
            InitialVelocityCheck = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                Collision.SwitchTiles(Projectile.position, Projectile.width, Projectile.height, Projectile.oldPosition, 3);
            }

            if (InsaneMode) {
                InsaneAI();
            }
            else {
                NormalAI();
            }
        }

        public void NormalAI()
        {
            if (Projectile.shimmerWet) {
                if (Projectile.velocity.Y > 10f) {
                    Projectile.velocity.Y *= 0.97f;
                }

                Projectile.velocity.Y -= 0.7f;
                if (Projectile.velocity.Y < -10f) {
                    Projectile.velocity.Y = -10f;
                }
            }
            else {
                Projectile.rotation += Projectile.velocity.X * 0.02f;

                Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, -16f, 16f);

                if (Math.Abs(Projectile.velocity.Y) <= 1f) {
                    if (Projectile.velocity.X > 0f && Projectile.velocity.X < 22) {
                        Projectile.velocity.X += 0.05f;
                    }

                    if (Projectile.velocity.X < 0f && Projectile.velocity.X > -22) {
                        Projectile.velocity.X -= 0.05f;
                    }
                }
                Projectile.velocity.Y += 0.2f;
            }
        }

        public void InsaneAI()
        {
            byte closestPlayer = FindClosestAlive(Projectile.position, Projectile.width, Projectile.height);
            Player player = Main.player[closestPlayer];
            Projectile.rotation = Utils.AngleFrom(Projectile.position, player.position);
            Projectile.velocity = Vector2.Zero;
            float distance = Vector2.Distance(Projectile.Center, player.Center);
            float amount = Math.Clamp((1 - 1 / distance) * 0.05f, 0, 1);
            Projectile.Center = Vector2.Lerp(Projectile.Center, player.Center, amount);
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (InsaneMode) {
                return false;
            }

            if (Projectile.velocity.X != oldVelocity.X && InitialVelocityCheck) {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
                Projectile.velocity.X = -oldVelocity.X;
                TimesHit += 1;
            }
            if (!InitialVelocityCheck) {
                byte closestPlayer = FindClosestAlive(Projectile.position, Projectile.width, Projectile.height);
                bool dir = Main.player[closestPlayer].position.X > Projectile.position.X;
                Projectile.velocity.X = dir ? 0.5f : -0.5f;
                InitialVelocityCheck = true;
            }
            if (Projectile.velocity.Y != oldVelocity.Y) {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                if (oldVelocity.Y >= 0.35) {
                    SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
                    Projectile.velocity.Y = -oldVelocity.Y * 0.95f;
                    TimesHit += 1;
                }
                byte closestPlayer = FindClosestAlive(Projectile.position, Projectile.width, Projectile.height);
                bool dir = Main.player[closestPlayer].position.X > Projectile.position.X;
                Projectile.velocity.X = Math.Abs(Projectile.velocity.X);
                Projectile.velocity.X *= dir ? 1 : -1;

            }
            if (TimesHit >= 20) {
                InsaneMode = true;
                Projectile.tileCollide = false;
            }
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.netMode == NetmodeID.SinglePlayer) {
                if (target.difficulty != 1 && target.difficulty != 2) {
                    target.DropItems();
                }

                target.ghost = true;
                target.statLife = 0;
                target.KillMe(PlayerDeathReason.ByProjectile(target.whoAmI, Projectile.whoAmI), 1, 0);
            }
            else {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)CalamityHunt.PacketType.TrollPlayer);
                packet.Write((byte)target.whoAmI);
                packet.Write((byte)Projectile.whoAmI);
                packet.Send();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.StrikeInstantKill();
        }

        private static byte FindClosestAlive(Vector2 Position, int Width, int Height)
        {
            byte result = 0;
            for (byte i = 0; i < 255; i++) {
                if (Main.player[i].active) {
                    result = i;
                    break;
                }
            }
            float num = -1f;
            for (byte j = 0; j < 255; j++) {
                if (Main.player[j].active && !Main.player[j].dead && !Main.player[j].ghost) {
                    float num2 = Math.Abs(Main.player[j].position.X + (Main.player[j].width / 2) - (Position.X + (Width / 2))) + Math.Abs(Main.player[j].position.Y + (Main.player[j].height / 2) - (Position.Y + (Height / 2)));
                    if (num == -1f || num2 < num) {
                        num = num2;
                        result = j;
                    }
                }
            }
            return result;
        }
    }
}
