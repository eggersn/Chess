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

            for (int i = 0; i < 4; i++)
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

            for (int i = 0; i < size; i++)
            {
                weights[i] = BitConverter.ToSingle(buffer, i * 4);
            }

            return weights;
        }

        public static void InitializeWeights(int[] Dimensions)
        {
            for (int i = 0; i < 3; i++)
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
            for (int i = 0; i < 4; i++)
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

            for (int i = 0; i < Weights.Length; i++)
            {
                temp = BitConverter.GetBytes(Weights[i]);

                for (int j = 0; j < 4; j++)
                {
                    buffer[i * 4 + j] = temp[j];
                }
            }

            File.WriteAllBytes(path, buffer);
        }

        public static void WeightInitialization(int[] dim, string path, int fac, float eX, float varX)
        {
            if (!File.Exists(path))
            {
                FileStream f = File.Create(path);
                f.Close();
            }

            Random r = new Random(DateTime.Now.Millisecond);

            float variance = (float)fac / ((dim[2] * dim[1] * dim[3] * ((float)Math.Pow(eX, 2) / varX + 1)));
            byte[] buffer = new byte[dim[0] * dim[2] * dim[1] * dim[3] * 4];


            for (int i = 0; i < dim[0]; i++)
            {
                for (int j = 0; j < dim[2]; j++)
                {
                    for (int k = 0; k < dim[1]; k++)
                    {
                        for (int l = 0; l < dim[3]; l++)
                        {
                            float value = ((float)r.NextDouble() - 0.5f) * 2 * variance;
                            byte[] temp = BitConverter.GetBytes(value);

                            for (int m = 0; m < 4; m++)
                            {
                                buffer[(i * dim[2] * (int)Math.Pow(dim[1], 2) + j * dim[1] * dim[3] + k * dim[3] + l) * 4 + m] = temp[m];
                            }
                        }
                    }
                }
            }

            File.WriteAllBytes(path, buffer);
        }
    }
}
