using MotorCore_Garage.Models;
using MotorCore_Garage.Services;

namespace MotorCore_Garage.Pages
{
    public partial class VehiculosPage : ContentPage
    {
        // Opciones del filtro: "Todos" más los tres estados de desgaste.
        static readonly string[] Filtros = { "Todos", "Buen estado", "Próximo", "Vencido" };

        public VehiculosPage()
        {
            InitializeComponent();
            FiltroEstado.ItemsSource = Filtros;
            FiltroEstado.SelectedIndex = 0;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Mostrar();
        }

        void OnFiltrar(object? sender, EventArgs e) => Mostrar();

        void Mostrar()
        {
            var termino = (Buscar.Text ?? "").Trim().ToLowerInvariant();
            var filtro = FiltroEstado.SelectedIndex;
            Lista.Children.Clear();

            var resultados = Taller.Vehiculos
                .Where(v => termino.Length == 0 || v.Placa.ToLowerInvariant().Contains(termino) || v.Titulo.ToLowerInvariant().Contains(termino))
                .Where(v => filtro <= 0 || Estado(v) == (filtro switch { 1 => Desgaste.Ok, 2 => Desgaste.Proximo, _ => Desgaste.Vencido }))
                .ToList();

            if (resultados.Count == 0)
            {
                Lista.Children.Add(new Label
                {
                    Text = "No hay vehículos con esos filtros",
                    Style = (Style)Application.Current!.Resources["Pie"],
                    HorizontalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 24, 0, 0)
                });
                return;
            }

            foreach (var v in resultados)
            {
                var id = v.Id;
                Lista.Children.Add(Vistas.VehiculoCard(v,
                    () => Shell.Current.GoToAsync("expediente", new Dictionary<string, object> { ["VehiculoId"] = id })));
            }
        }

        static Desgaste Estado(Vehiculo v) => Vistas.Peor(v).Estado;
    }
}
