namespace DevTimeMonitor.Views
{
    partial class Report
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Report));
            this.lblTitle = new System.Windows.Forms.PictureBox();
            this.Title = new System.Windows.Forms.Label();
            this.TotalCharacters = new System.Windows.Forms.PictureBox();
            this.CharactersByAI = new System.Windows.Forms.PictureBox();
            this.CharactersByUser = new System.Windows.Forms.PictureBox();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblAI = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.gbosCodeStatics = new System.Windows.Forms.GroupBox();
            this.lblUserPercent = new System.Windows.Forms.Label();
            this.lblAIPercent = new System.Windows.Forms.Label();
            this.lblUserNumber = new System.Windows.Forms.Label();
            this.lblAINumber = new System.Windows.Forms.Label();
            this.lblTotalNumber = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.lblTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalCharacters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CharactersByAI)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CharactersByUser)).BeginInit();
            this.gbosCodeStatics.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("lblTitle.BackgroundImage")));
            this.lblTitle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.lblTitle.InitialImage = ((System.Drawing.Image)(resources.GetObject("lblTitle.InitialImage")));
            this.lblTitle.Location = new System.Drawing.Point(140, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(78, 70);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.TabStop = false;
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.Location = new System.Drawing.Point(106, 85);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(152, 18);
            this.Title.TabIndex = 1;
            this.Title.Text = "DevTimeMonitor";
            // 
            // TotalCharacters
            // 
            this.TotalCharacters.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("TotalCharacters.BackgroundImage")));
            this.TotalCharacters.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.TotalCharacters.InitialImage = null;
            this.TotalCharacters.Location = new System.Drawing.Point(20, 200);
            this.TotalCharacters.Name = "TotalCharacters";
            this.TotalCharacters.Size = new System.Drawing.Size(78, 70);
            this.TotalCharacters.TabIndex = 3;
            this.TotalCharacters.TabStop = false;
            // 
            // CharactersByAI
            // 
            this.CharactersByAI.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("CharactersByAI.BackgroundImage")));
            this.CharactersByAI.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.CharactersByAI.InitialImage = ((System.Drawing.Image)(resources.GetObject("CharactersByAI.InitialImage")));
            this.CharactersByAI.Location = new System.Drawing.Point(140, 200);
            this.CharactersByAI.Name = "CharactersByAI";
            this.CharactersByAI.Size = new System.Drawing.Size(78, 70);
            this.CharactersByAI.TabIndex = 4;
            this.CharactersByAI.TabStop = false;
            // 
            // CharactersByUser
            // 
            this.CharactersByUser.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("CharactersByUser.BackgroundImage")));
            this.CharactersByUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.CharactersByUser.InitialImage = ((System.Drawing.Image)(resources.GetObject("CharactersByUser.InitialImage")));
            this.CharactersByUser.Location = new System.Drawing.Point(252, 200);
            this.CharactersByUser.Name = "CharactersByUser";
            this.CharactersByUser.Size = new System.Drawing.Size(78, 70);
            this.CharactersByUser.TabIndex = 5;
            this.CharactersByUser.TabStop = false;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.lblTotal.Location = new System.Drawing.Point(36, 180);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(47, 17);
            this.lblTotal.TabIndex = 6;
            this.lblTotal.Text = "Total";
            // 
            // lblAI
            // 
            this.lblAI.AutoSize = true;
            this.lblAI.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAI.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.lblAI.Location = new System.Drawing.Point(168, 180);
            this.lblAI.Name = "lblAI";
            this.lblAI.Size = new System.Drawing.Size(27, 17);
            this.lblAI.TabIndex = 7;
            this.lblAI.Text = "AI";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUser.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.lblUser.Location = new System.Drawing.Point(272, 180);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(43, 17);
            this.lblUser.TabIndex = 8;
            this.lblUser.Text = "User";
            // 
            // gbosCodeStatics
            // 
            this.gbosCodeStatics.Controls.Add(this.lblUserPercent);
            this.gbosCodeStatics.Controls.Add(this.lblAIPercent);
            this.gbosCodeStatics.Controls.Add(this.lblUserNumber);
            this.gbosCodeStatics.Controls.Add(this.lblAINumber);
            this.gbosCodeStatics.Controls.Add(this.lblTotalNumber);
            this.gbosCodeStatics.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbosCodeStatics.ForeColor = System.Drawing.Color.DimGray;
            this.gbosCodeStatics.Location = new System.Drawing.Point(12, 150);
            this.gbosCodeStatics.Name = "gbosCodeStatics";
            this.gbosCodeStatics.Size = new System.Drawing.Size(326, 176);
            this.gbosCodeStatics.TabIndex = 10;
            this.gbosCodeStatics.TabStop = false;
            this.gbosCodeStatics.Text = "Code statistics";
            // 
            // lblUserPercent
            // 
            this.lblUserPercent.AutoSize = true;
            this.lblUserPercent.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserPercent.ForeColor = System.Drawing.Color.LightSlateGray;
            this.lblUserPercent.Location = new System.Drawing.Point(240, 145);
            this.lblUserPercent.Margin = new System.Windows.Forms.Padding(0);
            this.lblUserPercent.MinimumSize = new System.Drawing.Size(80, 0);
            this.lblUserPercent.Name = "lblUserPercent";
            this.lblUserPercent.Size = new System.Drawing.Size(80, 17);
            this.lblUserPercent.TabIndex = 16;
            this.lblUserPercent.Text = "0.00%";
            this.lblUserPercent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAIPercent
            // 
            this.lblAIPercent.AutoSize = true;
            this.lblAIPercent.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAIPercent.ForeColor = System.Drawing.Color.LightSlateGray;
            this.lblAIPercent.Location = new System.Drawing.Point(127, 145);
            this.lblAIPercent.Margin = new System.Windows.Forms.Padding(0);
            this.lblAIPercent.MinimumSize = new System.Drawing.Size(80, 0);
            this.lblAIPercent.Name = "lblAIPercent";
            this.lblAIPercent.Size = new System.Drawing.Size(80, 17);
            this.lblAIPercent.TabIndex = 15;
            this.lblAIPercent.Text = "0.00%";
            this.lblAIPercent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUserNumber
            // 
            this.lblUserNumber.AutoSize = true;
            this.lblUserNumber.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserNumber.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.lblUserNumber.Location = new System.Drawing.Point(239, 122);
            this.lblUserNumber.Margin = new System.Windows.Forms.Padding(0);
            this.lblUserNumber.MinimumSize = new System.Drawing.Size(80, 0);
            this.lblUserNumber.Name = "lblUserNumber";
            this.lblUserNumber.Size = new System.Drawing.Size(80, 17);
            this.lblUserNumber.TabIndex = 13;
            this.lblUserNumber.Text = "0";
            this.lblUserNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAINumber
            // 
            this.lblAINumber.AutoSize = true;
            this.lblAINumber.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAINumber.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.lblAINumber.Location = new System.Drawing.Point(126, 122);
            this.lblAINumber.Margin = new System.Windows.Forms.Padding(0);
            this.lblAINumber.MinimumSize = new System.Drawing.Size(80, 0);
            this.lblAINumber.Name = "lblAINumber";
            this.lblAINumber.Size = new System.Drawing.Size(80, 17);
            this.lblAINumber.TabIndex = 12;
            this.lblAINumber.Text = "0";
            this.lblAINumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalNumber
            // 
            this.lblTotalNumber.AutoSize = true;
            this.lblTotalNumber.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalNumber.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.lblTotalNumber.Location = new System.Drawing.Point(6, 122);
            this.lblTotalNumber.Margin = new System.Windows.Forms.Padding(0);
            this.lblTotalNumber.MinimumSize = new System.Drawing.Size(80, 0);
            this.lblTotalNumber.Name = "lblTotalNumber";
            this.lblTotalNumber.Size = new System.Drawing.Size(80, 17);
            this.lblTotalNumber.TabIndex = 11;
            this.lblTotalNumber.Text = "0";
            this.lblTotalNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.BackColor = System.Drawing.SystemColors.Window;
            this.lblDate.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDate.ForeColor = System.Drawing.Color.SlateGray;
            this.lblDate.Location = new System.Drawing.Point(116, 108);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(127, 13);
            this.lblDate.TabIndex = 11;
            this.lblDate.Text = "23/09/2023 18:23";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.BackColor = System.Drawing.SystemColors.Window;
            this.lblVersion.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.Color.SlateGray;
            this.lblVersion.Location = new System.Drawing.Point(291, 404);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(47, 13);
            this.lblVersion.TabIndex = 12;
            this.lblVersion.Text = "v1.0.0";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Window;
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.FlatAppearance.BorderColor = System.Drawing.SystemColors.Window;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.Window;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Window;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(321, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(29, 28);
            this.button1.TabIndex = 14;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // Report
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(350, 426);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.lblAI);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.CharactersByUser);
            this.Controls.Add(this.CharactersByAI);
            this.Controls.Add(this.TotalCharacters);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.gbosCodeStatics);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximumSize = new System.Drawing.Size(350, 426);
            this.MinimumSize = new System.Drawing.Size(350, 426);
            this.Name = "Report";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DevTimeMonitor";
            ((System.ComponentModel.ISupportInitialize)(this.lblTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalCharacters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CharactersByAI)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CharactersByUser)).EndInit();
            this.gbosCodeStatics.ResumeLayout(false);
            this.gbosCodeStatics.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox lblTitle;
        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.PictureBox TotalCharacters;
        private System.Windows.Forms.PictureBox CharactersByAI;
        private System.Windows.Forms.PictureBox CharactersByUser;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblAI;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.GroupBox gbosCodeStatics;
        private System.Windows.Forms.Label lblUserNumber;
        private System.Windows.Forms.Label lblAINumber;
        private System.Windows.Forms.Label lblTotalNumber;
        private System.Windows.Forms.Label lblUserPercent;
        private System.Windows.Forms.Label lblAIPercent;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Button button1;
    }
}