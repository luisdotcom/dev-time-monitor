using DevTimeMonitor.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DevTimeMonitor.Views
{
    public partial class Report : Form
    {
        private readonly TbUser user;
        public Report()
        {
            InitializeComponent();
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    SettingsPage settingsPage = SettingsPage.GetLiveInstanceAsync().GetAwaiter().GetResult();
                    user = context.Users.Where(u => u.UserName == settingsPage.UserName).FirstOrDefault();
                }
                SetTotals();
                SetDays();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SetTotals()
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    DateTime today = DateTime.Now;
                    DateTime mondayOfCurrentWeek = today.Date.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday); 

                    List<TbTracker> data = context.Trackers.Where(t => t.UserId == user.Id && t.CreationDate >= mondayOfCurrentWeek).ToList();
                    if (data.Count > 0)
                    {
                        int totalCharacters = 0;
                        int totalCharactersByCopilot = 0;
                        int totalCharactersByUser = 0;

                        for (int i = 0; i < data.Count; i++)
                        {
                            totalCharacters += data[i].CharactersTracked;
                            totalCharactersByCopilot += data[i].CharactersByCopilot;
                        }

                        totalCharactersByUser += totalCharacters - totalCharactersByCopilot;

                        double totalCharactersByCopilotPercent = 0.0;
                        double totalCharactersByUserPercent = 0.0;

                        if (totalCharacters > 0)
                        {
                            totalCharactersByCopilotPercent = (double)totalCharactersByCopilot / totalCharacters;
                            totalCharactersByUserPercent = (double)totalCharactersByUser / totalCharacters;

                        }

                        lblTotalNumber.Text = totalCharacters.ToString();
                        lblUserNumber.Text = totalCharactersByUser.ToString();
                        lblAINumber.Text = totalCharactersByCopilot.ToString();
                        lblUserPercent.Text = (totalCharactersByUserPercent * 100).ToString("0.00") + "%";
                        lblAIPercent.Text = (totalCharactersByCopilotPercent * 100).ToString("0.00") + "%";
                        lblDate.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }
        private void SetDays()
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    TbDailyLog dailyLog = context.DailyLogs.Where(d => d.UserId == user.Id).FirstOrDefault();
                    if (dailyLog != null)
                    {
                        chBxMonday.Checked = dailyLog.Monday;
                        chBxTuesday.Checked = dailyLog.Tuesday;
                        chBxWednesday.Checked = dailyLog.Wednesday;
                        chBxThursday.Checked = dailyLog.Thursday;
                        chBxFriday.Checked = dailyLog.Friday;
                        chBxSaturday.Checked = dailyLog.Saturday;
                        chBxSunday.Checked = dailyLog.Sunday;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }
    }
}
