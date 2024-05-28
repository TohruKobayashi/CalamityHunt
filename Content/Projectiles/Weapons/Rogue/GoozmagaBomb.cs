using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityHunt.Content.Projectiles.Weapons.Rogue
{
	public class GoozmagaBomb : ModProjectile
	{
		const float maxStacks = 8;

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.width = 46;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 720;
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
			if (Projectile.localAI[1] == 0)
            {
				SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, Projectile.Center);
				Projectile.localAI[1] = 1;
            }
			// check if the projectile has a target host
			bool hasTarget = Projectile.ai[0] > 0;
			NPC target = hasTarget ? Main.npc[(int)Projectile.ai[0] - 1] : null;
			// orbit
			if (target != null && target.chaseable && target.active)
			{
				Projectile.velocity = Vector2.Zero;
				Projectile.localAI[1]++;
				float distance = target.width >= target.height ? target.width : target.height;
				distance += 30;
				double deg = Main.GlobalTimeWrappedHourly * 360 * (1+ Math.Clamp(Projectile.localAI[0], 0, 0.5f * maxStacks)) + Projectile.localAI[1];
				double rad = deg * (Math.PI / 180);
				float hyposx = target.Center.X - (int)(Math.Cos(rad) * distance) - Projectile.width / 2;
				float hyposy = target.Center.Y - (int)(Math.Sin(rad) * distance) - Projectile.height / 2;

				Projectile.position = new Vector2(hyposx, hyposy);

			}
			// die if it's in orbit mode and the target isn't present
			if (Projectile.ai[0] > 0 && !target.active)
            {
				Projectile.Kill();
			}
			bool speedGate = Math.Clamp(Projectile.localAI[0], 0, 0.5f * maxStacks) == 0.5f * maxStacks ? true : false;
			if (speedGate)
			{
				Color color = new Color(Main.rand.Next(150, 255), Main.rand.Next(150, 255), Main.rand.Next(150, 255));
				ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), Projectile.Center, Vector2.Zero, color, 1f);
			}
			else
			{
				Dust.NewDustPerfect(Projectile.Center, DustID.TintableDust, Vector2.Zero, 22, Color.Black, 2f).noGravity = true;
			}
			// animeme
			Projectile.frameCounter++;
			if (Projectile.frameCounter > 4)
            {
				Projectile.frame++;
				Projectile.frameCounter = 0;
            }
			if (Projectile.frame > 3)
            {
				Projectile.frame = 0;
            }
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
			// only deal meaningful damage if it's orbiting
			if (Projectile.ai[0] <= 0)
			{
				modifiers.SetMaxDamage(1);
			}
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			bool projCount = false;
			bool hasMinis = false;
			bool amIOrbitting = Projectile.ai[0] > 0 ? true : false;
			if (!amIOrbitting)
			{
				// check if the target already has an orbital
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile proj = Main.projectile[i];
					if (proj.type == ModContent.ProjectileType<GoozmagaBomb>() && proj.whoAmI != Projectile.whoAmI && proj.ai[0] == target.whoAmI + 1 && proj.active)
					{
						projCount = true;
						break;
					}
				}
				// check if the target already has mini orbitals
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile proj = Main.projectile[i];
					if (proj.type == ModContent.ProjectileType<GoozmagaShrapnel>() && proj.localAI[0] == target.whoAmI && proj.active)
					{
						hasMinis = true;
						break;
					}
				}
				// if it has an orbital, slightly speed up the target's orbital and kill this projectile
				if (projCount)
				{
					for (int i = 0; i < Main.maxProjectiles; i++)
					{
						Projectile proj = Main.projectile[i];
						if (proj.type == ModContent.ProjectileType<GoozmagaBomb>() && proj.whoAmI != Projectile.whoAmI && proj.ai[0] == target.whoAmI + 1 && proj.active)
						{
							proj.localAI[0] += 0.5f;
							if (proj.localAI[0] == maxStacks / 2)
							{
								SoundEngine.PlaySound(SoundID.Item4, proj.Center);
							}
							else if (proj.localAI[0] < maxStacks / 2)
							{
								SoundEngine.PlaySound(SoundID.Item28 with { Volume = SoundID.Item28.Volume * 0.8f }, Projectile.Center);
							}
							else
							{ 
							}
							if (proj.timeLeft < maxStacks * 60)
							{
								proj.timeLeft += 60;
							}
							for (int s = 0; s < 20; s++)
							{
								Color color = new Color(Main.rand.Next(150, 255), Main.rand.Next(150, 255), Main.rand.Next(150, 255));
								Vector2 inward = Projectile.Center + Main.rand.NextVector2Circular(1, 1);
								ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), inward, -inward.DirectionTo(Projectile.Center) * Main.rand.NextFloat(3f), color, 1f);
							}
							Projectile.Kill();
							break;
						}
					}
				}
				// if it doesn't have an orbital, BECOME the orbital
				if (target.active && !projCount && Projectile.ai[0] == 0)
				{
					SoundEngine.PlaySound(SoundID.Item62, Projectile.position);
					Projectile.ai[0] = target.whoAmI + 1;
					// if it's a stealth strike, spawn two extra orbitals that do 50% damage
					if (Projectile.ai[1] == 1 && !hasMinis)
					{
						for (int d = 0; d < 20; d++)
						{
							Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), DustID.TintableDust, Main.rand.NextVector2CircularEdge(5, 5), 200, Color.Black, Main.rand.NextFloat(1, 3)).noGravity = true;
						}
						SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
						for (int i = 0; i < 2; i++)
						{
							int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, -20), ModContent.ProjectileType<GoozmagaShrapnel>(), (int)Projectile.damage / 2, Projectile.knockBack, Main.myPlayer, Projectile.whoAmI, i + 1);
							Main.projectile[p].timeLeft += 20 * i;
							Main.projectile[p].localAI[0] = -1;
						}
					}
				}
			}
        }

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
			for (int d = 0; d < 20; d++)
			{
				Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), DustID.TintableDust, Main.rand.NextVector2CircularEdge(5, 5), 200, Color.Black, Main.rand.NextFloat(1, 3)).noGravity = true;
			}
		}
    }
}
