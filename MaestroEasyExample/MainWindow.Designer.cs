namespace Pololu.Usc.MaestroEasyExample
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            this.mainTextBox = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.stopPlayback = new System.Windows.Forms.Button();
            this.mode3RadioButton = new System.Windows.Forms.RadioButton();
            this.mode2RadioButton = new System.Windows.Forms.RadioButton();
            this.mode1RadioButton = new System.Windows.Forms.RadioButton();
            this.modeSelectRadioBtnGrp = new System.Windows.Forms.Panel();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.modeSelectRadioBtnGrp.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTextBox
            // 
            this.mainTextBox.Location = new System.Drawing.Point(12, 12);
            this.mainTextBox.Multiline = true;
            this.mainTextBox.Name = "mainTextBox";
            this.mainTextBox.ReadOnly = true;
            this.mainTextBox.Size = new System.Drawing.Size(423, 76);
            this.mainTextBox.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // stopPlayback
            // 
            this.stopPlayback.Location = new System.Drawing.Point(360, 114);
            this.stopPlayback.Name = "stopPlayback";
            this.stopPlayback.Size = new System.Drawing.Size(75, 23);
            this.stopPlayback.TabIndex = 1;
            this.stopPlayback.Text = "Stop";
            this.stopPlayback.UseVisualStyleBackColor = true;
            this.stopPlayback.Click += new System.EventHandler(this.stopPlayback_Click);
            // 
            // mode3RadioButton
            // 
            this.mode3RadioButton.AutoSize = true;
            this.mode3RadioButton.Location = new System.Drawing.Point(3, 49);
            this.mode3RadioButton.Name = "mode3RadioButton";
            this.mode3RadioButton.Size = new System.Drawing.Size(57, 17);
            this.mode3RadioButton.TabIndex = 2;
            this.mode3RadioButton.TabStop = true;
            this.mode3RadioButton.Text = "mode3";
            this.mode3RadioButton.UseVisualStyleBackColor = true;
            // 
            // mode2RadioButton
            // 
            this.mode2RadioButton.AutoSize = true;
            this.mode2RadioButton.Location = new System.Drawing.Point(3, 26);
            this.mode2RadioButton.Name = "mode2RadioButton";
            this.mode2RadioButton.Size = new System.Drawing.Size(57, 17);
            this.mode2RadioButton.TabIndex = 1;
            this.mode2RadioButton.TabStop = true;
            this.mode2RadioButton.Text = "mode2";
            this.mode2RadioButton.UseVisualStyleBackColor = true;
            // 
            // mode1RadioButton
            // 
            this.mode1RadioButton.AutoSize = true;
            this.mode1RadioButton.Location = new System.Drawing.Point(3, 3);
            this.mode1RadioButton.Name = "mode1RadioButton";
            this.mode1RadioButton.Size = new System.Drawing.Size(57, 17);
            this.mode1RadioButton.TabIndex = 0;
            this.mode1RadioButton.TabStop = true;
            this.mode1RadioButton.Text = "mode1";
            this.mode1RadioButton.UseVisualStyleBackColor = true;
            // 
            // modeSelectRadioBtnGrp
            // 
            this.modeSelectRadioBtnGrp.Controls.Add(this.mode3RadioButton);
            this.modeSelectRadioBtnGrp.Controls.Add(this.mode1RadioButton);
            this.modeSelectRadioBtnGrp.Controls.Add(this.mode2RadioButton);
            this.modeSelectRadioBtnGrp.Location = new System.Drawing.Point(12, 94);
            this.modeSelectRadioBtnGrp.Name = "modeSelectRadioBtnGrp";
            this.modeSelectRadioBtnGrp.Size = new System.Drawing.Size(83, 70);
            this.modeSelectRadioBtnGrp.TabIndex = 3;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 172);
            this.Controls.Add(this.modeSelectRadioBtnGrp);
            this.Controls.Add(this.stopPlayback);
            this.Controls.Add(this.mainTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainWindow";
            this.Text = "MaestroEasyExample in C#";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyDown);
            this.modeSelectRadioBtnGrp.ResumeLayout(false);
            this.modeSelectRadioBtnGrp.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mainTextBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button stopPlayback;
        private System.Windows.Forms.RadioButton mode3RadioButton;
        private System.Windows.Forms.RadioButton mode2RadioButton;
        private System.Windows.Forms.RadioButton mode1RadioButton;
        private System.Windows.Forms.Panel modeSelectRadioBtnGrp;
        private System.IO.Ports.SerialPort serialPort1;
    }
}

