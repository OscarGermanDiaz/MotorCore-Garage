using MotorCore_Garage.Models;
using MotorCore_Garage.Services;

namespace MotorCore_Garage.Pages
{
    public partial class ClienteInicioPage : ContentPage
    {
        public ClienteInicioPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var cliente = Taller.ClientePorId(Taller.ClienteActualId);
            Saludo.Text = cliente is null ? "Hola" : $"Hola, {cliente.Nombre.Split(' ')[0]}";

            var vehiculos = Taller.VehiculosDeCliente(Taller.ClienteActualId).ToList();
            Resumen.Text = vehiculos.Count switch
            {
                0 => "Aún no tienes vehículos registrados",
                1 => "1 vehículo registrado",
                _ => $"{vehiculos.Count} vehículos registrados"
            };

            var proxima = Taller.CitasDeCliente(Taller.ClienteActualId)
                .Where(c => c.Fecha.Date >= DateTime.Today)
                .OrderBy(c => c.Fecha)
                .FirstOrDefault();
            ProximaCita.Children.Clear();
            ProximaCita.Children.Add(proxima is null
                ? new Label { Text = "No tienes citas programadas", Style = (Style)Application.Current!.Resources["Pie"] }
                : Vistas.CitaCard(proxima));

            Vehiculos.Children.Clear();
            foreach (var v in vehiculos)
            {
                var id = v.Id;
                Vehiculos.Children.Add(Vistas.VehiculoCard(v,
                    () => Shell.Current.GoToAsync("expediente", new Dictionary<string, object> { ["VehiculoId"] = id })));
            }
        }

        async void OnAgendar(object? sender, EventArgs e) => await Vistas.AgendarCitaWhatsApp();
    }
}
