using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.ComponentModel.DataAnnotations;

namespace spelprojekt_James
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D bird, birddown, birdup, background, pipedown, pipeup, deathbase, background2;

        Rectangle birdrec = new Rectangle(325 / 2, 960 / 2, 60, 60);

        KeyboardState keyinput;
        KeyboardState oldkeyinput;

        Rectangle backgroundrec = new Rectangle(0, 0, 540, 960);
        Rectangle backgroundrec2 = new Rectangle(0, 0, 540, 0);
        Rectangle deathbaserec = new Rectangle(0, 960 - 200, 540, 200); // 960 - 200 pga line up med bottom 

        //Text
        SpriteFont Font;
        string displaytext; //ska ändras
        int score = 0;
        Vector2 scorePosition = new Vector2(540 / 2, 100);
        string gameoverText = "Game Over";

        //Gravitation
        //float istället för vector2 pga bara Y-led ska röra sig
        float gravity = 0.4f; // acceleration nedåt varje frame
        float velocity = 0; //hur snabbt fågeln rör sig upp eller ner (0 pga att den står stilla när spelet börjar)
        float flapStyrka = -6f; // hur högt/starkt fågeln ska hoppa när man trycker på space (-6 kändes lagom högt)


        // max heights
        int MaxHeightBottom = 960 - 225; // deathbase ska döda
        int MaxHeightTop = 0;

        //Gamover
        bool GameOver = false;

        //single parralax background
        int ParralaxBackgroundSpeed = -5;


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

            background2 = Content.Load<Texture2D>("background-day2");
            background = Content.Load<Texture2D>("background-day");
            birdup = Content.Load<Texture2D>("yellowbird-upflap");
            birddown = Content.Load<Texture2D>("yellowbird-downflap");
            bird = Content.Load<Texture2D>("yellowbird-midflap");

            deathbase = Content.Load<Texture2D>("base");


            Font = Content.Load<SpriteFont>("Font");

        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            oldkeyinput = keyinput;
            keyinput = Keyboard.GetState();

            //junping mech
            if (keyinput.IsKeyDown(Keys.Space) && oldkeyinput.IsKeyUp(Keys.Space))
            {
                velocity = flapStyrka; // skickar velocity till flapStyrka så att fågeln hoppar upp

            }

            velocity += gravity; // om man inte hoppar så åker man neråt med gravitationen

            birdrec.Y += (int)velocity; // flyttar fågeln till antingen gravitation eller upp med flapStryka, omvandla float till int

            //animation
            if (velocity < 0)
            {
                bird = birdup; // FlapUp när Velocity är större än 0
            }
            else if (velocity > 0)
            {
                bird = birddown; //Vingar ner när velocity är mindre än 0
            }
            else
            {
                bird = bird; // natural position
            }

            // Hamnar man utanför = GameOver
            if (birdrec.Y <= MaxHeightTop || birdrec.Y >= MaxHeightBottom)
            {
                GameOver = true;
                score = 0; //reset score
                velocity = 0;
            }

            //GameOver Actions
            if (GameOver)
            {
                displaytext = gameoverText;
            }
            else
            {
                displaytext = score.ToString();
            }

            //parallax background
            backgroundrec.X += ParralaxBackgroundSpeed;
            backgroundrec.X %= background.Width;

            if (backgroundrec.X >= 0)
            {
                backgroundrec2.X = backgroundrec2.X - background.Width;
            }
            else
            {
                backgroundrec2.X = backgroundrec2.X + background.Width;
            }


            base.Update(gameTime);
        }
        

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(background, backgroundrec, Color.White);
            _spriteBatch.Draw(background2, backgroundrec, Color.White);

            _spriteBatch.Draw(deathbase, deathbaserec, Color.White);
            _spriteBatch.Draw(bird, birdrec, Color.White);
            _spriteBatch.DrawString(Font, displaytext, scorePosition, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
