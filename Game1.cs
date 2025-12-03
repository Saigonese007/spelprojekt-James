using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace spelprojekt_James
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D bird, birddown, birdup, background, pipedown, pipeup, deathbase;

        Rectangle birdrec = new Rectangle(325 / 2, 960 / 2, 60, 60);

        KeyboardState keyinput;
        KeyboardState oldkeyinput;

        Rectangle backgroundrec = new Rectangle(0, 0, 540, 960);

        SpriteFont Font;
        string displaytext;
        int score = 0;
        Vector2 scorePosition = new Vector2(400 / 2, 100);
        string gameoverText = "Game Over";

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // screen ratio 9:16 (540x960)
            _graphics.PreferredBackBufferWidth = 540;   
            _graphics.PreferredBackBufferHeight = 960;  
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Texture2D>("background-day");
            birdup = Content.Load<Texture2D>("yellowbird-upflap");
            birddown = Content.Load<Texture2D>("yellowbird-downflap");
            bird = Content.Load<Texture2D>("yellowbird-midflap");


            Font = Content.Load<SpriteFont>("Font");

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();



            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(background, backgroundrec, Color.White);
            _spriteBatch.Draw(bird, birdrec, Color.White);
            _spriteBatch.DrawString(Font, displaytext, scorePosition, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
