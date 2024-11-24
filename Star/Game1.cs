using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Threading;

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
            _tree = new(1, 4);

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

            if (_fraemCount == 10000) Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            var mean = _molecularCloud.Points.Aggregate((p1,p2) => p1 + p2);
            mean /= _molecularCloud.Points.Count();

            var sd = _molecularCloud.Points.
                Select(p => (p - mean) * (p - mean)).
                Aggregate((p1, p2) => p1 + p2).
                Div(_molecularCloud.Points.Count()).
                Sqrt();

            var maxSd = Math.Max(sd.X, Math.Max(sd.Y, sd.Z));
            
            var centralPoints = 
                _molecularCloud.Points.
                OrderBy(p => p.Y).
                Skip((int)(_molecularCloud.Points.Count() * 0.1)).
                Take((int)(_molecularCloud.Points.Count() * 0.8));

            var range = centralPoints.First().Y - centralPoints.Last().Y;

            var scale = 500 / range;

            mean = centralPoints.Aggregate((p1, p2) => p1 + p2);
            mean /= centralPoints.Count();

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            foreach(var particle in _molecularCloud.Points)
            {
                _spriteBatch.Draw(_pointTexture, new Rectangle((int)((particle.X - mean.X) * scale) + 1000, (int)((particle.Y  - mean.Y) * scale) + 500, 1, 1), Color.Red);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}