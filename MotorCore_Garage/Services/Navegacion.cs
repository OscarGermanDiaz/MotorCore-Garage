using MotorCore_Garage.Models;

namespace MotorCore_Garage.Services
{
    // Elige la raíz de la app según el rol: monitoreo para el taller, cliente para el dueño del vehículo.
    public static class Navegacion
    {
        public static void IrAlInicio()
        {
            Application.Current!.Windows[0].Page = Taller.RolActual == Rol.Cliente
                ? new ClienteShell()
                : new AppShell();
        }

        public static void IrAlLogin() =>
            Application.Current!.Windows[0].Page = new NavigationPage(new Pages.LoginPage());
    }
}
