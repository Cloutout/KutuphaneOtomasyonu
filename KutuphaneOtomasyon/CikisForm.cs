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
        string connectionString = "Data Source=MERT;Initial Catalog=kutuphaneDB;Integrated Security=True;";
        private FormGiris _girisForm;
        private SerialPort _serialPort;
        private System.Timers.Timer timer;

        public delegate void TableReleasedEventHandler(int tableNumber);
        public event TableReleasedEventHandler OnTableReleased;

        public CikisForm(FormGiris girisForm)
        {
            InitializeComponent();
            _girisForm = girisForm;
            InitializeTimer();
            MasalariGetir();
            UpdateTableStatus();
            CheckMolaModu();
            
            _girisForm.OnTableAvailabilityChanged += OnTableAvailabilityChanged;

            
            _serialPort = new SerialPort("COM7", 9600);
            _serialPort.DataReceived += SerialPort_DataReceived;
            //_serialPort.Open();
        }
        private void InitializeTimer()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 60000;
            timer.Elapsed += TimerElapsed;
            timer.Start();
        }
        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckMolaDuration();
        }
        private void CheckMolaDuration()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT MasaNo, MolaBaslangicZamani FROM Tbl_Masalar WHERE MolaModu = 1";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int masaNo = Convert.ToInt32(reader["MasaNo"]);
                    DateTime molaBaslangicZamani = Convert.ToDateTime(reader["MolaBaslangicZamani"]);
                    if (DateTime.Now.Subtract(molaBaslangicZamani).TotalMinutes >= 1)
                    {
                        ReleaseTable(masaNo);
                    }
                }
            }
        }
        private void CheckMolaModu()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT MasaNo, MolaModu FROM Tbl_Masalar";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int masaNo = Convert.ToInt32(reader["MasaNo"]);

                    if (reader["MolaModu"] != DBNull.Value)
                    {
                        bool molaModu = Convert.ToBoolean(reader["MolaModu"]);
                        if (molaModu)
                        {
                            UpdateTableVisual(masaNo, Color.Yellow);
                        }
                    }
                }
            }
        }


        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
           
                string kartVerisi = _serialPort.ReadLine().Trim();
                if (!string.IsNullOrEmpty(kartVerisi))
                {
                    
                    this.Invoke(new Action(() =>
                    {
                        ReleaseTableByStudentNumber(kartVerisi);
                    }));
                }
            
        }

        private void OnTableAvailabilityChanged(int tableNumber)
        {
            UpdateTableVisual(tableNumber, GetColorBasedOnAvailability(tableNumber));
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
            bool isAvailable = IsTableAvailable(tableNumber);
            return isAvailable ? Color.Green : Color.Red;
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

        private void ReleaseTable(int masaNo)
        {
           
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Tbl_Masalar SET isAvaible = 1, ogrenciNo = NULL, MolaModu = 0, MolaBaslangicZamani = NULL WHERE MasaNo = @masaNo";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@masaNo", masaNo);
                    command.Parameters.AddWithValue("@molaBaslangicZamani", DateTime.Now);

                    connection.Open();
                    command.ExecuteNonQuery();

                    MessageBox.Show($"Masa numarası: {masaNo}\n\nMasa başarıyla boşaltıldı.", "Masa Boşaltıldı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    
                    UpdateTableVisual(masaNo, Color.Green);

                    
                    OnTableReleased?.Invoke(masaNo);

                    _girisForm.UpdateTableStatus();
                    UpdateTableStatus();
            }
           
        }

        private void ReleaseTableByStudentNumber(string ogrenciNo)
        {
            
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Tbl_Masalar SET isAvaible = 1, ogrenciNo = NULL, MolaModu = 0, MolaBaslangicZamani = NULL WHERE ogrenciNo = @ogrenciNo";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Öğrenci numarası: {ogrenciNo}\n\nMasa başarıyla boşaltıldı.", "Masa Boşaltıldı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UpdateTableStatus();

                        
                        for (int i = 1; i <= toplamMasaSayisi; i++)
                        {
                            OnTableReleased?.Invoke(i);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Öğrenci numarası: {ogrenciNo}\n\nEşleşen kayıt bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        UpdateTableVisual(masaNo, isAvailable ? Color.Green : Color.Red);
                    }
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

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string kartVerisi = Microsoft.VisualBasic.Interaction.InputBox("Lütfen Kartınızı Okutun!", "RFID Kart Okuma", "");

                if (!string.IsNullOrEmpty(kartVerisi))
                {
                    if (textBox1.Text == "1")
                    {
                        SetMolaModuByStudentNumber(kartVerisi, true);
                    }
                    else if (textBox1.Text == "2")
                    {
                        ReleaseTableByStudentNumber(kartVerisi);
                    }
                }
            }
        }

        private void SetMolaModuByStudentNumber(string ogrenciNo, bool molaModu)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DateTime molaBaslangicZamani = DateTime.Now;
                string query = "UPDATE Tbl_Masalar SET MolaModu = @molaModu, MolaBaslangicZamani = @molaBaslangicZamani WHERE ogrenciNo = @ogrenciNo";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@molaModu", molaModu);
                command.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
                command.Parameters.AddWithValue("@molaBaslangicZamani", molaBaslangicZamani);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show($"Öğrenci numarası: {ogrenciNo}\n\nMola moduna geçildi.", "Mola Modu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateTableStatus();
                }
                else
                {
                    MessageBox.Show($"Öğrenci numarası: {ogrenciNo}\n\nEşleşen kayıt bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                _girisForm.UpdateTableStatus();
                UpdateTableVisualByStudentNumber(ogrenciNo, molaModu);
            }
        }

        private void UpdateTableVisualByStudentNumber(string ogrenciNo, bool molaModu)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT MasaNo, MolaModu FROM Tbl_Masalar WHERE ogrenciNo = @ogrenciNo";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int masaNo = Convert.ToInt32(reader["MasaNo"]);
                    bool masaMolaModu = Convert.ToBoolean(reader["MolaModu"]);
                    if (masaMolaModu)
                    {
                        UpdateTableVisual(masaNo, Color.Yellow);
                    }
                    else
                    {
                        UpdateTableVisual(masaNo, masaMolaModu ? Color.Yellow : Color.Green);
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void SetMolaModu(int masaNo, bool molaModu)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Tbl_Masalar SET MolaModu = @molaModu WHERE MasaNo = @masaNo";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@molaModu", molaModu);
                command.Parameters.AddWithValue("@masaNo", masaNo);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private bool GetMolaModu(int masaNo)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT MolaModu FROM Tbl_Masalar WHERE MasaNo = @masaNo";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@masaNo", masaNo);

                connection.Open();
                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    return (bool)result;
                }
                else
                {
                    return false;
                }
            }
        }














    }
}
