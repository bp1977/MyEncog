using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog;

namespace MyEncog

{
    class Program
    {
        static void Main(string[] args)
        {
            ado ado = new ado();
            ado.ConnectToDB();
            //ado.SQLToFile("select * from courses","C:/Users/Brian/Documents/datasets/courses.txt");

            NetData train = new NetData();
            NetData test = new NetData();
            NNet nnet = new NNet();
            string sqltrain;
            string sqltest;

            sqltrain = SQLNetData.TrainHeader() + SQLNetData.MainInput() + SQLNetData.TrainWhere();
            sqltest = SQLNetData.TestHeader() + SQLNetData.MainInput() + SQLNetData.TestWhere();

            train.SQLtoTrainData(sqltrain,ado.Conn);
            test.SQLtoTestData(sqltest, ado.Conn);

            //Utils.DataTableToFile(train.DataTable,"E:/Users/Brian/datasets/encogtrain.txt");
            ////Utils.DataTableToFile(test.DataTable, "E:/Users/Brian/datasets/encogtest.txt");

            //train.DataTable = Utils.DataTableFromFile("E:/Users/Brian/datasets/encogtrain.txt");

            //Utils.DataTableToFile(train.DataTable, "E:/Users/Brian/datasets/encogtrain2.txt");

            //train.DataTable = Utils.DataTableFromFile("E:/Users/Brian/datasets/encogtrain2.txt");

            //Utils.DataTableToFile(train.DataTable, "E:/Users/Brian/datasets/encogtrain3.txt");

            while (true)
            {

                for (int nodes = 16; nodes < 42; nodes = nodes + 2)
                {
                    nnet.Create(train.ColCount - 2, nodes);
                    nnet.Train(train, test);
                }
            }
            Console.Write("\n\nPress ENTER to continue.");
            Console.ReadLine();
        }
    }
}
