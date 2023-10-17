using DevTimeMonitor.DTOs;
using DevTimeMonitor.Entities;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DevTimeMonitor.Views
{
    public partial class Login : Form
    {
        private bool signUp = false;
        private static readonly SettingsHelper settingsHelper = new SettingsHelper();

        public Login()
        {
            InitializeComponent();
        }
        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
            Close();
        }

        private void LblSignUp_Click(object sender, EventArgs e)
        {
            signUp = !signUp;

            lblName.Visible = signUp;
            txtBxName.Visible = signUp;
            btnLogin.Text = signUp ? "Accept" : "LogIn";
            lblSignUp.Text = signUp ? "Cancel" : "SignUp";

            txtBxName.Text = "";
            txtBxUserName.Text = "";
            txtBxPassword.Text = "";
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (signUp)
            {
                bool valid = true;
                if (txtBxPassword.Text.Trim() == "")
                {
                    lblError.Text = "The password is necessary";
                    valid = false;
                }

                if (txtBxUserName.Text.Trim() == "")
                {
                    lblError.Text = "The user name is necessary";
                    valid = false;
                }

                if (txtBxName.Text.Trim() == "")
                {
                    lblError.Text = "The name is necessary";
                    valid = false;
                }
                if (valid)
                {
                    using (var context = new ApplicationDBContext())
                    {
                        if (context.Users.Where(u => u.UserName == txtBxUserName.Text).Any())
                        {
                            lblError.Text = "Username already registered.";
                            return;
                        }
                        if (context.Users.Where(u => u.Name == txtBxName.Text).Any())
                        {
                            lblError.Text = "Name already registered.";
                            return;
                        }
                        TbUser newUser = new TbUser()
                        {
                            Name = txtBxName.Text,
                            UserName = txtBxUserName.Text,
                            Password = txtBxPassword.Text,
                        };

                        context.Users.Add(newUser);
                        context.SaveChanges();

                        txtBxName.Text = "";
                        txtBxPassword.Text = "";
                        LblSignUp_Click(null, null);
                    }
                }
            }
            else
            {
                bool valid = true;
                if (txtBxPassword.Text.Trim() == "")
                {
                    lblError.Text = "The password is necessary";
                    valid = false;
                }

                if (txtBxUserName.Text.Trim() == "")
                {
                    lblError.Text = "The user name is necessary";
                    valid = false;
                }

                if (valid)
                {
                    using (var context = new ApplicationDBContext())
                    {
                        bool exists = context.Users.Where(user => user.UserName == txtBxUserName.Text && user.Password == txtBxPassword.Text).Any();
                        if (exists)
                        {
                            SettingsDTO settings = settingsHelper.ReadSettings();
                            settings.User = txtBxUserName.Text;
                            settings.Configured = true;
                            settingsHelper.UpdateSettings(settings);

                            lblError.Text = "LogIn successful, Now you can start DevTimeMonitor.";
                            btnLogin.Visible = false;
                            lblSignUp.Visible = false;
                        }
                        else
                        {
                            txtBxPassword.Text = "";
                            lblError.Text = "Incorrect user or password";
                        }
                    }
                }
            }
        }
    }
}
