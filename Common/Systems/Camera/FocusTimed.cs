using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace CalamityHunt.Common.Systems.Camera;

public class FocusTimed : ICameraModifier
{
    public FocusTimed(int riseTime, int waitTime, int fallTime, string identity = null)
    {
        id = identity;
        this.riseTime = riseTime;
        this.waitTime = waitTime;
        this.fallTime = fallTime;
    }

    private string id;
    private int riseTime;
    private int waitTime;
    private int fallTime;
    private int time;

    public static Vector2 focusTarget;

    public string UniqueIdentity => id;

    public bool Finished => time > riseTime + waitTime + fallTime;

    public void Update(ref CameraInfo cameraPosition)
    {
        if (!Main.gamePaused) {
            time++;
        }

        float totalTime = Utils.GetLerpValue(0, riseTime, time, true) * Utils.GetLerpValue(riseTime + waitTime + fallTime, riseTime + waitTime, time, true);
        Vector2 newPosition = Vector2.Lerp(cameraPosition.CameraPosition, focusTarget - Main.ScreenSize.ToVector2() / 2f, (float)MathHelper.SmoothStep(0, 1, totalTime));
        cameraPosition.CameraPosition = newPosition;
    }
}
