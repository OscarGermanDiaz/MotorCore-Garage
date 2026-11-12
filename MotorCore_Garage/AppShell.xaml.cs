using MotorCore_Garage.Pages;

namespace MotorCore_Garage
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("detalle", typeof(DetallePage));
        }
    }
}
