using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Objects;

namespace Tetris.Helpers
{
    public static class DefaultShapeUtil
    {
        public static List<(int, int)> GetShape(Shape shape)
        {
            List<(int, int)> shapeArray = new List<(int y, int x)>{ };

            switch (shape)
            {
                case Shape.I:
                    shapeArray = new List<(int y, int x)> 
                        { 
                            (1,0),
                            (1,1),
                            (1,2),
                            (1,3),
                        };
                    break;

                case Shape.O:
                    shapeArray = new List<(int y, int x)> 
                        {
                            (0,2),
                            (0,3),
                            (1,2),
                            (1,3),
                        };
                    break;

                default: break;
            }

            return shapeArray;
        }
    }
}
