using DirectXMono.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Myra;
using Myra.Graphics2D.UI;

namespace DirectXMono
{
    public class GameOfLife : Game
    {
        private Desktop _desktop;

        Texture2D gridTexture;
        Texture2D cellTexture;

        float timer = 5;         //Initialize a 10 second timer
        const float TIMER = 0.1f;

        //tile height and width defines the area a cell can inhabit
        int tileWidth;
        int tileHeight;

        //Thickness of grid
        int gridThickness;

        List<Cell> cells;

        //Width and height in cell size instead of pixels
        int cellsX;
        int cellsY;

        //Previous position of mouse when clicked
        int prevMouseX;
        int prevMouseY;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameOfLifeLogic logic;
        public GameOfLife()
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

            //Currently not adjustable, stay at one
            gridThickness = 1;

            logic = new GameOfLifeLogic();

            InitializeCells();

            //Glider
            cells[180].IsActive = true;
            cells[180 + 80].IsActive = true;
            cells[180 + 80*2].IsActive = true;
            cells[180 + 80 * 2 - 1].IsActive = true;
            cells[180 + 80 - 2].IsActive = true;

            base.Initialize();
        }

        private void InitializeCells()
        {
            //Cell definition, creates cells to fill grid
            cells = new List<Cell>();
            cellsX = _graphics.PreferredBackBufferWidth / tileWidth;
            cellsY = _graphics.PreferredBackBufferHeight / tileHeight;

            for (int y = 0; y < cellsY; y++)
            {
                for (int x = 0; x < cellsX; x++)
                {
                    cells.Add(new Cell(x * tileWidth, y * tileHeight, tileWidth, tileHeight));
                }
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            gridTexture = Content.Load<Texture2D>("grid");
            cellTexture = Content.Load<Texture2D>("cell");
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            if(mouseState.LeftButton == ButtonState.Pressed)
            {
                if (mouseState.X >= 0 && mouseState.X <= _graphics.PreferredBackBufferWidth)
                {
                    if (mouseState.Y >= 0 && mouseState.Y <= _graphics.PreferredBackBufferHeight)
                    {
                        int mouseXCells = mouseState.X / tileWidth;
                        int mouseYCells = mouseState.Y / tileHeight;

                        //If mouse is at same position as previous gameloop, do nothing
                        if (mouseXCells != prevMouseX || mouseYCells != prevMouseY) {
                            var cellToModify = cells[mouseYCells * cellsX + mouseXCells];

                            cellToModify.IsActive = !cellToModify.IsActive;
                        }

                        prevMouseX = mouseXCells;
                        prevMouseY = mouseYCells;
                    }
                }
            }

            //Only update grid every x amount of time
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer -= elapsed;
            if (timer < 0)
            {
                logic.UpdateCells(cells, cellsX, cellsY);
                timer = TIMER;   //Reset Timer
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //Set background
            GraphicsDevice.Clear(Color.SlateGray);

            //Begin drawing
            _spriteBatch.Begin();

            //Draw grid
            for (int y = 0; y < _graphics.PreferredBackBufferHeight; y++)
            {
                for (int x = 0; x < _graphics.PreferredBackBufferWidth; x++)
                {
                    if (x % tileHeight == 0 || y % tileWidth == 0)
                        _spriteBatch.Draw(gridTexture, new Rectangle(x * gridThickness, y * gridThickness, gridThickness, gridThickness), Color.White);
                }
            }

            int indexTest = 0;
            //Draw cells if active
            foreach (Cell cell in cells)
            {
                if(cell.IsActive)
                    _spriteBatch.Draw(cellTexture, new Rectangle(cell.XPosition, cell.YPosition, cell.Width, cell.Height), Color.White);
                indexTest++;
            }

            //End drawing
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
