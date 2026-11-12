using Microsoft.Maui.Graphics;

namespace MotorCore_Garage.Models
{
    public static class Recurso
    {
        public static Color Obtener(string clave) => (Color)Application.Current!.Resources[clave];
    }

    public record Vehiculo(string Placa, string Modelo, string Km, int Salud, string Estado, string Nivel)
    {
        public Color Fondo => Recurso.Obtener(Nivel);
        public Color Texto => Nivel == "Warning" ? Recurso.Obtener("Gray950") : Recurso.Obtener("White");
        public double Progreso => Salud / 100.0;
    }

    public record Alerta(string Titulo, string Detalle, string Prioridad, string Nivel)
    {
        public Color Fondo => Recurso.Obtener(Nivel);
        public Color Texto => Nivel == "Warning" ? Recurso.Obtener("Gray950") : Recurso.Obtener("White");
    }

    public record Orden(string Id, string Descripcion, string Estado, string Nivel)
    {
        public Color Fondo => Recurso.Obtener(Nivel);
        public Color Texto => Nivel == "Warning" || Nivel == "Gray200" ? Recurso.Obtener("Gray950") : Recurso.Obtener("White");
    }

    public record Componente(string Nombre, int Porcentaje, string Nivel)
    {
        public double Progreso => Porcentaje / 100.0;
        public Color Fondo => Recurso.Obtener(Nivel);
    }

    public static class DatosDemo
    {
        public static List<Vehiculo> Vehiculos { get; } =
        [
            new("ABC-123", "Toyota Hilux 2019", "152.340 km", 86, "Al día", "Success"),
            new("XYZ-482", "Ford Ranger 2021", "88.910 km", 54, "Atención", "Warning"),
            new("LMN-907", "Nissan Frontier 2018", "204.500 km", 32, "Crítico", "Danger"),
            new("QRS-310", "Chevrolet S10 2020", "61.200 km", 91, "Al día", "Success")
        ];

        public static List<Alerta> Alertas { get; } =
        [
            new("Frenos · ABC-123", "Desgaste estimado en 800 km", "Alta", "Danger"),
            new("Aceite · XYZ-482", "Cambio recomendado en 300 km", "Media", "Warning"),
            new("Batería · LMN-907", "Vida útil estimada: 30 días", "Baja", "Success")
        ];

        public static List<Orden> Ordenes { get; } =
        [
            new("OT-1042", "Cambio de pastillas · Hilux", "En curso", "Warning"),
            new("OT-1041", "Cambio de aceite · Ranger", "Pendiente", "Gray200"),
            new("OT-1040", "Revisión de batería · Frontier", "Completada", "Success"),
            new("OT-1039", "Alineación · S10", "Completada", "Success")
        ];

        public static List<Componente> Componentes { get; } =
        [
            new("Frenos", 32, "Danger"),
            new("Aceite", 68, "Warning"),
            new("Batería", 85, "Success"),
            new("Neumáticos", 54, "Warning")
        ];
    }
}
