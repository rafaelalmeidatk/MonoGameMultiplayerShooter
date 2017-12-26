namespace MultiplayerShooter.Library
{
    public enum PacketType
    {
        /* Player Login*/

        Login,

        /* Players */
        NewPlayer,
        AllPlayers,
        PlayerPosition,
        UpdatePlayerPosition,

        /* Projectile */
        CreateProjectile
    }
}
