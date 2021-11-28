using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Myra;
using Myra.Graphics2D.UI;
using System;

namespace DirectXMono
{
    public class InfiniteGrid : Game
    {
        private Desktop _desktop;

        Texture2D gridTexture;
        Texture2D xyTexture;

        //tile height and width defines the area a cell can inhabit
        double tileWidth;
        double tileHeight;

        //Thickness of grid
        int gridThickness;

        //Width and height in cell size instead of pixels
        double screenPosX;
        double screenPosY;

        //Previous position of mouse when clicked
        double prevMouseX;
        double prevMouseY;

        double mouseDifferenceX;
        double mouseDifferenceY;

        int prevScrollValue;
        int zoomLevel;
        int zoomMax = 7;
        int zoomMin = -7;

        MouseState prevMouseState;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public InfiniteGrid()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //To Accomodate the right and bottom tile pixels that are not within the window area
            _graphics.PreferredBackBufferWidth = _graphics.PreferredBackBufferWidth + 1;
            _graphics.PreferredBackBufferHeight = _graphics.PreferredBackBufferHeight + 1;
        }

        protected override void Initialize()
        {
            tileWidth = 10;
            tileHeight = 10;

            screenPosX = +(_graphics.PreferredBackBufferWidth/2);
            screenPosY = +(_graphics.PreferredBackBufferHeight/2);

            zoomLevel = 0;

            //Currently not adjustable, stay at one
            gridThickness = 1;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            gridTexture = Content.Load<Texture2D>("grid");
            xyTexture = Content.Load<Texture2D>("grid - black");
        }

        protected override void Update(GameTime gameTime)
        {
            Drag();
            Zoom();

            base.Update(gameTime);
        }

        public void Zoom()
        {
            MouseState mouseState = Mouse.GetState();
            int scrollValue = mouseState.ScrollWheelValue/120;

            if (prevScrollValue != scrollValue)
            {
                zoomLevel += scrollValue - prevScrollValue;

                if (zoomLevel > zoomMax)
                    zoomLevel = zoomMax;

                if (zoomLevel < zoomMin)
                    zoomLevel = zoomMin;
            }

            
            prevScrollValue = scrollValue;
        }

        public void Drag()
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (mouseState.X >= 0 && mouseState.X <= _graphics.PreferredBackBufferWidth)
                {
                    if (mouseState.Y >= 0 && mouseState.Y <= _graphics.PreferredBackBufferHeight)
                    {
                        double mouseX = mouseState.X;
                        double mouseY = mouseState.Y;

                        if (prevMouseState.LeftButton != ButtonState.Pressed)
                        {
                            prevMouseX = mouseX;
                            prevMouseY = mouseY;
                        }

                        //If mouse is at same position as previous gameloop, do nothing
                        mouseDifferenceX = mouseX - prevMouseX;
                        mouseDifferenceY = mouseY - prevMouseY;

                        prevMouseX = mouseX;
                        prevMouseY = mouseY;
                    }
                }
            }
            else
            {
                mouseDifferenceX = 0;
                mouseDifferenceY = 0;
            }

            prevMouseState = mouseState;
        }

        protected override void Draw(GameTime gameTime)
        {
            //Set background
            GraphicsDevice.Clear(Color.SlateGray);

            //Begin drawing
            _spriteBatch.Begin();

            DrawGrid();

            //End drawing
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawGrid()
        {
            //Offset screen position based on mouse movement
            screenPosX += mouseDifferenceX;
            screenPosY += mouseDifferenceY;

            //Draw grid
            for (int y = 0; y < _graphics.PreferredBackBufferHeight; y++)
            {
                for (int x = 0; x < _graphics.PreferredBackBufferWidth; x++)
                {
                    if ((x - screenPosX) % (tileHeight + zoomLevel) == 0 || (y - screenPosY) % (tileWidth + zoomLevel) == 0)
                        _spriteBatch.Draw(gridTexture, new Rectangle(x * gridThickness, y * gridThickness, gridThickness, gridThickness), Color.White);

                    if ((x - screenPosX) == 0)
                        _spriteBatch.Draw(xyTexture, new Rectangle(x * gridThickness, y * gridThickness, gridThickness, gridThickness), Color.Black);

                    if ((y - screenPosY) == 0)
                        _spriteBatch.Draw(xyTexture, new Rectangle(x * gridThickness, y * gridThickness, gridThickness, gridThickness), Color.Black);
                }
            }
        }
    }
}
