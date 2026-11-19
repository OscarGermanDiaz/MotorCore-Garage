using System.Text.Json;
using MotorCore_Garage.Models;
using MotorCore_Garage.Services;

namespace MotorCore_Garage.Pages
{
    public partial class MasPage : ContentPage
    {
        bool _cargando;

        public MasPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _cargando = true;
            Usuario.Text = Taller.UsuarioActual;
            Rol.Text = Taller.RolActual == Models.Rol.Administrador ? "Administrador" : "Mecánico";

            var admin = Taller.RolActual == Models.Rol.Administrador;
            BotonReportes.IsVisible = admin;
            TituloUsuarios.IsVisible = admin;
            PanelUsuarios.IsVisible = admin;

            Bahias.Value = Taller.Bahias;
            TextoBahias.Text = $"{Taller.Bahias} bahías en el taller";
            Bahias.IsEnabled = admin;
            _cargando = false;

            Catalogo.Children.Clear();
            foreach (var c in Taller.Catalogo)
            {
                var fila = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(new GridLength(120)) }, ColumnSpacing = 12 };
                fila.Add(new Label { Text = c.Nombre, Style = (Style)Application.Current!.Resources["Cuerpo"], VerticalOptions = LayoutOptions.Center });
                var entrada = new Entry { Text = c.IntervaloKm.ToString(), Keyboard = Keyboard.Numeric, FontSize = 16, IsEnabled = admin, HorizontalTextAlignment = TextAlignment.End };
                entrada.Completed += (_, _) => GuardarIntervalo(c, entrada);
                entrada.Unfocused += (_, _) => GuardarIntervalo(c, entrada);
                fila.Add(entrada, 1, 0);
                Catalogo.Children.Add(fila);
            }
        }

        void GuardarIntervalo(ComponenteDef c, Entry entrada)
        {
            if (int.TryParse(entrada.Text, out var km) && km > 0)
            {
                c.IntervaloKm = km;
                Taller.GuardarIntervalo(c);
            }
            else
            {
                entrada.Text = c.IntervaloKm.ToString();
            }
        }

        void OnBahias(object? sender, ValueChangedEventArgs e)
        {
            if (_cargando) return;
            Taller.Bahias = (int)e.NewValue;
            TextoBahias.Text = $"{Taller.Bahias} bahías en el taller";
        }

        async void OnClientes(object? sender, EventArgs e) => await Shell.Current.GoToAsync("clientes");

        async void OnReportes(object? sender, EventArgs e) => await Shell.Current.GoToAsync("reportes");

        async void OnRespaldo(object? sender, EventArgs e)
        {
            var ruta = Path.Combine(FileSystem.AppDataDirectory, $"respaldo_{DateTime.Now:yyyyMMdd_HHmmss}.json");
            var datos = new
            {
                Clientes = Taller.Clientes,
                Vehiculos = Taller.Vehiculos,
                Citas = Taller.Citas,
                Servicios = Taller.Servicios,
                Catalogo = Taller.Catalogo
            };
            await File.WriteAllTextAsync(ruta, JsonSerializer.Serialize(datos, new JsonSerializerOptions { WriteIndented = true }));
            Taller.Bitacorar("creó un respaldo");
            await DisplayAlertAsync("Respaldo listo", $"Se guardó en:\n{ruta}", "Entendido");
        }

        async void OnCerrarSesion(object? sender, EventArgs e)
        {
            var confirmar = await DisplayAlertAsync("Cerrar sesión", "¿Seguro que quieres salir?", "Salir", "Cancelar");
            if (!confirmar) return;
            Taller.CerrarSesion();
            Application.Current!.Windows[0].Page = new NavigationPage(new LoginPage());
        }
    }
}
