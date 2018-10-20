using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Chess
{
    class WeightManager
    {
        static string path = Program.directory + @"Chess Weights";
        static string[] gatenames = { "Forget", "Input", "Output", "Cell" };


        public static void WeightReader()
        {
            Variables.InputWeights = new float[4][];
            Variables.HiddenWeights = new float[4][];
            Variables.Biases = new float[4][];

            for(int i = 0; i < 4; i++)
            {
                Variables.InputWeights[i] = ConvertBytes(path + "\\Input_" + gatenames[i] + "Gate.txt", i, Program.Dimensions[3] * (int)Math.Pow(Program.Dimensions[1], 2));
                Variables.HiddenWeights[i] = ConvertBytes(path + "\\Hidden_" + gatenames[i] + "Gate.txt", i, Program.Dimensions[3] * (int)Math.Pow(Program.Dimensions[1], 2));
                Variables.Biases[i] = ConvertBytes(path + "\\Bias_" + gatenames[i] + "Gate.txt", i, Program.Dimensions[3] * Program.Dimensions[1]);
            }
        }

        private static float[] ConvertBytes(string path, int count, int size)
        {
            byte[] buffer = File.ReadAllBytes(path);
            float[] weights = new float[size];

            for(int i = 0; i < size; i++)
            {
                weights[i] = BitConverter.ToSingle(buffer, i * 4);
            }

            return weights;
        }

        public static void InitializeWeights(int[] Dimensions)
        {
            for(int i = 0; i < 3; i++)
            {
                WeightInitialization(new int[] { Dimensions[3], Dimensions[1], 1, Dimensions[1] }, path + "\\Hidden_" + gatenames[i] + "Gate.txt", 1, 0, (float)Math.Sqrt((double)6 / (Dimensions[3] * Math.Pow(Dimensions[1], 2))));
                WeightInitialization(new int[] { Dimensions[3], Dimensions[1], 1, Dimensions[2] }, path + "\\Input_" + gatenames[i] + "Gate.txt", 1, 0, (float)Math.Sqrt((double)6 / (Dimensions[3] * Dimensions[1] * Dimensions[2])));
                WeightInitialization(new int[] { Dimensions[3] * Dimensions[1], 1, 1, 1 }, path + "\\Bias_" + gatenames[i] + "Gate.txt", 1, 0, (float)Math.Sqrt((double)6 / (Dimensions[3] * Dimensions[1])));
            }

            WeightInitialization(new int[] { Dimensions[3], Dimensions[1], 1, Dimensions[1] }, path + "\\Hidden_" + gatenames[3] + "Gate.txt", 1, 0, 4 * (float)Math.Sqrt((double)6 / (Dimensions[3] * Math.Pow(Dimensions[1], 2))));
            WeightInitialization(new int[] { Dimensions[3], Dimensions[1], 1, Dimensions[2] }, path + "\\Input_" + gatenames[3] + "Gate.txt", 1, 0, 4 * (float)Math.Sqrt((double)6 / (Dimensions[3] * Dimensions[1] * Dimensions[2])));
            WeightInitialization(new int[] { Dimensions[3] * Dimensions[1], 1, 1, 1 }, path + "\\Bias_" + gatenames[3] + "Gate.txt", 1, 0, 4 * (float)Math.Sqrt((double)6 / (Dimensions[3] * Dimensions[1])));
        }

        public static void WeightWriter()
        {
            for(int i = 0; i < 4; i++)
            {
                WriteGateWeights(Variables.HiddenWeights[i], path + "\\Hidden_" + gatenames[i] + "Gate.txt");
                WriteGateWeights(Variables.InputWeights[i], path + "\\Input_" + gatenames[i] + "Gate.txt");
                WriteGateWeights(Variables.Biases[i], path + "\\Bias_" + gatenames[i] + "Gate.txt");
            }
        }

        private static void WriteGateWeights(float[] Weights, string path)
        {
            byte[] buffer = new byte[Weights.Length * 4];
            byte[] temp = new byte[4];

            for(int i = 0; i < Weights.Length; i++)
            {
                temp = BitConverter.GetBytes(Weights[i]);

                for(int j = 0; j < 4; j++)
                {
                    buffer[i * 4 + j] = temp[j];
                }
            }

            File.WriteAllBytes(path, buffer);
        }

        public static void WeightInitialization(int[] dim, string path, int fac, float eX, float varX)
        {
            if(!File.Exists(path))
            {
                FileStream f = File.Create(path);
                f.Close();
            }

            Random r = new Random(DateTime.Now.Millisecond);

            float variance = (float)fac / ((dim[2] * dim[1] * dim[3] * ((float)Math.Pow(eX, 2) / varX + 1)));
            byte[] buffer = new byte[dim[0] * dim[2] * dim[1] * dim[3] * 4];


            for(int i = 0; i < dim[0]; i++)
            {
                for(int j = 0; j < dim[2]; j++)
                {
                    for(int k = 0; k < dim[1]; k++)
                    {
                        for(int l = 0; l < dim[3]; l++)
                        {
                            float value = ((float)r.NextDouble() - 0.5f) * 2 * variance;
                            byte[] temp = BitConverter.GetBytes(value);

                            for(int m = 0; m < 4; m++)
                            {
                                buffer[(i * dim[2] * (int)Math.Pow(dim[1], 2) + j * dim[1] * dim[3] + k * dim[3] + l) * 4 + m] = temp[m];
                            }
                        }
                    }
                }
            }

            File.WriteAllBytes(path, buffer);
        }

        public static void SaveWeights(string name)
        {
            bool existing = false;
            bool falseInput = true;

            if(Directory.Exists(Program.directory + "\\Saved Weights\\" + name))
            {
                while(falseInput)
                {
                    falseInput = false;
                    Console.WriteLine("\tDirectory is already existing...\n\tDo you want to overwrite it? (y|n)");
                    string input = Console.ReadLine();

                    if(input == "n")
                    {
                        Console.WriteLine("Aborting,\nPress any key to continue...");
                        Console.ReadLine();

                        existing = true;
                    }
                    else if(input != "y")
                    {
                        Console.WriteLine("Invalid Userinput");
                        falseInput = true;
                    }
                }
            }

            if(!existing)
            {
                string[] filePaths = Directory.GetFiles(path);
                Directory.CreateDirectory(Program.directory + "\\Saved Weights\\" + name);

                for(int i = 0; i < filePaths.Length; i++)
                {
                    string filename = filePaths[i].Split('\\')[filePaths[i].Split('\\').Length - 1];
                    File.Copy(filePaths[i], Program.directory + "\\Saved Weights\\" + name + "\\" + filename);
                }
            }
        }

        public static void LoadWeights()
        {
            string[] directoryPaths = Directory.GetDirectories(Program.directory + "\\Saved Weights");
            string[] directoryNames = new string[directoryPaths.Length];
            string tempPath = Program.directory + "\\Saved weights\\Temporary";
            bool available = false;

            while(!available)
            {
                Console.WriteLine("\nAvailable Saved Weights:");

                for(int i = 0; i < directoryPaths.Length; i++)
                {
                    directoryNames[i] = directoryPaths[i].Split('\\')[directoryPaths[i].Split('\\').Length - 1];
                    Console.WriteLine("- " + directoryNames[i]);
                }

                Console.WriteLine("\nType in the name of the Saved Weights you want to load: ");
                string name = Console.ReadLine();

                for(int i = 0; i < directoryNames.Length; i++)
                {
                    if(name == directoryNames[i])
                    {
                        string[] files = Directory.GetFiles(directoryPaths[i]);
                        string[] destFiles = Directory.GetFiles(path);

                        if(!Directory.Exists(tempPath))
                        {
                            Directory.CreateDirectory(tempPath);
                        }

                        for(int j = 0; j < files.Length; j++)
                        {
                            string tempFile = tempPath + "\\" + files[j].Split('\\')[files[j].Split('\\').Length - 1];
                            if(File.Exists(tempFile))
                            {
                                File.Delete(tempFile);
                            }
                            File.Move(destFiles[j], tempFile);
                            File.Copy(files[j], destFiles[j]);
                        }

                        available = true;
                        Console.WriteLine("Overwritten files are moved to temporary folder");
                    }
                }

                if(!available)
                {
                    Console.WriteLine("Saved Weights {0} are not available", name);
                    Console.ReadKey();
                    Console.Clear();
                    Console.WriteLine("[0] Save Weights\t[1] Load Weights\t[2] Delete Weights\n[3] Return to Main Menu\n1");
                }
            }
        }
    }
}
