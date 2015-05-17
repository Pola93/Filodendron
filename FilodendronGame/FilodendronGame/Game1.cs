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
        };
        public enum SoundState
        {
            Paused,
            Playing,
            Stopped,
        }
        public GameState currentGameState = GameState.MainMenu;
        //Screen adjustment
        int screenWidth = 800, screenHeight = 600;

        cButton cBtnPlay;
        cButton cBtnOptions;
        cButton cBtnQuit;
        cButton cBtnBack;
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
            // full screen mode settings
            // graphics.PreferredBackBufferWidth = 1366;    
            // graphics.PreferredBackBufferHeight = 768; 
            // graphics.IsFullScreen = true; 

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
            cBtnPlay.setPosition(new Vector2(350, 300));
            cBtnOptions = new cButton(Content.Load<Texture2D>(@"textures/optionsButton"), graphics.GraphicsDevice);
            cBtnOptions.setPosition(new Vector2(350, 370));
            cBtnQuit = new cButton(Content.Load<Texture2D>(@"textures/exitButton"), graphics.GraphicsDevice);
            cBtnQuit.setPosition(new Vector2(350, 440));
            cBtnBack = new cButton(Content.Load<Texture2D>(@"textures/backButton"), graphics.GraphicsDevice);
            cBtnBack.setPosition(new Vector2(100, 440));
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
            //  if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            //  {
            //     this.Exit();
            //  }


            MouseState mouse = Mouse.GetState();
            switch (currentGameState)
            {
                case GameState.MainMenu:

                    //  soundBackgroundInstance.IsLooped = true;
                    soundBackgroundInstance.Play();
                    if (cBtnPlay.isClicked == true)
                        currentGameState = GameState.Playing;
                    cBtnPlay.update(mouse);
                    if (cBtnOptions.isClicked == true) currentGameState = GameState.Options;
                    cBtnOptions.update(mouse);
                    if (cBtnQuit.isClicked == true)
                        this.Exit();
                    cBtnQuit.update(mouse);

                    break;
                case GameState.Playing:
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        currentGameState = GameState.MainMenu;
                    break;
                case GameState.Options:
                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {


                        if (soundBackgroundInstance.Volume < 0.99f)
                        {
                            soundBackgroundInstance.Volume += 0.01f;
                        }
                        else
                            soundBackgroundInstance.Volume = 1.0f;

                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    {

                        if (soundBackgroundInstance.Volume > 0.01f)
                        {
                            soundBackgroundInstance.Volume -= 0.01f;
                        }
                        else
                            soundBackgroundInstance.Volume = 0.0f;
                    }



                    if (cBtnBack.isClicked == true) currentGameState = GameState.MainMenu;
                    cBtnBack.update(mouse);
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

            spriteBatch.Begin();
            switch (currentGameState)
            {
                case GameState.MainMenu:
                    spriteBatch.Draw(Content.Load<Texture2D>(@"textures/MainMenu"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    cBtnPlay.draw(spriteBatch);
                    cBtnOptions.draw(spriteBatch);
                    cBtnQuit.draw(spriteBatch);
                    break;
                case GameState.Playing:
                    break;
                case GameState.Options:
                    spriteBatch.Draw(Content.Load<Texture2D>(@"textures/purebackground"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    drawText();
                    cBtnBack.draw(spriteBatch);

                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        private void drawText()
        {
            spriteBatch.DrawString(font, "Ha³as muzyki ustawiony na " + /*Settings.musicVolume*/ (soundBackgroundInstance.Volume * 100).ToString("F0") + "%", new Vector2(100, 100), Color.Yellow);
            spriteBatch.DrawString(font, "Aby zmniejszyæ naciœnij klawisz DOWN, aby zwiêkszyæ UP", new Vector2(100, 150), Color.Yellow);
        }

    }
}
