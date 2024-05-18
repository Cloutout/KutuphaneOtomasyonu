namespace KutuphaneOtomasyon
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Form1 ve Form2 i�in �rnekler olu�turun
            FormGiris girisForm = new FormGiris(); // Create an instance of FormGiris
            CikisForm cikisForm = new CikisForm(girisForm); // Pass the FormGiris instance to CikisForm constructor

            // �ki formu ayn� anda g�sterin
            girisForm.Show();
            cikisForm.Show();

            // Ana uygulamay� ba�lat�n
            Application.Run();
        }
    }
}