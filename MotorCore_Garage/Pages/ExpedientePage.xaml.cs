using System.Globalization;
using Microsoft.Maui.Graphics;
using MotorCore_Garage.Models;
using MotorCore_Garage.Services;

namespace MotorCore_Garage.Pages
{
    [QueryProperty(nameof(VehiculoId), "VehiculoId")]
    public partial class ExpedientePage : ContentPage
    {
        static readonly CultureInfo Mexico = new("es-MX");
        Vehiculo? _vehiculo;

        public ExpedientePage()
        {
            InitializeComponent();
        }

        public int VehiculoId
        {
            set => Cargar(Taller.VehiculoPorId(value));
        }

        void Cargar(Vehiculo? v)
        {
            _vehiculo = v;
            if (v is null) return;

            // El cliente solo consulta: no registra kilometraje, servicios ni notas, y solo ve sus vehículos.
            if (!Taller.VehiculoEsDelUsuario(v))
            {
                _vehiculo = null;
                Titulo.Text = "Vehículo no disponible";
                return;
            }
            var soloLectura = Taller.EsCliente;
            BotonKm.IsVisible = !soloLectura;
            BotonServicio.IsVisible = !soloLectura;
            TabNotas.IsVisible = !soloLectura;

            Titulo.Text = v.Titulo;
            Placa.Text = $"Placas {v.Placa}";
            Vin.Text = $"VIN {v.Vin}";
            Km.Text = $"Kilometraje actual: {v.KmActual:N0} km";
            Propietario.Text = $"Propietario: {Taller.ClientePorId(v.ClienteId)?.Nombre}";
            Notas.Text = v.Notas;

            Componentes.Children.Clear();
            foreach (var e in Taller.Estados(v))
                Componentes.Children.Add(Vistas.ComponenteFila(e));

            Grafica.Drawable = new KilometrajeDrawable(Taller.Kilometrajes(v));
            Grafica.Invalidate();
            PintarTab(0);
        }

        void PintarTab(int indice)
        {
            PanelHistorial.IsVisible = indice == 0;
            PanelKm.IsVisible = indice == 1;
            PanelNotas.IsVisible = indice == 2;
            PonerEstilo(TabHistorial, indice == 0);
            PonerEstilo(TabKm, indice == 1);
            PonerEstilo(TabNotas, indice == 2);

            if (indice == 0) PintarHistorial();
        }

        static void PonerEstilo(Button boton, bool activo)
        {
            boton.Style = (Style)Application.Current!.Resources[activo ? "BotonPrimario" : "BotonSecundario"];
            boton.HeightRequest = 44;
            boton.FontSize = 14;
        }

        void PintarHistorial()
        {
            PanelHistorial.Children.Clear();
            if (_vehiculo is null) return;

            var registros = Taller.Historial(_vehiculo);
            if (registros.Count == 0)
            {
                PanelHistorial.Children.Add(new Label { Text = "Sin servicios registrados", Style = (Style)Application.Current!.Resources["Pie"] });
                return;
            }

            foreach (var r in registros)
            {
                var textos = new VerticalStackLayout { Spacing = 2 };
                textos.Add(new Label { Text = $"{r.Fecha:dd MMM yyyy} · {r.Km:N0} km", Style = (Style)Application.Current!.Resources["Pie"] });
                textos.Add(new Label { Text = r.Servicio, Style = (Style)Application.Current!.Resources["CuerpoStrong"] });
                textos.Add(new Label { Text = $"{r.Costo.ToString("C0", Mexico)} · Mecánico: {r.Mecanico}", Style = (Style)Application.Current!.Resources["Cuerpo"], FontSize = 14 });
                PanelHistorial.Children.Add(Vistas.Tarjeta(textos));
            }
        }

        void OnTab(object? sender, EventArgs e)
        {
            if (sender is Button b && int.TryParse(b.CommandParameter?.ToString(), out var indice))
                PintarTab(indice);
        }
        async void OnServicio(object? sender, EventArgs e)
        {
            if (_vehiculo is null) return;
            await Shell.Current.GoToAsync("servicio", new Dictionary<string, object> { ["VehiculoId"] = _vehiculo.Id });
        }


        async void OnNotas(object? sender, EventArgs e)
        {
            if (_vehiculo is null) return;
            _vehiculo.Notas = Notas.Text ?? "";
            Taller.GuardarVehiculo(_vehiculo);
            await Task.CompletedTask;
        }

        async void OnKilometraje(object? sender, EventArgs e)
        {
            if (_vehiculo is null) return;
            await Shell.Current.GoToAsync("kilometraje", new Dictionary<string, object> { ["VehiculoId"] = _vehiculo.Id });
        }

        async void OnVolver(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    // Gráfica de líneas del kilometraje a lo largo del tiempo.
    public class KilometrajeDrawable : IDrawable
    {
        readonly List<(DateTime Fecha, int Km)> _puntos;

        public KilometrajeDrawable(List<(DateTime Fecha, int Km)> puntos)
        {
            _puntos = puntos;
        }

        public void Draw(ICanvas canvas, RectF d)
        {
            if (_puntos.Count < 2) return;

            var margen = new RectF(d.Left + 44, d.Top + 8, d.Width - 52, d.Height - 32);
            var minKm = _puntos.Min(p => p.Km);
            var maxKm = _puntos.Max(p => p.Km);
            var rango = Math.Max(1, maxKm - minKm);
            var t0 = _puntos[0].Fecha.Ticks;
            var t1 = _puntos[^1].Fecha.Ticks;
            var trango = Math.Max(1, t1 - t0);

            canvas.StrokeColor = Color.FromArgb("#D0D4D8");
            canvas.StrokeSize = 1;
            canvas.DrawLine(margen.Left, margen.Bottom, margen.Right, margen.Bottom);
            canvas.FontColor = Color.FromArgb("#6E6E6E");
            canvas.FontSize = 10;
            canvas.DrawString($"{maxKm:N0}", new RectF(d.Left, margen.Top - 6, 44, 14), HorizontalAlignment.Left, VerticalAlignment.Top);
            canvas.DrawString($"{minKm:N0}", new RectF(d.Left, margen.Bottom - 8, 44, 14), HorizontalAlignment.Left, VerticalAlignment.Top);

            var path = new PathF();
            for (int i = 0; i < _puntos.Count; i++)
            {
                var x = margen.Left + (float)((_puntos[i].Fecha.Ticks - t0) / (double)trango) * margen.Width;
                var y = margen.Bottom - (float)((_puntos[i].Km - minKm) / (double)rango) * margen.Height;
                if (i == 0) path.MoveTo(x, y); else path.LineTo(x, y);
            }
            canvas.StrokeColor = Color.FromArgb("#E8701A");
            canvas.StrokeSize = 3;
            canvas.StrokeLineJoin = LineJoin.Round;
            canvas.DrawPath(path);

            canvas.FillColor = Color.FromArgb("#E8701A");
            foreach (var p in _puntos)
            {
                var x = margen.Left + (float)((p.Fecha.Ticks - t0) / (double)trango) * margen.Width;
                var y = margen.Bottom - (float)((p.Km - minKm) / (double)rango) * margen.Height;
                canvas.FillCircle(x, y, 4);
            }
        }
    }
}
