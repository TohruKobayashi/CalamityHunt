using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Rogue
{
	public class GoozmagaShrapnel : ModProjectile
	{
		public ref float MomIndex => ref Projectile.ai[0];
		public ref float TargetIndex => ref Projectile.localAI[0];
		public ref float PlanetoidNum => ref Projectile.localAI[1];

		public override void SetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 22;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 1440;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Throwing;
			if (ModLoader.HasMod("CalamityMod"))
			{
				DamageClass d;
				Mod calamity = ModLoader.GetMod("CalamityMod");
				calamity.TryFind<DamageClass>("RogueDamageClass", out d);
				Projectile.DamageType = d;
			}
		}

		public override void AI()
		{
			Projectile mom = Main.projectile[(int)MomIndex];

			Dust.NewDustPerfect(Projectile.Center, DustID.TintableDust, Vector2.Zero, 100, Color.Black, 1f).noGravity = true;

			bool hasATarget = TargetIndex != -1;

			if (mom != null && mom.type == ModContent.ProjectileType<GoozmagaBomb>() && !hasATarget)
			{
				bool hasTarget = mom.ai[0] > -1;
				NPC target = hasTarget ? Main.npc[(int)mom.ai[0]] : null;
				TargetIndex = hasTarget ? mom.ai[0] : -1;
			}
			if (hasATarget)
			{
				NPC target = Main.npc[(int)TargetIndex - 1];
				if (target != null)
				{
					Projectile.localAI[1] += PlanetoidNum == 1 ? 0.75f : 1f;
					float distance = 100;
					distance = target.width >= target.height ? target.width : target.height;
					distance += Projectile.ai[1] == 1 ? 20 : 60;
					double deg = 120 * Projectile.ai[1] + Main.GlobalTimeWrappedHourly * 660 +PlanetoidNum;
					double rad = deg * (Math.PI / 180);
					float hyposx = target.Center.X - (int)(Math.Cos(rad) * distance) - Projectile.width / 2;
					float hyposy = target.Center.Y - (int)(Math.Sin(rad) * distance) - Projectile.height / 2;

					Projectile.position = new Vector2(hyposx, hyposy);
				}
				if (!target.active)
				{
					Projectile.Kill();
				}
			}
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item89 with { Pitch = SoundID.Item62.Pitch + 0.2f, Volume = SoundID.Item62.Volume - 0.4f}, Projectile.position);
			for (int d = 0; d < 10; d++)
			{
				Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(3, 3), DustID.TintableDust, Main.rand.NextVector2CircularEdge(5, 5), 200, Color.Black, 0.8f).noGravity = true;
			}
		}
	}
}
