using CoffeeManagement.DAO;
using CoffeeManagement.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoffeeManagement
{
    public partial class fTableManager : Form
    {
        private Account loginAccount;

        public Account LoginAccount
        {
            get { return loginAccount; }
            set
            {
                loginAccount = value;
                ChangeAccount(loginAccount.Type);
            }
        }

        public fTableManager(Account acc)
        {
            InitializeComponent();

            this.LoginAccount = acc;
            LoadTable();
            LoadCategory();
        }

        void ChangeAccount(int type)
        {
            adminToolStripMenuItem.Enabled = type == 1;
            accountInformationToolStripMenuItem.Text += " (" + loginAccount.DisplayName + ")";
        }

        void f_UpdateAccount(object sender, AccountEvent e)
        {
            accountInformationToolStripMenuItem.Text = "Account Information (" + e.Acc.DisplayName + ")";
        }

        public void LoadTable()
        {
            flpTable.Controls.Clear();
            List<Table> tableList = TableDAO.Instance.LoadTableList();

            foreach (Table item in tableList)
            {
                Button btn = new Button() { Width = TableDAO.TableWidth, Height = TableDAO.TableHeight };
                btn.Text = item.Name + Environment.NewLine + item.Status;
                btn.Click += btn_Click;
                btn.Tag = item;

                switch (item.Status)
                {
                    case "Empty":
                        btn.BackColor = Color.Aqua;
                        break;
                    default:
                        btn.BackColor = Color.Red;
                        break;
                }

                flpTable.Controls.Add(btn);
            }
        }

        void LoadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetListCatagory();
            cbCategory.DataSource = listCategory;
            cbCategory.DisplayMember = "Name";
        }

        void LoadFoodListByCategoryID(int id)
        {
            List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cbFood.DataSource = listFood;
            cbFood.DisplayMember = "Name";
        }

        void btn_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).ID;
            lsvBill.Tag = (sender as Button).Tag;
            ShowBill(tableID);
        }

        void ShowBill(int id)
        {
            lsvBill.Items.Clear();
            List<CoffeeManagement.DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);
            float totalPrice = 0;

            foreach (CoffeeManagement.DTO.Menu item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());
                totalPrice += item.TotalPrice;

                lsvBill.Items.Add(lsvItem);
            }
            // Load discount
            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(id);

            // Có thể có trường hợp không có bill nào được tạo cho bàn đó hoặc cột discount trong bill đó bằng null
            if (idBill != -1)
            {
                int discount = BillDAO.Instance.GetDiscountByID(idBill);
                nmDiscount.Value = discount;
            }
            else
            {
                nmDiscount.Value = 0;
            }

            // Show total price after discount
            totalPrice = totalPrice - (totalPrice / 100) * (float)nmDiscount.Value;
            txbTotalPrice.Text = totalPrice.ToString("c");
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAdmin f = new fAdmin();
            f.loginAccount = loginAccount;
            f.InsertFood += f_InsertFood;
            f.UpdateFood += f_UpdateFood;
            f.DeleteFood += f_DeleteFood;
            f.InsertCategory += f_InsertCategory;
            f.UpdateCategory += f_UpdateCategory;
            f.DeleteCategory += f_DeleteCategory;
            f.InsertTable += f_InsertTable;
            f.UpdateTable += f_UpdateTable;
            f.DeleteTable += f_DeleteTable;
            f.ShowDialog();
        }

        void f_InsertFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).Id);
            if (lsvBill.Tag != null) ShowBill((lsvBill.Tag as Table).ID);
        }

        void f_UpdateFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).Id);
            if (lsvBill.Tag != null) ShowBill((lsvBill.Tag as Table).ID);
        }

        void f_DeleteFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).Id);
            if (lsvBill.Tag != null) ShowBill((lsvBill.Tag as Table).ID);
            LoadTable();
        }

        void f_InsertCategory(object sender, EventArgs e)
        {
            LoadCategory();
            if (lsvBill.Tag != null) ShowBill((lsvBill.Tag as Table).ID);
        }

        void f_UpdateCategory(object sender, EventArgs e)
        {
            LoadCategory();
            if (lsvBill.Tag != null) ShowBill((lsvBill.Tag as Table).ID);
        }

        void f_DeleteCategory(object sender, EventArgs e)
        {
            LoadCategory();
            if (lsvBill.Tag != null) ShowBill((lsvBill.Tag as Table).ID);
        }

        void f_InsertTable(object sender, EventArgs e)
        {
            LoadTable();
        }

        void f_UpdateTable(object sender, EventArgs e)
        {
            LoadTable();
        }

        void f_DeleteTable(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void personalInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAccountProfile f = new fAccountProfile(loginAccount);
            f.UpdateAccount += f_UpdateAccount;
            f.ShowDialog();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;

            ComboBox cb = sender as ComboBox;

            if (cb.SelectedItem == null)
                return;

            Category selected = cb.SelectedItem as Category;
            id = selected.Id;

            LoadFoodListByCategoryID(id);
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;

            if (table == null)
            {
                MessageBox.Show("Please choose a table to add food");
                return;
            }

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int foodID = (cbFood.SelectedItem as Food).ID;
            int count = (int)nmFoodCount.Value;

            if (idBill == -1)
            {
                BillDAO.Instance.InsertBill(table.ID);
                BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
                TableDAO.Instance.ChangeTableStatus(table.ID, "Occupied");
            }
            else
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
            }

            nmFoodCount.Value = 1;

            ShowBill(table.ID);
            LoadTable();
        }

        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;

            // Nếu bàn chưa được chọn hoặc chưa chọn món ăn trong bill thì thông báo lỗi
            if (table == null || lsvBill.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please choose a row in the bill to delete");
                return;
            }

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int idFood = FoodDAO.Instance.GetIDFoodByName(lsvBill.SelectedItems[0].SubItems[0].Text);

            // Nếu không tìm thấy idBill hoặc idFood thì thông báo lỗi
            if (idBill == -1 || idFood == -1)
            {
                MessageBox.Show("Error");
                return;
            }

            // Xóa món ăn trong bill đó
            BillInfoDAO.Instance.DeleteBillInfo(idBill, idFood);
            TableDAO.Instance.ChangeTableStatus(table.ID, "Occupied");

            // Hiển thị lại bill và load lại Table
            ShowBill(table.ID);
            LoadTable();
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;

            if (table == null)
            {
                MessageBox.Show("Please choose a table to check out");
                return;
            }

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            double totalPrice = Convert.ToDouble(txbTotalPrice.Text.Split(',')[0].Replace(".", ""));

            if (idBill != -1)
            {
                int discount = BillDAO.Instance.GetDiscountByID(idBill);
            
                if (MessageBox.Show($"Are you sure you want to check out the bill for table {table.Name}?\nTotal price: {totalPrice} - Discount: {discount}%", "Notification", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    BillDAO.Instance.CheckOut(idBill, (float)totalPrice);
                    TableDAO.Instance.ChangeTableStatus(table.ID, "Empty");
                    LoadTable();

                    nmDiscount.Value = 0;
                    lsvBill.Tag = null;
                    cbCategory.SelectedIndex = 0;
                    cbFood.SelectedIndex = 0;
                    nmFoodCount.Value = 1;
                    txbTotalPrice.Text = "0";
                    lsvBill.Items.Clear();
                }
            } else
            {
                MessageBox.Show("This table has no bill to check out");
            }
        }

        private void btnDiscount_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);

            // Nếu chưa có bàn được chọn và chưa có món ăn trong bill thì thông báo lỗi
            if (table != null)
            {
                if (idBill != -1)
                {   
                    if (MessageBox.Show("Are you sure you want to discount this bil?", "Notification", 
                        MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        return;
                    }
                    int discount = (int)nmDiscount.Value;
                    double totalPrice = Convert.ToDouble(txbTotalPrice.Text.Split(',')[0].Replace(".", ""));
                    double finalTotalPrice = totalPrice - (totalPrice / 100) * discount;
                    txbTotalPrice.Text = finalTotalPrice.ToString("c");

                    // update column discount in table dbo.Bill in database
                    BillDAO.Instance.UpdateDiscount(idBill, discount);
                }
                else
                {
                    MessageBox.Show("This table has no bill to discount");
                }
            }
            else
            {
                MessageBox.Show("Please choose a table to discount");
            }

            ShowBill(table.ID);
        }
    }
}
