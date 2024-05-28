using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityHunt.Common.Players;

public class AuricSoulPlayer : ModPlayer
{
    public bool goozmaSoul;
    public bool yharonSoul;

    public override void PostUpdate()
    {
        if (goozmaSoul) {
            Player.GetDamage(DamageClass.Generic) += 0.03f;
            Player.GetCritChance(DamageClass.Generic) += 0.03f;
            Player.GetKnockback(DamageClass.Summon) += 0.03f;
            Player.moveSpeed += 0.03f;
            Player.GetAttackSpeed(DamageClass.Melee) += 0.03f;
            Player.endurance += 0.01f;
            Player.statDefense += 3;
        }
        if (yharonSoul) {
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
        if (goozmaSoul) {
            Player.statLifeMax2 += 10;
            Player.statManaMax2 += 20;
        }
        if (yharonSoul) {
            Player.statLifeMax2 += 10;
            Player.statManaMax2 += 20;
        }
    }

    public override void SaveData(TagCompound tag)
    {
        if (goozmaSoul) {
            tag["goozmaSoul"] = true;
        }

        if (yharonSoul) {
            tag["yharonSoul"] = true;
        }
        base.SaveData(tag);
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.ContainsKey("goozmaSoul")) {
            goozmaSoul = tag.GetBool("goozmaSoul");
        }
        if (tag.ContainsKey("yharonSoul")) {
            yharonSoul = tag.GetBool("yharonSoul");
        }
        base.LoadData(tag);
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)CalamityHunt.PacketType.SyncPlayer);
        packet.Write((byte)Player.whoAmI);
        packet.Write(goozmaSoul);
        packet.Write(yharonSoul);
        packet.Send(toWho, fromWho);
    }

    // Called in ExampleMod.Networking.cs
    public void ReceivePlayerSync(BinaryReader reader)
    {
        goozmaSoul = reader.ReadBoolean();
        yharonSoul = reader.ReadBoolean();
    }

    public override void CopyClientState(ModPlayer targetCopy)
    {
        AuricSoulPlayer clone = (AuricSoulPlayer)targetCopy;
        clone.goozmaSoul = goozmaSoul;
        clone.yharonSoul = yharonSoul;
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        AuricSoulPlayer clone = (AuricSoulPlayer)clientPlayer;

        if (goozmaSoul != clone.goozmaSoul || yharonSoul != clone.yharonSoul)
            SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
    }
}
