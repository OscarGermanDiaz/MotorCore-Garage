using MotorCore_Garage.Models;

namespace MotorCore_Garage.Pages
{
    [QueryProperty(nameof(Vehiculo), "Vehiculo")]
    public partial class DetallePage : ContentPage
    {
        public DetallePage()
        {
            InitializeComponent();
            foreach (var componente in DatosDemo.Componentes)
                ListaComponentes.Children.Add(Vistas.Componente(componente));
        }

        public Vehiculo Vehiculo
        {
            set => BindingContext = value;
        }

        async void OnVolver(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
