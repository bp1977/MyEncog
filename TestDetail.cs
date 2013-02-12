using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyEncog
{  
        
    class TestDetail
    {
        public TestDetail(int p_idin, int r_idin, int finposin, double oddsin)
        {
            this.PId = p_idin;
            this.RId = r_idin;
            this.FinPos = finposin;
            this.Odds = oddsin;
        }

        private int p_id;
        public int PId
        {
            get { return p_id; }
            set { p_id = value; }
        }
        private int r_id;
        public int RId
        {
            get { return r_id; }
            set { r_id = value; }
        }
        private int finpos;
        public int FinPos
        {
            get { return finpos; }
            set { finpos = value; }
        }
        private double  odds;
        public double  Odds
        {
            get { return odds; }
            set { odds = value; }
        }

        private double pred;
        public double Pred
        {
            get { return pred; }
            set { pred = value; }
        }

      
    }
}
