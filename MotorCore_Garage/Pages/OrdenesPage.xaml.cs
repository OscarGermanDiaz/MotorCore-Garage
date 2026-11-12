using MotorCore_Garage.Models;

namespace MotorCore_Garage.Pages
{
    public partial class OrdenesPage : ContentPage
    {
        public OrdenesPage()
        {
            InitializeComponent();
            foreach (var orden in DatosDemo.Ordenes)
                ListaOrdenes.Children.Add(Vistas.Orden(orden));
        }
    }
}
