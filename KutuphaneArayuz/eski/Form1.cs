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
        

    }

}

