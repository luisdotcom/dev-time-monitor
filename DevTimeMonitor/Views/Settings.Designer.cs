namespace DevTimeMonitor.Views
{
    partial class Settings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.Title = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.lblTotal = new System.Windows.Forms.Label();
            this.BtnSave = new System.Windows.Forms.Button();
            this.txtBxConnectionString = new System.Windows.Forms.TextBox();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.chBxActivateExtension = new System.Windows.Forms.CheckBox();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.txtBxMessage = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.lblTitle)).BeginInit();
            this.SuspendLayout();
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.BackColor = System.Drawing.Color.Transparent;
            this.Title.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.ForeColor = System.Drawing.Color.SlateGray;
            this.Title.Location = new System.Drawing.Point(106, 85);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(152, 18);
            this.Title.TabIndex = 3;
            this.Title.Text = "DevTimeMonitor";
            // 
            // lblTitle
            // 
            this.lblTitle.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("lblTitle.BackgroundImage")));
            this.lblTitle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.lblTitle.InitialImage = ((System.Drawing.Image)(resources.GetObject("lblTitle.InitialImage")));
            this.lblTitle.Location = new System.Drawing.Point(140, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(78, 70);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.TabStop = false;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Black;
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(321, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(29, 28);
            this.button1.TabIndex = 15;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.SlateGray;
            this.lblTotal.Location = new System.Drawing.Point(23, 125);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(121, 13);
            this.lblTotal.TabIndex = 19;
            this.lblTotal.Text = "Connection string";
            // 
            // BtnSave
            // 
            this.BtnSave.BackColor = System.Drawing.Color.SlateBlue;
            this.BtnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSave.FlatAppearance.BorderSize = 0;
            this.BtnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSave.ForeColor = System.Drawing.SystemColors.Window;
            this.BtnSave.Location = new System.Drawing.Point(178, 313);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(148, 23);
            this.BtnSave.TabIndex = 22;
            this.BtnSave.Text = "SAVE";
            this.BtnSave.UseVisualStyleBackColor = false;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // txtBxConnectionString
            // 
            this.txtBxConnectionString.Location = new System.Drawing.Point(26, 144);
            this.txtBxConnectionString.Multiline = true;
            this.txtBxConnectionString.Name = "txtBxConnectionString";
            this.txtBxConnectionString.Size = new System.Drawing.Size(300, 50);
            this.txtBxConnectionString.TabIndex = 26;
            this.txtBxConnectionString.TextChanged += new System.EventHandler(this.TxtBxConnectionString_TextChanged);
            // 
            // BtnCancel
            // 
            this.BtnCancel.BackColor = System.Drawing.Color.Transparent;
            this.BtnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnCancel.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateBlue;
            this.BtnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnCancel.ForeColor = System.Drawing.Color.SlateBlue;
            this.BtnCancel.Location = new System.Drawing.Point(26, 313);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(148, 23);
            this.BtnCancel.TabIndex = 27;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = false;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.label1.Location = new System.Drawing.Point(35, 216);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 29;
            // 
            // chBxActivateExtension
            // 
            this.chBxActivateExtension.AutoSize = true;
            this.chBxActivateExtension.ForeColor = System.Drawing.Color.SlateGray;
            this.chBxActivateExtension.Location = new System.Drawing.Point(26, 219);
            this.chBxActivateExtension.Name = "chBxActivateExtension";
            this.chBxActivateExtension.Size = new System.Drawing.Size(134, 17);
            this.chBxActivateExtension.TabIndex = 30;
            this.chBxActivateExtension.Text = "Autostart the extension";
            this.chBxActivateExtension.UseVisualStyleBackColor = true;
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.BackColor = System.Drawing.Color.SlateBlue;
            this.btnTestConnection.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTestConnection.FlatAppearance.BorderSize = 0;
            this.btnTestConnection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestConnection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTestConnection.ForeColor = System.Drawing.SystemColors.Window;
            this.btnTestConnection.Location = new System.Drawing.Point(26, 190);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(300, 23);
            this.btnTestConnection.TabIndex = 31;
            this.btnTestConnection.Text = "TEST CONNECTION";
            this.btnTestConnection.UseVisualStyleBackColor = false;
            this.btnTestConnection.Click += new System.EventHandler(this.BtnTestConnection_Click);
            // 
            // txtBxMessage
            // 
            this.txtBxMessage.BackColor = System.Drawing.Color.Black;
            this.txtBxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBxMessage.ForeColor = System.Drawing.Color.Crimson;
            this.txtBxMessage.Location = new System.Drawing.Point(26, 242);
            this.txtBxMessage.Multiline = true;
            this.txtBxMessage.Name = "txtBxMessage";
            this.txtBxMessage.ReadOnly = true;
            this.txtBxMessage.Size = new System.Drawing.Size(300, 33);
            this.txtBxMessage.TabIndex = 0;
            this.txtBxMessage.TabStop = false;
            this.txtBxMessage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.SlateBlue;
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogin.ForeColor = System.Drawing.SystemColors.Window;
            this.btnLogin.Location = new System.Drawing.Point(26, 284);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(300, 23);
            this.btnLogin.TabIndex = 32;
            this.btnLogin.Text = "LOGIN";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.BtnLogin_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(350, 350);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtBxMessage);
            this.Controls.Add(this.btnTestConnection);
            this.Controls.Add(this.chBxActivateExtension);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.txtBxConnectionString);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(350, 350);
            this.MinimumSize = new System.Drawing.Size(350, 350);
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DevTimeMonitor";
            ((System.ComponentModel.ISupportInitialize)(this.lblTitle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.PictureBox lblTitle;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.TextBox txtBxConnectionString;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chBxActivateExtension;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.TextBox txtBxMessage;
        private System.Windows.Forms.Button btnLogin;
    }
}