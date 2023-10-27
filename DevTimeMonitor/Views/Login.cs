using DevTimeMonitor.Entities;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DevTimeMonitor.Views
{
    public partial class Login : Form
    {
        private bool signUp = false;
        private SettingsPage settingsPage;
        public Login()
        {
            InitializeComponent();
            settingsPage = SettingsPage.GetLiveInstanceAsync().GetAwaiter().GetResult();
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
            btnLogin.Text = signUp ? "SignUp" : "LogIn";
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
                    try
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

                            LblSignUp_Click(null, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        lblError.Text = ex.Message;
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
                    try
                    {
                        using (var context = new ApplicationDBContext())
                        {
                            TbUser user = context.Users.Where(u => u.UserName == txtBxUserName.Text && u.Password == txtBxPassword.Text).FirstOrDefault();
                            if (user != null)
                            {
                                settingsPage = SettingsPage.GetLiveInstanceAsync().GetAwaiter().GetResult();
                                settingsPage.UserName = user.UserName;
                                settingsPage.Name = user.Name;
                                settingsPage.Password = user.Password;
                                settingsPage.Logged = true;
                                settingsPage.SaveAsync().GetAwaiter().GetResult();

                                lblError.Text = "Logged in!";
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
                    catch (Exception ex)
                    {
                        lblError.Text = ex.Message;
                    }
                }
            }
        }
    }
}
