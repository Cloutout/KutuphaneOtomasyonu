using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KutuphaneOtomasyon
{
    public partial class GuvenlikEkraniForm : Form
    {
        string connectionString = "Data Source=MERT;Initial Catalog=kutuphaneDB;Integrated Security=True;";
        int toplamMasaSayisi;
        public GuvenlikEkraniForm()
        {
            InitializeComponent();
            MasalariGetir();
        }
    }

    private void MasalariGetir()
    {

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT COUNT(*) AS MasaAdet FROM Tbl_Masalar";
            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            int masaAdet = (int)command.ExecuteScalar();
            toplamMasaSayisi = masaAdet;

            int resimBoyutu = 100;
            int sıraSayısı = (int)Math.Ceiling((double)masaAdet / 3);

            for (int i = 0; i < masaAdet; i++)
            {
                PictureBox pictureBox = new PictureBox
                {
                    Name = "masa" + (i + 1),
                    Image = Image.FromFile("C:\\Users\\merti\\OneDrive\\Masaüstü\\RFID\\KutuphaneOtomasyonu-main\\KutuphaneOtomasyon\\Resources\\desk.png"),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Width = resimBoyutu,
                    Height = resimBoyutu,
                    Location = new Point((i % 16) * (resimBoyutu + 10), (i / 16) * (resimBoyutu + 30))
                };

                Label label = new Label
                {
                    Text = "Masa No: " + (i + 1),
                    AutoSize = true,
                    Location = new Point(pictureBox.Location.X, pictureBox.Location.Y + resimBoyutu)
                };

                masalarGroupBox.Controls.Add(label);
                masalarGroupBox.Controls.Add(pictureBox);
            }
        }

    }
}
