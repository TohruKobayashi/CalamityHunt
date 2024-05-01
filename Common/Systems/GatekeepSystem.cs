using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems;

public sealed class GatekeepSystem : ModSystem
{
    public bool Undercover { get; private set; }

    public override void OnModLoad()
    {
        base.OnModLoad();

        Undercover = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space);
        if (Undercover)
            SoundEngine.PlaySound(SoundID.Unlock);
    }
}
