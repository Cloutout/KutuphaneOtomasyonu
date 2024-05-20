using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO.Ports;

namespace KutuphaneOtomasyon
{
    public partial class FormGiris : Form
    {
        int toplamMasaSayisi;
        int seciliMasaNumarasi;
        public delegate void TableAvailabilityChangedEventHandler(int tableNumber);
        public event TableAvailabilityChangedEventHandler OnTableAvailabilityChanged;

        public FormGiris()
        {
            InitializeComponent();
            MasalariGetir();
            UpdateTableStatus();
        }

        string connectionString = "Data Source=MERT;Initial Catalog=kutuphaneDB;Integrated Security=True";

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
                int s�raSay�s� = (int)Math.Ceiling((double)masaAdet / 3);

                for (int i = 0; i < masaAdet; i++)
                {
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Name = "masa" + (i + 1).ToString();
                    pictureBox.Image = Image.FromFile("C:\\Users\\merti\\OneDrive\\Masa�st�\\KutuphaneOtomasyonu\\KutuphaneArayuz\\KutuphaneOtomasyon\\Resources\\desk.png");
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
        }

        private void masaNoTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (int.TryParse(masaNoTextBox.Text, out int seciliMasaNumarasi))
                {
                    if (seciliMasaNumarasi >= 1 && seciliMasaNumarasi <= toplamMasaSayisi)
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            string query = "SELECT isAvaible FROM Tbl_Masalar WHERE MasaNo = @masaNo";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@masaNo", seciliMasaNumarasi);

                            connection.Open();
                            bool isAvailable = (bool)command.ExecuteScalar();
                            if (isAvailable)
                            {
                                MessageBox.Show("L�tfen kart�n�z� okutunuz.", "Kart Okuma", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                string ogrenciKartNumarasi = ReadCardNumberFromArduino();

                                query = "SELECT COUNT(*) FROM Tbl_Ogrenciler WHERE ogrenciNo = @ogrenciNo";
                                command.CommandText = query;
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@ogrenciNo", ogrenciKartNumarasi);
                                int ogrenciCount = (int)command.ExecuteScalar();

                                if (ogrenciCount > 0)
                                {
                                    // Check if the student already has a table assigned
                                    query = "SELECT COUNT(*) FROM Tbl_Masalar WHERE ogrenciNo = @ogrenciNo";
                                    command.CommandText = query;
                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@ogrenciNo", ogrenciKartNumarasi);
                                    int masaCount = (int)command.ExecuteScalar();

                                    if (masaCount > 0)
                                    {
                                        MessageBox.Show("Bu ��renci numaras�na zaten bir masa atanm��! L�tfen ba�ka bir kart okutunuz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        query = "UPDATE Tbl_Masalar SET isAvaible = 0, ogrenciNo = (SELECT ogrenciNo FROM Tbl_Ogrenciler WHERE ogrenciNo = @ogrenciNo) WHERE MasaNo = @masaNo";
                                        command.CommandText = query;
                                        command.Parameters.AddWithValue("@masaNo", seciliMasaNumarasi);
                                        command.ExecuteNonQuery();

                                        Control[] controls = masalarGroupBox.Controls.Find("masa" + seciliMasaNumarasi.ToString(), true);
                                        if (controls.Length > 0 && controls[0] is PictureBox pictureBox)
                                        {
                                            pictureBox.BackColor = Color.Red;
                                        }

                                        MessageBox.Show($"Se�ilen masa numaras�: {seciliMasaNumarasi}\n\nMasay� ba�ar�yla se�tiniz.", "Masa Se�ildi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                        OnTableAvailabilityChanged?.Invoke(seciliMasaNumarasi);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Ge�ersiz kart numaras�! L�tfen ge�erli bir kart okutunuz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Se�ilen masa dolu! L�tfen ba�ka bir masa se�in.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ge�ersiz masa numaras�! L�tfen ge�erli bir masa numaras� girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private string ReadCardNumberFromArduino()
        {
           /* using (SerialPort serialPort = new SerialPort("COM3", 9600)) // Replace "COM3" with your actual COM port and 9600 with your baud rate
            {
                serialPort.Open();
                string cardNumber = serialPort.ReadLine();
                serialPort.Close();
                return cardNumber.Trim(); // Use Trim() to remove any trailing newline characters
            }*/
           return "111";
        }

        private void UpdateTableStatus()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT MasaNo, isAvaible FROM Tbl_Masalar";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int masaNo = Convert.ToInt32(reader["MasaNo"]);
                    bool isAvailable = Convert.ToBoolean(reader["isAvaible"]);

                    Control[] controls = masalarGroupBox.Controls.Find("masa" + masaNo.ToString(), true);
                    if (controls.Length > 0 && controls[0] is PictureBox)
                    {
                        PictureBox pictureBox = (PictureBox)controls[0];

                        if (isAvailable)
                        {
                            pictureBox.BackColor = Color.Green;
                        }
                        else
                        {
                            pictureBox.BackColor = Color.Red;
                        }
                    }
                    OnTableAvailabilityChanged?.Invoke(masaNo);
                }
            }
        }
    }

}
