using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectXMono.Objects
{
    public class Cell
    {
        private int _xPosition;
        private int _yPosition;
        private int _width;
        private int _height;
        private bool _isActive;
        public bool IsActive { get => _isActive; set => _isActive = value; }
        public int XPosition { get => _xPosition; }
        public int YPosition { get => _yPosition; }
        public int Width { get => _width; }
        public int Height { get => _height; }

        public Cell(int xPosition, int yPosition, int width, int height)
        {
            _xPosition = xPosition;
            _yPosition = yPosition;
            _width = width;
            _height = height;
            _isActive = false;
        }
    }
}
