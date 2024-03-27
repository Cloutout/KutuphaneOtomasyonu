namespace WinFormsApp1
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
            DeskNumberTextBox = new TextBox();
            panel = new FlowLayoutPanel();
            label1 = new Label();
            SuspendLayout();
            // 
            // DeskNumberTextBox
            // 
            DeskNumberTextBox.Location = new Point(23, 122);
            DeskNumberTextBox.Name = "DeskNumberTextBox";
            DeskNumberTextBox.Size = new Size(288, 23);
            DeskNumberTextBox.TabIndex = 0;
            // 
            // panel
            // 
            panel.Location = new Point(436, 69);
            panel.Name = "panel";
            panel.Size = new Size(1097, 705);
            panel.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(23, 95);
            label1.Name = "label1";
            label1.Size = new Size(175, 15);
            label1.TabIndex = 2;
            label1.Text = "Lütfen Masa Numarasını Giriniz!";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1555, 797);
            Controls.Add(label1);
            Controls.Add(panel);
            Controls.Add(DeskNumberTextBox);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox DeskNumberTextBox;
        private FlowLayoutPanel panel;
        private Label label1;
    }
}