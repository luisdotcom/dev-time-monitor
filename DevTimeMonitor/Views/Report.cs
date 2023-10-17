using DevTimeMonitor.DTOs;
using DevTimeMonitor.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DevTimeMonitor.Views
{
    public partial class Report : Form
    {
        private static readonly SettingsHelper settingsHelper = new SettingsHelper();
        private static readonly SettingsDTO settings = settingsHelper.ReadSettings();
        private readonly TbUser user;
        public Report()
        {
            InitializeComponent();
            using (var context = new ApplicationDBContext())
            {
                user = context.Users.Where(u => u.UserName == settings.User).FirstOrDefault();
            }
            SetTotals();
            SetDays();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SetTotals()
        {
            using (var context = new ApplicationDBContext())
            {
                List<TbTracker> data = context.Trackers.Where(t => t.UserId == user.Id).ToList();
                if (data.Count > 0)
                {
                    int totalCharacters = 0;
                    int totalCharactersByUser = 0;
                    int totalCharactersByAI = 0;

                    for (int i = 0; i < data.Count; i++)
                    {
                        totalCharacters += data[i].CharactersTracked;
                        totalCharactersByUser += data[i].KeysPressed;
                    }

                    totalCharactersByAI += totalCharacters - totalCharactersByUser;

                    double totalCharactersByUserPercent = 0.0;
                    double totalCharactersByAIPercent = 0.0;
                    if (totalCharacters > 0)
                    {
                        totalCharactersByUserPercent = (double)totalCharactersByUser / totalCharacters;
                        totalCharactersByAIPercent = (double)totalCharactersByAI / totalCharacters;
                    }

                    lblTotalNumber.Text = totalCharacters.ToString();
                    lblUserNumber.Text = totalCharactersByUser.ToString();
                    lblAINumber.Text = totalCharactersByAI.ToString();
                    lblUserPercent.Text = (totalCharactersByUserPercent * 100).ToString("0.00") + "%";
                    lblAIPercent.Text = (totalCharactersByAIPercent * 100).ToString("0.00") + "%";
                    lblDate.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm");
                }
            }
        }
        private void SetDays()
        {
            using (var context = new ApplicationDBContext())
            {
                TbDailyLog dailyLog = context.DailyLogs.Where(d => d.UserId == user.Id).FirstOrDefault();
                if (dailyLog != null)
                {
                    chBxMonday.Checked = dailyLog.Monday;
                    chBxTuesday.Checked = dailyLog.Tuesday;
                    chBxWednesday.Checked = dailyLog.Wednesday;
                    chBxThursday.Checked = dailyLog.Thursday;
                    chBxFriday.Checked = dailyLog.Friday;
                }
            }
        }
    }
}
