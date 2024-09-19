using CoffeeManagement.DAO;
using CoffeeManagement.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoffeeManagement
{
    public partial class fLogin : Form
    {
        public fLogin()
        {
            InitializeComponent();
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string userName = txbUsername.Text;
            string passWord = txbPassword.Text;

            if (Login(userName, passWord))
            {
                Account loginAccount = AccountDAO.Instance.getAccountByUserName(userName);
                    
                txbUsername.Focus();

                fTableManager f = new fTableManager(loginAccount);
                this.Hide();
                f.ShowDialog();
                this.Show();

                txbUsername.Text = "";
                txbPassword.Text = "";
            }
            else
            {
                MessageBox.Show("Wrong username or password");
            }
        }

        bool Login(string userName, string passWord)
        {
            return AccountDAO.Instance.Login(userName, passWord);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void fLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Do you want to exit?", "Notification", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
            {
                e.Cancel = true;
            }
        }
    }
}
