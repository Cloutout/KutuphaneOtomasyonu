using System;
using System.Drawing;
using System.Windows.Forms;

namespace KutuphaneOtomasyon
{
    public partial class FormGiris : Form
    {
        public FormGiris()
        {
            InitializeComponent();
            MasaSayisi(12);
        }

        private void MasaSayisi(int sayý)
        {
            int resimBoyutu = 100;
            int sýraSayýsý = (int)Math.Ceiling((double)sayý / 3);

            for (int i = 0; i < sýraSayýsý; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int masaNumarasý = i * 3 + j + 1;

                    if (masaNumarasý > sayý)
                        break;

                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Name = "masa" + masaNumarasý.ToString();
                    pictureBox.Image = Image.FromFile("C:\\Users\\merti\\OneDrive\\Masaüstü\\KutuphaneArayuz\\KutuphaneOtomasyon\\Resources\\desk.png");
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox.Width = resimBoyutu;
                    pictureBox.Height = resimBoyutu;
                    pictureBox.Location = new Point(j * (resimBoyutu + 10), i * (resimBoyutu + 30));

                    Label label = new Label();
                    label.Text = "Masa No: " + masaNumarasý;
                    label.AutoSize = true;
                    label.Location = new Point(pictureBox.Location.X, pictureBox.Location.Y + resimBoyutu);

                    masalarGroupBox.Controls.Add(label);
                    masalarGroupBox.Controls.Add(pictureBox);
                }
            }
        }

    }
}
