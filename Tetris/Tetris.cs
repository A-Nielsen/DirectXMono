using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Myra;
using Myra.Graphics2D.UI;
using System;
using Tetris.Objects;
using System.Linq;

namespace DirectXMono
{
    public class Tetris : Game
    {
        private Desktop _desktop;

        Texture2D gridTexture;
        Texture2D gridTextureBlack;
        Texture2D demoTetrominoTex;

        float timer = 5;         //Initialize a 10 second timer
        const float TIMER = 1f;

        //tile height and width defines the area a cell can inhabit
        double tileWidth;
        double tileHeight;

        //Thickness of grid
        int gridThickness;

        double tetrisOffset;
        int leftLimit;
        int rightLimit;

        bool gameRunning;

        Tetromino tetromino;

        List<Tetromino> tetrominoes = new List<Tetromino>();

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public Tetris()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //60 fps
            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);

            //To Accomodate the right and bottom tile pixels that are not within the window area
            _graphics.PreferredBackBufferWidth = _graphics.PreferredBackBufferWidth + 1;
            _graphics.PreferredBackBufferHeight = _graphics.PreferredBackBufferHeight + 1;
        }

        protected override void Initialize()
        {
            tileWidth = 20;
            tileHeight = 20;

            gameRunning = false;

            tetromino = new Tetromino(Shape.I);

            tetrisOffset = _graphics.PreferredBackBufferWidth / 2 / tileWidth;
            leftLimit = (int)tetrisOffset - 5;
            rightLimit = (int)tetrisOffset + 5;

            //Currently not adjustable, stay at one
            gridThickness = 1;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            gridTexture = Content.Load<Texture2D>("grid");

            gridTextureBlack = Content.Load<Texture2D>("grid - black");

            demoTetrominoTex = Content.Load<Texture2D>("Cell");

            BuildUI();
        }

        private void BuildUI()
        {

            MyraEnvironment.Game = this;

            var grid = new Grid
            {
                RowSpacing = 8,
                ColumnSpacing = 8
            };

            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            // Button
            var start = new TextButton
            {
                GridColumn = 0,
                GridRow = 1,
                Text = "Start"
            };

            var fps = new Label
            {
                Id = "FPS",
                Text = "0"
            };
            grid.Widgets.Add(fps);

            var loops = new Label
            {
                Id = "LOOPS",
                Text = "0",
                GridColumn = 1,
                GridRow = 1,

            };
            grid.Widgets.Add(loops);

            start.Click += (s, a) =>
            {
                start.Visible = false;
                gameRunning = true;
            };

            grid.Widgets.Add(start);

            // Add it to the desktop
            _desktop = new Desktop();
            _desktop.Root = grid;
        }

        protected override void Update(GameTime gameTime)
        {
            if (gameRunning)
            {
                //Only update grid every x amount of time
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                timer -= elapsed;
                if (timer < 0)
                {
                    GameLogic();
                    timer = TIMER;   //Reset Timer
                }
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            CheckInput();
            LimitTetroX();

            //Set background
            GraphicsDevice.Clear(Color.SlateGray);

            //Begin drawing
            _spriteBatch.Begin();

            if (gameRunning)
            {
                DrawGrid();
                RenderGame();
            }

            //End drawing
            _spriteBatch.End();

            _desktop.Render();

            double framerate = (1 / gameTime.ElapsedGameTime.TotalSeconds);
            var fpsWidget = (Label)_desktop.Root.FindWidgetById("FPS");
            fpsWidget.Text = "FPS: " + Math.Round(framerate).ToString();

            base.Draw(gameTime);
        }

        public void CheckInput()
        {
            KeyboardState state = Keyboard.GetState();
            //Check key input
            if (state.IsKeyDown(Keys.Left))
            {
                tetromino.PosX -= 1;
            }    

            if (state.IsKeyDown(Keys.Right))
            {
                tetromino.PosX += 1;
            }

            //Rotate and move based on key input
        }

        public void DrawGrid()
        {
            int height = _graphics.PreferredBackBufferHeight;
            int width = _graphics.PreferredBackBufferWidth;

            //Draw grid
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //grid
                    if (x % tileHeight == 0 || y % tileWidth == 0)
                        _spriteBatch.Draw(gridTexture, new Rectangle(x * gridThickness, y * gridThickness, gridThickness, gridThickness), Color.White);

                    //game limits
                    if (x / tileHeight == leftLimit || x / tileHeight == rightLimit)
                        _spriteBatch.Draw(gridTextureBlack, new Rectangle(x * gridThickness, y * gridThickness, gridThickness, gridThickness), Color.White);
                }
            }
        }

        public void GameLogic()
        {
            //Check if tetromino touches buttom, or another tetromino
            bool bottomCollided = CheckBottomCollision();

            if (!bottomCollided)
            {
                //Move tetromino down
                tetromino.PosY += 1;
            }
            else
            {
                Tetromino bottomed = new Tetromino(tetromino.TetrominoShape);
                bottomed.ShapePos = tetromino.ShapePos;
                bottomed.PosY = tetromino.PosY;
                bottomed.PosX = tetromino.PosX;

                tetrominoes.Add(bottomed); 
                tetromino = new Tetromino(Shape.O);
            }
        }

        public void LimitTetroX()
        {
            //Get tetromino far left position
            int tetrominoLeft = (int)tetrisOffset + tetromino.PosX + tetromino.ShapePos
                .OrderBy(t => t.x)
                .FirstOrDefault().x;

            //If tetromino is over left limit, move right one
            int tetrominoOverLeft = tetrominoLeft - leftLimit;
            if (tetrominoOverLeft < 0)
                tetromino.PosX++;

            //Get tetromino far right position
            int tetrominoRight = (int)tetrisOffset + tetromino.PosX + tetromino.ShapePos
                .OrderByDescending(t => t.x)
                .FirstOrDefault().x;

            //If tetromino is over far right limit, move left one
            int tetrominoOverRight =  rightLimit - tetrominoRight - 1;
            if (tetrominoOverRight < 0)
                tetromino.PosX--;
        }

        public bool CheckBottomCollision()
        {
            int heightTiles = _graphics.PreferredBackBufferHeight / (int)tileHeight;
            //check lowest playing field point collision
            foreach ((int y, int x) i in tetromino.ShapePos)
            {
                if (heightTiles - 1 == i.y + tetromino.PosY)
                    return true;

                foreach (Tetromino tetrominoToCompare in tetrominoes)
                {
                    foreach ((int y, int x) iToCompare in tetrominoToCompare.ShapePos)
                    {
                        if (i.y + tetromino.PosY == iToCompare.y + tetrominoToCompare.PosY - 1)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void RenderGame()
        {
            int loops = 0;

            //draw settled tetrominoes
            foreach(Tetromino tetro in tetrominoes) 
            {
                RenderTetromino(tetro, loops);
            }

            //Draw active tetromino
            RenderTetromino(tetromino, loops);

            var loopWidget = (Label)_desktop.Root.FindWidgetById("LOOPS");
            loops = loops / (1 + tetrominoes.Count());
            loopWidget.Text = "LOOPS/TETROMINO: " + loops.ToString();

        }

        public void RenderTetromino(Tetromino tetrominoToRender, int loops)
        {
            int maxY = tetrominoToRender.ShapePos.Max(x => x.y);
            int minY = tetrominoToRender.ShapePos.Min(x => x.y);
            int maxX = tetrominoToRender.ShapePos.Max(x => x.x);
            int minX = tetrominoToRender.ShapePos.Min(x => x.x);
            foreach ((int y, int x) i in tetrominoToRender.ShapePos)
            {
                for (int y = minY; y < maxY + 1; y++)
                {
                    for (int x = minX; x < maxX + 1; x++)
                    {
                        if (x % tileHeight == 0 || y % tileWidth == 0)
                        {
                            //X and Y coordinates for top left corner of squares
                            int xTileWidth = (i.x + (int)tetrisOffset + tetrominoToRender.PosX) * (int)tileWidth;
                            int yTileHeight = (i.y + tetrominoToRender.PosY) * (int)tileHeight;
                            _spriteBatch.Draw(demoTetrominoTex, new Rectangle(xTileWidth, yTileHeight, (int)tileWidth, (int)tileHeight), Color.White);
                            loops++;
                        }
                    }
                }
            }
        }
    }
}


/*          
 *          
 *          
            foreach(Tetromino tetro in tetrominoes) 
            { 
                foreach ((int y, int x) i in tetro.ShapePos)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (x % tileHeight == 0 || y % tileWidth == 0)
                            {
                                int xTileWidth = (i.x + (int)tetrisOffset) * (int)tileWidth;
                                int yTileHeight = (i.y + tetro.PosY) * (int)tileHeight;
                                _spriteBatch.Draw(demoTetrominoTex, new Rectangle(xTileWidth, yTileHeight, (int)tileWidth, (int)tileHeight), Color.White);
                            }
                        }
                    }
                }
            }
 *          
 *          foreach ((int y, int x) i in tetromino.ShapePos)
            {
                int xTileWidth = (i.x + (int)tetrisOffset + tetromino.PosX) * (int)tileWidth;
                int yTileHeight = (i.y + tetromino.PosY) * (int)tileHeight;

                for (int y = 0; y < yTileHeight; y++)
                {
                    for (int x = 0; x < xTileWidth; x++)
                    {
                        if (x % tileHeight == 0 || y % tileWidth == 0)
                        {
                            _spriteBatch.Draw(demoTetrominoTex, new Rectangle(xTileWidth, yTileHeight, (int)tileWidth, (int)tileHeight), Color.White);
                        }
                    }
                    loops++;
                }
            }
            var fpsWidget = (Label)_desktop.Root.FindWidgetById("LOOPS");
            fpsWidget.Text = "LOOPS/TETROMINO: " + loops.ToString();
*/