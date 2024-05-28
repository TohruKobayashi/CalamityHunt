using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityHunt.Common.Players
{
    public class AuricSoulPlayer : ModPlayer
    {
        public bool pureSoul;
        public bool goozmaSoul;
        public bool yharonSoul;

        public override void SaveData(TagCompound tag)
        {
            tag["HuntOfTheOldGod:PureSoul"] = pureSoul;
            tag["HuntOfTheOldGod:GoozmaSoul"] = goozmaSoul;
            tag["HuntOfTheOldGod:YharonSoul"] = yharonSoul;
        }

        public override void LoadData(TagCompound tag)
        {
            pureSoul = tag.GetBool("HuntOfTheOldGod:PureSoul");
            goozmaSoul = tag.GetBool("HuntOfTheOldGod:GoozmaSoul");
            yharonSoul = tag.GetBool("HuntOfTheOldGod:YharonSoul");
        }

        public override void PostUpdate()
        {
            if (goozmaSoul)
            {
                Player.GetDamage(DamageClass.Generic) += 0.03f;
                Player.GetCritChance(DamageClass.Generic) += 0.03f;
                Player.GetKnockback(DamageClass.Summon) += 0.03f;
                Player.moveSpeed += 0.03f;
                Player.GetAttackSpeed(DamageClass.Melee) += 0.03f;
                Player.endurance += 0.01f;
                Player.statDefense += 3;
            }
            if (yharonSoul)
            {
                Player.GetDamage(DamageClass.Generic) += 0.03f;
                Player.GetCritChance(DamageClass.Generic) += 0.03f;
                Player.GetKnockback(DamageClass.Summon) += 0.03f;
                Player.moveSpeed += 0.03f;
                Player.GetAttackSpeed(DamageClass.Melee) += 0.03f;
                Player.endurance += 0.01f;
                Player.statDefense += 3;
            }
        }

        public override void ResetEffects()
        {
            if (goozmaSoul)
            {
                Player.statLifeMax2 += 10;
                Player.statManaMax2 += 20;
            }
            if (yharonSoul)
            {
                Player.statLifeMax2 += 10;
                Player.statManaMax2 += 20;
            }
        }
    }
}
