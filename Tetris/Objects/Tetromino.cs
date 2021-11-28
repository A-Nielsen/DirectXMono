using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Helpers;

namespace Tetris.Objects
{
    public class Tetromino
    {
        private bool _isActive;
        private int _rotationState;
        private int _posX;
        private int _posY;
        private Shape _tetrominoShape;
        private new List<(int x, int y)> _shapePos;

        public bool IsActive { get => _isActive; set => _isActive = value; }
        public int RotationState { get => _rotationState; set => _rotationState = value; }
        public int PosX { get => _posX; set => _posX = value; }
        public int PosY { get => _posY; set => _posY = value; }
        public Shape TetrominoShape { get => _tetrominoShape; }
        public List<(int y, int x)> ShapePos { get => _shapePos; set => _shapePos = value; }

        public Tetromino(Shape shape)
        {
            _tetrominoShape = shape;
            _shapePos = DefaultShapeUtil.GetShape(shape);
        }
    }
}
