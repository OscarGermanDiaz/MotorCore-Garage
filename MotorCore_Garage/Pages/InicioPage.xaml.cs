using System.Globalization;
using MotorCore_Garage.Models;
using MotorCore_Garage.Services;

namespace MotorCore_Garage.Pages
{
    public partial class InicioPage : ContentPage
    {
        static readonly CultureInfo Mexico = new("es-MX");

        public InicioPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Refrescar();
        }

        void Refrescar()
        {
            var hoy = DateTime.Today;
            var hora = DateTime.Now.Hour;
            var saludo = hora < 12 ? "Buenos días" : hora < 19 ? "Buenas tardes" : "Buenas noches";
            Encabezado.Text = Taller.NombreTaller;
            Saludo.Text = $"{saludo}, {Taller.UsuarioActual}";
            var fecha = hoy.ToString("dddd d 'de' MMMM", Mexico);
            Fecha.Text = char.ToUpper(fecha[0]) + fecha[1..];

            var alertas = Taller.Alertas();
            CitasHoy.Text = Taller.CitasEn(hoy).ToString();
            EnTaller.Text = Taller.Citas.Count(c => c.Fecha.Date == hoy && (c.Estado == "Recibido" || c.Estado == "En proceso")).ToString();
            Urgentes.Text = alertas.Count(a => a.Componente.Estado == Desgaste.Vencido).ToString();

            var ocupadas = Taller.Citas.Count(c => c.Fecha.Date == hoy && c.Estado == "En proceso");
            Bahias.Text = $"{ocupadas} de {Taller.Bahias}";
            BarraBahias.Progress = Math.Min(1.0, ocupadas / (double)Taller.Bahias);

            ListaCitas.Children.Clear();
            var citas = Taller.Citas.Where(c => c.Fecha.Date == hoy).OrderBy(c => c.Fecha).ToList();
            if (citas.Count == 0)
                ListaCitas.Children.Add(new Label { Text = "Sin citas para hoy", Style = (Style)Application.Current!.Resources["Pie"] });
            foreach (var c in citas)
                ListaCitas.Children.Add(Vistas.CitaCard(c));

            ListaAlertas.Children.Clear();
            var principales = alertas.Take(3).ToList();
            if (principales.Count == 0)
                ListaAlertas.Children.Add(new Label { Text = Taller.Vehiculos.Count == 0 ? "Registra un cliente y su vehículo para ver alertas" : "Sin alertas de mantenimiento", Style = (Style)Application.Current!.Resources["Pie"] });
            foreach (var a in principales)
                ListaAlertas.Children.Add(Vistas.AlertaCard(a,
                    agendar: () => Shell.Current.GoToAsync("nuevacita"),
                    contactar: async () => await Vistas.Contactar(this, a.Cliente)));
        }

        async void OnFab(object? sender, EventArgs e)
        {
            var accion = await DisplayActionSheetAsync("Acciones rápidas", "Cancelar", null,
                "Nuevo cliente", "Nuevo vehículo", "Registrar servicio", "Nueva cita", "Recepción de vehículo", "Registrar kilometraje");
            switch (accion)
            {
                case "Nuevo cliente": await Shell.Current.GoToAsync("nuevocliente"); break;
                case "Nuevo vehículo": await Shell.Current.GoToAsync("nuevovehiculo"); break;
                case "Registrar servicio": await Shell.Current.GoToAsync("servicio"); break;
                case "Nueva cita": await Shell.Current.GoToAsync("nuevacita"); break;
                case "Recepción de vehículo": await Shell.Current.GoToAsync("recepcion"); break;
                case "Registrar kilometraje": await Shell.Current.GoToAsync("kilometraje"); break;
            }
        }
    }
}
