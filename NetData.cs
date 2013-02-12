using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace MyEncog
{
    class NetData
    {

        private int rowcount;
        private int colcount;
        private int[] ids;
        private double[][] data;
        private double[][] targets;
        private Hashtable testResults;
        private string testresultdesc;
        private DataTable dataTable;

        public double profit { get; set; }
        public double strikerate { get; set; }
        public double turnover { get; set; }
        public int wins { get; set; }
        public int epochs { get; set; }
        public int hiddennodes { get; set; }
        public string resultfield { get; set; }
                

        public int RowCount
        {
            get { return rowcount; }
            set { rowcount = value; }
        }

        public int ColCount
        {
            get { return colcount; }
            set { colcount = value; }
        }
        public int[] Ids
        {
            get { return ids; }
            set { ids = value; }
        }
        public double[][] Data
        {
            get { return data; }
            set { data = value; }
        }

        public double[][] Targets
        {
            get { return targets; }
            set { targets = value; }
        }

        public Hashtable TestResults
        {
            get { return testResults; }
            set { testResults = value; }
        }

        public string TestResultDesc
        {
            get { return testresultdesc; }
            set { testresultdesc = value; }
        }

        public DataTable DataTable
        {
            get { return dataTable; }
            set { dataTable = value; }
        }


        public void DataTabletoTrainData(DataTable dataTable)
        {

            rowcount = dataTable.Rows.Count;
            colcount = dataTable.Columns.Count;

            data = new double[rowcount][];
            ids = new int[rowcount];
            targets = new double[rowcount][];

            for (int i = 0; i < rowcount; i++)
            {
                targets[i] = new double[1] { Convert.ToDouble(dataTable.Rows[i][colcount - 1]) };
                ids[i] = Convert.ToInt32(dataTable.Rows[i][0]);
                data[i] = new double[colcount - 2];

                for (int j = 0; j < colcount - 2; j++)
                {
                    data[i][j] = Convert.ToDouble((Convert.IsDBNull(dataTable.Rows[i][j + 1]) ? 0 : dataTable.Rows[i][j + 1]));

                }

            }

        }
        public void DataTabletoTestData(DataTable dataTable)
        {
            rowcount = dataTable.Rows.Count;
            colcount = dataTable.Columns.Count;

            TestDetail test;

            data = new double[rowcount][];
            ids = new int[rowcount];
            targets = new double[rowcount][];
            testResults = new Hashtable();

            for (int i = 0; i < rowcount; i++)
            {
                targets[i] = new double[1] { Convert.ToDouble(dataTable.Rows[i][colcount - 1]) };
                ids[i] = Convert.ToInt32(dataTable.Rows[i][0]);
                data[i] = new double[colcount - 5];
                test = new TestDetail(ids[i], Convert.ToInt32(dataTable.Rows[i][1]), Convert.ToInt32(dataTable.Rows[i][2]), Convert.ToDouble(dataTable.Rows[i][3]));
                testResults.Add(test.PId, test);

                for (int j = 0; j < colcount - 5; j++)
                {
                    data[i][j] = Convert.ToDouble((Convert.IsDBNull(dataTable.Rows[i][j + 4]) ? 0 : dataTable.Rows[i][j + 4]));
                }

            }

        }

        public void SQLtoTestData(string sql,SqlConnection conn)
        {

            //Read from the database
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandTimeout = 99999;
            dataTable = new DataTable();
            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataTable);
            DataTabletoTestData(dataTable);
            conn.Close();
            
        }

        public void SQLtoTrainData(string sql, SqlConnection conn)
        {

            //Read from the database
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandTimeout = 99999;
            dataTable = new DataTable();
            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataTable);
            DataTabletoTrainData(dataTable);
            conn.Close();

        }
        public void SaveTest(string netfile) 
        {
       
            string sqlinsert;

            
            string tsql1;
            string tsql2;
            string netsql;
            netsql = "";
            if ( netsql.Length > 4000) 
            {

                tsql1 = netsql.Substring(0, 4000);
                tsql2 = netsql.Substring(4000);
            }
            else
            {
                tsql1 = netsql;
                tsql2 = "";
            }

            tsql1.Replace("'","''");
            tsql2.Replace("'","''");

            sqlinsert = "insert into nnets (NFILEPATH,NDESC,PROFIT,WINS,LOSSES,RUNS,TOVER,SRATE,MINRES,MAXRES,TWHERE,TSQL1,TSQL2,COURSEID,DIST,epochs,hnodes,resultfield,dateadded)";
            sqlinsert = sqlinsert + " values ('" + netfile + "','" + testresultdesc + "'," + profit + "," + wins + ",0,'0'," + turnover + ",";
            sqlinsert = sqlinsert + strikerate + ",0,0,'','" + tsql1 + "','" + tsql2;
            sqlinsert = sqlinsert + "','0',''," + epochs + "," + hiddennodes + ",'" + resultfield + "',getdate())";

            ado ado = new ado();
            ado.ConnectToDB();
            ado.Exectue(sqlinsert);

        }
        public double CalcTestResult() 
        {
            
            Hashtable races = new Hashtable();
            TestDetail race;
            TestDetail td;
            
            //foreach (DictionaryEntry de in testResults)
            for (int i = 0;i < rowcount;i++)
            {

                td = (TestDetail)testResults[ids[i]];
                td.Pred = targets[i][0];
                if (races.ContainsKey(td.RId) == false)
                {
                    races.Add(td.RId, td);

                }
                else
                {
                    race = (TestDetail)races[td.RId];
                    if (td.Pred > race.Pred) {
                        races[td.RId] = td;
                    }
                }
            }
            
            profit = 0;
            strikerate = 0;
            wins = 0;
            TestDetail winners;
            foreach (DictionaryEntry de in races)
            {

                winners = (TestDetail)de.Value;
                if (winners.FinPos == 1)
                {
                    profit = profit + winners.Odds;
                    wins++;

                }
                else 
                {
                    profit--;
                }
            }

            strikerate = Convert.ToDouble( wins) / races.Count;
            turnover = Convert.ToDouble(profit) / races.Count;
            
            testresultdesc = "SR: " + Math.Round(strikerate,3) .ToString() + " Profit: " + Math.Round(profit,3).ToString() + " TO:" + Math.Round(turnover,3).ToString() + " wins:" + wins.ToString() + " races: " + races.Count;
            Console.WriteLine(testresultdesc);
            return strikerate;
        }
    }
}
