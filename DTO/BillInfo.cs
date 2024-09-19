using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeManagement.DTO
{
    public class BillInfo
    {
        public BillInfo(int id, int idBill, int idFood, int count)
        {
            this.id = id;
            this.idBill = idBill;
            this.idFood = idFood;
            this.count = count;
        }

        public BillInfo(DataRow dataRow)
        {
            this.id = (int)dataRow["id"];
            this.idBill = (int)dataRow["idBill"];
            this.idFood = (int)dataRow["idFood"];
            this.count = (int)dataRow["count"];
        }

        private int id;
        private int idBill;
        private int idFood;
        private int count;

        public int Id { get; private set; }
        public int IdBill { get; private set; }
        public int IdFood { get; private set; }
        public int Count { get; private set; }
    }
}
