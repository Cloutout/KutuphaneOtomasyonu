using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        SQLiteConnection connection;

        public Form1()
        {
            InitializeComponent();
            InitializeDatabase();
            CreateDesks();
            DeskNumberTextBox.KeyDown += DeskNumberTextBox_KeyDown;
        }

        private void InitializeDatabase()
        {
            string connectionString = "Data Source=desks.db;Version=3;";
            connection = new SQLiteConnection(connectionString);
            connection.Open();

            string createTableQuery = "CREATE TABLE IF NOT EXISTS desks (ID INTEGER PRIMARY KEY AUTOINCREMENT, DeskNumber INTEGER UNIQUE, StudentName TEXT, StudentNumber TEXT, isAvailable TEXT)";
            SQLiteCommand command = new SQLiteCommand(createTableQuery, connection);
            command.ExecuteNonQuery();


            string insertDataQuery = "INSERT INTO desks (StudentName, StudentNumber, isAvailable) VALUES " +
                         "('','' ,'Empty'), ('', '', 'Empty'), ('', '', 'Empty'), " +
                         "('', '', 'Empty'), ('', '', 'Empty'), ('', '', 'Empty'), " +
                         "('', '', 'Empty'), ('', '', 'Empty'), ('', '', 'Empty'), " +
                         "('', '', 'Empty')";
            SQLiteCommand insertCommand = new SQLiteCommand(insertDataQuery, connection);
            insertCommand.ExecuteNonQuery();

        }

        private void CreateDesks()
        {
            int deskCount = 10;
            int columns = 3;

            int rows = deskCount / columns;

            if (deskCount % columns != 0)
            {
                rows++;
            }

            panel.Size = new Size(columns * 120, rows * 120);

            for (int i = 1; i <= deskCount; i++)
            {
                PictureBox pictureBox = new PictureBox();
                pictureBox.Name = "pictureBox" + i;
                pictureBox.Width = 100;
                pictureBox.Height = 100;

                int row = (i - 1) / columns;
                int column = (i - 1) % columns;

                pictureBox.Location = new Point(column * 120, row * 120);

                pictureBox.Image = Image.FromFile(@"C:\Users\merti\OneDrive\Masaüstü\KutuphaneArayuz\WinFormsApp1\Resources\desk.png");

                Label label = new Label();
                label.Text = "Masa No: " + i.ToString();
                label.AutoSize = true;
                label.BackColor = Color.Transparent;
                label.Location = new Point(pictureBox.Width - label.Width, 0);

                pictureBox.Controls.Add(label);

                string isAvailable = CheckDeskAvailability(i);
                SetDeskColor(pictureBox, isAvailable);

                panel.Controls.Add(pictureBox);
            }
        }

        private string CheckDeskAvailability(int deskNumber)
        {
            string query = "SELECT isAvailable FROM desks WHERE DeskNumber = @deskNumber";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@deskNumber", deskNumber);
                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    return result.ToString();
                }
            }

            return "Not Found";
        }

        private void SetDeskColor(PictureBox pictureBox, string isAvailable)
        {
            switch (isAvailable)
            {
                case "Empty":
                    pictureBox.BackColor = Color.LightGreen;
                    break;
                case "Occupied":
                    pictureBox.BackColor = Color.LightCoral;
                    break;
                case "Break":
                    pictureBox.BackColor = Color.LightBlue;
                    break;
            }
        }

        private void DeskNumberTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (int.TryParse(DeskNumberTextBox.Text, out int selectedDesk))
                {
                    string isAvailable = CheckDeskAvailability(selectedDesk);

                    if (isAvailable == "Empty")
                    {
                        MessageBox.Show("Lütfen öðrenci kartýnýzý okutunuz.");

                
                        string updateQuery = "UPDATE desks SET isAvailable = 'Occupied' WHERE DeskNumber = @deskNumber";

                        using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@deskNumber", selectedDesk);
                            command.ExecuteNonQuery();
                        }

                        PictureBox pictureBox = panel.Controls.Find("pictureBox" + selectedDesk, true).FirstOrDefault() as PictureBox;
                        SetDeskColor(pictureBox, "Occupied");
                        MessageBox.Show("Masa seçimi baþarýlý! Öðrenciye masa atandý.");
                    }
                    else if (isAvailable == "Occupied")
                    {
                        MessageBox.Show("Seçilen masa dolu! Lütfen baþka masa seçiniz.");
                    }
                }
                else
                {
                    MessageBox.Show("Geçersiz masa numarasý! Lütfen doðru bir masa numarasý giriniz.");
                }

                DeskNumberTextBox.Clear();
            }
        }

        //Masalarý oluþturmak için kullanýlan fonksiyon
        /* public void MasaSayisi(int sayý)
         {
             int resimBoyutu = 100;
             int sýraSayýsý = (int)Math.Ceiling((double)sayý / 3);

             for (int i = 0; i < sayý; i++)
             {
                 PictureBox pictureBox = new PictureBox();
                 pictureBox.Name = "masa" + (i + 1).ToString();
                 pictureBox.Image = Image.FromFile("C:\\Users\\merti\\OneDrive\\Masaüstü\\KutuphaneOtomasyonu\\KutuphaneArayuz\\KutuphaneOtomasyon\\Resources\\desk.png");
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
         }*/
        /*private void masaNoTextBox_KeyDown(object sender, KeyEventArgs e)
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
        
        632r1ü

    }

}
*/














using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace KutuphaneOtomasyon
    {
        public partial class FormGiris : Form
        {
            int toplamMasaSayisi;
            string ogrenciNo;
            string connectionString = "Data Source=MERT;Initial Catalog=kutuphaneDB;Integrated Security=True";

            public FormGiris()
            {
                InitializeComponent();
                try
                {
                    MasalariGetir();
                    UpdateTableStatus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                        int sýraSayýsý = (int)Math.Ceiling((double)masaAdet / 3);

                        for (int i = 0; i < masaAdet; i++)
                        {
                            PictureBox pictureBox = new PictureBox
                            {
                                Name = "masa" + (i + 1),
                                Image = Image.FromFile("C:\\Users\\merti\\OneDrive\\Masaüstü\\KutuphaneOtomasyonu\\KutuphaneArayuz\\KutuphaneOtomasyon\\Resources\\desk.png"),
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
                    MessageBox.Show($"Error retrieving tables: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void RFIDCardScanned(string rfidCardNumber)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Check if RFID card number matches an existing student
                        string query = "SELECT ogrenciNo FROM Tbl_Ogrenciler WHERE rfidCardNumber = @rfidCardNumber";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@rfidCardNumber", rfidCardNumber);

                        object result = command.ExecuteScalar();
                        if (result == null)
                        {
                            MessageBox.Show("Geçersiz kart numarasý! Lütfen geçerli bir kart kullanýn.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        ogrenciNo = result.ToString();

                        // Check if the student already has a table assigned
                        query = "SELECT COUNT(*) FROM Tbl_Masalar WHERE ogrenciNo = @ogrenciNo";
                        command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);

                        int existingTables = (int)command.ExecuteScalar();
                        if (existingTables > 0)
                        {
                            MessageBox.Show("Bu öðrenci zaten bir masa almýþ.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Prompt the user to select a table number
                        MessageBox.Show("Lütfen masa numarasýný seçiniz.", "Masa Seçimi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void masaNoTextBox_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    try
                    {
                        if (int.TryParse(masaNoTextBox.Text, out int seciliMasaNumarasi))
                        {
                            if (seciliMasaNumarasi >= 1 && seciliMasaNumarasi <= toplamMasaSayisi)
                            {
                                AssignTableToStudent(ogrenciNo, seciliMasaNumarasi);
                            }
                            else
                            {
                                MessageBox.Show("Geçersiz masa numarasý! Lütfen geçerli bir masa numarasý girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            private void AssignTableToStudent(string ogrenciNo, int masaNo)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "SELECT isAvaible FROM Tbl_Masalar WHERE MasaNo = @masaNo";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@masaNo", masaNo);

                        connection.Open();
                        bool isAvailable = (bool)command.ExecuteScalar();
                        if (isAvailable)
                        {
                            MessageBox.Show($"Seçilen masa numarasý: {masaNo}\n\nMasayý baþarýyla seçtiniz.", "Masa Seçildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            UpdateTableVisual(masaNo, Color.Red);

                            query = "UPDATE Tbl_Masalar SET isAvaible = 0, ogrenciNo = @ogrenciNo WHERE MasaNo = @masaNo";
                            command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
                            command.Parameters.AddWithValue("@masaNo", masaNo);
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            MessageBox.Show("Seçilen masa dolu! Lütfen baþka bir masa seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show($"Error updating table status: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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





