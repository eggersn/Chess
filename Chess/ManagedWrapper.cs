using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class ManagedWrapper
    {
        [DllImport("RNN_Chess.dll")]
        public static extern IntPtr new_RNN_Chess(int[] Dimensions);

        [DllImport("RNN_Chess.dll")]
        unsafe public static extern int InitializeVariables(IntPtr instance, float** InputWeights, float** RecurrentWeights, float** Biases);

        [DllImport("RNN_Chess.dll")]
        public static extern int InitializeConstants(IntPtr instance, float learningrate);

        [DllImport("RNN_Chess.dll")]
        unsafe public static extern int UpdateWeightMatrices(IntPtr instance, float** InputWeights, float** RecurrentWeights, float** Biases);

        [DllImport("RNN_Chess.dll")]
        unsafe public static extern void UpdateDimensions(IntPtr instance, int[] Dimensions);

        [DllImport("RNN_Chess.dll")]
        unsafe public static extern float* RunRNN(IntPtr instance, float* InputState);

        [DllImport("RNN_Chess.dll")]
        public static extern int ErrorCalculation(IntPtr instance, int color);

        [DllImport("RNN_Chess.dll")]
        public static extern int BackPropagation(IntPtr instance);

        [DllImport("RNN_Chess.dll")]
        public static extern int FreeWorkSpace(IntPtr instance);
    }
}
