namespace MultiplayerShooter.Library.Projectiles
{
    public class ProjectileData
    {
        public int Id { get; set; }
        public byte FromPlayerId { get; set; }
        public ProjectileType Type { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
    }
}
