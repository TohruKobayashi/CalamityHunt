using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Runtime.Intrinsics;
using CalamityHunt.Common.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Summoner
{
    public class SlimeCaneGemCounter : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;

            Main.projFrames[Type] = 6;
            Main.projPet[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 60;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
        }


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.GetModPlayer<SlimeCanePlayer>().slimes)
                Projectile.timeLeft = 2;

            Projectile.velocity = Vector2.Zero;
            PlaceCountersAbovePlayerPleasinglyForDebugging();

            if (++Projectile.frameCounter >= 4) 
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6) 
                    Projectile.frame = 0;
            }
        }

        private void PlaceCountersAbovePlayerPleasinglyForDebugging()
        {
            Vector2 pos = new Vector2(Main.LocalPlayer.Center.X, Main.LocalPlayer.position.Y - 14);
            GetAmountOfCountersPlayerHas(out int amtOfCounters, out List<int> counterIDs);

            const int distance = 14;
            for (int i = 0; i < counterIDs.Count; i++)
            {
                int counterID = counterIDs[i];

                if (counterID == Projectile.whoAmI)
                {
                    pos.X += i * distance;

                    int totalWidth = distance * counterIDs.Count;
                    pos.X -= totalWidth / 2;

                    pos.X += distance / 2;
                }
            }

            pos.Y += Main.LocalPlayer.gfxOffY;
            Projectile.Center = pos;
        }

        /// <summary>
        /// Returns the amount of counters the player has for the specified minion counter.
        /// </summary>
        /// <returns></returns>
        public void GetAmountOfCountersPlayerHas(out int amtOfCounters, out List<int> counterIDs)
        {
            amtOfCounters = 0;
            counterIDs = new List<int>();
            
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.owner == Main.myPlayer && proj.type == Projectile.type)
                {
                    amtOfCounters++;
                    counterIDs.Add(proj.whoAmI);
                }
            }
        }

        public override bool? CanDamage() => false;

        public override bool? CanCutTiles() => false;
    }
}
