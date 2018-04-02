class HeaderDecoder
{
    public static int GetPlayerCount(byte header)
    {
        return (0x1F & header);
    }

    public static bool IsTickPacket(byte header)
    {
        return (0x80 & header) > 0;
    }

    public static bool HasBullet(byte header)
    {
        return (0x40 & header) > 0;
    }

    public static bool HasWeapon(byte header)
    {
        return (0x20 & header) > 0;
    }
}
