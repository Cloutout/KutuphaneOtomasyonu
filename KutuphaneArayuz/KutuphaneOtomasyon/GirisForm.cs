using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SqlTypes;

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
                    int sýraSayýsý = (int)Math.Ceiling((double)masaAdet / 3);

                    for (int i = 0; i < masaAdet; i++)
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
                                    MessageBox.Show($"Seçilen masa numarasý: {seciliMasaNumarasi}\n\nMasayý baþarýyla seçtiniz.", "Masa Seçildi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    // Masa dolu olduðunda arka planý kýrmýzý yap
                                    Control[] controls = masalarGroupBox.Controls.Find("masa" + seciliMasaNumarasi.ToString(), true);
                                    if (controls.Length > 0 && controls[0] is PictureBox)
                                    {
                                        PictureBox pictureBox = (PictureBox)controls[0];
                                        pictureBox.BackColor = Color.Red;
                                    }
                                    

                                    // Masa durumunu veritabanýnda güncelle
                                    query = "UPDATE Tbl_Masalar SET isAvaible = 0 WHERE MasaNo = @masaNo";
                                    command.CommandText = query;
                                    command.ExecuteNonQuery();
                                }
                                else
                                {
                                    MessageBox.Show("Seçilen masa dolu! Lütfen baþka bir masa seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }

                                OnTableAvailabilityChanged?.Invoke(seciliMasaNumarasi);

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
