using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace FileManager
{
    class FilePanel
    {      
        public static int PanelHeight = 20;
        public static int PanelWidth = 60;
        private int _top;
        public int Top
        {
            get { return this._top; }
            set
            {
                if (0 <= value && value <= Console.WindowHeight - FilePanel.PanelHeight)
                    this._top = value;
                else
                    throw new Exception(String.Format("Выход координаты top ({0}) файловой панели за допустимое значение", value));
            }
        }
        private int _left;
        public int Left
        {
            get { return this._left; }
            set
            {
                if (0 <= value && value <= Console.WindowWidth - FilePanel.PanelWidth)
                    this._left = value;
                else
                    throw new Exception(String.Format("Выход координаты left ({0}) файловой панели за пределы окна", value));
            }
        }
        private int _height = FilePanel.PanelHeight;
        public int Height
        {
            get { return this._height; }             
            set
            {
                if (0 < value && value <= Console.WindowHeight)
                    this._height = value;
                else
                    throw new Exception(String.Format("Высота файловой панели {0} больше размера окна", value));
            }
        }
        private int _width = FilePanel.PanelWidth;
        public int Width
        {
            get { return this._width; }
            set
            {
                if (0 < value && value <= Console.WindowWidth)
                    this._width = value;
                else
                    throw new Exception(String.Format("Ширина файловой панели {0} больше размера окна", value));
            }
        }     
        private string _path;
        public string Path
        {
            get { return this._path; }               
            set
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(value);
                if (directoryInfo.Exists)
                    this._path = value;
                else
                    throw new Exception(String.Format("Путь {0} не существует", value));
            }
        }       
        private int _activeObjectIndex = 0;
        private int _firstObjectIndex = 0;
        private readonly int _displayedObjectsAmount = PanelHeight - 2;
        private bool _active;
        public bool Active
        {
            get { return this._active; }           
            set { this._active = value; }             
        }
        private bool _discs;
        public bool IsDiscs
        {
            get { return this._discs; }             
        }
        private readonly List<FileSystemInfo> _fsObjects = new List<FileSystemInfo>();
        public FilePanel()
        {
            this.SetDiscs();
        } 
        public FilePanel(string path)
        {
            this._path = path;
            this.SetLists();
        }
        public FileSystemInfo GetActiveObject()
        {
            if (this._fsObjects != null && this._fsObjects.Count != 0)
            {
                return this._fsObjects[this._activeObjectIndex];
            }
            throw new Exception("Список объектов панели пуст");
        }
        public bool FindFile(string name)
        {
            int index = 0;
            foreach (FileSystemInfo file in this._fsObjects)
            {
                if (file != null && file.Name == name)
                {
                    this._activeObjectIndex = index;
                    if (this._activeObjectIndex > this._displayedObjectsAmount)
                    {
                        this._firstObjectIndex = _activeObjectIndex;
                    }
                    this.UpdateContent(false);
                    return true;
                }
                index++;
            }
            return false;
        }  
        public void KeyboardProcessing(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    this.ScrollUp();
                    break;
                case ConsoleKey.DownArrow:
                    this.ScrollDown();
                    break;
                case ConsoleKey.Home:
                    this.GoBegin();
                    break;
                case ConsoleKey.End:
                    this.GoEnd();
                    break;
                case ConsoleKey.PageUp:
                    this.PageUp();
                    break;
                case ConsoleKey.PageDown:
                    this.PageDown();
                    break;
            }
        }   
        private void ScrollDown()
        {
            if (this._activeObjectIndex >= this._firstObjectIndex + this._displayedObjectsAmount - 1)
            {
                this._firstObjectIndex += 1;
                if (this._firstObjectIndex + this._displayedObjectsAmount >= this._fsObjects.Count)
                {
                    this._firstObjectIndex = this._fsObjects.Count - this._displayedObjectsAmount;
                }
                this._activeObjectIndex = this._firstObjectIndex + this._displayedObjectsAmount - 1;
                this.UpdateContent(false);
            }
            else
            {
                if (this._activeObjectIndex >= this._fsObjects.Count - 1)
                {
                    return;
                }
                this.DeactivateObject(this._activeObjectIndex);
                this._activeObjectIndex++;
                this.ActivateObject(this._activeObjectIndex);
            }
        }
        private void ScrollUp()
        {
            if (this._activeObjectIndex <= this._firstObjectIndex)
            {
                this._firstObjectIndex -= 1;
                if (this._firstObjectIndex < 0)
                {
                    this._firstObjectIndex = 0;
                }
                this._activeObjectIndex = _firstObjectIndex;
                this.UpdateContent(false);
            }
            else
            {
                this.DeactivateObject(this._activeObjectIndex);
                this._activeObjectIndex--;
                this.ActivateObject(this._activeObjectIndex);
            }
        }
        private void GoEnd()
        {
            if (this._firstObjectIndex + this._displayedObjectsAmount < this._fsObjects.Count)
            {
                this._firstObjectIndex = this._fsObjects.Count - this._displayedObjectsAmount;
            }
            this._activeObjectIndex = this._fsObjects.Count - 1;
            this.UpdateContent(false);
        }
        private void GoBegin()
        {
            this._firstObjectIndex = 0;
            this._activeObjectIndex = 0;
            this.UpdateContent(false);
        }
        private void PageDown()
        {
            if (this._activeObjectIndex + this._displayedObjectsAmount < this._fsObjects.Count)
            {
                this._firstObjectIndex += this._displayedObjectsAmount;
                this._activeObjectIndex += this._displayedObjectsAmount;
            }
            else
            {
                this._activeObjectIndex = this._fsObjects.Count - 1;
            }
            this.UpdateContent(false);
        }
        private void PageUp()
        {
            if (this._activeObjectIndex > this._displayedObjectsAmount)
            {
                this._firstObjectIndex -= this._displayedObjectsAmount;
                if (this._firstObjectIndex < 0)
                {
                    this._firstObjectIndex = 0;
                }

                this._activeObjectIndex -= this._displayedObjectsAmount;

                if (this._activeObjectIndex < 0)
                {
                    this._activeObjectIndex = 0;
                }
            }
            else
            {
                this._firstObjectIndex = 0;
                this._activeObjectIndex = 0;
            }
            this.UpdateContent(false);
        }   
        public void SetLists()
        {
            if (this._fsObjects.Count != 0)
            {
                this._fsObjects.Clear();
            }
            this._discs = false;
            DirectoryInfo levelUpDirectory = null;
            this._fsObjects.Add(levelUpDirectory);
            //Directories
            string[] directories = Directory.GetDirectories(this._path);
            foreach (string directory in directories)
            {
                DirectoryInfo di = new DirectoryInfo(directory);
                this._fsObjects.Add(di);
            }
            //Files
            string[] files = Directory.GetFiles(this._path);
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                this._fsObjects.Add(fi);
            }
        }
        public void SetDiscs()
        {
            if (this._fsObjects.Count != 0)
            {
                this._fsObjects.Clear();
            }
            this._discs = true;
            DriveInfo[] discs = DriveInfo.GetDrives();
            foreach (DriveInfo disc in discs)
            {
                if (disc.IsReady)
                {
                    DirectoryInfo di = new DirectoryInfo(disc.Name);
                    this._fsObjects.Add(di);
                }
            }
        }
        public void Show()
        {
            this.Clear();
            FileConsole.PrintFrameDoubleLine(this._left, this._top, this._width, this._height, ConsoleColor.Green, ConsoleColor.Black);
            StringBuilder caption = new StringBuilder();
            if (this._discs)
            {
                caption.Append(' ').Append("Диски:").Append(' ');
            }
            else
            {
                caption.Append(' ').Append(this._path).Append(' ');
            }
            FileConsole.PrintString(caption.ToString(), this._left + this._width / 2 - caption.ToString().Length / 2, this._top, ConsoleColor.Yellow, ConsoleColor.Black);
            this.PrintContent();
        }
        public void Clear()
        {
            for (int i = 0; i < this._height; i++)
            {
                string space = new String(' ', this._width);
                Console.SetCursorPosition(this._left, this._top + i);
                Console.Write(space);
            }
        }
        private void PrintContent()
        {
            if (this._fsObjects.Count == 0)
            {
                return;
            }
            int count = 0;
            int lastElement = this._firstObjectIndex + this._displayedObjectsAmount;
            if (lastElement > this._fsObjects.Count)
            {
                lastElement = this._fsObjects.Count;
            }
            if (this._activeObjectIndex >= this._fsObjects.Count)
            {
                _activeObjectIndex = 0;
            }
            for (int i = this._firstObjectIndex; i < lastElement; i++)
            {
                Console.SetCursorPosition(this._left + 1, this._top + count + 1);
                
                if (i == this._activeObjectIndex && this._active == true)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                }
                this.PrintObject(i);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.BackgroundColor = ConsoleColor.Black;
                count++;
            }
        }
        private void ClearContent()
        {
            for (int i = 1; i < this._height - 1; i++)
            {
                string space = new String(' ', this._width - 2);
                Console.SetCursorPosition(this._left + 1, this._top + i);
                Console.Write(space);
            }
        }
        private void PrintObject(int index)
        {
            if (index < 0 || this._fsObjects.Count <= index)
            {
                throw new Exception(String.Format("Невозможно вывести объект c индексом {0}. Выход индекса за диапазон", index));
            }
            int currentCursorTopPosition = Console.CursorTop;
            int currentCursorLeftPosition = Console.CursorLeft;
            if (!this._discs && index == 0)
            {
                Console.Write("..");
                return;
            }
            Console.Write("{0}", _fsObjects[index].Name);
            Console.SetCursorPosition(currentCursorLeftPosition + this._width / 2, currentCursorTopPosition);
            if (_fsObjects[index] is DirectoryInfo)
            {
                //Console.Write("{0}", ((DirectoryInfo)_fsObjects[index]).LastWriteTime);
            }
            else
            {
                Console.Write("{0}", ((FileInfo)_fsObjects[index]).Length);
            }
        }
        public void UpdatePanel()
        {
            this._firstObjectIndex = 0;
            this._activeObjectIndex = 0;
            this.Show();
        }
        public void UpdateContent(bool updateList)
        {
            if (updateList)
            {
                this.SetLists();
            }
            this.ClearContent();
            this.PrintContent();
        }
        private void ActivateObject(int index)
        {
            int offsetY = this._activeObjectIndex - this._firstObjectIndex;
            Console.SetCursorPosition(this._left + 1, this._top + offsetY + 1);        
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.DarkBlue;            
            this.PrintObject(index);         
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
        }
        private void DeactivateObject(int index)
        {
            int offsetY = this._activeObjectIndex - this._firstObjectIndex;
            Console.SetCursorPosition(this._left + 1, this._top + offsetY + 1);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;
            this.PrintObject(index);
        }
    }
}
