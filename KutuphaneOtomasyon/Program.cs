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

            // Create the main forms
            FormGiris girisForm = new FormGiris();
            CikisForm cikisForm = new CikisForm(girisForm);

            // Register events
            girisForm.RegisterToExitFormEvents(cikisForm);

            // Show both forms
            girisForm.Show();
            cikisForm.Show();

            // Run the application with the main form
            Application.Run(girisForm);
        }
    }
}
