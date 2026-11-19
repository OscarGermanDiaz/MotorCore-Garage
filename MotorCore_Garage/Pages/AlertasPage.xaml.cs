using MotorCore_Garage.Models;
using MotorCore_Garage.Services;

namespace MotorCore_Garage.Pages
{
    public partial class AlertasPage : ContentPage
    {
        public AlertasPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Mostrar();
        }

        void Mostrar()
        {
            Lista.Children.Clear();
            var alertas = Taller.Alertas();

            if (alertas.Count == 0)
            {
                Lista.Children.Add(new Label
                {
                    Text = "Sin alertas: todos los componentes están en buen estado",
                    Style = (Style)Application.Current!.Resources["Pie"],
                    HorizontalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 32, 0, 0)
                });
                return;
            }

            foreach (var grupo in new[] { GrupoAlerta.Vencidas, GrupoAlerta.Proximas, GrupoAlerta.Programadas })
            {
                var items = alertas.Where(a => a.Grupo == grupo).ToList();
                if (items.Count == 0) continue;

                Lista.Children.Add(new Label
                {
                    Text = $"{Titulo(grupo)} ({items.Count})",
                    Style = (Style)Application.Current!.Resources["Seccion"],
                    Margin = new Thickness(0, 8, 0, 0)
                });
                foreach (var a in items)
                {
                    Lista.Children.Add(Vistas.AlertaCard(a,
                        agendar: () => Shell.Current.GoToAsync("nuevacita"),
                        contactar: async () => await Vistas.Contactar(this, a.Cliente)));
                }
            }
        }

        static string Titulo(GrupoAlerta g) => g switch
        {
            GrupoAlerta.Vencidas => "Vencidas",
            GrupoAlerta.Proximas => "Próximas",
            _ => "Programadas"
        };
    }
}
