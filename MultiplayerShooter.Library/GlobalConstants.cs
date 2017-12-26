namespace MultiplayerShooter.Library
{
    public class GlobalConstants
    {
        //--------------------------------------------------
        // Networking

        public const string APPNAME = "multiplayerShooter";
        public const string HOSTNAME = "localhost";
        public const int PORT = 14242;
        
        //--------------------------------------------------
        // Game
        public const int MAX_PLAYERS = 5;
        
        //--------------------------------------------------
        // Map Tags

        public const int PLAYER_TAG = 1;
        public const int PROJECTILES_TAG = 1;

        //--------------------------------------------------
        // Map Render Layers Constants

        public const int BACKGROUND_RENDER_LAYER = 10;
        public const int TILED_MAP_RENDER_LAYER = 9;
        public const int WATER_RENDER_LAYER = 6;
        public const int MISC_RENDER_LAYER = 5;
        public const int ENEMIES_RENDER_LAYER = 4;
        public const int PLAYER_RENDER_LAYER = 3;
        public const int PARTICLES_RENDER_LAYER = 2;
        public const int HUD_BACK_RENDER_LAYER = 1;
        public const int HUD_FILL_RENDER_LAYER = 0;

        //--------------------------------------------------
        // Layer Masks

        public const int PLAYER_LAYER = 1;
        public const int ENEMY_LAYER = 2;
        public const int PROJECTILES_LAYER = 3;
    }
}
