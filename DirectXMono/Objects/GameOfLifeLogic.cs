using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DirectXMono.Objects
{
    public class GameOfLifeLogic
    {
        public GameOfLifeLogic()
        {

        }

        public void UpdateCells(List<Cell> cells, int cellsX, int cellsY)
        {
            //Cells are updated after the game logic has run, because otherwise the cells will not act correctly
            List<Tuple<int, bool>> cellsToUpdate = new List<Tuple<int, bool>>();

            //How far in the loop we are
            int cellIndex = 0;

            foreach(Cell cell in cells)
            {
                int nearbyCells = 0;

                int cellY = cell.YPosition;
                int cellWidth = cell.Width;
                int cellHeight = cell.Height;
                int cellXPos = cell.XPosition / cellWidth;
                int cellYPos = cell.YPosition / cellHeight;

                //Check how many neighboring cells there are
                //If cell is in top row, we can't check rows above
                if (cellYPos > 0)
                {
                    //Top left
                    if (cellXPos > 0)
                        if (cells[cellIndex - cellsX - 1].IsActive) nearbyCells++;
                    //Top
                    if (cells[cellIndex - cellsX].IsActive) nearbyCells++;
                    //Top right
                    if(cellXPos < 79)
                        if (cells[cellIndex - cellsX + 1].IsActive) nearbyCells++;
                }

                //Left
                if (cellXPos > 0)
                    if (cells[cellIndex - 1].IsActive) nearbyCells++;
                //Right
                if (cellXPos < 79)
                    if (cells[cellIndex + 1].IsActive) nearbyCells++;

                //If cell is in bbottom row, we can't check rows below
                if (cellYPos < cellsY-1)
                {
                    //Bottom left
                    if (cellXPos > 0)
                        if (cells[cellIndex + cellsX - 1].IsActive) nearbyCells++;
                    //Bottom
                    if (cells[cellIndex + cellsX].IsActive) nearbyCells++;
                    //Bottom right
                    if (cellXPos < 79)
                        if (cells[cellIndex + cellsX + 1].IsActive) nearbyCells++;
                }

                if (cell.IsActive)
                {
                    //If cells have less than 2 or more than 3 neighbors, they die
                    if (nearbyCells > 1 && nearbyCells < 4)
                        cellsToUpdate.Add(new Tuple<int, bool>(cellIndex, true));
                    else
                        cellsToUpdate.Add(new Tuple<int, bool>(cellIndex, false));
                }
                else
                {
                    //If cell is dead, but has 3 neighbors, it comes alive
                    if (nearbyCells == 3)
                        cellsToUpdate.Add(new Tuple<int, bool>(cellIndex, true));
                }

                cellIndex++;
            }

            //Update the cells
            foreach(Tuple<int, bool> cell in cellsToUpdate)
            {
                cells[cell.Item1].IsActive = cell.Item2;
            }
        }
    }
}
