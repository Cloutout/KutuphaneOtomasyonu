namespace KutuphaneOtomasyon
{
    partial class FormGiris
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
            masaSecmeLabel = new Label();
            masalarGroupBox = new GroupBox();
            masaNoTextBox = new TextBox();
            gostergePB = new PictureBox();
            gostergePB2 = new PictureBox();
            gostergePB3 = new PictureBox();
            gostergeLabel = new Label();
            gostergeLabel2 = new Label();
            gostergeLabel3 = new Label();
            ogrenciNoTextBox = new TextBox();
            ((System.ComponentModel.ISupportInitialize)gostergePB).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gostergePB2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gostergePB3).BeginInit();
            SuspendLayout();
            // 
            // masaSecmeLabel
            // 
            masaSecmeLabel.AutoSize = true;
            masaSecmeLabel.Location = new Point(12, 84);
            masaSecmeLabel.Name = "masaSecmeLabel";
            masaSecmeLabel.Size = new Size(275, 15);
            masaSecmeLabel.TabIndex = 1;
            masaSecmeLabel.Text = "Lütfen Seçmek İstediğiniz Masa Numarasını Giriniz!";
            // 
            // masalarGroupBox
            // 
            masalarGroupBox.AutoSize = true;
            masalarGroupBox.Location = new Point(373, 30);
            masalarGroupBox.Name = "masalarGroupBox";
            masalarGroupBox.Size = new Size(401, 540);
            masalarGroupBox.TabIndex = 2;
            masalarGroupBox.TabStop = false;
            masalarGroupBox.Text = "Masalar";
            // 
            // masaNoTextBox
            // 
            masaNoTextBox.Location = new Point(12, 102);
            masaNoTextBox.Name = "masaNoTextBox";
            masaNoTextBox.Size = new Size(275, 23);
            masaNoTextBox.TabIndex = 3;
            masaNoTextBox.KeyDown += masaNoTextBox_KeyDown;
            // 
            // gostergePB
            // 
            gostergePB.BackColor = Color.Chartreuse;
            gostergePB.Image = Properties.Resources.desk;
            gostergePB.Location = new Point(12, 478);
            gostergePB.Name = "gostergePB";
            gostergePB.Size = new Size(76, 74);
            gostergePB.SizeMode = PictureBoxSizeMode.StretchImage;
            gostergePB.TabIndex = 4;
            gostergePB.TabStop = false;
            // 
            // gostergePB2
            // 
            gostergePB2.BackColor = Color.Yellow;
            gostergePB2.Image = Properties.Resources.desk;
            gostergePB2.Location = new Point(108, 478);
            gostergePB2.Name = "gostergePB2";
            gostergePB2.Size = new Size(76, 74);
            gostergePB2.SizeMode = PictureBoxSizeMode.StretchImage;
            gostergePB2.TabIndex = 5;
            gostergePB2.TabStop = false;
            // 
            // gostergePB3
            // 
            gostergePB3.BackColor = Color.Red;
            gostergePB3.Image = Properties.Resources.desk;
            gostergePB3.Location = new Point(211, 478);
            gostergePB3.Name = "gostergePB3";
            gostergePB3.Size = new Size(76, 74);
            gostergePB3.SizeMode = PictureBoxSizeMode.StretchImage;
            gostergePB3.TabIndex = 6;
            gostergePB3.TabStop = false;
            // 
            // gostergeLabel
            // 
            gostergeLabel.AutoSize = true;
            gostergeLabel.Location = new Point(12, 555);
            gostergeLabel.Name = "gostergeLabel";
            gostergeLabel.Size = new Size(63, 15);
            gostergeLabel.TabIndex = 7;
            gostergeLabel.Text = "Boş Koltuk";
            // 
            // gostergeLabel2
            // 
            gostergeLabel2.AutoSize = true;
            gostergeLabel2.Location = new Point(124, 555);
            gostergeLabel2.Name = "gostergeLabel2";
            gostergeLabel2.Size = new Size(34, 15);
            gostergeLabel2.TabIndex = 8;
            gostergeLabel2.Text = "Mola";
            // 
            // gostergeLabel3
            // 
            gostergeLabel3.AutoSize = true;
            gostergeLabel3.Location = new Point(211, 555);
            gostergeLabel3.Name = "gostergeLabel3";
            gostergeLabel3.Size = new Size(69, 15);
            gostergeLabel3.TabIndex = 9;
            gostergeLabel3.Text = "Dolu Koltuk";
            // 
            // ogrenciNoTextBox
            // 
            ogrenciNoTextBox.Location = new Point(12, 154);
            ogrenciNoTextBox.Name = "ogrenciNoTextBox";
            ogrenciNoTextBox.Size = new Size(245, 23);
            ogrenciNoTextBox.TabIndex = 10;
            // 
            // FormGiris
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(832, 647);
            Controls.Add(ogrenciNoTextBox);
            Controls.Add(gostergeLabel3);
            Controls.Add(gostergeLabel2);
            Controls.Add(gostergeLabel);
            Controls.Add(gostergePB3);
            Controls.Add(gostergePB2);
            Controls.Add(gostergePB);
            Controls.Add(masaNoTextBox);
            Controls.Add(masalarGroupBox);
            Controls.Add(masaSecmeLabel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormGiris";
            Text = "Giriş Ekranı";
            ((System.ComponentModel.ISupportInitialize)gostergePB).EndInit();
            ((System.ComponentModel.ISupportInitialize)gostergePB2).EndInit();
            ((System.ComponentModel.ISupportInitialize)gostergePB3).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label masaSecmeLabel;
        private GroupBox masalarGroupBox;
        private TextBox masaNoTextBox;
        private PictureBox gostergePB;
        private PictureBox gostergePB2;
        private PictureBox gostergePB3;
        private Label gostergeLabel;
        private Label gostergeLabel2;
        private Label gostergeLabel3;
        private TextBox ogrenciNoTextBox;
    }
}
