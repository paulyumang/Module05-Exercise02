using Microsoft.Maui.Controls;
using Module07DataAccess.Services;
using MySql.Data.MySqlClient;

namespace Module07DataAccess
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private readonly DatabaseConnectionServices _dbConnectionService;

        public MainPage()
        {
            InitializeComponent();
            // Initializes The Database Connection
            _dbConnectionService = new DatabaseConnectionServices();
        }

        private async void OnTestConnectionClicked(object sender, EventArgs e)
        {
            var connectionString = _dbConnectionService.GetConnectionString();

            // Connection Testing
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    ConnectionStatusLabel.Text = "Connection Successful!";
                    ConnectionStatusLabel.TextColor = Colors.Green;
                }
            }
            catch (Exception ex)
            {
                ConnectionStatusLabel.Text = $"Connection Failed: {ex.Message}";
                ConnectionStatusLabel.TextColor = Colors.Red;
            }
        }

        private async void OpenViewEmployee(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ViewEmployee");
        }
    }
}
