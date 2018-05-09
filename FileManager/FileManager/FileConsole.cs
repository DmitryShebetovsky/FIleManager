using System;

namespace FileManager
{
    internal struct FrameLine
    {
        public string topLeft;
        public string topRight;
        public string bottomLeft;
        public string bottomRight;
        public string lineX;
        public string lineY;
        public FrameLine(int n)
        {
            topLeft = "┌";
            topRight = "┐";
            bottomLeft = "└";
            bottomRight = "┘";
            lineX = "─";
            lineY = "│";
        }
    }
    internal struct FrameDoubleLine
    {
        public string topLeft;
        public string topRight;
        public string bottomLeft;
        public string bottomRight;
        public string lineA;
        public string lineB;
        public FrameDoubleLine(int n)
        {
            topLeft = "╔";
            topRight = "╗";
            bottomLeft = "╚";
            bottomRight = "╝";
            lineA = "═";
            lineB = "║";
        }
    }
    internal static class FileConsole
    {
        public static void PrintFrameLine(int positionX, int positionY, int sizeX, int sizeY, ConsoleColor text,
            ConsoleColor background)
        {
            var SizeX = positionX + sizeX;
            var SizeY = positionY + sizeY;
            var f = new FrameLine(0);
            Console.ForegroundColor = text;
            Console.BackgroundColor = background;
            for (var y = positionY; y < SizeY; y++)
                for (var x = positionX; x < SizeX; x++)
                {
                    if (y == positionY && x == positionX)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(f.topLeft);
                    }
                    if (y == positionY && x > positionX && x < SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(f.lineX);
                    }
                    if (y == positionY && x == SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(f.topRight);
                    }
                    if (y > positionY && y < SizeY - 1 && x == positionX ||
                        y > positionY && y < SizeY - 1 && x == SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(f.lineY);
                    }
                    if (y == SizeY - 1 && x == positionX)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(f.bottomLeft);
                    }
                    if (y == SizeY - 1 && x > positionX && x < SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(f.lineX);
                    }
                    if (y == SizeY - 1 && x == SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(f.bottomRight);
                    }
                }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        public static void PrintFrameDoubleLine(int positionX, int positionY, int sizeX, int sizeY, ConsoleColor text,
            ConsoleColor background)
        {
            var SizeY = positionY + sizeY;
            var SizeX = positionX + sizeX;
            var f = new FrameDoubleLine(0);
            Console.ForegroundColor = text;
            Console.BackgroundColor = background;

            for (var y = positionY; y < SizeY; y++)
                for (var x = positionX; x < SizeX; x++)
                {
                    if (y == positionY && x == positionX)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(f.topLeft);
                    }
                    if (y == positionY && x > positionX && x < SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(f.lineA);
                    }
                    if (y == positionY && x == SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(f.topRight);
                    }
                    if (y > positionY && y < SizeY - 1 && x == positionX ||
                        y > positionY && y < SizeY - 1 && x == SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(f.lineB);
                    }
                    if (y == SizeY - 1 && x == positionX)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(f.bottomLeft);
                    }
                    if (y == SizeY - 1 && x > positionX && x < SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(f.lineA);
                    }
                    if (y == SizeY - 1 && x == SizeX - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(f.bottomRight);
                    }
                }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        public static void PrintString(string str, int X, int Y, ConsoleColor text, ConsoleColor background)
        {
            Console.ForegroundColor = text;
            Console.BackgroundColor = background;

            Console.SetCursorPosition(X, Y);
            Console.Write(str);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}