using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Objects.StaticTetrominoes
{
    public class SquareRow
    {
        private Square[] _squares;
        public Square[] Squares { get => _squares; set => _squares = value; }

        public SquareRow(int rowWidth)
        {
            _squares = new Square[rowWidth];
        }

        //check if row is full, return bool
        public bool FullRow()
        {
            foreach(Square square in _squares)
            {
                if(square is null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
