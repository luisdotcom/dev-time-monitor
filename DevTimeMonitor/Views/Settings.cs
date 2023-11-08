using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DevTimeMonitor.Views
{
    public partial class Settings : Form
    {
        private static SettingsPage settingsPage;
        public Settings()
        {
            InitializeComponent();
            settingsPage = SettingsPage.GetLiveInstanceAsync().GetAwaiter().GetResult();
            txtBxConnectionString.Text = settingsPage.ConnectionString;
            chBxActivateExtension.Checked = settingsPage.Autostart;

            if (settingsPage.Logged)
            {
                btnLogin.Text = "CHANGE USER";
            }
            else
            {
                btnLogin.Enabled = false;
            }
        }
        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void BtnTestConnection_Click(object sender, EventArgs e)
        {
            btnTestConnection.Enabled = false;
            btnTestConnection.Text = "Trying to connect to the database";
            btnTestConnection.ForeColor = Color.White;

            await TestConnectionAsync();
        }
        private async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(txtBxConnectionString.Text))
                {
                    await connection.OpenAsync();
                    await Task.Delay(3000);
                    btnTestConnection.Text = "Connection established";
                    btnTestConnection.BackColor = Color.DarkSeaGreen;
                    connection.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                btnTestConnection.Text = "Connection not established";
                btnTestConnection.BackColor = Color.Crimson;

                txtBxMessage.Text = ex.Message;

                return false;
            }
        }
        private void TxtBxConnectionString_TextChanged(object sender, EventArgs e)
        {
            if (btnTestConnection.Text != "TEST CONNECTION")
            {
                btnTestConnection.Text = "TEST CONNECTION";
                btnTestConnection.BackColor = Color.DarkSlateBlue;
                btnTestConnection.Enabled = true;
                txtBxMessage.Text = "";
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private async void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                settingsPage = await SettingsPage.GetLiveInstanceAsync();
                settingsPage.Autostart = chBxActivateExtension.Checked;

                if (txtBxConnectionString.Text != settingsPage.ConnectionString)
                {
                    if (await TestConnectionAsync())
                    {
                        settingsPage.ConnectionString = txtBxConnectionString.Text;
                        await settingsPage.SaveAsync();

                        Login login = new Login();
                        login.Show();
                        Close();
                    }
                }
                else
                {
                    await settingsPage.SaveAsync();
                    txtBxMessage.Text = "Configuration saved.";
                    btnLogin.Enabled = true;
                }
            }
            catch
            {
                txtBxMessage.Text = "The configuration could not be saved.";
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show();
            Close();
        }
    }
}
