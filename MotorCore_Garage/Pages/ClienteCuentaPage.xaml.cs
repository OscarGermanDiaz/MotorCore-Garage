using MotorCore_Garage.Services;

namespace MotorCore_Garage.Pages
{
    public partial class ClienteCuentaPage : ContentPage
    {
        public ClienteCuentaPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var cliente = Taller.ClientePorId(Taller.ClienteActualId);
            NombreDelTaller.Text = Taller.NombreTaller;
            Nombre.Text = cliente?.Nombre ?? "—";
            Telefono.Text = cliente?.Telefono ?? "—";
            Usuario.Text = Taller.UsuarioActual;
        }

        async void OnCerrarSesion(object? sender, EventArgs e)
        {
            var confirmar = await DisplayAlertAsync("Cerrar sesión", "¿Seguro que quieres salir?", "Salir", "Cancelar");
            if (!confirmar) return;
            Taller.CerrarSesion();
            Navegacion.IrAlLogin();
        }
    }
}
