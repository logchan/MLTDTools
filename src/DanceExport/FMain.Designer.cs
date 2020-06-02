namespace OpenMLTD.DanceExport {
    partial class FMain {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.logBox = new System.Windows.Forms.RichTextBox();
            this.clearLogBtn = new System.Windows.Forms.Button();
            this.mainGroup = new System.Windows.Forms.GroupBox();
            this.avatar1 = new System.Windows.Forms.PictureBox();
            this.idolCombo1 = new System.Windows.Forms.ComboBox();
            this.mainGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.avatar1)).BeginInit();
            this.SuspendLayout();
            // 
            // logBox
            // 
            this.logBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logBox.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logBox.Location = new System.Drawing.Point(13, 739);
            this.logBox.Name = "logBox";
            this.logBox.Size = new System.Drawing.Size(1094, 163);
            this.logBox.TabIndex = 0;
            this.logBox.Text = "";
            // 
            // clearLogBtn
            // 
            this.clearLogBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.clearLogBtn.Location = new System.Drawing.Point(1113, 739);
            this.clearLogBtn.Name = "clearLogBtn";
            this.clearLogBtn.Size = new System.Drawing.Size(39, 37);
            this.clearLogBtn.TabIndex = 1;
            this.clearLogBtn.Text = "X";
            this.clearLogBtn.UseVisualStyleBackColor = true;
            this.clearLogBtn.Click += new System.EventHandler(this.clearLogBtn_Click);
            // 
            // mainGroup
            // 
            this.mainGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainGroup.Controls.Add(this.idolCombo1);
            this.mainGroup.Controls.Add(this.avatar1);
            this.mainGroup.Location = new System.Drawing.Point(13, 12);
            this.mainGroup.Name = "mainGroup";
            this.mainGroup.Size = new System.Drawing.Size(1139, 721);
            this.mainGroup.TabIndex = 2;
            this.mainGroup.TabStop = false;
            // 
            // avatar1
            // 
            this.avatar1.Location = new System.Drawing.Point(50, 138);
            this.avatar1.Name = "avatar1";
            this.avatar1.Size = new System.Drawing.Size(160, 160);
            this.avatar1.TabIndex = 0;
            this.avatar1.TabStop = false;
            this.avatar1.Visible = false;
            // 
            // idolCombo1
            // 
            this.idolCombo1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.idolCombo1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.idolCombo1.FormattingEnabled = true;
            this.idolCombo1.Location = new System.Drawing.Point(50, 304);
            this.idolCombo1.Name = "idolCombo1";
            this.idolCombo1.Size = new System.Drawing.Size(160, 37);
            this.idolCombo1.TabIndex = 5;
            this.idolCombo1.Visible = false;
            // 
            // FMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1164, 914);
            this.Controls.Add(this.mainGroup);
            this.Controls.Add(this.clearLogBtn);
            this.Controls.Add(this.logBox);
            this.Name = "FMain";
            this.Text = "Dance Export";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FMain_FormClosing);
            this.Shown += new System.EventHandler(this.FMain_Shown);
            this.mainGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.avatar1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox logBox;
        private System.Windows.Forms.Button clearLogBtn;
        private System.Windows.Forms.GroupBox mainGroup;
        private System.Windows.Forms.ComboBox idolCombo1;
        private System.Windows.Forms.PictureBox avatar1;
    }
}

