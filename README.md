# MotorCore Garage

Sistema predictivo y de gestión de mantenimiento automotriz para talleres.

MotorCore Garage permite llevar el control de la flota de vehículos de un taller, anticipar fallas de componentes mediante alertas predictivas, y gestionar las órdenes de trabajo y los servicios programados.

> **Estado del proyecto:** prototipo de interfaz. La navegación y la UI están implementadas con datos de demostración (sin persistencia ni conexión a servicios externos). Ver [Estado actual y hoja de ruta](#estado-actual-y-hoja-de-ruta).

---

## Tabla de contenidos

- [Funcionalidades](#funcionalidades)
- [Capturas](#capturas)
- [Tecnologías](#tecnologías)
- [Requisitos](#requisitos)
- [Instalación y ejecución](#instalación-y-ejecución)
- [Estructura del proyecto](#estructura-del-proyecto)
- [Sistema de diseño](#sistema-de-diseño)
- [Datos de demostración](#datos-de-demostración)
- [Estado actual y hoja de ruta](#estado-actual-y-hoja-de-ruta)
- [Contribuir](#contribuir)
- [Licencia](#licencia)

---

## Funcionalidades

La aplicación tiene cuatro secciones principales, accesibles desde la barra de pestañas inferior.

### Inicio
- Resumen de la **salud de la flota** con porcentaje global y barra de progreso.
- Lista de **alertas predictivas** con prioridad (Alta, Media, Baja).
- Próximos **servicios programados**.
- Botón para programar un servicio.

### Vehículos
- Listado de la flota con placa, modelo, kilometraje, estado y salud (%).
- Indicador de color según el estado: *Al día* (verde), *Atención* (amarillo), *Crítico* (rojo).
- Filtros por estado (*Todos*, *Atención*, *Al día*) y campo de búsqueda por placa o modelo.
- Al tocar un vehículo se abre su **detalle**.

### Detalle del vehículo
- Kilometraje, salud general y próximo servicio.
- **Predicción de desgaste** por componente (frenos, aceite, batería, neumáticos) con porcentaje de vida útil restante.
- Recomendación del sistema para el vehículo.
- Botón para programar un servicio.

### Órdenes
- Órdenes de trabajo con identificador (OT-xxxx), descripción, estado (*Pendiente*, *En curso*, *Completada*) y técnico asignado.
- Pestañas de estado: *Abiertas*, *En curso*, *Cerradas*.

### Alertas
- Lista completa de alertas predictivas con el componente, el vehículo, el detalle de la predicción y su prioridad.

---

## Capturas

<!-- Agrega aquí capturas de pantalla de cada sección, por ejemplo:
![Inicio](docs/img/inicio.png)
![Vehículos](docs/img/vehiculos.png)
-->
Pendiente: agregar capturas en `docs/img/`.

---

## Tecnologías

| Área | Tecnología |
|---|---|
| Framework | [.NET MAUI](https://dotnet.microsoft.com/apps/maui) sobre .NET 10 |
| Lenguaje | C# (nullable habilitado, implicit usings) |
| UI | XAML con compilación en tiempo de build (`MauiXamlInflator=SourceGen`) |
| Navegación | Shell (`TabBar` + navegación por ruta `detalle`) |
| Tipografía | Open Sans (Regular y Semibold) |
| Plataformas | Android (API 21+), iOS y Mac Catalyst (15.0+), Windows (10.0.17763+) |

---

## Requisitos

- **[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)** (probado con 10.0.401).
- Carga de trabajo de MAUI:
  ```bash
  dotnet workload install maui
  ```
  En Linux solo se puede compilar el target de Android. Para iOS / Mac Catalyst se requiere macOS con Xcode, y para Windows, Windows con el SDK de Windows 10/11.
- **Android:** Android SDK (instalado automáticamente por Visual Studio o por `dotnet workload`) y un emulador o dispositivo con depuración USB.
- **Visual Studio 2022** con la carga de trabajo *Desarrollo de .NET Multiplataforma con la interfaz de usuario de la aplicación* (.NET MAUI), o **Visual Studio Code** con la extensión *C# Dev Kit*.

---

## Instalación y ejecución

1. Clona el repositorio:
   ```bash
   git clone https://github.com/OscarGermanDiaz/MotorCore-Garage.git
   cd MotorCore-Garage
   ```

2. Restaura las dependencias:
   ```bash
   dotnet restore MotorCore_Garage.slnx
   ```

3. Compila y ejecuta en Android:
   ```bash
   dotnet build MotorCore_Garage/MotorCore_Garage.csproj -f net10.0-android
   dotnet build MotorCore_Garage/MotorCore_Garage.csproj -f net10.0-android -t:Run
   ```

   En Windows también puedes ejecutar:
   ```bash
   dotnet build MotorCore_Garage/MotorCore_Garage.csproj -f net10.0-windows10.0.19041.0 -t:Run
   ```

4. Alternativamente, abre `MotorCore_Garage.slnx` en Visual Studio, selecciona el dispositivo de destino y presiona **F5**.

### Compilar para todas las plataformas

En Windows, el `.csproj` incluye automáticamente los targets de Windows; en macOS, los de iOS y Mac Catalyst; en Linux, solo Android. Para forzar un target específico usa `-f <tfm>`.

---

## Estructura del proyecto

```
MotorCore-Garage/
├── .gitignore                       Archivos generados e IDE excluidos de git
├── MotorCore_Garage.slnx            Solución
├── README.md
├── .vscode/settings.json            Configuración de VS Code (solución por defecto)
└── MotorCore_Garage/
    ├── App.xaml / App.xaml.cs       Punto de entrada; carga de recursos globales
    ├── AppShell.xaml / .cs          Shell: pestañas y registro de rutas
    ├── MauiProgram.cs               Configuración de la app y fuentes
    ├── MotorCore_Garage.csproj      Proyecto MAUI (targets, versiones, íconos, fuentes)
    ├── Models/
    │   └── DatosDemo.cs             Modelos (Vehiculo, Alerta, Orden, Componente) y datos de demo
    ├── Pages/
    │   ├── InicioPage               Resumen de la flota
    │   ├── VehiculosPage            Listado de vehículos
    │   ├── DetallePage              Detalle de un vehículo (ruta "detalle")
    │   ├── OrdenesPage              Órdenes de trabajo
    │   ├── AlertasPage              Alertas predictivas
    │   └── Vistas.cs                Constructores de tarjetas reutilizables
    ├── Resources/
    │   ├── Colors.xaml              Paleta de colores
    │   ├── Styles/Styles.xaml       Estilos implícitos por tipo de control (modo claro/oscuro)
    │   ├── Styles/Componentes.xaml  Estilos con nombre (tipografía, tarjetas, botones, páginas)
    │   ├── Fonts/                   Open Sans
    │   ├── AppIcon/ y Splash/       Ícono y pantalla de inicio (SVG)
    │   └── Images/, Raw/            Recursos adicionales
    └── Platforms/                   Código específico: Android, iOS, MacCatalyst, Windows
```

### Convenciones

- **Modelos** en `Models/`; **páginas** en `Pages/` con su par `.xaml` / `.xaml.cs`.
- Las tarjetas que se repiten se construyen con métodos estáticos en `Vistas.cs` en lugar de duplicar XAML.
- Los estilos de texto y contenedores se referencian por nombre (`Style="{StaticResource Titulo}"`); no usar colores ni tamaños fijos en las páginas.
- Las pantallas se navegan con `Shell.Current.GoToAsync`; los parámetros se pasan con `[QueryProperty]`.

---

## Sistema de diseño

### Paleta

| Token | Valor | Uso |
|---|---|---|
| `Primary` | `#512BD4` | Color principal, pestañas, botones |
| `Secondary` | `#DFD8F7` | Fondos de acento |
| `Tertiary` | `#2B0B98` | Títulos de recomendación |
| `Success` | `#1E9E5A` | Estado correcto / al día |
| `Warning` | `#E8A317` | Atención |
| `Danger` | `#D93025` | Crítico / prioridad alta |
| `Gray100`–`Gray950` | escala de grises | Fondos, bordes y texto |

Los estilos de `Styles.xaml` respetan el modo claro y oscuro del sistema mediante `AppThemeBinding`.

### Estilos principales (`Componentes.xaml`)

| Clave | Uso |
|---|---|
| `Display`, `Titulo`, `Seccion` | Encabezados |
| `CuerpoStrong`, `Cuerpo`, `Pie`, `Etiqueta` | Texto de cuerpo, secundario y etiquetas |
| `Tarjeta`, `Chip` | Contenedores |
| `BotonPrimario` | Botón de acción principal |
| `Pagina` | Fondo y barra de navegación de cada página |

---

## Datos de demostración

Actualmente la app usa datos fijos definidos en `Models/DatosDemo.cs`:

- **4 vehículos** (Hilux, Ranger, Frontier, S10) con kilometraje y salud.
- **3 alertas** predictivas (frenos, aceite, batería).
- **4 órdenes de trabajo** (OT-1039 a OT-1042).
- **4 componentes** con porcentaje de vida útil.

Algunos textos de las pantallas están escritos directamente en el XAML (por ejemplo, la salud global "86%" en Inicio). Al conectar una fuente de datos real, estos valores deben enlazarse al modelo.

---

## Estado actual y hoja de ruta

### Implementado
- [x] Navegación por pestañas (Shell) y ruta de detalle.
- [x] Pantallas: Inicio, Vehículos, Detalle, Órdenes y Alertas.
- [x] Sistema de estilos con soporte de modo claro y oscuro.
- [x] Compilación para Android verificada.

### Pendiente
- [ ] Persistencia de datos (base de datos local o API).
- [ ] Lógica de predicción de desgaste real (hoy los porcentajes son fijos).
- [ ] Acción **Programar servicio** (botones en Inicio y Detalle).
- [ ] Filtros por estado y búsqueda en Vehículos.
- [ ] Pestañas de estado en Órdenes.
- [ ] Enlazar textos fijos de las pantallas al modelo (salud global, próximos servicios, recomendaciones, técnico asignado).
- [ ] Refactor a MVVM (ViewModels con enlace de datos) y separar la presentación de `DatosDemo`.
- [ ] Proyecto de pruebas unitarias para los modelos y la lógica de predicción.
- [ ] Capturas de pantalla y guía de contribución.

---

## Contribuir

1. Crea una rama desde `main`: `git checkout -b feature/nombre-del-cambio`.
2. Haz tus cambios y verifica que el proyecto compila (`dotnet build -f net10.0-android`).
3. Confirma con mensajes de commit claros y abre un Pull Request hacia `main`.

No subas archivos generados (`bin/`, `obj/`), claves de firma (`*.keystore`, `*.jks`) ni credenciales; el `.gitignore` ya los excluye.

---

## Licencia

Pendiente de definir por el autor del proyecto.
