using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Variables
    {
        public static int winningColor;
        public static int stateCount;
        public static float[][] InputState;
        public static float[] Results;

        public static float[][] InputWeights;
        public static float[][] HiddenWeights;
        public static float[][] Biases;

        public static void ResetVariables()
        {
            winningColor = 0;
            stateCount = 0;
            InputState = null;
            Results = null;
        }
    }
}
