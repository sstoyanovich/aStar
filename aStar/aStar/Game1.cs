using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace aStar
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 



    public class node
    {
        
        public int xPos;
        public int yPos;
        public int F;
        public int G;
        public int H;
        public node parent;
        public  bool isEqual(node other)
        {
            return (xPos == other.xPos && yPos == other.yPos);
        }
                
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int HEIGHT = 600;                       //Height of frame
        const int WIDTH = 860;                        //Width of frame

        Texture2D boxSprite;
        SpriteFont font;
        int red = 0;
        int green = 0;
        int[,] map = new int[172, 120];
        

        int startX;
        int startY;
        int destX;
        int destY;

        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;                       //sets the game to the specified width and height
            IsMouseVisible = true;
            initializeMap();

            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            boxSprite = Content.Load<Texture2D>(@"Sprites\box");
            font = Content.Load<SpriteFont>(@"Fonts\SpriteFont1");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //Makes walls
            MouseState mouseState = Mouse.GetState();
            if (IsActive
                && mouseState.LeftButton == ButtonState.Pressed)
            {
                calcTile(1);
            }
            //Sets up start location
            if (IsActive
                && mouseState.RightButton == ButtonState.Pressed)
              
            {
                if(red < 1)
                    calcTile(3);
                red++;
            }
            //Sets up destination location
            if (IsActive
                && Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (green < 1)
                    calcTile(4);
                green++;
            }
            //Runs algorithm
            if(Keyboard.GetState().IsKeyDown(Keys.Enter))
                aStar();
            //Resets gameboard
            if (Keyboard.GetState().IsKeyDown(Keys.R))
                initializeMap();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            //draws the screen
            drawMap();
            spriteBatch.DrawString(font, "R: Reset,D: Destination, RMB: Start, LMB: Wall", new Vector2(100, 0), Color.Black);
            


            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void drawMap()
        {
            for (int i = 0; i < 172; i++)
            {
                for (int j = 0; j < 120; j++)
                {
                   if( map[i, j] == 0)
                       spriteBatch.Draw(boxSprite, new Vector2(i * 5,j * 5 ), Color.White);
                   if (map[i, j] == 1)
                       spriteBatch.Draw(boxSprite, new Vector2(i * 5,j * 5), Color.Black);
                   if (map[i, j] == 2)
                       spriteBatch.Draw(boxSprite, new Vector2(i * 5, j * 5), Color.Blue);
                   if (map[i, j] == 3)
                   {
                       spriteBatch.Draw(boxSprite, new Vector2(i * 5, j * 5), Color.Red);         //start point
                       startX = i;
                       startY = j;
                   }
                   if (map[i, j] == 4)
                   {
                       spriteBatch.Draw(boxSprite, new Vector2(i * 5, j * 5), Color.Green);          //destination
                       destX = i;
                       destY = j;
                   }
                   if (map[i, j] == 5)
                       spriteBatch.Draw(boxSprite, new Vector2(i * 5, j * 5), Color.Brown);
                   if (map[i, j] == 6)
                       spriteBatch.Draw(boxSprite, new Vector2(i * 5, j * 5), Color.Yellow);
                }
            }
        }

        private void initializeMap()
        {
            for (int i = 0; i < 172; i++)
            {
                for (int j = 0; j < 120; j++)
                {
                    map[i, j] = 0;                        //sets blank map
                }
            }
            red = 0;
            green = 0;
        }

        private void calcTile(int color)
        {
            double xPos = Mouse.GetState().X;
            double yPos = Mouse.GetState().Y;
            int x = (int)xPos / 5;
            int y = (int)yPos / 5;                                //turns the x and y pixel positions into indeces for the arrays
            x = (int)MathHelper.Clamp((float)x, 0, 171);
            y = (int)MathHelper.Clamp((float)y, 0,119 );
            map[x, y] = color;

        }

        private void aStar()
        {
            
            List<node> open = new List<node>();
            List<node> closed = new List<node>();
            node active = new node();
            active.xPos = startX;
            active.yPos = startY;
            open.Add(active);
            

            while ((active.xPos != destX || active.yPos != destY))  
            {  

                for (int i = 0; i < 8; i++)
                {
                    int x = 0;
                    int y = 0;
                    //west
                    if (i == 0)
                    {
                        x = -1;
                        y = 0;
                    }
                    //northwest
                    if (i == 1)
                    {
                        x = -1;
                        y = -1;
                    }
                    //north
                    if (i == 2)
                    {
                        x = 0;
                        y = -1;
                    }
                    //northeast
                    if (i == 3)
                    {
                        x = 1;
                        y = -1;
                    }
                    //east
                    if (i == 4)
                    {
                        x = 1;
                        y = 0;
                    }
                    //southeast
                    if (i == 5)
                    {
                        x = 1;
                        y = 1;
                    }
                    //south
                    if (i == 6)
                    {
                        x = 0;
                        y = 1;
                    }
                    //southwest
                    if (i == 7)
                    {
                        x = -1;
                        y = 1;
                    }


                    node temp = new node();
                    temp.parent = active;
                    temp.xPos = temp.parent.xPos + x;
                    temp.yPos = temp.parent.yPos + y;
                    if ((temp.xPos >= 0 && temp.xPos< 172) && (temp.yPos >= 0 && temp.yPos < 120))
                    {
                        
                        //calculates F G and H values
                        if (Math.Abs(x) == Math.Abs(y))
                            temp.G = 14;
                        else
                            temp.G = 10;
                        temp.G = temp.G + temp.parent.G;
                        temp.H = (Math.Abs(temp.xPos - destX) + Math.Abs(temp.yPos - destY)) * 10;
                        temp.F = temp.G + temp.H;


                        //checks if node is in open
                        bool containsOpen = false;
                        bool containsClosed = false;
                        if (map[temp.xPos, temp.yPos] != 1)
                        {
                            for (int k = 0; k < open.Count; k++)
                            {
                                if (open.ElementAt(k).isEqual(temp))
                                {
                                    containsOpen = true;
                                    if (active.G + temp.G < open.ElementAt(k).G)
                                    {
                                        open.ElementAt(k).parent = active;
                                        temp.G = active.G + open.ElementAt(k).G;
                                        temp.F = temp.G + temp.H;
                                    }
                                }
                            }
                            for (int k = 0; k < closed.Count; k++)
                            {
                                if (closed.ElementAt(k).isEqual(temp))
                                {
                                    containsClosed = true;
                                }
                            }

                            if (!containsOpen && !containsClosed)
                            {
                                open.Add(temp);
                                map[temp.xPos, temp.yPos] = 2;
                            }
                        }

                    }
                }

                //finds lowest F value
                int min = 1;
                for (int k = 1; k < open.Count; k++)
                {
                   
                    if (open.ElementAt(k).F <= open.ElementAt(min).F)
                        min = k;
                }
                
                
                closed.Add(active);
                
                map[open.ElementAt(min).xPos, open.ElementAt(min).yPos] = 5;
                
                active = open.ElementAt(min);
                open.RemoveAt(min);
                
        }
            while (active.xPos != startX || active.yPos != startY)
            {
                map[active.xPos,active.yPos] = 6;
                active = active.parent;
            }

           
        }
    }
}
