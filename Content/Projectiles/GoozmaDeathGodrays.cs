using CalamityHunt.Common.Graphics.SlimeMonsoon;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using CalamityHunt.Core;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityHunt.Content.Projectiles
{
    public class GoozmaDeathGodrays : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.damage = 0;
            Projectile.timeLeft = 300;
            Projectile.hide = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref NPC Host => ref Main.npc[(int)Projectile.ai[1]];

        public List<float> rotations;
        public List<float> accelerations;
        public List<float> waits;
        public List<float> lengths;

        public override void OnSpawn(IEntitySource source)
        {
            rotations = new List<float>();
            accelerations = new List<float>();
            waits = new List<float>();
            lengths = new List<float>();

            int count = Main.rand.Next(20, 30);
            for (int i = 0; i < count; i++)
            {
                rotations.Add(MathHelper.TwoPi / (float)count * i + Main.rand.NextFloat(-0.5f, 0.5f));
                accelerations.Add(Main.rand.NextFloat(-3.1415f, 3.1415f));
                waits.Add(Main.rand.Next(200));
                lengths.Add(Main.rand.NextFloat(0.7f, 2f));
            }
        }

        public override void AI()
        {
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<Goozma>() && n.active))
                Projectile.active = false;
            else
                Projectile.ai[1] = Main.npc.First(n => n.type == ModContent.NPCType<Goozma>() && n.active).whoAmI;

            Projectile.Center = Host.Center;

            for (int i = 0; i < rotations.Count; i++)
            {
                rotations[i] += (float)Math.Sin(Time * 0.9f + accelerations[i]) * 0.02f;
            }

            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCs.Add(index);

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

            for (int i = 0; i < rotations.Count; i++)
            {
                Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Host.localAI[0] + rotations[i] * 0.5f);
                glowColor.A = 0;
                float power = Utils.GetLerpValue(waits[i], waits[i] + 70, Time, true) * Utils.GetLerpValue(300, 290, Time, true);
                Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, glowColor * power * 0.5f * (0.5f + lengths[i] * 0.5f), rotations[i] + Time * (1f + i * 0.001f) * 0.005f, texture.Size() * new Vector2(0.5f, 1f), new Vector2(1f, 2f * lengths[i]) * (0.2f + power * 0.8f), 0, 0);
                Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, glowColor * power * 0.8f * (0.5f + lengths[i] * 0.5f), rotations[i] + Time * (1f + i * 0.001f) * 0.005f, texture.Size() * new Vector2(0.5f, 1f), new Vector2(0.555f, 1f * lengths[i]) * (0.2f + power * 0.8f), 0, 0);
            }

            return false;
        }
    }
}
