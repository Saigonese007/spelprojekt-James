using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace spelprojekt_James
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D bird, birddown, birdup, background, pipedown, pipeup, deathbase, background2, birdmid, Startknappnonhover, titel, startknapphover, startKnapp;

        Rectangle birdrec = new Rectangle(325 / 2, 960 / 2, 60, 60);

        KeyboardState keyinput;
        KeyboardState oldkeyinput;

        MouseState mouseinput;

        Rectangle backgroundrec = new Rectangle(0, 0, 540, 960);
        Rectangle backgroundrec2 = new Rectangle(540, 0, 540, 960);
        Rectangle deathbaserec = new Rectangle(0, 960 - 200, 540, 200); // 960 - 200 pga line up med bottom 

        //Text
        SpriteFont Font;
        int score = 0;
        Vector2 scorePosition = new Vector2(540 / 2 - 20, 100);
        Texture2D gameOverBild;
        Rectangle gameOverBildPos;
        Texture2D RestartText;
        Rectangle RestartTextPos;

        //Gravitation
        //float istället för vector2 pga bara Y-led ska röra sig
        float gravity = 0.4f; // acceleration nedåt varje frame (basically som tyngdkraft)
        float velocity = 0; //hur snabbt fågeln rör sig upp eller ner (0 pga att den står stilla när spelet börjar)
        float flapStyrka = -6f; // hur högt/starkt fågeln ska hoppa när man trycker på space (-6 kändes lagom högt)

        // max heights
        int MaxHeightBottom;// deathbase ska döda
        int MaxHeightTop = 0;

        //single parralax background
        int BackgroundSpeed = -2;

        //Meny
        string gameState = "menu"; // möjliga värden: "menu", "playing", "gameover"
        Rectangle StartknappRec, titelrec;

        ///Pipes
        List<Rectangle> topPipes = new List<Rectangle>();
        List<Rectangle> bottomPipes = new List<Rectangle>();

        int pipeWidth = 80;  // pipe är 52 i bredd men det är för litet
        int pipeHeight = 320;  // pipe är 320 i höjd
        int pipeGap = 200; // mellanrum mellan top och bottom pipe
        int pipeSpeed; // ska vara samma som backgroundspeed
        int pipeSpawn = 150; // frames för när varje ny pipe ska komma
        int pipeTimer = 0; // ska kolla varje gång pipeSpawn ska köra

        Random rnd = new Random(); // random variabel
 

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

            MaxHeightBottom = deathbaserec.Y - birdrec.Height;

            pipeSpeed = BackgroundSpeed;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            background2 = Content.Load<Texture2D>("background-day2");
            background = Content.Load<Texture2D>("background-day");
            birdup = Content.Load<Texture2D>("yellowbird-upflap");
            birddown = Content.Load<Texture2D>("yellowbird-downflap");
            birdmid = Content.Load<Texture2D>("yellowbird-midflap");
            bird = birdmid;

            deathbase = Content.Load<Texture2D>("base");
            Font = Content.Load<SpriteFont>("Font");

            gameOverBild = Content.Load<Texture2D>("gameoverResized");
            gameOverBildPos = new Rectangle(540 / 2 - gameOverBild.Width / 2, 960 / 2 - gameOverBild.Height / 2, gameOverBild.Width, gameOverBild.Height);

            Startknappnonhover = Content.Load<Texture2D>("FlappyBirdStartImg");
            titel = Content.Load<Texture2D>("FlappyBirdTitle");
            titelrec = new Rectangle(540 / 2 - 200, 50, titel.Width, titel.Height);

            RestartText = Content.Load<Texture2D>("RestartText");
            RestartTextPos = new Rectangle(540 / 2 - (RestartText.Width / 4), 300, RestartText.Width / 2, RestartText.Height / 2);

            StartknappRec = new Rectangle(540 / 2 - 125, 450, Startknappnonhover.Width, Startknappnonhover.Height);
            startknapphover = Content.Load<Texture2D>("startKnappHover");
            
            //pipes
            pipeup = Content.Load<Texture2D>("pipe-green");
            pipedown = Content.Load<Texture2D>("pipe-green-upsidedown");

        }


        protected override void Update(GameTime gameTime)
        {

            oldkeyinput = keyinput;
            keyinput = Keyboard.GetState();

            mouseinput = Mouse.GetState();

            //Meny 

            if (gameState == "menu")
            {
                // kolla om vänster musknapp klickas på play-knappen
                if (mouseinput.LeftButton == ButtonState.Pressed && new Rectangle(mouseinput.X, mouseinput.Y, 1, 1).Intersects(StartknappRec))
                {
                    velocity = 0;
                    birdrec.Y = 960/2;
                    score = 0;
                    gameState = "playing";
                }

                if (keyinput.IsKeyDown(Keys.Space) && oldkeyinput.IsKeyUp(Keys.Space))
                {
                    // starta spelet
                    velocity = 0;
                    birdrec.Y = 960 / 2 ;
                    score = 0;

                    gameState = "playing"; // byt läge till playing
                }

                //hovering (behöver fix)
                if (StartknappRec.Contains(mouseinput.X, mouseinput.Y))
                {
                    startKnapp = startknapphover;
                }
                else
                {
                    startKnapp = Startknappnonhover;
                }

                return; //stopp här
            }

            //gamestate == playing
            if (gameState == "playing")
            {
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
                    bird = birdmid; // natural position dvs mid
                }

                // Hamnar man utanför = GameOver
                if (birdrec.Y <= MaxHeightTop || birdrec.Y >= MaxHeightBottom)
                {
                    gameState = "gameover";
                    score = 0; //reset score
                    velocity = 0;

                }
            }

            // Background - flytta båda bakgrundsbilderna åt vänster varje frame
            if (gameState == "playing")
            {
                backgroundrec.X += BackgroundSpeed;
                backgroundrec2.X += BackgroundSpeed;

                if (backgroundrec.X <= -540) // om första bakgrunden har åkt helt ut till vänster, flytta den till höger om den andra, 540 px pga backgroundwidth då själva bilden är height: 512 px width: 288 px

                {
                    backgroundrec.X = backgroundrec2.X + 540;
                }
                if (backgroundrec2.X <= -540) // om andra bakgrunden har åkt helt ut till vänster, flytta den till höger till den första

                {
                    backgroundrec2.X = backgroundrec.X + 540;
                }
            }
            //Pipes (inte klar)
            if (gameState == "playing")
            {
                pipeTimer++; // pipetimer ska ökas hela tiden för att kunna räkna när pipeSpawn ska spawna pipes

                if (pipeTimer >= pipeSpawn)
                {
                    pipeTimer = 0; //reset pipetimer så att ny pipespawn kan komma efter när den har ökas till 150


                }
            }
            
            // Gamestate = gameover
            if (gameState == "gameover")
            {
                if (keyinput.IsKeyDown(Keys.Enter) && oldkeyinput.IsKeyUp(Keys.Enter))
                {
                    birdrec.Y = 960 / 2;
                    velocity = 0;
                    gameState = "playing";
                }
            }

         

            


                base.Update(gameTime);
        }
        

        protected override void Draw(GameTime gameTime)
        {

            _spriteBatch.Begin();
            if (gameState == "menu")
            {
                _spriteBatch.Draw(titel, titelrec, Color.White); // titelbild
                _spriteBatch.Draw(startKnapp, StartknappRec, Color.White); // play-knapp

                _spriteBatch.End();
                return;

            }
            _spriteBatch.Draw(background, backgroundrec, Color.White);
            _spriteBatch.Draw(background2, backgroundrec2, Color.White);
            _spriteBatch.Draw(deathbase, deathbaserec, Color.White);

            _spriteBatch.Draw(bird, birdrec, Color.White);
            _spriteBatch.DrawString(Font, score.ToString(), scorePosition, Color.White);

            if (gameState == "gameover")
            {
                _spriteBatch.Draw(gameOverBild,gameOverBildPos,Color.White );
                _spriteBatch.Draw(RestartText, RestartTextPos, Color.Black);
            }
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
