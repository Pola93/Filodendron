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
        public GameState currentGameState = GameState.MainMenu;
        //Screen adjustment
        int screenWidth = 800, screenHeight = 600;
   
        cButton cBtnPlay;
        cButton cBtnOptions;
        cButton cBtnExit;
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
            cBtnExit = new cButton(Content.Load<Texture2D>(@"textures/exitButton"), graphics.GraphicsDevice);
            cBtnExit.setPosition(new Vector2(350, 440));
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
            switch(currentGameState)
            {
                case GameState.MainMenu:
                    if (cBtnPlay.isClicked == true) currentGameState = GameState.Playing;
                    cBtnPlay.update(mouse);
                    if (cBtnOptions.isClicked == true) currentGameState = GameState.Options;
                    cBtnOptions.update(mouse);
                    if (cBtnExit.isClicked == true)
                    this.Exit();
                    break;
                case GameState.Playing:
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        currentGameState = GameState.MainMenu;
                    break;
                case GameState.Options:
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
                    cBtnExit.draw(spriteBatch);
                    break;
                case GameState.Playing:
                    break;
                case GameState.Options:
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
