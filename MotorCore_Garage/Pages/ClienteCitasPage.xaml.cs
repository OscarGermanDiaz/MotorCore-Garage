using MotorCore_Garage.Services;

namespace MotorCore_Garage.Pages
{
    public partial class ClienteCitasPage : ContentPage
    {
        public ClienteCitasPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Lista.Children.Clear();
            var citas = Taller.CitasDeCliente(Taller.ClienteActualId)
                .OrderBy(c => c.Fecha < DateTime.Today)   // próximas primero
                .ThenBy(c => c.Fecha)
                .ToList();
            Vacio.IsVisible = citas.Count == 0;
            foreach (var c in citas) Lista.Children.Add(Vistas.CitaCard(c));
        }

        async void OnAgendar(object? sender, EventArgs e) => await Vistas.AgendarCitaWhatsApp();
    }
}
