using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class ManagedObject
    {
        private IntPtr RNN_Chess_instance;

        public ManagedObject(int[] Dimensions)
        {
            RNN_Chess_instance = ManagedWrapper.new_RNN_Chess(Dimensions);
        }

        unsafe public int InitializeVariables(float[][] InputWeights, float[][] RecurrentWeights, float[][] Biases)
        {
            float*[] InputWeightsPtr = new float*[InputWeights.Length];
            for(int i = 0; i < InputWeights.Length; i++)
            {
                fixed (float* ptr = &InputWeights[i][0])
                {
                    InputWeightsPtr[i] = ptr;
                }
            }

            float*[] RecurrentWeightsPtr = new float*[RecurrentWeights.Length];
            for(int i = 0; i < RecurrentWeights.Length; i++)
            {
                fixed (float* ptr = &RecurrentWeights[i][0])
                {
                    RecurrentWeightsPtr[i] = ptr;
                }
            }

            float*[] BiasesPtr = new float*[Biases.Length];
            for(int i = 0; i < Biases.Length; i++)
            {
                fixed (float* ptr = &Biases[i][0])
                {
                    BiasesPtr[i] = ptr;
                }
            }

            fixed (float** IWPtr = InputWeightsPtr)
            {
                fixed(float** RWPtr = RecurrentWeightsPtr)
                {
                    fixed(float** BPtr = BiasesPtr)
                    {
                        return ManagedWrapper.InitializeVariables(RNN_Chess_instance, IWPtr, RWPtr, BPtr);
                    }
                }
            }
        }

        public int InitializeConstants(float learningrate)
        {
            return ManagedWrapper.InitializeConstants(RNN_Chess_instance, learningrate);
        }

        unsafe public int UpdateWeightMatrices(float[][] InputWeights, float[][] RecurrentWeights, float[][] Biases)
        {
            float*[] InputWeightsPtr = new float*[InputWeights.Length];
            for(int i = 0; i < InputWeights.Length; i++)
            {
                fixed (float* ptr = &InputWeights[i][0])
                {
                    InputWeightsPtr[i] = ptr;
                }
            }

            float*[] RecurrentWeightsPtr = new float*[RecurrentWeights.Length];
            float*[] BiasesPtr = new float*[Biases.Length];

            for(int i = 0; i < RecurrentWeights.Length; i++)
            {
                fixed (float* ptr = &RecurrentWeights[i][0])
                {
                    RecurrentWeightsPtr[i] = ptr;
                }
                fixed (float* ptr = &Biases[i][0])
                {
                    BiasesPtr[i] = ptr;
                }
            }

            fixed (float** IWPtr = InputWeightsPtr)
            {
                fixed (float** RWPtr = RecurrentWeightsPtr)
                {
                    fixed (float** BPtr = BiasesPtr)
                    {
                        return ManagedWrapper.UpdateWeightMatrices(RNN_Chess_instance, IWPtr, RWPtr, BPtr);
                    }
                }
            }
        }

        public void UpdateDimensions(int[] Dimensions)
        {
            ManagedWrapper.UpdateDimensions(RNN_Chess_instance, Dimensions);
        }

        public int ErrorCalculation(int color)
        {
            return ManagedWrapper.ErrorCalculation(RNN_Chess_instance, color);
        }

        public int BackPropagation()
        {
            return ManagedWrapper.BackPropagation(RNN_Chess_instance);
        }

        unsafe public float[] RunRNN(float[] InputState, int size)
        {
            fixed(float* ISPtr = InputState)
            {
                float* results =  ManagedWrapper.RunRNN(RNN_Chess_instance, ISPtr);

                float[] res = new float[size];

                for(int i = 0; i < size; i++)
                {
                    res[i] = *(results + i);
                    res[i] = (res[i] + 0.5f) * 7;
                }

                return res;
            }
        }

        public int FreeWorkSpace()
        {
            return ManagedWrapper.FreeWorkSpace(RNN_Chess_instance);
        }

    }
}
