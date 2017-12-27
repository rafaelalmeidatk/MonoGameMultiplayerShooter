using Microsoft.Xna.Framework;
using MultiplayerShooter.Client.Managers;
using MultiplayerShooter.Client.Scenes;
using Nez;
using Nez.BitmapFonts;

namespace MultiplayerShooter.Client
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameMain : Core
    {
        public static BitmapFont bigBitmapFont;
        public static BitmapFont smallBitmapFont;

        public GameMain() : base(width: 854, height: 480, isFullScreen: false, enableEntitySystems: true, windowTitle: "Multiplayer Shooter")
        {
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            debugRenderEnabled = true;
            pauseOnFocusLost = false;

            IsFixedTimeStep = true;

            // Register Global Managers
            registerGlobalManager(new InputManager());
            registerGlobalManager(new NetworkManager());
        }

        protected override void LoadContent()
        {
            bigBitmapFont = content.Load<BitmapFont>(Client.Content.Fonts.titleFont);
            smallBitmapFont = content.Load<BitmapFont>(Client.Content.Fonts.smallFont);
        }

        protected override void Initialize()
        {
            base.Initialize();
            Scene.setDefaultDesignResolution(427, 240, Scene.SceneResolutionPolicy.FixedHeight);

            // PP Fix
            scene = Scene.createWithDefaultRenderer();
            base.Update(new GameTime());
            base.Draw(new GameTime());

            // Set first scene
            scene = new SceneMap();
        }
    }
}
