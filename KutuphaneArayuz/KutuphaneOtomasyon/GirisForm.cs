using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KutuphaneOtomasyon
{
    public partial class FormGiris : Form
    {
        int toplamMasaSayisi = 10;
        int seciliMasaNumarasi;

        public FormGiris()
        {
            InitializeComponent();
            MasaSayisi(toplamMasaSayisi);
        }

        public void MasaSayisi(int sayý)
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

                    
                    seciliMasaNumarasi = masaNumarasý;
                }
            }
        }
        public void ogrencilerList()
        {
            Dictionary<string,int> ogrenciler = new Dictionary<string,int>();

            ogrenciler["Ahmet"]=123;
            ogrenciler["Mehmet"] = 321;
            ogrenciler["Veli"] = 456;
            ogrenciler["Ali"] = 654;
            ogrenciler["Ayþe"] = 789;
            ogrenciler["Fatma"] = 987;
        }


        private void masaNoTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int secilenMasaNumarasi;
                if (int.TryParse(masaNoTextBox.Text, out secilenMasaNumarasi))
                {
                    if (secilenMasaNumarasi >= 1 && secilenMasaNumarasi <= toplamMasaSayisi)
                    {
                        MessageBox.Show($"Seçilen masa numarasý: {secilenMasaNumarasi}\n\nMasayý baþarýyla seçtiniz.", "Masa Seçildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Geçersiz masa numarasý! Lütfen geçerli bir masa numarasý girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Masa numarasý sayýsal bir deðer olmalýdýr.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
