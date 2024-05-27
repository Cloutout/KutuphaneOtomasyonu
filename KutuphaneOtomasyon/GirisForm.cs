using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using System.Data.SqlClient;
using KutuphaneOtomasyon.Properties;
using Microsoft.VisualBasic.ApplicationServices;

namespace KutuphaneOtomasyon
{
    public partial class FormGiris : Form
    {
        int toplamMasaSayisi;
        int seciliMasaNumarasi;
        public delegate void TableAvailabilityChangedEventHandler(int tableNumber);
        public event TableAvailabilityChangedEventHandler OnTableAvailabilityChanged;

        private SerialPort serialPort;
        private string connectionString = "Data Source=MERT;Initial Catalog=kutuphaneDB;Integrated Security=True";

        public FormGiris()
        {
            InitializeComponent();
            MasalariGetir();
            UpdateTableStatus();
            InitializeSerialPort();
        }

        public void RegisterToExitFormEvents(CikisForm cikisForm)
        {
            cikisForm.OnTableReleased += OnTableReleased;
        }

        private void OnTableReleased(int masaNo)
        {
            UpdateTableStatus();
        }

        private void InitializeSerialPort()
        {
            string portName = "COM3";
            int baudRate = 9600;

            serialPort = new SerialPort(portName, baudRate);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
            //serialPort.Open();

        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            
                string data = serialPort.ReadLine().Trim();
                this.Invoke(new Action(() => ProcessReceivedData(data)));
           
        }

        private void ProcessReceivedData(string data)
        {
            string ogrenciKartNumarasi = data;
            if (seciliMasaNumarasi == 0)
            {
                MessageBox.Show("L�tfen �nce masa numaras�n� giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    query = "SELECT isAvaible, MolaModu FROM Tbl_Masalar WHERE MasaNo = @masaNo";
                    command.CommandText = query;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@masaNo", seciliMasaNumarasi);
                    SqlDataReader reader = command.ExecuteReader();
                    //command.Parameters.AddWithValue("@ogrenciNo", ogrenciKartNumarasi);
                    //int masaCount = (int)command.ExecuteScalar();

                    if (reader.Read())
                    {
                        bool isAvailable = Convert.ToBoolean(reader["isAvaible"]);
                        bool molaModu = reader["MolaModu"] != DBNull.Value && Convert.ToBoolean(reader["MolaModu"]);


                        if (isAvailable)
                        {
                            AssignTableToStudent(ogrenciKartNumarasi);
                        }
                        else if (molaModu)
                        {
                            RemoveMolaModuFromTable();
                        }
                        else
                        {
                            MessageBox.Show("Se�ilen masa dolu! L�tfen ba�ka bir masa se�in.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Ge�ersiz kart numaras�! L�tfen ge�erli bir kart okutunuz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AssignTableToStudent(string ogrenciNo)
        {
            string query = "SELECT COUNT(*) FROM Tbl_Masalar WHERE ogrenciNo = @ogrenciNo";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
                connection.Open();
                int masaCount = (int)command.ExecuteScalar();

                if (masaCount >= 1)
                {
                    MessageBox.Show("��renci zaten bir masaya atanm��! Bir ��renci yaln�zca bir masa kullanabilir.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            query = "UPDATE Tbl_Masalar SET isAvaible = 0, ogrenciNo = @ogrenciNo WHERE MasaNo = @masaNo AND isAvaible = 1";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
                command.Parameters.AddWithValue("@masaNo", seciliMasaNumarasi);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show($"Masa numaras�: {seciliMasaNumarasi}\n\n��renciye masa ba�ar�yla atanm��t�r.", "Masa Atama", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    OnTableAvailabilityChanged?.Invoke(seciliMasaNumarasi);
                }
                else
                {
                    MessageBox.Show($"Masa numaras�: {seciliMasaNumarasi}\n\nMasa ��renciye atanamad� veya zaten dolu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            UpdateTableStatus();
        }
        private void RemoveMolaModuFromTable()
        {
            string query = "UPDATE Tbl_Masalar SET MolaModu = 0 WHERE MasaNo = @masaNo";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@masaNo", seciliMasaNumarasi);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show($"Masa numaras�: {seciliMasaNumarasi}\n\nMasa mola modundan ��kar�lm��t�r.", "Mola Modu Kald�r�ld�", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    OnTableAvailabilityChanged?.Invoke(seciliMasaNumarasi);
                }
                else
                {
                    MessageBox.Show($"Masa numaras�: {seciliMasaNumarasi}\n\nMasa mola modundan ��kar�lamad�.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            UpdateTableStatus();
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
                int s�raSay�s� = (int)Math.Ceiling((double)masaAdet / 3);

                for (int i = 0; i < masaAdet; i++)
                {
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Name = "masa" + (i + 1).ToString();
                    pictureBox.Image = Image.FromFile("C:\\Users\\merti\\OneDrive\\Masa�st�\\RFID\\KutuphaneOtomasyonu-main\\KutuphaneOtomasyon\\Resources\\desk.png");
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox.Width = resimBoyutu;
                    pictureBox.Height = resimBoyutu;
                    pictureBox.Location = new Point((i % 16) * (resimBoyutu + 10), (i / 16) * (resimBoyutu + 30));

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
                                MessageBox.Show("L�tfen kart�n�z� okutunuz.", "Kart Okuma", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        public void UpdateTableStatus()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT MasaNo, isAvaible, ISNULL(MolaModu, 0) AS MolaModu FROM Tbl_Masalar";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int masaNo = Convert.ToInt32(reader["MasaNo"]);
                    bool isAvailable = Convert.ToBoolean(reader["isAvaible"]);
                    bool molaModu = Convert.ToBoolean(reader["MolaModu"]);

                    Color color;

                    if (molaModu)
                    {
                        color = Color.Yellow;
                    }
                    else
                    {
                        color = isAvailable ? Color.Green : Color.Red;
                    }

                    Control[] controls = masalarGroupBox.Controls.Find("masa" + masaNo.ToString(), true);
                    if (controls.Length > 0 && controls[0] is PictureBox)
                    {
                        PictureBox pictureBox = (PictureBox)controls[0];
                        pictureBox.BackColor = color;
                    }
                    OnTableAvailabilityChanged?.Invoke(masaNo);
                }
            }
        }
    }
}
