using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KutuphaneOtomasyon
{
    public partial class FormGiris : Form
    {
        int toplamMasaSayisi = 12;
        int seciliMasaNumarasi;

        public FormGiris()
        {
            InitializeComponent();
            MasaSayisi(toplamMasaSayisi);
        }

        //TODO sayý yazýlan textboxýn yazýlýp entera basýldýðýnda silinmesi gerekiyor


        //Masalarý oluþturmak için kullanýlan fonksiyon
        public void MasaSayisi(int sayý)
        {
            int resimBoyutu = 100;
            int sýraSayýsý = (int)Math.Ceiling((double)sayý / 3);

            for (int i = 0; i < sayý; i++)
            {
                PictureBox pictureBox = new PictureBox();
                pictureBox.Name = "masa" + (i + 1).ToString();
                pictureBox.Image = Image.FromFile("C:\\Users\\merti\\OneDrive\\Masaüstü\\KutuphaneOtomasyon\\Resources\\desk.png");
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox.Width = resimBoyutu;
                pictureBox.Height = resimBoyutu;
                pictureBox.Location = new Point((i % 3) * (resimBoyutu + 10), (i / 3) * (resimBoyutu + 30));

                Label label = new Label();
                label.Text = "Masa No: " + (i + 1);
                label.AutoSize = true;
                label.Location = new Point(pictureBox.Location.X, pictureBox.Location.Y + resimBoyutu);

                masalarGroupBox.Controls.Add(label);
                masalarGroupBox.Controls.Add(pictureBox);

                seciliMasaNumarasi = i + 1;
            }
        }

        //dummy datalar (neyi denemek için yazdýðýmý unuttum)
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

        //kullanýcýnýn masa numarasýný seçtiðinde gerçekleþen event
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
            }
        }
    }
}
