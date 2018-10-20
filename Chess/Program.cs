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
            bool falseInput = false;

            if(!Directory.Exists(directory + @"\Chess Weights"))
            {
                Directory.CreateDirectory(directory + @"\Chess Weights");
            }

            while(!exit)
            {
                Console.WriteLine("[0] Run Chess\t[1] Get Input State\t[2] Get Chess Pieces\n[3] Initialize Weights\t[4] Get Chess Games\t[5] Set Color\t[6] Exit");

                string input = Console.ReadLine();

                switch(input)
                {
                    case "0":
                        falseInput = RunChess.RunRNN();
                        break;
                    case "1":
                        int value = 0;
                        Console.WriteLine("\t[0] Screenshot\t[1] From File");
                        falseInput = getConsoleInput("\t[0] Screenshot\t[1] From File", ref value);
                        if(!falseInput)
                            RunChess.GetInputState(folderpath, value);
                        break;
                    case "2":
                        ChessPieces.FindPieces(imagepath, folderpath);
                        break;
                    case "3":
                        WeightManager.InitializeWeights(Dimensions);
                        break;
                    case "4":
                        int sampleSize = 0, offset = 0;
                        falseInput = getConsoleInput("\tSample Size: ", ref sampleSize);
                        if(!falseInput)
                            falseInput = getConsoleInput("\tSample Size: ", ref offset);
                        if(!falseInput)
                            DataBase.GetChessGames(sampleSize, offset);
                        break;
                    case "5":
                        int color = 0;
                        falseInput =getConsoleInput("\t[0] Black\t[1] White", ref color);
                        break;
                    case "6":
                        exit = true;
                        break;
                }
                if(!exit)
                {
                    Console.WriteLine("Complete.\nPress any key to continue...");
                    Console.ReadKey();
                }
                Console.Clear();
            }
        }

        public static bool getConsoleInput(string description, ref int value)
        {
            Console.Write(description);
            try
            {
                value = int.Parse(Console.ReadLine());
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
