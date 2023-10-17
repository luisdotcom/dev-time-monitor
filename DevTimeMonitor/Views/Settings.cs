using DevTimeMonitor.DTOs;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DevTimeMonitor.Views
{
    public partial class Settings : Form
    {
        private static bool configured = false;
        private static readonly SettingsHelper settingsHelper = new SettingsHelper();
        public Settings()
        {
            InitializeComponent();
            SettingsDTO settings = settingsHelper.ReadSettings();
            txtBxConnectionString.Text = settings.DefaultConnection;
        }
        private void BtnClose_Click(object sender, EventArgs e)
        {
            if (configured)
            {
                Login login = new Login();
                login.Show();
            }
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(txtBxConnectionString.Text))
                {
                    connection.Open();

                    SettingsDTO settings = settingsHelper.ReadSettings();
                    settings.DefaultConnection = txtBxConnectionString.Text;
                    settingsHelper.UpdateSettings(settings);

                    connection.Close();

                    configured = true;
                    lblMessage.Text = "Settings established.";

                    BtnCancel.Visible = false;
                    BtnSave.Visible = false;
                }
            }
            catch (SqlException ex)
            {
                lblMessage.Text = "Could not connect, check the connection string.";
            }
            catch
            {
                lblMessage.Text = "The configuration could not be saved.";
            }
        }
    }
}
