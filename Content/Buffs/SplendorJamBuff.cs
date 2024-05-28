using CalamityHunt.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Buffs
{
    public class SplendorJamBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.GetModPlayer<SplendorJamPlayer>().stressedOut) {
                float checkStress = player.GetModPlayer<SplendorJamPlayer>().checkStress;
                if (checkStress >= 0.5 && checkStress < 0.75) {
                    player.GetDamage(DamageClass.Generic) += 0.7f;
                    player.GetDamage(DamageClass.Generic).Flat += 12f;
                    player.GetCritChance(DamageClass.Generic) += 3f;
                    player.GetArmorPenetration(DamageClass.Generic) += 10f;
                }
                else if (checkStress >= 0.75 && checkStress < 1) {
                    player.GetDamage(DamageClass.Generic) += 0.16f;
                    player.GetDamage(DamageClass.Generic).Flat += 16f;
                    player.GetCritChance(DamageClass.Generic) += 7f;
                    player.GetArmorPenetration(DamageClass.Generic) += 20f;

                }
                else if (checkStress >= 1) {
                    player.GetDamage(DamageClass.Generic) += 0.35f;
                    player.GetDamage(DamageClass.Generic).Flat += 20f;
                    player.GetCritChance(DamageClass.Generic) += 10f;
                    player.GetArmorPenetration(DamageClass.Generic) += 40f;
                }
                else {
                    player.GetDamage(DamageClass.Generic) += 0.02f;
                    player.GetDamage(DamageClass.Generic).Flat += 8f;
                    player.GetCritChance(DamageClass.Generic) += 1f;
                    player.GetArmorPenetration(DamageClass.Generic) += 5f;
                }
            }
            else {
                player.DelBuff(buffIndex);
            }
        }
    }
}
