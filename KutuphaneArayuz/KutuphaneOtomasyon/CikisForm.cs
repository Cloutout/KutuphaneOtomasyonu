using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;

namespace KutuphaneOtomasyon
{
    public partial class CikisForm : Form
    {
        int toplamMasaSayisi;
        string connectionString = "Data Source=DESKTOP-N7TL0LE;Initial Catalog=kutuphaneDB;Integrated Security=True;";
        private FormGiris _girisForm;
        private SerialPort _serialPort;

        public CikisForm(FormGiris girisForm)
        {
            InitializeComponent();
            _girisForm = girisForm; // Giriş formu örneğini atama

            MasalariGetir();
            UpdateTableStatus();

            // Giriş formundan gelen etkinliği dinle
            _girisForm.OnTableAvailabilityChanged += OnTableAvailabilityChanged;

            // Seri port ayarları
            _serialPort = new SerialPort("COM7", 9600);
            _serialPort.DataReceived += SerialPort_DataReceived;
            _serialPort.Open();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string kartVerisi = _serialPort.ReadLine().Trim();
                if (!string.IsNullOrEmpty(kartVerisi))
                {
                    // Seri porttan gelen veriyi işlemek için Invoke kullan
                    this.Invoke(new Action(() =>
                    {
                        ReleaseTableByStudentNumber(kartVerisi);
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Seri porttan veri okuma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnTableAvailabilityChanged(int tableNumber)
        {
            UpdateTableVisual(tableNumber, GetColorBasedOnAvailability(tableNumber)); // Duruma göre renk güncelle
        }

        private bool IsTableAvailable(int tableNumber)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT isAvaible FROM Tbl_Masalar WHERE MasaNo = @masaNo";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@masaNo", tableNumber);

                connection.Open();
                bool isAvailable = (bool)command.ExecuteScalar();
                return isAvailable;
            }
        }

        private Color GetColorBasedOnAvailability(int tableNumber)
        {
            bool isAvailable = IsTableAvailable(tableNumber); // Durumu kontrol et
            return isAvailable ? Color.Green : Color.Red;
        }

        private void MasalariGetir()
        {
            try
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
                            Image = Image.FromFile("C:\\Users\\Furkan\\Downloads\\KutuphaneOtomasyonu-main (3)\\KutuphaneOtomasyonu-main\\KutuphaneArayuz\\KutuphaneOtomasyon\\Resources\\desk.png"),
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            Width = resimBoyutu,
                            Height = resimBoyutu,
                            Location = new Point((i % 3) * (resimBoyutu + 10), (i / 3) * (resimBoyutu + 30))
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
            catch (Exception ex)
            {
                MessageBox.Show($"Tablolar alınırken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReleaseTable(int masaNo)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Tbl_Masalar SET isAvaible = 1, ogrenciNo = NULL WHERE MasaNo = @masaNo";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@masaNo", masaNo);

                    connection.Open();
                    command.ExecuteNonQuery();

                    MessageBox.Show($"Masa numarası: {masaNo}\n\nMasa başarıyla boşaltıldı.", "Masa Boşaltıldı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Masanın durumunu güncelledikten sonra görsel arayüzde de güncelle
                    UpdateTableVisual(masaNo, Color.Green);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReleaseTableByStudentNumber(string ogrenciNo)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Tbl_Masalar SET isAvaible = 1, ogrenciNo = NULL WHERE ogrenciNo = @ogrenciNo";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Öğrenci numarası: {ogrenciNo}\n\nMasa başarıyla boşaltıldı.", "Masa Boşaltıldı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UpdateTableStatus();
                    }
                    else
                    {
                        MessageBox.Show($"Öğrenci numarası: {ogrenciNo}\n\nEşleşen kayıt bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTableStatus()
        {
            try
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
                        UpdateTableVisual(masaNo, isAvailable ? Color.Green : Color.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tablo durumu güncellenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTableVisual(int masaNo, Color color)
        {
            Control[] controls = masalarGroupBox.Controls.Find("masa" + masaNo.ToString(), true);
            if (controls.Length > 0 && controls[0] is PictureBox pictureBox)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => pictureBox.BackColor = color));
                }
                else
                {
                    pictureBox.BackColor = color;
                }
            }
        }
    }
}
