using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityHunt.Common.Systems;

public sealed class BossDownedSystem : ModSystem
{
    public const string KeyPrefix = "downedBoss";
    public const string GoozmaKey = "Goozma";

    public static BossDownedSystem Instance => ModContent.GetInstance<BossDownedSystem>();

    public bool GoozmaDowned
    {
        get => downedBoss[GoozmaKey];
        set => downedBoss[GoozmaKey] = value;
    }

    private readonly Dictionary<string, bool> downedBoss = new()
    {
        { GoozmaKey, false },
    };

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write(GoozmaDowned);
        base.NetSend(writer);
    }

    public override void NetReceive(BinaryReader reader)
    {
        GoozmaDowned = reader.ReadBoolean();
        base.NetReceive(reader);
    }

    public override void SaveWorldData(TagCompound tag)
    {
        foreach (string entry in downedBoss.Keys) {
            tag[KeyPrefix + entry] = downedBoss[entry];
        }
    }

    public override void LoadWorldData(TagCompound tag)
    {
        foreach (string entry in downedBoss.Keys) {
            downedBoss[entry] = tag.GetBool(KeyPrefix + entry);
        }
    }
}
