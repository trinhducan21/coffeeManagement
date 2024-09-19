using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeManagement.DTO
{
    public class Bill
    {
        public Bill(int id, DateTime? dateCheckin, DateTime? dateCheckOut, int status, int discount = 0)
        {
            this.ID = id;
            this.DateCheckIn = dateCheckin;
            this.DateCheckOut = dateCheckOut;
            this.Status = status;
            this.Discount = discount;
        }

        public Bill(DataRow row)
        {
            this.ID = (int)row["id"];
            this.DateCheckIn = (DateTime?)row["dateCheckin"];

            var dateCheckOutTemp = row["dateCheckOut"];
            if (dateCheckOutTemp.ToString() != "")
                this.DateCheckOut = (DateTime?)dateCheckOutTemp;

            this.Status = (int)row["status"];

            if (row["discount"].ToString() != "") this.Discount = (int)row["discount"];
        }

        private int iD;
        private DateTime? dateCheckIn;
        private DateTime? dateCheckOut;
        private int status;
        private int discount;

        public int ID { get; private set; }
        public DateTime? DateCheckIn { get; private set; }
        public DateTime? DateCheckOut { get; private set; }
        public int Status { get; private set; }
        public int Discount { get; private set; }
    }
}
