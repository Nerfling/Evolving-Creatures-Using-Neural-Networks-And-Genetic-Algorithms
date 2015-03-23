using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Savant
{
    class NeuralNetwork
    {
        const double e = 2.7183;

        public double Fitness;

        public struct Dendrite
        {
            public double Weight;
        }

        public struct Neuron
        {
            public Dendrite[] Dendrites;
            public int DendriteCount;
            public double Bias;
            public double Value;
            public double Delta;
        }

        public struct Layer
        {
            public Neuron[] Neurons;
            public int NeuronCount;
        }

        public struct NN
        {
            public Layer[] Layers;
            public int LayerCount;
            public double LearningRate;
        }

        public NN Network;

        public NeuralNetwork(double LearningRate, int InputCount, int HiddenLayerCount, int OutputCount)
        {
            double[] ArrayOfLayers = new double[3];
            ArrayOfLayers[0] = InputCount;
            ArrayOfLayers[1] = HiddenLayerCount;
            ArrayOfLayers[2] = OutputCount;
            Random random = new Random();

            Fitness = 0;

            Network.LayerCount = ArrayOfLayers.Length;
            if (Network.LayerCount < 2)
                return;

            Network.LearningRate = LearningRate;

            Network.Layers = new Layer[Network.LayerCount];

            for (int i = 0; i < Network.LayerCount; i++)
            {
                Network.Layers[i].NeuronCount = (int)ArrayOfLayers[i];
                Network.Layers[i].Neurons = new Neuron[Network.Layers[i].NeuronCount];
                for (int j = 0; j < ArrayOfLayers[i]; j++)
                {
                    if (i == ArrayOfLayers.Length - 1)
                    {
                        Network.Layers[i].Neurons[j].Bias = GetRand();
                        Network.Layers[i].Neurons[j].DendriteCount = (int)ArrayOfLayers[i - 1];
                        Network.Layers[i].Neurons[j].Dendrites = new Dendrite[Network.Layers[i].Neurons[j].DendriteCount];
                        for (int k = 0; k < ArrayOfLayers[i - 1]; k++)
                        {
                            Network.Layers[i].Neurons[j].Dendrites[k].Weight = GetRand();
                        }
                    }
                    else if (i == 0)
                    {
                        //Only init Dendrites, not Bias
                    }
                    else
                    {
                        Network.Layers[i].Neurons[j].Bias = GetRand();
                        Network.Layers[i].Neurons[j].DendriteCount = (int)ArrayOfLayers[i - 1];
                        Network.Layers[i].Neurons[j].Dendrites = new Dendrite[Network.Layers[i].Neurons[j].DendriteCount];
                        for (int k = 0; k < ArrayOfLayers[i - 1]; k++)
                        {
                            Network.Layers[i].Neurons[j].Dendrites[k].Weight = GetRand();
                        }
                    }
                }
            }
        }

        public double[] Run(double[] ArrayOfInputs)
        {
            if (ArrayOfInputs.Length != Network.Layers[0].NeuronCount)
                return null;

            for (int i = 0; i < Network.LayerCount; i++)
            {
                for (int j = 0; j < Network.Layers[i].NeuronCount; j++)
                {
                    if (i == 0)
                    {
                        Network.Layers[i].Neurons[j].Value = ArrayOfInputs[j];
                    }
                    else
                    {
                        Network.Layers[i].Neurons[j].Value = 0;
                        for (int k = 0; k < Network.Layers[i - 1].NeuronCount; k++)
                        {
                            Network.Layers[i].Neurons[j].Value = Network.Layers[i].Neurons[j].Value + Network.Layers[i - 1].Neurons[k].Value * Network.Layers[i].Neurons[j].Dendrites[k].Weight;
                        }
                        Network.Layers[i].Neurons[j].Value = Activation(Network.Layers[i].Neurons[j].Value); // + Network.Layers[i].Neurons[j].Bias);
                    }
                }
            }

            double[] outputresult = new double[Network.Layers[Network.LayerCount - 1].NeuronCount];
            for (int i = 0; i < Network.Layers[Network.LayerCount - 1].NeuronCount; i++)
            {
                outputresult[i] = (Network.Layers[Network.LayerCount - 1].Neurons[i].Value);
                if (outputresult[i] > 1)
                    outputresult[i] = 1;
                if (outputresult[i] < -1)
                    outputresult[i] = -1;
            }

            return outputresult;
        }

        public double Activation(double Value)
        {
            return BipolarSigmoid(Value);
        }

        public void Randomize()
        {
            Random random = new Random();

            for (int i = 0; i < Network.LayerCount; i++)
            {
                for (int j = 0; j < Network.Layers[i].NeuronCount; j++)
                {
                    if (i == 2)
                    {
                        Network.Layers[i].Neurons[j].Bias = GetRand();
                        for (int k = 0; k < Network.Layers[i - 1].NeuronCount; k++)
                        {
                            Network.Layers[i].Neurons[j].Dendrites[k].Weight = GetRand();
                        }
                    }
                    else if (i == 0)
                    {
                        //Only init Dendrites, not Bias
                    }
                    else
                    {
                        Network.Layers[i].Neurons[j].Bias = GetRand();
                        for (int k = 0; k < Network.Layers[i - 1].NeuronCount; k++)
                        {
                            Network.Layers[i].Neurons[j].Dendrites[k].Weight = GetRand();
                        }
                    }
                }
            }
        }

        public double[] GetWeights()
        {
            double[] Weights = new double[GetNrOfDendrites()];

            int NrOfDendrites = 0;
            for (int i = 0; i < Network.LayerCount; i++)
            {
                for (int j = 0; j < Network.Layers[i].NeuronCount; j++)
                {
                    if (i == 2)
                    {
                        for (int k = 0; k < Network.Layers[i - 1].NeuronCount; k++)
                        {
                            Weights[NrOfDendrites] = Network.Layers[i].Neurons[j].Dendrites[k].Weight;
                            NrOfDendrites++;
                        }
                    }
                    else if (i == 0)
                    {
                        //Only init Dendrites, not Bias
                    }
                    else
                    {
                        for (int k = 0; k < Network.Layers[i - 1].NeuronCount; k++)
                        {
                            Weights[NrOfDendrites] = Network.Layers[i].Neurons[j].Dendrites[k].Weight;
                            NrOfDendrites++;
                        }
                    }

                }
            }

            return Weights;
        }

        public void SetWeights(double[] Weights)
        {
            int NrOfDendrites = 0;
            for (int i = 0; i < Network.LayerCount; i++)
            {
                for (int j = 0; j < Network.Layers[i].NeuronCount; j++)
                {
                    if (i == 2)
                    {
                        for (int k = 0; k < Network.Layers[i - 1].NeuronCount; k++)
                        {
                            Network.Layers[i].Neurons[j].Dendrites[k].Weight = Weights[NrOfDendrites];
                            NrOfDendrites++;
                        }
                    }
                    else if (i == 0)
                    {
                        //Only init Dendrites, not Bias
                    }
                    else
                    {
                        for (int k = 0; k < Network.Layers[i - 1].NeuronCount; k++)
                        {
                            Network.Layers[i].Neurons[j].Dendrites[k].Weight = Weights[NrOfDendrites];
                            NrOfDendrites++;
                        }
                    }

                }
            }
        }

        public int GetNrOfDendrites()
        {
            int NrOfDendrites = 0;
            for (int i = 0; i < Network.LayerCount; i++)
            {
                for (int j = 0; j < Network.Layers[i].NeuronCount; j++)
                {
                    if (i == 2)
                    {
                        for (int k = 0; k < Network.Layers[i - 1].NeuronCount; k++)
                        {
                            NrOfDendrites++;
                        }
                    }
                    else if (i == 0)
                    {
                        //Only init Dendrites, not Bias
                    }
                    else
                    {
                        for (int k = 0; k < Network.Layers[i - 1].NeuronCount; k++)
                        {
                            NrOfDendrites++;
                        }
                    }

                }
            }
            return NrOfDendrites;
        }

        public void SetBias(double[] Bias)
        {
            int BiasCount = 0;
            for (int i = 0; i < Network.LayerCount; i++)
            {
                for (int j = 0; j < Network.Layers[i].NeuronCount; j++)
                {
                    if (i == Network.LayerCount - 1)
                    {
                        Network.Layers[i].Neurons[j].Bias = Bias[BiasCount];
                        BiasCount++;
                    }
                    else if (i == 0)
                    {
                        //Only init Dendrites, not Bias
                    }
                    else
                    {
                        Network.Layers[i].Neurons[j].Bias = Bias[BiasCount];
                        BiasCount++;
                    }
                }
            }
        }

        public double[] GetBias()
        {
            int BiasCount = 0;
            double[] Bias = new double[GetBiasCount()];
            for (int i = 0; i < Network.LayerCount; i++)
            {
                for (int j = 0; j < Network.Layers[i].NeuronCount; j++)
                {
                    if (i == Network.LayerCount - 1)
                    {
                        Bias[BiasCount] = Network.Layers[i].Neurons[j].Bias;
                        BiasCount++;
                    }
                    else if (i == 0)
                    {
                        //Only init Dendrites, not Bias
                    }
                    else
                    {
                        Bias[BiasCount] = Network.Layers[i].Neurons[j].Bias;
                        BiasCount++;
                    }
                }
            }
            return Bias;
        }

        public int GetBiasCount()
        {
            int BiasCount = 0;
            for (int i = 0; i < Network.LayerCount; i++)
            {
                for (int j = 0; j < Network.Layers[i].NeuronCount; j++)
                {
                    if (i == Network.LayerCount - 1)
                    {
                        BiasCount++;
                    }
                    else if (i == 0)
                    {
                        //Only init Dendrites, not Bias
                    }
                    else
                    {
                        BiasCount++;
                    }
                }
            }
            return BiasCount;
        }

        public double GetRand()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            return (random.NextDouble() * 2) - 1;
        }

        public double BipolarSigmoid(double x)
        {
            return (1 / (1 + Math.Exp(x * -1)));
        }
    }
}
