using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Collections;
namespace MyEncog
{
    class ado
    {
        SqlConnection conn;
        private int rowcount;
        private int colcount;
        private int[] ids;
        private double[][] data;
        private double[] targets;
        private Hashtable testResults;
        public int RowCount
        {
            get{return rowcount;}
            set{rowcount = value;}
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
            get { return data ; }
            set { data = value; }
        }


        public double[] Targets
        {
            get { return targets; }
            set { targets = value; }
        }

        public SqlConnection Conn
        {
            get { return conn; }
            set { conn = value; }
        }


        public void ConnectToDB() 
        {
            conn = new SqlConnection();
            conn.ConnectionString ="Server=BRIAN-PC;Database=nhdb; Trusted_Connection=yes;";
            conn.Open();
                    
        }
        public int Exectue(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql,conn);
            return cmd.ExecuteNonQuery();
        }
        public void SQLtoTrainData(string sql)
        {
            
            //Read from the database
            SqlCommand cmd = new SqlCommand(sql, conn);
            DataTable dataTable = new DataTable();
            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataTable);
            conn.Close();
            rowcount = dataTable.Rows.Count;
            colcount = dataTable.Columns.Count;

            data = new double[rowcount][];
            ids = new int[rowcount];
            targets = new double[rowcount];
            testResults = new Hashtable();
            TestDetail test;

            int i = 0;
            for (; i < rowcount; i++)
            {

                
                targets[i] = Convert.ToDouble(dataTable.Rows[i][colcount -1]);
                ids[i] = Convert.ToInt32(dataTable.Rows[i][0]);
                data[i] = new double [colcount];
                test = new TestDetail(ids[i], Convert.ToInt32(dataTable.Rows[i][1]), Convert.ToInt32(dataTable.Rows[i][2]), Convert.ToDouble(dataTable.Rows[i][3]));
                testResults.Add(test.RId, test);

                for (int j = 0; j < colcount; j++)
                {
                    data[i][j] = Convert.ToDouble (dataTable.Rows[i][j]);
                }
                
            }
            
        }
        public void SQLToFile(string sql, string filename)
        {

            StreamWriter writer = new StreamWriter(filename);
            SqlCommand cmd = new SqlCommand();
            
            cmd.Connection = conn;
            cmd.CommandText = sql;
            using (SqlDataReader sdr = cmd.ExecuteReader())
                using (writer)
                {
                    while (sdr.Read())
                    {

                        for (int i = 0; i < sdr.FieldCount - 1; i++)
                        {
                            writer.Write(sdr[i].ToString() + ",");
                        }
                        writer.WriteLine(sdr[sdr.FieldCount - 1].ToString());
                    }

                }
            //writer.Close;
        }

        
        public void SQLtoTestData(string sql)
        {

            //Read from the database
            SqlCommand cmd = new SqlCommand(sql, conn);
            DataTable dataTable = new DataTable();
            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataTable);
            conn.Close();
            rowcount = dataTable.Rows.Count;
            colcount = dataTable.Columns.Count;

            data = new double[rowcount][];
            ids = new int[rowcount];
            targets = new double[rowcount];
            int i = 0;
            for (; i < rowcount; i++)
            {
                targets[i] = Convert.ToDouble(dataTable.Rows[i][colcount - 1]);
                ids[i] = Convert.ToInt32(dataTable.Rows[i][0]);
                data[i] = new double[colcount];
                for (int j = 0; j < colcount; j++)
                {
                    data[i][j] = Convert.ToDouble(dataTable.Rows[i][j]);
                }

            }

        }
    }
}
