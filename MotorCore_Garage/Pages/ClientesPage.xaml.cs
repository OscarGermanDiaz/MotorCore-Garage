using MotorCore_Garage.Models;
using MotorCore_Garage.Services;

namespace MotorCore_Garage.Pages
{
    public partial class ClientesPage : ContentPage
    {
        public ClientesPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Mostrar(Buscar.Text ?? "");
        }

        void OnBuscar(object? sender, TextChangedEventArgs e) => Mostrar(e.NewTextValue ?? "");

        void Mostrar(string termino)
        {
            termino = termino.Trim();
            Lista.Children.Clear();

            var resultados = Taller.Clientes.Where(c => Coincide(c, termino)).ToList();
            if (resultados.Count == 0)
            {
                Lista.Children.Add(new Label
                {
                    Text = "No hay clientes con ese dato",
                    Style = (Style)Application.Current!.Resources["Pie"],
                    HorizontalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 24, 0, 0)
                });
                return;
            }

            foreach (var c in resultados)
                Lista.Children.Add(Tarjeta(c));
        }

        static bool Coincide(Cliente c, string termino)
        {
            if (termino.Length == 0) return true;
            var t = termino.ToLowerInvariant();
            return c.Nombre.ToLowerInvariant().Contains(t)
                || c.Telefono.Contains(t)
                || Taller.VehiculosDeCliente(c.Id).Any(v => v.Placa.ToLowerInvariant().Contains(t));
        }

        static Border Tarjeta(Cliente c)
        {
            var placas = string.Join(" · ", Taller.VehiculosDeCliente(c.Id).Select(v => v.Placa));
            var textos = new VerticalStackLayout { Spacing = 2 };
            textos.Add(new Label { Text = c.Nombre, Style = (Style)Application.Current!.Resources["CuerpoStrong"] });
            textos.Add(new Label { Text = $"Tel. {c.Telefono}", Style = (Style)Application.Current!.Resources["Pie"] });
            textos.Add(new Label { Text = placas, Style = (Style)Application.Current!.Resources["Pie"] });
            var borde = Vistas.Tarjeta(textos);
            var id = c.Id;
            borde.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => Shell.Current.GoToAsync("cliente", new Dictionary<string, object> { ["ClienteId"] = id }))
            });
            return borde;
        }

        async void OnVolver(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
