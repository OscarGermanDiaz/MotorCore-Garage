using MotorCore_Garage.Models;
using MotorCore_Garage.Services;

namespace MotorCore_Garage.Pages
{
    [QueryProperty(nameof(VehiculoId), "VehiculoId")]
    public partial class RegistrarKmPage : ContentPage
    {
        List<Vehiculo> _vehiculos = new();

        public RegistrarKmPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _vehiculos = Taller.Vehiculos.ToList();
            Vehiculo.ItemsSource = _vehiculos.Select(v => $"{v.Titulo} · {v.Placa}").ToList();
        }

        // Llega desde el expediente: preselecciona el vehículo.
        public int VehiculoId
        {
            set
            {
                _vehiculos = Taller.Vehiculos.ToList();
                Vehiculo.ItemsSource = _vehiculos.Select(v => $"{v.Titulo} · {v.Placa}").ToList();
                var indice = _vehiculos.FindIndex(v => v.Id == value);
                if (indice >= 0) Vehiculo.SelectedIndex = indice;
            }
        }

        Vehiculo? Seleccionado => Vehiculo.SelectedIndex >= 0 ? _vehiculos[Vehiculo.SelectedIndex] : null;

        void OnVehiculo(object? sender, EventArgs e)
        {
            var v = Seleccionado;
            UltimoKm.Text = v is null ? "" : $"Último kilometraje registrado: {v.KmActual:N0} km";
            Km.Text = "";
            ActualizarVista();
        }

        void OnKmCambio(object? sender, TextChangedEventArgs e) => ActualizarVista();

        // Vista previa: cómo cambiarían las alertas con el kilometraje escrito.
        void ActualizarVista()
        {
            Vista.Children.Clear();
            ErrorKm.IsVisible = false;
            var v = Seleccionado;
            if (v is null || !int.TryParse(Km.Text, out var km)) return;

            var simulado = new Vehiculo { Id = v.Id, ClienteId = v.ClienteId, KmActual = km };
            foreach (var kv in v.UltimoServicioKm) simulado.UltimoServicioKm[kv.Key] = kv.Value;

            var alertas = Taller.Estados(simulado).Where(e => e.Estado != Desgaste.Ok).ToList();
            if (alertas.Count == 0)
            {
                Vista.Children.Add(new Label { Text = "Sin alertas nuevas con este kilometraje", Style = (Style)Application.Current!.Resources["Pie"] });
            }
            foreach (var e in alertas)
            {
                var fila = new HorizontalStackLayout { Spacing = 8 };
                fila.Add(Ui.ChipDesgaste(e.Estado));
                fila.Add(new Label { Text = $"{e.Nombre} · {Vistas.TextoRestante(e)}", Style = (Style)Application.Current!.Resources["Cuerpo"], FontSize = 14, VerticalOptions = LayoutOptions.Center });
                Vista.Children.Add(fila);
            }
        }

        async void OnGuardar(object? sender, EventArgs e)
        {
            var v = Seleccionado;
            if (v is null)
            {
                Mostrar("Elige el vehículo");
                return;
            }
            if (!int.TryParse(Km.Text, out var km))
            {
                Mostrar("Escribe el kilometraje en números");
                return;
            }

            var error = Taller.RegistrarKilometraje(v, km);
            if (error is not null)
            {
                Mostrar(error);
                return;
            }

            await Ui.Snackbar(Snack, SnackTexto, "Kilometraje guardado", 1200);
            await Shell.Current.GoToAsync("..");
        }

        void Mostrar(string mensaje)
        {
            ErrorKm.Text = mensaje;
            ErrorKm.IsVisible = true;
        }

        async void OnVolver(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
