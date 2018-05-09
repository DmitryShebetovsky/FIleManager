using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace FileManager
{
    internal class FileManager
    {
        public static int HeightKeys = 3;
        public static int BottomOffset = 2;
        public event OnKey KeyPress;
        readonly List<FilePanel> _panels = new List<FilePanel>();
        private int _activePanelIndex;      
        static FileManager()
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(120, 25);
            Console.SetBufferSize(120, 25);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        public FileManager()
        {
            FilePanel filePanel = new FilePanel();
            filePanel.Top = 0;
            filePanel.Left = 0;
            this._panels.Add(filePanel);
            filePanel = new FilePanel();
            //filePanel.Top = FilePanel.PanelHeight;
            filePanel.Top = 0;
            filePanel.Left = 60;
            this._panels.Add(filePanel);
            _activePanelIndex = 0;
            this._panels[this._activePanelIndex].Active = true;
            KeyPress += this._panels[this._activePanelIndex].KeyboardProcessing;
           foreach (FilePanel fp in _panels)
            {
                fp.Show();
            }
            this.ShowKeys();
        }
        public void Explore()
        {
            bool exit = false;
            while (!exit)
            {
                if (Console.KeyAvailable)
                {
                    this.ClearMessage();

                    ConsoleKeyInfo userKey = Console.ReadKey(true);
                    switch (userKey.Key)
                    {
                        case ConsoleKey.Tab:
                            this.ChangeActivePanel();
                            break;
                        case ConsoleKey.Enter:
                            this.ChangeDirectoryOrRunProcess();
                            break;
                        case ConsoleKey.F1:
                            this.ViewFile();
                            break;
                        case ConsoleKey.F2:
                            this.FindFile();
                            break;
                        case ConsoleKey.F3:
                            this.Copy();
                            break;
                        case ConsoleKey.F4:
                            this.Move();
                            break;
                        case ConsoleKey.F5:
                            this.CreateDirectory();
                            break;
                        case ConsoleKey.F6:
                            this.Rename();
                            break;
                        case ConsoleKey.F7:
                            this.Delete();
                            break;
                        case ConsoleKey.Escape:
                            exit = true;
                            Console.ResetColor();
                            Console.Clear();
                            break;
                        case ConsoleKey.DownArrow:
                            goto case ConsoleKey.PageUp;
                        case ConsoleKey.UpArrow:
                            goto case ConsoleKey.PageUp;
                        case ConsoleKey.End:
                            goto case ConsoleKey.PageUp;
                        case ConsoleKey.Home:
                            goto case ConsoleKey.PageUp;
                        case ConsoleKey.PageDown:
                            goto case ConsoleKey.PageUp;
                        case ConsoleKey.PageUp:
                            this.KeyPress(userKey);
                            break;
                    }
                }
            }
        }
        private string AksName(string message)
        {
            string name;
            Console.CursorVisible = true;
            do
            {
                this.ClearMessage();
                this.ShowMessage(message);
                name = Console.ReadLine();
            } while (name.Length == 0);
            Console.CursorVisible = false;
            this.ClearMessage();
            return name;
        }
        private void Copy()
        {
            foreach (FilePanel panel in _panels)
            {
                if (panel.IsDiscs)
                    return;
            }
            if (this._panels[0].Path == this._panels[1].Path)
                return;
            try
            {
                string destPath = this._activePanelIndex == 0 ? this._panels[1].Path : this._panels[0].Path;
                FileSystemInfo fileObject = this._panels[this._activePanelIndex].GetActiveObject();
                FileInfo currentFile = fileObject as FileInfo;
                if (currentFile != null)
                {
                    string fileName = currentFile.Name;
                    string destName = Path.Combine(destPath, fileName);
                    File.Copy(currentFile.FullName, destName, true);
                }
                else
                {
                    string currentDir = ((DirectoryInfo)fileObject).FullName;
                    string destDir = Path.Combine(destPath, ((DirectoryInfo)fileObject).Name);
                    CopyDirectory(currentDir, destDir);
                }
                this.RefreshPannels();
            }
            catch (Exception e)
            {
                this.ShowMessage(e.Message);
                return;
            }
        }    
        private void CopyDirectory(string sourceDirName, string destDirName)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                CopyDirectory(subdir.FullName, temppath);
            }
        }
        private void Delete()
        {
            if (this._panels[this._activePanelIndex].IsDiscs)
                return;
            FileSystemInfo fileObject = this._panels[this._activePanelIndex].GetActiveObject();
            try
            {
                if (fileObject is DirectoryInfo)
                    ((DirectoryInfo)fileObject).Delete(true);
                else
                    ((FileInfo)fileObject).Delete();
                this.RefreshPannels();
            }
            catch (Exception e)
            {
                this.ShowMessage(e.Message);
                return;
            }
        }
        private void CreateDirectory()
        {
            if (this._panels[this._activePanelIndex].IsDiscs)
                return;
            string destPath = this._panels[this._activePanelIndex].Path;
            string dirName = this.AksName("Введите имя каталога: ");         
            try
            {
                string dirFullName = Path.Combine(destPath, dirName);
                DirectoryInfo dir = new DirectoryInfo(dirFullName);
                if (!dir.Exists)
                    dir.Create();
                else
                    this.ShowMessage("Каталог с таким именем уже существует");
                this.RefreshPannels();
            }
            catch (Exception e)
            {
                this.ShowMessage(e.Message);
            }
        }
        private void Move()
        {
            foreach (FilePanel panel in _panels)
            {
                if (panel.IsDiscs)
                    return;
            }
            if (this._panels[0].Path == this._panels[1].Path)
                return;
            try
            {
                string destPath = this._activePanelIndex == 0 ? this._panels[1].Path : this._panels[0].Path;
                FileSystemInfo fileObject = this._panels[this._activePanelIndex].GetActiveObject();
                string objectName = fileObject.Name;
                string destName = Path.Combine(destPath, objectName);
                if (fileObject is FileInfo)
                    ((FileInfo)fileObject).MoveTo(destName);
                else
                    ((DirectoryInfo)fileObject).MoveTo(destName);
                this.RefreshPannels();
            }
            catch (Exception e)
            {
                this.ShowMessage(e.Message);
                return;
            }
        }
        private void Rename()
        {
            if (this._panels[this._activePanelIndex].IsDiscs)
                return;
            FileSystemInfo fileObject = this._panels[this._activePanelIndex].GetActiveObject();
            string currentPath = this._panels[this._activePanelIndex].Path;
            string newName = this.AksName("Введите новое имя: ");
            string newFullName = Path.Combine(currentPath, newName);          
            try
            {
                if (fileObject is FileInfo)
                    ((FileInfo)fileObject).MoveTo(newFullName);
                else
                    ((DirectoryInfo)fileObject).MoveTo(newFullName);
                this.RefreshPannels();
            }
            catch (Exception e)
            {
                this.ShowMessage(e.Message);
            }
        }
        private void ViewFile()
        {
            if (this._panels[this._activePanelIndex].IsDiscs)
            {
                return;
            }
            FileSystemInfo fileObject = this._panels[this._activePanelIndex].GetActiveObject();
            if (fileObject is DirectoryInfo || fileObject == null)
            {
                return;
            }
            if (((FileInfo)fileObject).Length == 0)
            {
                this.ShowMessage("Файл пуст");
                return;
            }
            if (((FileInfo)fileObject).Length > 100000000)
            {
                this.ShowMessage("Файл слишком большой для просмотра");
                return;
            }
            this.DrawViewFileFrame(fileObject.Name);
            string fileContent = this.ReadFileToString(fileObject.FullName, Encoding.ASCII);
            int beginPosition = 0;
            int symbolCount = 0;
            bool endOfFile = false;
            bool beginFile = true;
            Stack<int> printSymbols = new Stack<int>();
            symbolCount = this.PrintStingFrame(fileContent, beginPosition);
            printSymbols.Push(symbolCount);
            bool exit = false;
            while (!exit)
            {
                endOfFile = (beginPosition + symbolCount) >= fileContent.Length;
                beginFile = (beginPosition <= 0);

                ConsoleKeyInfo userKey = Console.ReadKey(true);
                switch (userKey.Key)
                {
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                    case ConsoleKey.PageDown:
                        if (!endOfFile)
                        {
                            beginPosition += symbolCount;
                            symbolCount = this.PrintStingFrame(fileContent, beginPosition);
                            printSymbols.Push(symbolCount);
                        }
                        break;
                    case ConsoleKey.PageUp:
                        if (!beginFile)
                        {
                            if (printSymbols.Count != 0)
                            {
                                beginPosition -= printSymbols.Pop();
                                if (beginPosition < 0)
                                {
                                    beginPosition = 0;
                                }
                            }
                            else
                            {
                                beginPosition = 0;
                            }
                            symbolCount = this.PrintStingFrame(fileContent, beginPosition);
                        }
                        break;
                }
            }

            Console.Clear();
            foreach (FilePanel fp in _panels)
            {
                fp.Show();
            }
            this.ShowKeys();
        }

        private void DrawViewFileFrame(string file)
        {
            Console.Clear();
            FileConsole.PrintFrameDoubleLine(0, 0, Console.WindowWidth, Console.WindowHeight - 5, ConsoleColor.Green, ConsoleColor.Black);
            string fileName = String.Format(" {0} ", file);
            FileConsole.PrintString(fileName, (Console.WindowWidth - fileName.Length) / 2, 0, ConsoleColor.Green, ConsoleColor.Black);
            FileConsole.PrintFrameLine(0, Console.WindowHeight - 5, Console.WindowWidth, 4, ConsoleColor.Green, ConsoleColor.Black);
            FileConsole.PrintString("PageDown / PageUp - навигация, Esc - выход", 1, Console.WindowHeight - 4, ConsoleColor.Yellow, ConsoleColor.Black);
        }
        private string ReadFileToString(string fullFileName, Encoding encoding)
        {
            StreamReader SR = new StreamReader(fullFileName, encoding);
            string fileContent = SR.ReadToEnd();
            fileContent = fileContent.Replace("\a", " ").Replace("\b", " ").Replace("\f", " ").Replace("\r", " ").Replace("\v", " ");
            SR.Close();
            return fileContent;
        }
        private int PrintStingFrame(string text, int begin)
        {
            this.ClearFileViewFrame();

            int lastTopCursorPosition = Console.WindowHeight - 7;
            int lastLeftCursorPosition = Console.WindowWidth - 2;

            Console.SetCursorPosition(1, 1);

            int currentTopPosition = Console.CursorTop;
            int currentLeftPosition = Console.CursorLeft;

            int index = begin;
            while (true)
            {
                if (index >= text.Length)
                {
                    break;
                }

                Console.Write(text[index]);
                currentTopPosition = Console.CursorTop;
                currentLeftPosition = Console.CursorLeft;

                if (currentLeftPosition == 0 || currentLeftPosition == lastLeftCursorPosition)
                {
                    Console.CursorLeft = 1;
                }

                if (currentTopPosition == lastTopCursorPosition)
                {
                    break;
                }

                index++;
            }
            return index - begin;
        }

        private void ClearFileViewFrame()
        {
            int lastTopCursorPosition = Console.WindowHeight - 7;
            int lastLeftCursorPosition = Console.WindowWidth - 2;

            for (int row = 1; row < lastTopCursorPosition; row++)
            {
                Console.SetCursorPosition(1, row);
                string space = new String(' ', lastLeftCursorPosition);
                Console.Write(space);
            }
        }

        private void FindFile()
        {
            if (this._panels[this._activePanelIndex].IsDiscs)
            {
                return;
            }

            string fileName = this.AksName("Введите имя: ");

            if (!this._panels[this._activePanelIndex].FindFile(fileName))
            {
                this.ShowMessage("Файл/каталог в текущем каталоге не найден");
            }
        }

        private void RefreshPannels()
        {
            if (this._panels == null || this._panels.Count == 0)
            {
                return;
            }

            foreach (FilePanel panel in _panels)
            {
                if (!panel.IsDiscs)
                {
                    panel.UpdateContent(true);
                }
            }
        }

        private void ChangeActivePanel()
        {
            this._panels[this._activePanelIndex].Active = false;
            KeyPress -= this._panels[this._activePanelIndex].KeyboardProcessing;
            this._panels[this._activePanelIndex].UpdateContent(false);

            this._activePanelIndex++;
            if (this._activePanelIndex >= this._panels.Count)
            {
                this._activePanelIndex = 0;
            }

            this._panels[this._activePanelIndex].Active = true;
            KeyPress += this._panels[this._activePanelIndex].KeyboardProcessing;
            this._panels[this._activePanelIndex].UpdateContent(false);
        }

        private void ChangeDirectoryOrRunProcess()
        {
            FileSystemInfo fsInfo = this._panels[this._activePanelIndex].GetActiveObject();
            if (fsInfo != null)
            {
                if (fsInfo is DirectoryInfo)
                {
                    try
                    {
                        Directory.GetDirectories(fsInfo.FullName);
                    }
                    catch
                    {
                        return;
                    }

                    this._panels[this._activePanelIndex].Path = fsInfo.FullName;
                    this._panels[this._activePanelIndex].SetLists();
                    this._panels[this._activePanelIndex].UpdatePanel();
                }
                else
                {
                    Process.Start(((FileInfo)fsInfo).FullName);
                }
            }
            else
            {
                string currentPath = this._panels[this._activePanelIndex].Path;
                DirectoryInfo currentDirectory = new DirectoryInfo(currentPath);
                DirectoryInfo upLevelDirectory = currentDirectory.Parent;

                if (upLevelDirectory != null)
                {
                    this._panels[this._activePanelIndex].Path = upLevelDirectory.FullName;
                    this._panels[this._activePanelIndex].SetLists();
                    this._panels[this._activePanelIndex].UpdatePanel();
                }

                else
                {
                    this._panels[this._activePanelIndex].SetDiscs();
                    this._panels[this._activePanelIndex].UpdatePanel();
                }
            }
        }

        private void ShowKeys()
        {
            string[] menu = { "F1 Просмотр", " F2 Поиск", "F3 Копия", "F4 Перемещ", "F5 Создать", "F6 Переименов", "F7 Удаление", "ESC Выход" };

            //int cellLeft = this._panels[0].Left;
            //int cellTop = FilePanel.PanelHeight * this._panels.Count;
            //int cellWidth = FilePanel.PanelWidth / menu.Length;
            //int cellHeight = FileManager.HeightKeys;

            int cellLeft = this._panels[0].Left;
            int cellTop = FilePanel.PanelHeight;
            int cellWidth = FilePanel.PanelWidth/4;
            int cellHeight = FileManager.HeightKeys;


            for (int i = 0; i < menu.Length; i++)
            {
                FileConsole.PrintFrameLine(cellLeft + i * cellWidth, cellTop, cellWidth, cellHeight, ConsoleColor.Green, ConsoleColor.Black);
                FileConsole.PrintString(menu[i], cellLeft + i * cellWidth + 1, cellTop + 1, ConsoleColor.Yellow, ConsoleColor.Black);
            }
        }
       
        private void ShowMessage(string message)
        {
            FileConsole.PrintString(message, 0, Console.WindowHeight - BottomOffset, ConsoleColor.Green, ConsoleColor.Black);
        }

        private void ClearMessage()
        {
            FileConsole.PrintString(new String(' ', Console.WindowWidth), 0, Console.WindowHeight - BottomOffset, ConsoleColor.Green, ConsoleColor.Black);
        }
    }
}