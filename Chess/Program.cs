using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace Chess
{
    class Program
    {
        public static int color = 0;
        public static int[] Dimensions = { 1, 64, 64, 1, 0, 0 };
        public static float learningrate = -0.01f;

        public static string folderpath = @"..\..\..\Chess Pieces";
        public static string imagepath = @"..\..\..\Chess Pieces\Screenshot.jpg";
        public static string directory = @"..\..\..\";

        static void Main(string[] args)
        {
            if(!Directory.Exists(directory + "Chess Weights"))
            {
                directory = @"..\..\Chess\";
                folderpath = directory + @"Chess Pieces";
                imagepath = folderpath + @"Screenshot.jpg";
            }

            bool exit = false;

            if(!Directory.Exists(directory + @"\Chess Weights"))
            {
                Directory.CreateDirectory(directory + @"\Chess Weights");
            }

            while(!exit)
            {
                Console.WriteLine("[0] Run Chess\t[1] Setup RNN Network\t [2] Weight Manager\t[3] Exit");

                string input = Console.ReadLine();

                switch(input)
                {
                    case "0":
                        RunChessMenu();
                        break;
                    case "1":
                        SetupMenu();
                        break;
                    case "2":
                        WeightManagerMenu();
                        break;
                    case "3":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Please enter a valid command");
                        Console.ReadKey();
                        break;
                }
                Console.Clear();
            }
        }

        private static void RunChessMenu()
        {
            bool menuReturn = false;

            while(!menuReturn)
            {
                Console.Clear();
                Console.WriteLine("[0] Train Model\t[1] Change Parameters\t[2] Return to Main Menu");

                string input = Console.ReadLine();

                switch(input)
                {
                    case "0":
                        RunChess.RunRNN();
                        break;
                    case "1":
                        ChangeParameters();
                        break;
                    case "2":
                        menuReturn = true;
                        break;
                    default:
                        Console.WriteLine("Please enter a valid command");
                        Console.ReadKey();
                        break;
                }
                if(!menuReturn)
                {
                    Console.WriteLine("Complete.\nPress any key to continue...");
                    Console.ReadKey();
                }
                Console.Clear();
            }
        }

        private static void SetupMenu()
        {
            bool menuReturn = false;
            bool falseInput = false;

            while(!menuReturn)
            {
                Console.Clear();
                Console.WriteLine("[0] Get Chess Games\t[1] Initialize Weights\t[2] Return to Main Menu");

                string input = Console.ReadLine();

                switch(input)
                {
                    case "0":
                        Console.WriteLine("\tChess Games are used from \"http://www.chessgames.com\"");
                        int sampleSize = 0, offset = 0;
                        falseInput = getConsoleInput("\tSample Size: ", ref sampleSize);
                        if(!falseInput)
                            falseInput = getConsoleInput("\tSample Size: ", ref offset);
                        if(!falseInput)
                            DataBase.GetChessGames(sampleSize, offset);
                        break;
                    case "1":
                        WeightManager.InitializeWeights(Dimensions);
                        ChangeParameters();
                        break;
                    case "2":
                        menuReturn = true;
                        break;
                    default:
                        Console.WriteLine("Please enter a valid command");
                        Console.ReadKey();
                        break;
                }
                if(!menuReturn)
                {
                    Console.WriteLine("Complete.\nPress any key to continue...");
                    Console.ReadKey();
                }
                Console.Clear();
            }
        }

        private static void WeightManagerMenu()
        {
            bool menuReturn = false;

            while(!menuReturn)
            {
                Console.Clear();
                Console.WriteLine("[0] Save Weights\t[1] Load Weights\t([2] Delete Weights)\n[3] Return to Main Menu");

                string input = Console.ReadLine();

                switch(input)
                {
                    case "0":
                        getConsoleInput<string>("\tName of Saved Weights: ", ref input);
                        WeightManager.SaveWeights(input);
                        break;
                    case "1":
                        WeightManager.LoadWeights();
                        break;
                    case "2":
                        break;
                    case "3":
                        menuReturn = true;
                        break;
                    default:
                        Console.WriteLine("Please enter a valid command");
                        Console.ReadKey();
                        break;
                }
                if(!menuReturn)
                {
                    Console.WriteLine("Complete.\nPress any key to continue...");
                    Console.ReadKey();
                }
                Console.Clear();
            }
        }

        private static void ChangeParameters()
        {
            bool menuReturn = false;
            bool falseInput = false;

            while(!menuReturn)
            {
                Console.Clear();
                Console.WriteLine("[0] Learning Rate\t[1] Stack Size\t[2] Cancel");

                string input = Console.ReadLine();

                switch(input)
                {
                    case "0":
                        Console.WriteLine("\tCurrent Learning Rate:\t" + learningrate);
                        falseInput = getConsoleInput<float>("\tNew Learning Rate:\t", ref learningrate);
                        break;
                    case "1":
                        Console.WriteLine("\tCurrent Stack Size:\t" + Dimensions[5]);
                        falseInput = getConsoleInput<int>("\tNew Stack Size:\t", ref Dimensions[5]);
                        break;
                    case "2":
                        menuReturn = true;
                        break;
                    default:
                        Console.WriteLine("\nPlease enter a valid command...");
                        Console.ReadKey();
                        break;
                }
                if(falseInput)
                {
                    string[] varType = { "float", "int" };
                    Console.WriteLine("\nConsole Input cannot be parsed to a valid value of type \"" + varType[int.Parse(input)] + "\"");
                }
                if(!menuReturn)
                {
                    Console.WriteLine("\nComplete.\nPress any key to continue...");
                    Console.ReadKey();
                }
                Console.Clear();
            }
        }

        public static bool getConsoleInput<T>(string description, ref T value)
        {
            Console.Write(description);
            try
            {
                value = (T)Convert.ChangeType(Console.ReadLine(), typeof(T)); ;                
            }
            catch
            {
                Console.WriteLine("Invalid Userinput...");
                return true;
            }

            return false;
        }
    }
}
