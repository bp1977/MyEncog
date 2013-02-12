using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
namespace MyEncog
{
    class Utils
    {
        public static DataTable DataTableFromFile(string filename)
        {

            DataTable dt = new DataTable();
            string[] columns;
            double[] nums;
            string line;
            bool colsCreated;

            TextReader tr = new StreamReader(filename, Encoding.Default);
            colsCreated = false;

            while ((line = tr.ReadLine()) != null)
            {
                columns = line.Split(new string[] { "," }, StringSplitOptions.None);
                nums = new double[columns.Length];
                if(colsCreated == false )
                {
                    for (int i = 0;i<columns.Length;i++)
                    {
                        dt.Columns.Add(i.ToString(),typeof(Double));
                    }
                    colsCreated = true;
                }

                DataRow dr = dt.NewRow();

                for (int i = 0;i<columns.Length;i++)
                {
                    dr[i] = Convert.ToDouble((columns[i] == ""?"0":columns[i]));

                }

                dt.Rows.Add(dr);
            }
            return dt;
        }

        public static void DataTableToFile(DataTable dt, string filename)
        {
            
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            StreamWriter writer = new StreamWriter(filename);
            
            using (writer)
            {
                for (int row = 0;row < dt.Rows.Count;row++)
                {
                    for (int col = 0; col < dt.Columns.Count -1; col++)
                    {
                        writer.Write((dt.Rows[row][col].ToString() == ""?"0":dt.Rows[row][col].ToString())  + ",");
                    }
                    writer.WriteLine((dt.Rows[row][dt.Columns.Count - 1].ToString() == "" ? "0" : dt.Rows[row][dt.Columns.Count - 1].ToString()));
                }

            }
        }

    }
}
