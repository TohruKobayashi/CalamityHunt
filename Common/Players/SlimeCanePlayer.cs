using CalamityHunt.Content.Buffs;
using CalamityHunt.Content.Projectiles.Weapons.Summoner;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Players
{
    public class SlimeCanePlayer : ModPlayer
    {
        public bool slimes;

        public int highestOriginalDamage;

        public int SlimeRank()
        {
            int num = 0;

            int count = Player.ownedProjectileCounts[ModContent.ProjectileType<SlimeCaneGemCounter>()];

            if (count > 1)
                num = 1;            
            if (count > 2)
                num = 2;
            if (count > 3)
                num = 3;
            if (count > 4)
                num = HighestRank;

            return num;
        }

        public static readonly int HighestRank = 4;

        public int ValueFromSlimeRank(params int[] values)
        {
            if (values.Length != 5)
                return -1;

            return values[SlimeRank()];
        }

        public void SetSlimes()
        {
            for (int i = 0; i < 4; i++)
            {
                int slimeType;
                switch (i)
                {
                    case 1:
                        slimeType = ModContent.ProjectileType<EbonianBlinky>();
                        break;
                    case 2:
                        slimeType = ModContent.ProjectileType<DivinePinky>();
                        break;
                    case 3:
                        slimeType = ModContent.ProjectileType<StellarInky>();
                        break;
                    default:
                        slimeType = ModContent.ProjectileType<CrimulanClyde>();
                        break;
                }
                if (Player.ownedProjectileCounts[slimeType] <= 0)
                {
                    int p = Projectile.NewProjectile(Player.GetSource_FromThis(), Player.position.X - (10 * i), Player.position.Y, 0, 0, slimeType, highestOriginalDamage, 0, Player.whoAmI);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = highestOriginalDamage;
                }
            }

            int goozType = ModContent.ProjectileType<Goozmoem>();
            if (SlimeRank() >= HighestRank && Player.ownedProjectileCounts[goozType] <= 0)
            {
                int p = Projectile.NewProjectile(Player.GetSource_FromThis(), Player.position.X - 20, Player.position.Y, 0, 0, goozType, highestOriginalDamage, 0, Player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = highestOriginalDamage;
            }
        }

        public override void PostUpdateBuffs()
        {
            if (Player.HasBuff<SlimeCaneBuff>())
            {
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<SlimeCaneGemCounter>()] > 0)
                {
                    highestOriginalDamage = 0;
                    slimes = true;
                    int count = 0;

                    foreach (Projectile gem in Main.projectile.Where(n => n.owner == Player.whoAmI && n.type == ModContent.ProjectileType<SlimeCaneGemCounter>()))
                    {
                        count++;

                        if (highestOriginalDamage < gem.originalDamage)
                            highestOriginalDamage = gem.originalDamage;

                        if (count > 5)
                            gem.Kill();
                    }


                }

                int buffIndex = Player.FindBuffIndex(ModContent.BuffType<SlimeCaneBuff>());
                if (!slimes)
                    Player.DelBuff(buffIndex);
                else
                    Player.buffTime[buffIndex] = 18000;

                if (Player.whoAmI == Main.myPlayer)
                    SetSlimes();

            }
        }

        public override void ResetEffects()
        {
            slimes = false;
        }
    }
}
