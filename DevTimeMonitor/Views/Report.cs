using System;
using System.Windows.Forms;

namespace DevTimeMonitor.Views
{
    public partial class Report : Form
    { 
        public Report(string user, int totalFiles, int totalCharacters, int totalCharactersByUser, int totalCharactersByAI, double userPercent, double aiPercent)
        {
            InitializeComponent();

            lblTotalNumber.Text = totalCharacters.ToString();
            lblUserNumber.Text = totalCharactersByUser.ToString();
            lblAINumber.Text = totalCharactersByAI.ToString();
            lblUserPercent.Text = (userPercent*100).ToString("0.00") + "%";
            lblAIPercent.Text = (aiPercent*100).ToString("0.00") + "%";
            lblDate.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
