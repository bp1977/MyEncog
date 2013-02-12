using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Networks.Training.Lma;
using Encog.ML.SVM;
using Encog.ML.SVM.Training;
using Encog.Persist;
namespace MyEncog
{
    class NNet
    {
        private BasicNetwork network;
        private RPROPType rproptype;    
        public int hiddennodes { get;set; }

        private void Save(string filename)
        {
            EncogDirectoryPersistence.SaveObject(new FileInfo(filename), network);
        }
        private void Save(string filename, NetData resultnet) 
        {
            Save(filename);
        }
        public void Load(string filename)
        {
            network = (BasicNetwork)EncogDirectoryPersistence.LoadObject(new FileInfo(filename));
        }
        public void Create(int inputnodes,int hiddennodes)
        {
            network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, inputnodes));
            network.AddLayer(new BasicLayer(new ActivationTANH(), true, hiddennodes));
            network.AddLayer(new BasicLayer(new ActivationLinear(), false, 1));
            network.Structure.FinalizeStructure();
            network.Reset();
            this.hiddennodes = hiddennodes;
        }

        public void Train(NetData traindata, NetData testdata)
        {

            bool stop;
            double sr;
            double bestsr;
            string now;
            int totalepoch;
            int epoch;
            long timeId;
            string netfile;
            timeId = DateTime.Now.Millisecond + DateTime.Now.Year + DateTime.Now.Minute + DateTime.Now.Hour + DateTime.Now.Day;
            //// create training data
            IMLDataSet trainingSet = new BasicMLDataSet(traindata.Data, traindata.Targets);
            
            //// create training data
            IMLDataSet testingSet = new BasicMLDataSet(testdata.Data, testdata.Targets);

            //// train the neural network
            ResilientPropagation train = new ResilientPropagation(network, trainingSet);
            train.RType = rproptype;

            stop = false;
            bestsr = 0;
            totalepoch = 0;
            epoch = 10;
            int i = 0;
            now = DateTime.Now.ToString().Replace(":", "_").Replace("/", "_");
            Console.WriteLine("Begin train. Inputs : " + testdata.ColCount + " Nodes : " + (network.Flat.NeuronCount -testdata.ColCount).ToString()  );

            do
            {
                
                train.Iteration(epoch);
                totalepoch=totalepoch + epoch;
                Console.WriteLine("total epoch" + totalepoch.ToString());
                i = 0; 
                foreach (IMLDataPair pair in testingSet)
                {
                    
                    IMLData output = network.Compute(pair.Input);
                    testdata.Targets[i][0] = output[0];
                    i++;
                }

                sr = testdata.CalcTestResult();

                if (sr > bestsr)
                {
                    bestsr = sr;
                    netfile = "E:/Users/Brian/netfiles/encog/Date_" + now + "_eph_" + totalepoch.ToString() + "_v" + timeId++.ToString() + ".net";
                    testdata.epochs = totalepoch;
                    Save(netfile);
                    Console.WriteLine(netfile);
                }
                else if(sr < bestsr - .005)
                {
                    stop = true;
                }

                //Console.WriteLine("Epoch Error:" + train.Error);

            } while (stop == false);
 



        }
    }
}
