using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Star
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _pointTexture;
        private GiantMolecularCloud _molecularCloud;
        private Tree _tree;
        private int _fraemCount;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            var height = _graphics.GraphicsDevice.DisplayMode.Height;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.DisplayMode.Width;
            //_graphics.ToggleFullScreen();
            _graphics.ApplyChanges();

            _molecularCloud = new(height);
            _tree = new(3, 4, _molecularCloud.DiameterInLightYears);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pointTexture = Texture2D.FromFile(GraphicsDevice, "Content/blank.png");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            Physics.Step(_molecularCloud.Particles, _tree);
            _fraemCount++;

            if (_fraemCount == 100) Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            foreach(var particle in _molecularCloud.Points)
            {
                _spriteBatch.Draw(_pointTexture, new Rectangle((int)particle.X, (int)particle.Y, 1, 1), Color.Red);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}