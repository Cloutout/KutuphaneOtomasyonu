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

        public void MasaSayisi(int say�)
        {
            int resimBoyutu = 100;
            int s�raSay�s� = (int)Math.Ceiling((double)say� / 3);

            for (int i = 0; i < s�raSay�s�; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int masaNumaras� = i * 3 + j + 1;

                    if (masaNumaras� > say�)
                        break;

                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Name = "masa" + masaNumaras�.ToString();
                    pictureBox.Image = Image.FromFile("C:\\Users\\merti\\OneDrive\\Masa�st�\\KutuphaneArayuz\\KutuphaneOtomasyon\\Resources\\desk.png");
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox.Width = resimBoyutu;
                    pictureBox.Height = resimBoyutu;
                    pictureBox.Location = new Point(j * (resimBoyutu + 10), i * (resimBoyutu + 30));

                    Label label = new Label();
                    label.Text = "Masa No: " + masaNumaras�;
                    label.AutoSize = true;
                    label.Location = new Point(pictureBox.Location.X, pictureBox.Location.Y + resimBoyutu);

                    masalarGroupBox.Controls.Add(label);
                    masalarGroupBox.Controls.Add(pictureBox);

                    
                    seciliMasaNumarasi = masaNumaras�;
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
            ogrenciler["Ay�e"] = 789;
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
                        MessageBox.Show($"Se�ilen masa numaras�: {secilenMasaNumarasi}\n\nMasay� ba�ar�yla se�tiniz.", "Masa Se�ildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ge�ersiz masa numaras�! L�tfen ge�erli bir masa numaras� girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Masa numaras� say�sal bir de�er olmal�d�r.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
