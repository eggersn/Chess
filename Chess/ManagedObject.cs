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

        unsafe public int BackPropagation(int color)
        {
            return ManagedWrapper.BackPropagation(RNN_Chess_instance, color);
        }

        unsafe public float[] RunRNN(float[] InputState, int size)
        {
            fixed(float* ISPtr = InputState)
            {
                float* results =  ManagedWrapper.RunRNN(RNN_Chess_instance, ISPtr);

                float[] res = new float[size];

                for(int i = 0; i < size; i++)
                {
                    res[i] = *(ISPtr + i);
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
