using CoffeeManagement.DAO;
using CoffeeManagement.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoffeeManagement
{
    public partial class fAdmin : Form
    {
        public fAdmin()
        {
            InitializeComponent();
            Load();
            btnFirstBillPage_Click(this, EventArgs.Empty);
        }

        BindingSource foodList = new BindingSource();
        BindingSource accountList = new BindingSource();
        BindingSource categoryList = new BindingSource();
        BindingSource tableList = new BindingSource();

        public Account loginAccount;

        void Load()
        {             
            dtgvFood.DataSource = foodList;
            dtgvAccount.DataSource = accountList;
            dtgvCategory.DataSource = categoryList;
            dtgvTable.DataSource = tableList;

            LoadDateTimePickerBill();
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
            LoadListFood();
            LoadAccount(); 
            LoadCategory();
            LoadTable();
            AddFoodBinding();
            AddAccountBinding();
            AddCategoryBinding();
            AddTableBinding();
            LoadCategoryIntoCombobox(cbFoodCategory);
        }

        void LoadDateTimePickerBill()
        {
            DateTime today = DateTime.Now;
            dtpkFromDate.Value = new DateTime(today.Year, today.Month, 1);
            dtpkToDate.Value = dtpkFromDate.Value.AddMonths(1).AddDays(-1);
        }

        void LoadListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            dtgvRevenue.DataSource = BillDAO.Instance.GetBillListByDate(checkIn, checkOut);
        }

        void LoadListFood()
        {
            foodList.DataSource = FoodDAO.Instance.GetListFood();
        }

        void AddFoodBinding()
        {
            txbFoodName.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "name", true, DataSourceUpdateMode.Never));
            txbFoodID.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "id", true, DataSourceUpdateMode.Never));
            nmFoodPrice.DataBindings.Add(new Binding("Value", dtgvFood.DataSource, "price", true, DataSourceUpdateMode.Never));
        }

        void LoadAccount()
        {
            accountList.DataSource = AccountDAO.Instance.GetListAccount();
        }

        void AddAccountBinding()
        {
            txbUsername.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "userName", true, DataSourceUpdateMode.Never));
            txbDisplayName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "displayName", true, DataSourceUpdateMode.Never));
            nmAccountType.DataBindings.Add(new Binding("Value", dtgvAccount.DataSource, "type", true, DataSourceUpdateMode.Never));
        }

        void LoadCategory()
        {
            categoryList.DataSource = CategoryDAO.Instance.GetDataTableCategory();
        }

        void AddCategoryBinding()
        {
            txbCategoryID.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "id", true, DataSourceUpdateMode.Never));
            txbCategoryName.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "name", true, DataSourceUpdateMode.Never));
        }

        void LoadTable()
        {
            tableList.DataSource = TableDAO.Instance.GetDataTableTable();
        }

        void AddTableBinding()
        {
            txbTableName.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "name", true, DataSourceUpdateMode.Never));
            txbTableID.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "id", true, DataSourceUpdateMode.Never));
            txbTableStatus.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "status", true, DataSourceUpdateMode.Never));
        }


        void LoadCategoryIntoCombobox(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCatagory();
            cb.DisplayMember = "name";
        }

        List<Food> SearchFoodByName(string name)
        {
            List<Food> listFood = FoodDAO.Instance.SearchFoodByName(name);

            return listFood;
        }

        void AddAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.InsertAccount(userName, displayName, type))
            {
                MessageBox.Show("Added Account Successfully");
            }
            else
            {
                MessageBox.Show("Error When Adding Account");
            }

            LoadAccount();
        }

        void EditAccount(string userName, string displayName, int type)
        {
            if (loginAccount.UserName.Equals(userName))
            {
                MessageBox.Show("You can't edit your own account");
                return;
            }

            if (AccountDAO.Instance.EditAccount(userName, displayName, type))
            {
                MessageBox.Show("Edited Account Successfully");
            }
            else
            {
                MessageBox.Show("Error When Editing Account");
            }

            LoadAccount();
        }

        void DeleteAccount(string userName)
        {
            if (loginAccount.UserName.Equals(userName))
            {
                MessageBox.Show("You can't delete your own account");
                return;
            }

            if (AccountDAO.Instance.DeleteAccount(userName))
            {
                MessageBox.Show("Deleted Account Successfully");
            }
            else
            {
                MessageBox.Show("Error When Deleting Account");
            }

            LoadAccount();
        }

        void ResetPassword(string userName)
        {
            if (AccountDAO.Instance.ResetPassword(userName))
            {
                MessageBox.Show("Reset Password Successfully");
            }
            else
            {
                MessageBox.Show("Error When Resetting Password");
            }
        }

        private void txbFoodID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtgvFood.SelectedCells.Count > 0)
                {
                    int id = (int)dtgvFood.SelectedCells[0].OwningRow.Cells["idCategory"].Value;

                    Category category = CategoryDAO.Instance.GetCategoryByID(id);

                    cbFoodCategory.SelectedItem = category;

                    int index = -1;
                    int i = 0;
                    foreach (Category item in cbFoodCategory.Items)
                    {
                        if (item.Id == category.Id)
                        {
                            index = i;
                            break;
                        }
                        i++;
                    }

                    cbFoodCategory.SelectedIndex = index;
                }
            }
            catch { }
        }

        private void btnViewAllFood_Click(object sender, EventArgs e)
        {
            LoadListFood();
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int idCategory = (cbFoodCategory.SelectedItem as Category).Id;
            float price = (float)nmFoodPrice.Value;

            // Check if name is duplicated
            if (FoodDAO.Instance.checkFoodName(name))
            {
                MessageBox.Show("This food name is already existed. Please choose another name.");
                return;
            }

            if (FoodDAO.Instance.InsertFood(name, idCategory, price))
            {
                MessageBox.Show("Added Food Successfully");
                LoadListFood();
                if (insertFood != null) insertFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Error When Adding Food");
            }
        }

        private void btnEditFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int idCategory = (cbFoodCategory.SelectedItem as Category).Id;
            float price = (float)nmFoodPrice.Value;
            int id = Convert.ToInt32(txbFoodID.Text);

            int result = FoodDAO.Instance.UpdateFood(id, name, idCategory, price);

            if (result == 1)
            {
                MessageBox.Show("Edited Food Successfully");
                LoadListFood();
                if (updateFood != null) updateFood(this, new EventArgs());
            }
            else if (result == 0)
            {
                MessageBox.Show("Cannot edit food. There is another food with the same name.");
            }
            else
            {
                MessageBox.Show("Error When Editing Food");
            }
        }

        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbFoodID.Text);
            int result = FoodDAO.Instance.DeleteFood(id);

            if (result == 1)
            {
                MessageBox.Show("Deleted Food Successfully");
                LoadListFood();
                if (deleteFood != null) deleteFood(this, new EventArgs());
            }
            else if (result == -1)
            {
                MessageBox.Show("Cannot delete food. There is still unchecked bill which contains this food");
            }
            else
            {
                MessageBox.Show("Error When Deleting Food");
            }
        }

        private void btnSearchFood_Click(object sender, EventArgs e)
        {
            string foodName = txbSearchFood.Text.TrimStart();

            if (foodName == "")
            {
                MessageBox.Show("Please enter food name to search");
                return;
            }

            var foodList2 = SearchFoodByName(foodName);

            if (foodList2.Count == 0 || foodList2 == null)
            {
                MessageBox.Show("No food found");
                return;
            }

            foodList.DataSource = SearchFoodByName(foodName);
        }

        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUsername.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)nmAccountType.Value;

            if (AccountDAO.Instance.checkAccountName(userName))
            {
                MessageBox.Show("This account name is already existed. Please choose another name.");
                return;
            }

            AddAccount(userName, displayName, type);
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUsername.Text;

            DeleteAccount(userName);
        }

        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUsername.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)nmAccountType.Value;

            if (loginAccount.UserName.Equals(userName) && loginAccount.Type != type)
            {
                MessageBox.Show("You can't edit your own account type");
                return;
            }

            EditAccount(userName, displayName, type);
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            string userName = txbUsername.Text;

            ResetPassword(userName);
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            string name = txbCategoryName.Text;

            if (CategoryDAO.Instance.CheckCategoryName(name))
            {
                MessageBox.Show("This category name is already existed. Please choose another name.");
                return;
            }

            if (CategoryDAO.Instance.InsertCategory(name))
            {
                MessageBox.Show("Added Category Successfully");
                LoadCategory();
                if (insertCategory != null) insertCategory(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Error When Adding Category");
            }

            LoadCategoryIntoCombobox(cbFoodCategory);
        }

        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbCategoryID.Text);

            if (CategoryDAO.Instance.DeleteCategory(id) > 1)
            {
                MessageBox.Show("Deleted Category Successfully");
                LoadCategory();
                LoadListFood();
                LoadCategoryIntoCombobox(cbFoodCategory);
                if (deleteCategory != null) deleteCategory(this, new EventArgs());
            }
            else if (CategoryDAO.Instance.DeleteCategory(id) == 1)
            {
                MessageBox.Show("Cannot delete category. There is still unchecked bill which contains food of this category");
            }
            else
            {
                MessageBox.Show("Error When Deleting Category");
            }
        }

        private void btnEditCategory_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbCategoryID.Text);
            string name = txbCategoryName.Text;

            if (CategoryDAO.Instance.CheckCategoryName(name))
            {
                MessageBox.Show("This category name is already existed. Please choose another name.");
                return;
            }

            if (CategoryDAO.Instance.UpdateCategory(id, name))
            {
                MessageBox.Show("Edited Category Successfully");
                LoadCategory();
                LoadCategoryIntoCombobox(cbFoodCategory);
                if (updateCategory != null) updateCategory(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Error When Editing Category");
            }
        }

        private void btnAddTable_Click(object sender, EventArgs e)
        {
            string name = txbTableName.Text;

            if (TableDAO.Instance.CheckTableName(name))
            {
                MessageBox.Show("This table name is already existed. Please choose another name.");
                return;
            }

            if (TableDAO.Instance.InsertTable(name))
            {
                MessageBox.Show("Added Table Successfully");
                LoadTable();
                if (insertTable != null) insertTable(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Error When Adding Table");
            }
        }

        private void btnDeleteTable_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbTableID.Text);
            string status = txbTableStatus.Text;

            if (status == "Occupied")
            {
                MessageBox.Show("Cannot delete table if it is occupied");
                return;
            }

            if (TableDAO.Instance.DeleteTable(id))
            {
                MessageBox.Show("Deleted Table Successfully");
                LoadTable();
                if (deleteTable != null) deleteTable(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Error When Deleting Table");
            }
        }

        private void btnEditTable_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbTableID.Text);
            string name = txbTableName.Text;
            string status = txbTableStatus.Text;

            if (status == "Occupied")
            {
                MessageBox.Show("Cannot edit table if it is occupied");
                return;
            }

            if (TableDAO.Instance.CheckTableName(name))
            {
                MessageBox.Show("This table name is already existed. Please choose another name.");
                return;
            }

            if (TableDAO.Instance.UpdateTable(id, name))
            {
                MessageBox.Show("Edited Table Successfully");
                LoadTable();
                if (updateTable != null) updateTable(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Error When Editing Table");
            }
        }
        private void btnViewRevenue_Click(object sender, EventArgs e)
        {
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
            txbBillPage.Text = "0";
            txbBillPage.Text = "1";

            int page = Convert.ToInt32(txbBillPage.Text);
            int sumRecord = BillDAO.Instance.GetNumBillListByDate(dtpkFromDate.Value, dtpkToDate.Value);
            int lastPage = sumRecord / 10;

            if (page < lastPage)
            {
                btnNextBillPage.Enabled = true;
            }
            else
            {
                btnNextBillPage.Enabled = false;
            }
        }

        private void btnFirstBillPage_Click(object sender, EventArgs e)
        {
            txbBillPage.Text = "1";

            btnNextBillPage.Enabled = true;
        }

        private void btnPreviousBillPage_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(txbBillPage.Text);

            if (page > 1)
            {
                page--;
            }

            txbBillPage.Text = page.ToString();

            btnNextBillPage.Enabled = true;
        }

        private void txbBillPage_TextChanged(object sender, EventArgs e)
        {
            if (txbBillPage.Text == "" || txbBillPage.Text == "0")
            {
                return;
            }
            dtgvRevenue.DataSource = BillDAO.Instance.GetBillListByDateAndPage(dtpkFromDate.Value, dtpkToDate.Value, Convert.ToInt32(txbBillPage.Text));
        }

        private void btnNextBillPage_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(txbBillPage.Text);
            int sumRecord = BillDAO.Instance.GetNumBillListByDate(dtpkFromDate.Value, dtpkToDate.Value);

            int lastPage = sumRecord / 10;
            if (sumRecord % 10 != 0)
            {
                lastPage++;
            }

            if (page < lastPage)
            {
                page++;
                txbBillPage.Text = page.ToString();
            }

            btnNextBillPage.Enabled = page < lastPage;
        }

        private void btnLastBillPage_Click(object sender, EventArgs e)
        {
            int sumRecord = BillDAO.Instance.GetNumBillListByDate(dtpkFromDate.Value, dtpkToDate.Value);

            int lastPage = sumRecord / 10;

            if (sumRecord % 10 != 0)
            {
                lastPage++;
            }

            txbBillPage.Text = lastPage.ToString();

            btnNextBillPage.Enabled = false;
        }




        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add { insertFood += value; }
            remove { insertFood -= value; }
        }

        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }
        }

        private event EventHandler updateFood;
        public event EventHandler UpdateFood
        {
            add { updateFood += value; }
            remove { updateFood -= value; }
        }

        private event EventHandler insertCategory;
        public event EventHandler InsertCategory
        {
            add { insertCategory += value; }
            remove { insertCategory -= value; }
        }

        private event EventHandler deleteCategory;
        public event EventHandler DeleteCategory
        {
            add { deleteCategory += value; }
            remove { deleteCategory -= value; }
        }

        private event EventHandler updateCategory;
        public event EventHandler UpdateCategory
        {
            add { updateCategory += value; }
            remove { updateCategory -= value; }
        }

        private event EventHandler insertTable;
        public event EventHandler InsertTable
        {
            add { insertTable += value; }
            remove { insertTable -= value; }
        }

        private event EventHandler deleteTable;
        public event EventHandler DeleteTable
        {
            add { deleteTable += value; }
            remove { deleteTable -= value; }
        }

        private event EventHandler updateTable;
        public event EventHandler UpdateTable
        {
            add { updateTable += value; }
            remove { updateTable -= value; }
        }
    }
}
