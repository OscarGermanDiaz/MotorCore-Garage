using MotorCore_Garage.Models;

namespace MotorCore_Garage.Pages
{
    public partial class AlertasPage : ContentPage
    {
        public AlertasPage()
        {
            InitializeComponent();
            foreach (var alerta in DatosDemo.Alertas)
                ListaAlertas.Children.Add(Vistas.Alerta(alerta));
        }
    }
}
