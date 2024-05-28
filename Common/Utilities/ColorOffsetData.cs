namespace CalamityHunt.Common.Utilities;

public struct ColorOffsetData
{
    public ColorOffsetData(bool active = false, float offset = 0f)
    {
        this.active = active;
        this.offset = offset;
    }

    public bool active;
    public float offset;
}
