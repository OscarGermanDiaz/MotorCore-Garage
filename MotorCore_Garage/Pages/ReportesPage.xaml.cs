using Microsoft.Maui.Graphics;
using MotorCore_Garage.Models;
using MotorCore_Garage.Services;

namespace MotorCore_Garage.Pages
{
    public partial class ReportesPage : ContentPage
    {
        public ReportesPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Taller.RolActual != Rol.Administrador)
            {
                SinAcceso.IsVisible = true;
                return;
            }
            Pintar();
        }

        void Pintar()
        {
            // Servicios preventivos por mes (últimos seis meses)
            var meses = Enumerable.Range(0, 6).Select(i => DateTime.Today.AddMonths(-(5 - i))).ToList();
            var porMes = meses.Select(m => (
                Etiqueta: m.ToString("MMM", new System.Globalization.CultureInfo("es-MX")),
                Valor: (double)Taller.Servicios.Count(s => s.Fecha.Year == m.Year && s.Fecha.Month == m.Month))).ToList();
            GraficaServicios.Drawable = new BarrasDrawable(porMes, null, "#E8701A");
            GraficaServicios.Invalidate();

            // Horas administrativas frente a la meta
            var semanas = Taller.HorasAdministrativas
                .Select((h, i) => ($"S{i + 1}", h)).ToList();
            MetaHoras.Text = $"Meta: {Taller.MetaHorasSemana} h por semana";
            GraficaHoras.Drawable = new BarrasDrawable(semanas.Select(s => (s.Item1, s.h)).ToList(), Taller.MetaHorasSemana, "#2F3437");
            GraficaHoras.Invalidate();

            // Ocupación de los próximos 7 días
            Ocupacion.Children.Clear();
            for (int i = 0; i < 7; i++)
            {
                var dia = DateTime.Today.AddDays(i);
                var sat = Taller.Saturacion(dia);
                var estado = sat < 0.4 ? Desgaste.Ok : sat < 0.75 ? Desgaste.Proximo : Desgaste.Vencido;
                var fila = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };
                fila.Add(new Label { Text = dia.ToString("dddd d", new System.Globalization.CultureInfo("es-MX")), Style = (Style)Application.Current!.Resources["Cuerpo"], FontSize = 14 });
                fila.Add(new Label { Text = $"{sat:P0}", Style = (Style)Application.Current!.Resources["CuerpoStrong"], TextColor = Ui.ColorDesgaste(estado) }, 1, 0);
                Ocupacion.Children.Add(fila);
            }

            // Servicios más frecuentes según las citas registradas
            Frecuentes.Children.Clear();
            var top = Taller.Citas.GroupBy(c => c.Servicio).OrderByDescending(g => g.Count()).Take(5);
            foreach (var g in top)
                Frecuentes.Children.Add(new Label { Text = $"{g.Key} · {g.Count()}", Style = (Style)Application.Current!.Resources["Cuerpo"], FontSize = 14 });

            // Bitácora de actividad
            Bitacora.Children.Clear();
            foreach (var linea in Taller.Bitacora.Take(20).Select(b => $"{b.Fecha:dd/MM HH:mm} · {b.Texto}"))
                Bitacora.Children.Add(new Label { Text = linea, Style = (Style)Application.Current!.Resources["Pie"], FontSize = 13 });
        }

        async void OnVolver(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    // Barras verticales con una línea de meta opcional.
    public class BarrasDrawable : IDrawable
    {
        readonly List<(string Etiqueta, double Valor)> _datos;
        readonly double? _meta;
        readonly Color _color;

        public BarrasDrawable(List<(string Etiqueta, double Valor)> datos, double? meta, string color)
        {
            _datos = datos;
            _meta = meta;
            _color = Color.FromArgb(color);
        }

        public void Draw(ICanvas canvas, RectF d)
        {
            var area = new RectF(d.Left + 8, d.Top + 8, d.Width - 16, d.Height - 32);
            var max = Math.Max(_datos.Max(x => x.Valor), _meta ?? 0);
            if (max <= 0) max = 1;
            var ancho = area.Width / _datos.Count;

            canvas.FontColor = Color.FromArgb("#6E6E6E");
            canvas.FontSize = 11;
            for (int i = 0; i < _datos.Count; i++)
            {
                var (etiqueta, valor) = _datos[i];
                var alto = (float)(valor / max) * area.Height;
                var x = area.Left + i * ancho + ancho * 0.2f;
                canvas.FillColor = _color;
                canvas.FillRoundedRectangle(x, area.Bottom - alto, ancho * 0.6f, alto, 6);
                canvas.DrawString(etiqueta, new RectF(area.Left + i * ancho, area.Bottom + 4, ancho, 18), HorizontalAlignment.Center, VerticalAlignment.Top);
            }

            if (_meta is double meta)
            {
                var y = area.Bottom - (float)(meta / max) * area.Height;
                canvas.StrokeColor = Color.FromArgb("#C62828");
                canvas.StrokeSize = 2;
                canvas.StrokeDashPattern = new float[] { 6, 4 };
                canvas.DrawLine(area.Left, y, area.Right, y);
                canvas.StrokeDashPattern = null;
            }
        }
    }
}
