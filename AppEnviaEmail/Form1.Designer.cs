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
            comboDevice = new ComboBox();
            button1 = new Button();
            SuspendLayout();
            // 
            // button2
            // 
            button2.Location = new Point(109, 183);
            button2.Margin = new Padding(2, 2, 2, 2);
            button2.Name = "button2";
            button2.Size = new Size(141, 20);
            button2.TabIndex = 1;
            button2.Text = "Obtem Biometria";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(304, 183);
            button3.Margin = new Padding(2, 2, 2, 2);
            button3.Name = "button3";
            button3.Size = new Size(162, 20);
            button3.TabIndex = 2;
            button3.Text = "Verifica Biometria";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click_1;
            // 
            // comboDevice
            // 
            comboDevice.FormattingEnabled = true;
            comboDevice.Location = new Point(221, 107);
            comboDevice.Margin = new Padding(2, 2, 2, 2);
            comboDevice.Name = "comboDevice";
            comboDevice.Size = new Size(133, 23);
            comboDevice.TabIndex = 5;
            // 
            // button1
            // 
            button1.Location = new Point(205, 217);
            button1.Margin = new Padding(2);
            button1.Name = "button1";
            button1.Size = new Size(162, 20);
            button1.TabIndex = 6;
            button1.Text = "Verificar existência";
            button1.UseVisualStyleBackColor = true;
            button1.Click += BTN_VerificarExistencia;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(580, 280);
            Controls.Add(button1);
            Controls.Add(comboDevice);
            Controls.Add(button3);
            Controls.Add(button2);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(2, 2, 2, 2);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion
        private Button button2;
        private Button button3;
        private ComboBox comboDevice;
        private Button button1;
    }
}
