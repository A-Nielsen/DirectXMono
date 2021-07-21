using System;

namespace DirectXMono
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GameOfLife())
                game.Run();
        }
    }
}
