using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using MotorCore_Garage.Models;

namespace MotorCore_Garage.Pages
{
    // Constructores de las tarjetas que se repiten en las pantallas del diseño.
    public static class Vistas
    {
        static Style Estilo(string clave) => (Style)Application.Current!.Resources[clave];

        static Border Chip(string texto, Color fondo, Color textoColor) => new()
        {
            BackgroundColor = fondo,
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            Padding = new Thickness(8, 3),
            VerticalOptions = LayoutOptions.Center,
            Content = new Label { Text = texto, Style = Estilo("Etiqueta"), TextColor = textoColor }
        };

        static Grid FilaConTextos(VerticalStackLayout textos, View extremo)
        {
            var fila = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                ColumnSpacing = 12
            };
            fila.Add(textos);
            fila.Add(extremo, 1, 0);
            return fila;
        }

        public static Border Alerta(Alerta alerta)
        {
            var punto = new BoxView
            {
                WidthRequest = 10,
                HeightRequest = 10,
                CornerRadius = 5,
                BackgroundColor = alerta.Fondo,
                VerticalOptions = LayoutOptions.Center
            };
            var textos = new VerticalStackLayout { Spacing = 2 };
            textos.Add(new Label { Text = alerta.Titulo, Style = Estilo("CuerpoStrong") });
            textos.Add(new Label { Text = alerta.Detalle, Style = Estilo("Pie") });

            var fila = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                ColumnSpacing = 12
            };
            fila.Add(punto);
            fila.Add(textos, 1, 0);
            fila.Add(Chip(alerta.Prioridad, alerta.Fondo, alerta.Texto), 2, 0);

            return new Border { Style = Estilo("Tarjeta"), Content = fila };
        }

        public static Border Vehiculo(Vehiculo v, Action<Vehiculo> alTocar)
        {
            var tarjeta = new Border { Style = Estilo("Tarjeta") };
            var contenido = new VerticalStackLayout { Spacing = 8 };

            var textos = new VerticalStackLayout { Spacing = 2 };
            textos.Add(new Label { Text = v.Placa, Style = Estilo("Etiqueta") });
            var fila = FilaConTextos(textos, Chip(v.Estado, v.Fondo, v.Texto));
            contenido.Add(fila);
            contenido.Add(new Label { Text = v.Modelo, Style = Estilo("Seccion") });
            contenido.Add(new Label { Text = $"{v.Km} · Salud {v.Salud}%", Style = Estilo("Pie") });
            contenido.Add(new ProgressBar { Progress = v.Progreso, ProgressColor = v.Fondo, HeightRequest = 8 });

            tarjeta.Content = contenido;
            tarjeta.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => alTocar(v)) });
            return tarjeta;
        }

        public static Border Orden(Orden o)
        {
            var textos = new VerticalStackLayout { Spacing = 2 };
            textos.Add(new Label { Text = o.Id, Style = Estilo("Pie") });
            textos.Add(new Label { Text = o.Descripcion, Style = Estilo("CuerpoStrong") });
            textos.Add(new Label { Text = "Técnico asignado · Entrega 12 nov", Style = Estilo("Pie") });

            var contenido = new VerticalStackLayout { Spacing = 6 };
            contenido.Add(FilaConTextos(new VerticalStackLayout(), Chip(o.Estado, o.Fondo, o.Texto)));
            contenido.Add(textos);

            return new Border { Style = Estilo("Tarjeta"), Content = contenido };
        }

        public static Border Componente(Componente c)
        {
            var textos = new VerticalStackLayout { Spacing = 6 };
            var etiqueta = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }
            };
            etiqueta.Add(new Label { Text = c.Nombre, Style = Estilo("Cuerpo") });
            etiqueta.Add(new Label { Text = $"{c.Porcentaje}%", Style = Estilo("CuerpoStrong") }, 1, 0);
            textos.Add(etiqueta);
            textos.Add(new ProgressBar { Progress = c.Progreso, ProgressColor = c.Fondo, HeightRequest = 8 });
            return new Border { Style = Estilo("Tarjeta"), Content = textos };
        }
    }
}
