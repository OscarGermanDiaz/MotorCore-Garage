using MotorCore_Garage.Services;

namespace MotorCore_Garage.Pages
{
    public partial class RegistroPage : ContentPage
    {
        public RegistroPage()
        {
            InitializeComponent();
            Logo.Drawable = new LogotipoDrawable();
        }

        void OnMostrarChanged(object? sender, CheckedChangedEventArgs e)
        {
            Clave.IsPassword = !e.Value;
            Confirmar.IsPassword = !e.Value;
        }

        void OnCrear(object? sender, EventArgs e)
        {
            var taller = NombreTaller.Text?.Trim() ?? "";
            var usuario = UsuarioNuevo.Text?.Trim() ?? "";
            var clave = Clave.Text ?? "";
            var confirmar = Confirmar.Text ?? "";

            var error =
                taller.Length == 0 ? "Escribe el nombre del taller"
                : usuario.Length < 3 ? "El usuario debe tener al menos 3 caracteres"
                : !usuario.All(ch => char.IsLetterOrDigit(ch) || ch is '.' or '_' or '-') ? "El usuario solo puede tener letras, números, punto, guion o guion bajo"
                : clave.Length < 6 ? "La contraseña debe tener al menos 6 caracteres"
                : clave != confirmar ? "Las contraseñas no coinciden"
                : null;

            if (error is not null)
            {
                Mostrar(error);
                return;
            }

            var fallo = Taller.CrearCuentaInicial(taller, usuario, clave);
            if (fallo is not null)
            {
                Mostrar(fallo);
                return;
            }

            Application.Current!.Windows[0].Page = new AppShell();
        }

        void Mostrar(string mensaje)
        {
            Error.Text = mensaje;
            Error.IsVisible = true;
        }
    }
}
