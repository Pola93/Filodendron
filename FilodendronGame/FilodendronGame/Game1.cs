using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
namespace FilodendronGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public GeneralModelManager modelManager { get; protected set; }
        public Camera camera { get; protected set; }
        public Random rnd { get; protected set; }

        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public enum GameState
        {
            MainMenu,
            Options,
            Playing,
            Intro,
        };
        public enum SoundState
        {
            Paused,
            Playing,
            Stopped,
        }
        public GameState currentGameState = GameState.MainMenu;
        //Screen adjustment
        int screenWidth = 1366, screenHeight = 768;
        public int numberOfLifes = 1;
        public int numberOfCoins = 0;
        private int s = 1;
        private MouseState old;
        cButton cBtnPlay;
        cButton cBtnOptions;
        cButton cBtnQuit;
        cButton cBtnBack;
        cButton cBtnGameOver;
        cButton cBtnnext;
        cButton cBtnprev;
        cButton cBtnskip;
        SoundEffect soundBackground;
        SoundEffectInstance soundBackgroundInstance;
        // Don't like warnings
        // If you're not using variable right now - comment it
        // SoundEffect soundHyperspaceActivation;
        // SoundEffectInstance soundHyperspaceActivationInstance;
        SpriteFont font;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // Initialize model manager
            modelManager = new GeneralModelManager(this);
            Components.Add(modelManager);

            // Initialize Camera

            camera = new Camera(this);
            Components.Add(camera);

            rnd = new Random();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            IsMouseVisible = true;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            cBtnPlay = new cButton(Content.Load<Texture2D>(@"textures/startButton"), graphics.GraphicsDevice);
            cBtnPlay.setPosition(new Vector2(screenWidth/2 - 100, screenHeight /2 ));
            cBtnOptions = new cButton(Content.Load<Texture2D>(@"textures/optionsButton"), graphics.GraphicsDevice);
            cBtnOptions.setPosition(new Vector2(screenWidth / 2 - 100, screenHeight * 2 / 3));
            cBtnQuit = new cButton(Content.Load<Texture2D>(@"textures/exitButton"), graphics.GraphicsDevice);
            cBtnQuit.setPosition(new Vector2(screenWidth / 2 - 100, screenHeight * 5 / 6));
            cBtnBack = new cButton(Content.Load<Texture2D>(@"textures/backButton"), graphics.GraphicsDevice);
            cBtnBack.setPosition(new Vector2(screenWidth / 2 - 100, screenHeight * 5 / 6));
            cBtnGameOver = new cButton(Content.Load<Texture2D>(@"textures/przegrana"), graphics.GraphicsDevice);
            cBtnGameOver.setPosition(new Vector2(screenWidth / 2 - 100, screenHeight / 2));
            cBtnnext = new cButton(Content.Load<Texture2D>(@"textures/next"), graphics.GraphicsDevice);
            cBtnnext.setPosition(new Vector2(screenWidth - 175, screenHeight/2 - 100));
            cBtnprev = new cButton(Content.Load<Texture2D>(@"textures/prev"), graphics.GraphicsDevice);
            cBtnprev.setPosition(new Vector2(screenWidth - 175, screenHeight/2 + 100));
            cBtnskip = new cButton(Content.Load<Texture2D>(@"textures/skip"), graphics.GraphicsDevice);
            cBtnskip.setPosition(new Vector2(screenWidth - 175, screenHeight/2));

            soundBackground = Content.Load<SoundEffect>("Audio\\Waves\\ZombiePlant");
            soundBackgroundInstance = soundBackground.CreateInstance();
            // soundHyperspaceActivation = Content.Load<SoundEffect>("Audio\\Waves\\hyperspace_activate");
            //soundHyperspaceActivationInstance = soundHyperspaceActivation.CreateInstance();
            font = Content.Load<SpriteFont>(@"textures/myFont");
            //Settings.musicVolume = 0.75f;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            //if (cBtnQuit.isClicked == true)
            //{
            //    this.Exit();
            //}
       /*     int counter = 1;
            int limit = 5;
            float countDuration = 2f; //every  2s.
            float currentTime = 0f;

            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds; //Time passed since last Update
            if (currentTime >= countDuration)
            {
                counter++;
                currentTime -= countDuration; // "use up" the time
                //any actions to perform
            }
            if (counter >= limit)
            {
                counter = 0;//Reset the counter;
                s++;
                   
                //any actions to perform
            }*/
            
            MouseState mouse = Mouse.GetState();
            old = mouse;
            switch (currentGameState)
            {
                case GameState.MainMenu:
                    //  soundBackgroundInstance.IsLooped = true;
                    soundBackgroundInstance.Play();
                    
                    if (cBtnPlay.isClicked == true)
                    {
                        currentGameState = GameState.Intro;
                    }
                    
                    cBtnPlay.update(mouse);

                    if (cBtnOptions.isClicked == true)
                    {
                        currentGameState = GameState.Options;
                    }

                    cBtnOptions.update(mouse);
                    
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                       this.Exit();
                    }
                    
                    cBtnQuit.update(mouse);

                    break;

                case GameState.Playing:
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        currentGameState = GameState.MainMenu;
                    }
                   // if (Keyboard.GetState().IsKeyDown(Keys.Up)&& numberOfLifes <5)
                    if(numberOfCoins == 5)
                    {
                            numberOfLifes++;
                            numberOfCoins = 0;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    {
                        numberOfLifes = 0;

                    }
                    if(cBtnGameOver.isClicked == true)
                    {
                        this.Exit();
                    }
                    cBtnGameOver.update(mouse);
                    break;
                
                case GameState.Options:
                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {
                        if (soundBackgroundInstance.Volume < 0.99f)
                        {
                            soundBackgroundInstance.Volume += 0.01f;
                        }
                        else
                        {
                            soundBackgroundInstance.Volume = 1.0f;
                        }
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    {
                        if (soundBackgroundInstance.Volume > 0.01f)
                        {
                            soundBackgroundInstance.Volume -= 0.01f;
                        }
                        else
                        {
                            soundBackgroundInstance.Volume = 0.0f;
                        }
                    }
                    
                    if (cBtnBack.isClicked == true)
                    {
                        currentGameState = GameState.MainMenu;
                    }
                    
                    cBtnBack.update(mouse);
                    break;
                case GameState.Intro:
                    if (cBtnskip.isClicked == true)
                    {
                        currentGameState = GameState.Playing;
                    }
                    cBtnskip.update(mouse);
                    if(cBtnnext.isClicked == true && old.LeftButton == ButtonState.Released)
                    {
                        if (s < 5)
                            s++;
                        else if (s == 5)
                            currentGameState = GameState.Playing;
                    }
                    cBtnnext.update(mouse);
                    if (cBtnprev.isClicked == true && old.LeftButton == ButtonState.Released)
                    {
                        if (s > 1)
                            s--;
                    }
                    cBtnprev.update(mouse);


                    break;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
           
            graphics.GraphicsDevice.Clear(Color.SteelBlue);
            base.Draw(gameTime);
            spriteBatch.Begin();
            
            switch (currentGameState)
            {
                case GameState.Intro:
                    if(s == 1)
                    spriteBatch.Draw(Content.Load<Texture2D>(@"textures/filoden1f"), new Rectangle(screenWidth/2-363, 0, 726, screenHeight), Color.White);
                    else if(s == 2)
                        spriteBatch.Draw(Content.Load<Texture2D>(@"textures/filoden2f"), new Rectangle(screenWidth / 2 - 363, 0, 726, screenHeight), Color.White);
                    else if (s==3)
                        spriteBatch.Draw(Content.Load<Texture2D>(@"textures/filoden3f"), new Rectangle(screenWidth / 2 - 363, 0, 726, screenHeight), Color.White);
                    else if (s == 4)
                        spriteBatch.Draw(Content.Load<Texture2D>(@"textures/filoden4f"), new Rectangle(screenWidth / 2 - 363, 0, 726, screenHeight), Color.White);
                    else if (s == 5)
                        spriteBatch.Draw(Content.Load<Texture2D>(@"textures/filoden5f"), new Rectangle(screenWidth / 2 - 363, 0, 726, screenHeight), Color.White);
                    cBtnprev.draw(spriteBatch);
                    cBtnnext.draw(spriteBatch);
                    cBtnskip.draw(spriteBatch);
                    break;
                case GameState.MainMenu:
                    spriteBatch.Draw(Content.Load<Texture2D>(@"textures/MainMenu"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    cBtnPlay.draw(spriteBatch);
                    cBtnOptions.draw(spriteBatch);
                    cBtnQuit.draw(spriteBatch);
                    break;
                case GameState.Playing:
                    this.IsMouseVisible = false;
                    for (int i = 0; i <= numberOfLifes; i++)
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>(@"textures/life_icon"), new Rectangle(screenWidth - 100*i,screenHeight- 80, 50, 50), Color.White);
                    }
                    if(numberOfLifes == 0)
                    {
                        cBtnGameOver.draw(spriteBatch);
                    }
                    //spriteBatch.DrawString(font, "number of lifes" + numberOfLifes, new Vector2(100, 150), Color.Yellow);
                    spriteBatch.Draw(Content.Load<Texture2D>(@"textures/fanty"), new Rectangle(0, screenHeight - 100, 100, 80), Color.White);
                    spriteBatch.DrawString(font, " " + numberOfCoins, new Vector2(120, screenHeight - 80), Color.Yellow);
                    break;
                case GameState.Options:
                    spriteBatch.Draw(Content.Load<Texture2D>(@"textures/purebackground"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    drawText();
                    cBtnBack.draw(spriteBatch);

                    break;
            }
            spriteBatch.End();
            
        }
        private void drawText()
        {
            spriteBatch.DrawString(font, "Ha�as muzyki ustawiony na " + /*Settings.musicVolume*/ (soundBackgroundInstance.Volume * 100).ToString("F0") + "%", new Vector2(100, 100), Color.Yellow);
            spriteBatch.DrawString(font, "Aby zmniejszy� naci�nij klawisz DOWN", new Vector2(100, 150), Color.Yellow);
            spriteBatch.DrawString(font, "aby zwi�kszy� UP", new Vector2(100, 200), Color.Yellow);
        }
        public void Die()
        {
            if (numberOfLifes > 0)
                numberOfLifes--;
            modelManager.avatar.avatarPosition = new Vector3(-4000, 40, 4000);
        }
    }
}
