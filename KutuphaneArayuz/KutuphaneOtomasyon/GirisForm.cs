using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace KutuphaneOtomasyon
{
    public partial class FormGiris : Form
    {
        int toplamMasaSayisi;
        int seciliMasaNumarasi;
        public delegate void TableAvailabilityChangedEventHandler(int tableNumber);
        public event TableAvailabilityChangedEventHandler OnTableAvailabilityChanged;

        private SerialPort serialPort;
        private string connectionString = "Data Source=DESKTOP-N7TL0LE;Initial Catalog=kutuphaneDB;Integrated Security=True";

        public FormGiris()
        {
            InitializeComponent();
            MasalariGetir();
            UpdateTableStatus();
            InitializeSerialPort();
        }

        private void InitializeSerialPort()
        {
            string portName = "COM3";
            int baudRate = 9600;

            serialPort = new SerialPort(portName, baudRate);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
            serialPort.Open();
          
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort.ReadLine().Trim();
                this.Invoke(new Action(() => ProcessReceivedData(data)));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri okunamadý: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessReceivedData(string data)
        {
            string ogrenciKartNumarasi = data;
            if (seciliMasaNumarasi == 0)
            {
                MessageBox.Show("Lütfen önce masa numarasýný giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "SELECT COUNT(*) FROM Tbl_Ogrenciler WHERE ogrenciNo = @ogrenciNo";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ogrenciNo", ogrenciKartNumarasi);
                connection.Open();
                int ogrenciCount = (int)command.ExecuteScalar();

                if (ogrenciCount > 0)
                {
                    query = "SELECT COUNT(*) FROM Tbl_Masalar WHERE ogrenciNo = @ogrenciNo";
                    command.CommandText = query;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@ogrenciNo", ogrenciKartNumarasi);
                    int masaCount = (int)command.ExecuteScalar();

                    if (masaCount > 0)
                    {
                        MessageBox.Show("Bu öðrenci numarasýna zaten bir masa atanmýþ! Lütfen baþka bir kart okutunuz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                        MessageBox.Show($"Seçilen masa numarasý: {seciliMasaNumarasi}\n\nMasayý baþarýyla seçtiniz.", "Masa Seçildi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        OnTableAvailabilityChanged?.Invoke(seciliMasaNumarasi);
                    }
                }
                else
                {
                    MessageBox.Show("Geçersiz kart numarasý! Lütfen geçerli bir kart okutunuz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                int sýraSayýsý = (int)Math.Ceiling((double)masaAdet / 3);

                for (int i = 0; i < masaAdet; i++)
                {
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Name = "masa" + (i + 1).ToString();
                    pictureBox.Image = Image.FromFile("C:\\Users\\Furkan\\Downloads\\KutuphaneOtomasyonu-main (3)\\KutuphaneOtomasyonu-main\\KutuphaneArayuz\\KutuphaneOtomasyon\\Resources\\desk.png");
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
                }
            }
        }

        private void masaNoTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (int.TryParse(masaNoTextBox.Text, out seciliMasaNumarasi))
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
                                MessageBox.Show("Lütfen kartýnýzý okutunuz.", "Kart Okuma", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Seçilen masa dolu! Lütfen baþka bir masa seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Geçersiz masa numarasý! Lütfen geçerli bir masa numarasý girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
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
