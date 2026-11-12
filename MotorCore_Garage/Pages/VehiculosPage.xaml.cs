using MotorCore_Garage.Models;

namespace MotorCore_Garage.Pages
{
    public partial class VehiculosPage : ContentPage
    {
        public VehiculosPage()
        {
            InitializeComponent();
            foreach (var vehiculo in DatosDemo.Vehiculos)
                ListaVehiculos.Children.Add(Vistas.Vehiculo(vehiculo, AbrirDetalle));
        }

        static async void AbrirDetalle(Vehiculo vehiculo)
        {
            await Shell.Current.GoToAsync("detalle", new Dictionary<string, object> { ["Vehiculo"] = vehiculo });
        }
    }
}
