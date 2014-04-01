namespace QR_Code_Encoder
{
    partial class frmMain
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
            this.mainPicBox = new System.Windows.Forms.PictureBox();
            this.qrStringIn = new System.Windows.Forms.RichTextBox();
            this.lblText = new System.Windows.Forms.Label();
            this.bestErrorCMode = new System.Windows.Forms.RadioButton();
            this.normalMode = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.dynamicCheckBox = new System.Windows.Forms.CheckBox();
            this.generateCodeBtn = new System.Windows.Forms.Button();
            this.versionLBL = new System.Windows.Forms.Label();
            this.bchLVL = new System.Windows.Forms.Label();
            this.messageLengthLBL = new System.Windows.Forms.Label();
            this.debugModeChkBox = new System.Windows.Forms.CheckBox();
            this.maskLBL = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mainPicBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPicBox
            // 
            this.mainPicBox.Location = new System.Drawing.Point(12, 12);
            this.mainPicBox.Name = "mainPicBox";
            this.mainPicBox.Size = new System.Drawing.Size(366, 373);
            this.mainPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.mainPicBox.TabIndex = 1;
            this.mainPicBox.TabStop = false;
            // 
            // qrStringIn
            // 
            this.qrStringIn.Location = new System.Drawing.Point(384, 35);
            this.qrStringIn.Name = "qrStringIn";
            this.qrStringIn.Size = new System.Drawing.Size(148, 133);
            this.qrStringIn.TabIndex = 2;
            this.qrStringIn.Text = "";
            this.qrStringIn.TextChanged += new System.EventHandler(this.qrStringIn_TextChanged);
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(381, 19);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(31, 13);
            this.lblText.TabIndex = 3;
            this.lblText.Text = "Text:";
            // 
            // bestErrorCMode
            // 
            this.bestErrorCMode.AutoSize = true;
            this.bestErrorCMode.Checked = true;
            this.bestErrorCMode.Location = new System.Drawing.Point(384, 192);
            this.bestErrorCMode.Name = "bestErrorCMode";
            this.bestErrorCMode.Size = new System.Drawing.Size(122, 17);
            this.bestErrorCMode.TabIndex = 4;
            this.bestErrorCMode.TabStop = true;
            this.bestErrorCMode.Text = "Best Error Correction";
            this.bestErrorCMode.UseVisualStyleBackColor = true;
            // 
            // normalMode
            // 
            this.normalMode.AutoSize = true;
            this.normalMode.Location = new System.Drawing.Point(384, 215);
            this.normalMode.Name = "normalMode";
            this.normalMode.Size = new System.Drawing.Size(58, 17);
            this.normalMode.TabIndex = 5;
            this.normalMode.Text = "Normal";
            this.normalMode.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(384, 173);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Mode:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(384, 239);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Encoding Method:";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(384, 256);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(136, 17);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.Text = "Numeric\\AlphaNumeric";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // dynamicCheckBox
            // 
            this.dynamicCheckBox.AutoSize = true;
            this.dynamicCheckBox.Checked = true;
            this.dynamicCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dynamicCheckBox.Location = new System.Drawing.Point(385, 365);
            this.dynamicCheckBox.Name = "dynamicCheckBox";
            this.dynamicCheckBox.Size = new System.Drawing.Size(97, 17);
            this.dynamicCheckBox.TabIndex = 9;
            this.dynamicCheckBox.Text = "Dynamic Mode";
            this.dynamicCheckBox.UseVisualStyleBackColor = true;
            this.dynamicCheckBox.CheckedChanged += new System.EventHandler(this.dynamicCheckBox_CheckedChanged);
            // 
            // generateCodeBtn
            // 
            this.generateCodeBtn.Enabled = false;
            this.generateCodeBtn.Location = new System.Drawing.Point(479, 361);
            this.generateCodeBtn.Name = "generateCodeBtn";
            this.generateCodeBtn.Size = new System.Drawing.Size(61, 23);
            this.generateCodeBtn.TabIndex = 10;
            this.generateCodeBtn.Text = "Generate";
            this.generateCodeBtn.UseVisualStyleBackColor = true;
            this.generateCodeBtn.Click += new System.EventHandler(this.generateCodeBtn_Click);
            // 
            // versionLBL
            // 
            this.versionLBL.AutoSize = true;
            this.versionLBL.Location = new System.Drawing.Point(386, 279);
            this.versionLBL.Name = "versionLBL";
            this.versionLBL.Size = new System.Drawing.Size(45, 13);
            this.versionLBL.TabIndex = 11;
            this.versionLBL.Text = "Version:";
            // 
            // bchLVL
            // 
            this.bchLVL.AutoSize = true;
            this.bchLVL.Location = new System.Drawing.Point(451, 279);
            this.bchLVL.Name = "bchLVL";
            this.bchLVL.Size = new System.Drawing.Size(24, 13);
            this.bchLVL.TabIndex = 12;
            this.bchLVL.Text = "EC:";
            // 
            // messageLengthLBL
            // 
            this.messageLengthLBL.AutoSize = true;
            this.messageLengthLBL.Location = new System.Drawing.Point(386, 297);
            this.messageLengthLBL.Name = "messageLengthLBL";
            this.messageLengthLBL.Size = new System.Drawing.Size(92, 13);
            this.messageLengthLBL.TabIndex = 13;
            this.messageLengthLBL.Text = "Message Length: ";
            // 
            // debugModeChkBox
            // 
            this.debugModeChkBox.AutoSize = true;
            this.debugModeChkBox.Location = new System.Drawing.Point(384, 342);
            this.debugModeChkBox.Name = "debugModeChkBox";
            this.debugModeChkBox.Size = new System.Drawing.Size(88, 17);
            this.debugModeChkBox.TabIndex = 14;
            this.debugModeChkBox.Text = "Debug Mode";
            this.debugModeChkBox.UseVisualStyleBackColor = true;
            this.debugModeChkBox.CheckedChanged += new System.EventHandler(this.debugModeChkBox_CheckedChanged);
            // 
            // maskLBL
            // 
            this.maskLBL.AutoSize = true;
            this.maskLBL.Location = new System.Drawing.Point(386, 314);
            this.maskLBL.Name = "maskLBL";
            this.maskLBL.Size = new System.Drawing.Size(36, 13);
            this.maskLBL.TabIndex = 15;
            this.maskLBL.Text = "Mask:";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 397);
            this.Controls.Add(this.maskLBL);
            this.Controls.Add(this.debugModeChkBox);
            this.Controls.Add(this.messageLengthLBL);
            this.Controls.Add(this.bchLVL);
            this.Controls.Add(this.versionLBL);
            this.Controls.Add(this.generateCodeBtn);
            this.Controls.Add(this.dynamicCheckBox);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.normalMode);
            this.Controls.Add(this.bestErrorCMode);
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.qrStringIn);
            this.Controls.Add(this.mainPicBox);
            this.Name = "frmMain";
            this.Text = "QR Code Encoder";
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.mainPicBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox mainPicBox;
        private System.Windows.Forms.RichTextBox qrStringIn;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.RadioButton bestErrorCMode;
        private System.Windows.Forms.RadioButton normalMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox dynamicCheckBox;
        private System.Windows.Forms.Button generateCodeBtn;
        private System.Windows.Forms.Label versionLBL;
        private System.Windows.Forms.Label bchLVL;
        private System.Windows.Forms.Label messageLengthLBL;
        private System.Windows.Forms.CheckBox debugModeChkBox;
        private System.Windows.Forms.Label maskLBL;
    }
}

