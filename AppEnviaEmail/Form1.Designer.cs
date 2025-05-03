namespace AppEnviaEmail
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button2 = new Button();
            button3 = new Button();
            label1 = new Label();
            label2 = new Label();
            comboDevice = new ComboBox();
            SuspendLayout();
            // 
            // button2
            // 
            button2.Location = new Point(193, 321);
            button2.Name = "button2";
            button2.Size = new Size(202, 34);
            button2.TabIndex = 1;
            button2.Text = "Obtem Biometria";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(435, 321);
            button3.Name = "button3";
            button3.Size = new Size(232, 34);
            button3.TabIndex = 2;
            button3.Text = "Verifica Biometria";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.ControlText;
            label1.Location = new Point(173, 392);
            label1.Name = "label1";
            label1.Size = new Size(17, 25);
            label1.TabIndex = 3;
            label1.Text = " ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(61, 392);
            label2.Name = "label2";
            label2.Size = new Size(106, 25);
            label2.TabIndex = 4;
            label2.Text = "Mensagens:";
            // 
            // comboDevice
            // 
            comboDevice.FormattingEnabled = true;
            comboDevice.Location = new Point(324, 114);
            comboDevice.Name = "comboDevice";
            comboDevice.Size = new Size(188, 33);
            comboDevice.TabIndex = 5;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(828, 466);
            Controls.Add(comboDevice);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button3);
            Controls.Add(button2);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button button2;
        private Button button3;
        private Label label1;
        private Label label2;
        private ComboBox comboDevice;
    }
}
