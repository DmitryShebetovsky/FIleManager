using System;

namespace FileManager
{
    public delegate void OnKey(ConsoleKeyInfo key);
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "File Manager";
            var manager = new FileManager();
            manager.Explore();
        }
    }
}