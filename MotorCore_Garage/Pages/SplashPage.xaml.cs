using Microsoft.Maui.Graphics;
using MotorCore_Garage.Services;

namespace MotorCore_Garage.Pages
{
    public partial class SplashPage : ContentPage
    {
        bool _iniciado;

        public SplashPage()
        {
            InitializeComponent();
            Logo.Drawable = new LogotipoDrawable();
        }

        // Primera vez: registro del taller. Después: inicio de sesión.
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_iniciado) return;
            _iniciado = true;

            await Task.Delay(1500);
            Taller.Cargar();
            Page destino = Taller.HayCuentas() ? new LoginPage() : new RegistroPage();
            Application.Current!.Windows[0].Page = new NavigationPage(destino);
        }
    }

    // Logotipo: engrane (anillo punteado) con velocímetro.
    public class LogotipoDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF d)
        {
            var c = new PointF(d.Center.X, d.Center.Y);
            var r = Math.Min(d.Width, d.Height) / 2 - 6;
            canvas.StrokeColor = Color.FromArgb("#E8701A");
            canvas.StrokeSize = 10;
            canvas.StrokeDashPattern = new float[] { 9, 7 };
            canvas.DrawCircle(c, r - 5);
            canvas.StrokeDashPattern = null;
            canvas.StrokeColor = Color.FromArgb("#C9CDD1");
            canvas.StrokeSize = 5;
            canvas.DrawCircle(c, r * 0.72f);
            canvas.StrokeColor = Color.FromArgb("#E8701A");
            canvas.StrokeSize = 6;
            canvas.StrokeLineCap = LineCap.Round;
            var aguja = r * 0.5f;
            canvas.DrawLine(c.X, c.Y, c.X + aguja * 0.7f, c.Y - aguja * 0.7f);
            canvas.DrawArc(new RectF(c.X - r * 0.5f, c.Y - r * 0.5f, r, r), 200, 340, false, false);
            canvas.FillColor = Color.FromArgb("#E8701A");
            canvas.FillCircle(c, 5);
        }
    }
}
