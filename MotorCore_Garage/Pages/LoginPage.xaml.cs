using MotorCore_Garage.Models;
using MotorCore_Garage.Services;

namespace MotorCore_Garage.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Encabezado.Text = Taller.NombreTaller;
        }

        void OnMostrarChanged(object? sender, CheckedChangedEventArgs e)
        {
            Contrasena.IsPassword = !e.Value;
        }

        void OnEntrar(object? sender, EventArgs e)
        {
            var usuario = Usuario.Text?.Trim() ?? "";
            var clave = Contrasena.Text ?? "";

            if (usuario.Length == 0 || clave.Length == 0)
            {
                Mostrar("Escribe usuario y contraseña");
                return;
            }

            var error = Taller.Autenticar(usuario, clave);
            if (error is not null)
            {
                Mostrar(error);
                return;
            }

            Navegacion.IrAlInicio();
        }

        async void OnAgendarWhatsApp(object? sender, EventArgs e)
        {
            if (!await Vistas.AgendarCitaWhatsApp())
                Mostrar("El número de WhatsApp del taller aún no está configurado");
        }

        void Mostrar(string mensaje)
        {
            Error.Text = mensaje;
            Error.IsVisible = true;
        }
    }
}
