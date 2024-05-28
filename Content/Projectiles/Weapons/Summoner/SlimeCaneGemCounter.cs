using CalamityHunt.Common.Players;
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
        }
        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 60;
            Projectile.aiStyle = 164;
            Projectile.hide = true;
        }


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.GetModPlayer<SlimeCanePlayer>().slimes) {
                Projectile.timeLeft = 2;
            }

            if (++Projectile.frameCounter >= 4) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6) {
                    Projectile.frame = 0;
                }
            }
        }

        public override bool? CanDamage() => false;

        public override bool? CanCutTiles() => false;
    }
}
