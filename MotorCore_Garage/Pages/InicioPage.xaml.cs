using MotorCore_Garage.Models;

namespace MotorCore_Garage.Pages
{
    public partial class InicioPage : ContentPage
    {
        public InicioPage()
        {
            InitializeComponent();
            foreach (var alerta in DatosDemo.Alertas)
                ListaAlertas.Children.Add(Vistas.Alerta(alerta));
        }
    }
}
