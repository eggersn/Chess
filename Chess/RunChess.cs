using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Chess
{
    class RunChess
    {
        private static ManagedObject RNN_Chess;
        private static int[] ChessPieces_Count = { 2, 2, 2, 1, 1, 8 };

        public static bool RunRNN()
        {
            int offset = 0;
            bool[] validInputs = new bool[3];

            validInputs[0] = Program.getConsoleInput("\tOffset: ", ref offset);
            validInputs[1] = Program.getConsoleInput("\tTraining Samples: ", ref Program.Dimensions[5]);
            validInputs[2] = Program.getConsoleInput("\tEpochs: ", ref Program.Dimensions[4]);

            for(int i = 0; i < 3; i++)
            {
                if(validInputs[i])
                {
                    return true;
                }
            }

            if(RNN_Chess == null)
            {
                //DataBase.ConvertdefaultInput();
                DataBase.GetWorkspaceSize(offset, Program.Dimensions[5]);
                if(Variables.InputWeights == null)
                {
                    WeightManager.WeightReader();
                }
                RNN_Chess = new ManagedObject(Program.Dimensions);
                RNN_Chess.InitializeVariables(Variables.InputWeights, Variables.HiddenWeights, Variables.Biases);
                RNN_Chess.InitializeConstants(Program.learningrate);
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for(int o = 0; o < Program.Dimensions[4]; o++)
            {
                for(int i = 0; i < Program.Dimensions[5]; i++)
                {
                    DataBase.ConvertChessNotation(offset + i);
                    RNN_Chess.UpdateDimensions(Program.Dimensions);

                    for(int j = 0; j < Program.Dimensions[0]; j++)
                    {
                        Variables.Results = RNN_Chess.RunRNN(Variables.InputState[j], Program.Dimensions[1]);
                    }

                    for(int j = 0; j < Program.Dimensions[0]; j++)
                    {
                        RNN_Chess.ErrorCalculation(Variables.winningColor);
                    }

                    RNN_Chess.BackPropagation();
                }
                RNN_Chess.UpdateWeightMatrices(Variables.InputWeights, Variables.HiddenWeights, Variables.Biases);
            }

            WeightManager.WeightWriter();
            RNN_Chess.FreeWorkSpace();

            sw.Stop();

            Console.WriteLine("Time spent:\t\t" + (float)sw.Elapsed.TotalMilliseconds / 1000 + " s");
            Console.WriteLine("Time per Game:\t\t" + (float)sw.Elapsed.TotalMilliseconds / (1000 * Program.Dimensions[4] * Program.Dimensions[5]) + " s\n");

            RNN_Chess = null;
            Variables.ResetVariables();

            return false;
        }

        public static void GetInputState(string folderpath, int input)
        {
            Bitmap bmp;
            Graphics gr;

            int[] Available_ChessPieces = new int[12];
            string[,] cat = new string[8, 8];
            byte[] buffer = new byte[64];
            int dx = 0;
            int dy = 0;

            if(input == 0)
            {
                bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                gr = Graphics.FromImage(bmp);
                gr.CopyFromScreen(0, 0, 0, 0, bmp.Size);
            }
            else
            {
                bmp = new Bitmap(Program.directory + @"TempBitmap.PNG");
                gr = Graphics.FromImage(bmp);
            }

            for(int i = 0; i < 8; i++)
            {
                if(i > 2)
                {
                    dx = 1;
                    if(i > 6)
                    {
                        dx = 2;
                    }
                }
                else
                {
                    dx = 0;
                }

                for(int j = 0; j < 8; j++)
                {
                    if(j > 2)
                    {
                        dy = 1;
                        if(j > 6)
                        {
                            dy = 2;
                        }
                    }
                    else
                    {
                        dy = 0;
                    }

                    cat[i, j] = ChessPieces.CompareChessPiece(598 + 91 * i - dx, 179 + 91 * j - dy, bmp, folderpath);

                    if(cat[i, j].Contains(","))
                    {
                        int color = int.Parse(cat[i, j].Split(',')[0]);
                        int category = int.Parse(cat[i, j].Split(',')[1]);
                        int pieces = 0;

                        for(int k = 0; k < category; k++)
                        {
                            pieces += ChessPieces_Count[k];
                        }
                        pieces = (pieces + Available_ChessPieces[color * 6 + category]) * 2;

                        buffer[color * 32 + pieces] = byte.Parse(j.ToString());
                        buffer[color * 32 + pieces + 1] = byte.Parse(i.ToString());

                        Available_ChessPieces[color * 6 + category]++;
                    }
                }
            }

            File.WriteAllBytes(Program.directory + @"Chess Weights\InputState.txt", buffer);
        }
    }
}
