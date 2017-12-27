namespace MultiplayerShooter.Library.Networking
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
