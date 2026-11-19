using MotorCore_Garage.Models;
using MotorCore_Garage.Services;

namespace MotorCore_Garage.Pages
{
    public partial class RecepcionPage : ContentPage
    {
        static readonly string[] Niveles = { "Vacío", "1/4", "1/2", "3/4", "Lleno" };
        static readonly string[] Estados = { "Recibido", "En proceso", "Listo", "Entregado" };

        List<Cita> _citas = new();
        Vehiculo? _vehiculo;

        public RecepcionPage()
        {
            InitializeComponent();
            Combustible.ItemsSource = Niveles;
            Combustible.SelectedIndex = 2;
            Estado.ItemsSource = Estados;
            Estado.SelectedIndex = 0;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _citas = Taller.Citas
                .Where(c => c.Estado is "Programada" or "Recibido" or "En proceso" or "Listo")
                .OrderBy(c => c.Fecha)
                .ToList();
            Cita.ItemsSource = _citas.Select(c =>
                $"{c.Fecha:dd/MM HH:mm} · {Taller.ClientePorId(c.ClienteId)?.Nombre}").ToList();
        }

        void OnCita(object? sender, EventArgs e)
        {
            if (Cita.SelectedIndex < 0) return;
            var cita = _citas[Cita.SelectedIndex];
            _vehiculo = Taller.VehiculoPorId(cita.VehiculoId);
            if (_vehiculo is null) return;
            Vehiculo.Text = $"{_vehiculo.Titulo} · {_vehiculo.Placa}";
            UltimoKm.Text = $"Último kilometraje registrado: {_vehiculo.KmActual:N0} km";
            Km.Text = "";
        }

        async void OnGuardar(object? sender, EventArgs e)
        {
            if (!Validar(ErrorCita, Cita.SelectedIndex < 0, "Elige la cita del vehículo")) return;
            var vehiculo = _vehiculo!;

            if (!int.TryParse(Km.Text?.Trim(), out var km))
            {
                Validar(ErrorKm, true, "Escribe el kilometraje en números");
                return;
            }

            var error = Taller.RegistrarKilometraje(vehiculo, km);
            if (error is not null)
            {
                Validar(ErrorKm, true, error);
                return;
            }

            ErrorKm.IsVisible = false;
            var cita = _citas[Cita.SelectedIndex];
            cita.Estado = Estados[Estado.SelectedIndex];
            Taller.GuardarCita(cita);
            Taller.Bitacorar($"registró la recepción de {vehiculo.Placa} (estado: {cita.Estado})");
            await Ui.Snackbar(Snack, SnackTexto, "Recepción guardada", 1200);
            await Shell.Current.GoToAsync("..");
        }

        static bool Validar(Label error, bool fallo, string mensaje)
        {
            error.Text = mensaje;
            error.IsVisible = fallo;
            return !fallo;
        }

        async void OnVolver(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
