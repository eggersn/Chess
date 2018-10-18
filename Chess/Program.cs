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
        public static int[] Dimensions = { 1, 64, 64, 5, 0, 0 };
        public static float learningrate = -0.001f;

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
                        Console.WriteLine("\t[0] Screenshot\t[1] From File");
                        input = Console.ReadLine();
                        RunChess.GetInputState(folderpath, int.Parse(input));
                        break;
                    case "2":
                        ChessPieces.FindPieces(imagepath, folderpath);
                        break;
                    case "3":
                        WeightManager.InitializeWeights(Dimensions);
                        break;
                    case "4":
                        Console.Write("\tSample Size: ");
                        int sampleSize = int.Parse(Console.ReadLine());
                        Console.Write("\tOffset: ");
                        int offset = int.Parse(Console.ReadLine());
                        DataBase.GetChessGames(sampleSize, offset);
                        break;
                    case "5":
                        Console.WriteLine("\t[0] Black\t[1] White");
                        color = int.Parse(Console.ReadLine());
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
    }
}
