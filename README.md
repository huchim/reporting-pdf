# [Reporting](https://huchim.com/)
[![Visual Studio Team services](https://img.shields.io/vso/build/huchim/c81ea820-fe3c-4afc-be8c-f54f70bfab24/6.svg)]() [![NuGet Pre Release](https://img.shields.io/nuget/v/Jaguar.Reporting.Html.svg?style=flat-square)][nuget] [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Jaguar.Reporting.Html.svg?style=flat-square)][nuget]

[nuget]: https://www.nuget.org/packages/Jaguar.Reporting.Html

El generador de reportes es una herramienta que permite [#Mustache](https://github.com/jehugaleahsa/mustache-sharp) para mostrar los resultados de cualquier consulta. [#Mustache](https://github.com/jehugaleahsa/mustache-sharp) es una sintaxis para crear plantillas.

La ventaja de [#Mustache](https://github.com/jehugaleahsa/mustache-sharp) es que usted puede publicar su aplicación web y sin necesidad de recompilar, puede modificar tanto el resultado de su reporte como también la consulta que la genera. De igual forma puede usar otros formatos como [Excel](https://github.com/huchim/reporting-excel), [Csv](https://github.com/huchim/reporting-csv), Json o cualquier otro formato que implemente [IGeneratorEngine](https://github.com/huchim/reporting/blob/master/src/IGeneratorEngine.cs).

## Ventajas

El paquete de reportes ayuda a actualizar de manera dinámica tanto los datos del reporte, como la presentación del mismo.

- Soporte para distintos frameworks
- Al soportar .NET Core puede funcionar en diferentes sistemas operativos.
- La consulta SQL puede ser actualizada dinámicamente.
- Los parámetros de la consulta no se añaden directamente al SQL si no que se usa "[sp_executesql](https://stackoverflow.com/questions/4892166/how-does-sqlparameter-prevent-sql-injection)" (o PREPARE, todo depende del gestor)
- Es compatible con cualquier conexión que implemente [IDbConnection](https://msdn.microsoft.com/en-us/library/system.data.idbconnection(v=vs.110).aspx)  como [MySqlConnection](https://dev.mysql.com/doc/connector-net/en/connector-net-ref-mysqlclient-mysqlconnection.html) o [SqlConnection](https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnection(v=vs.110).aspx).



## Instalación

Este generador es compatible (por el momento con .NET Framework 4.0, .NET Core 1.1 o superior) y se instala por medio de un paquete [nuget](https://www.nuget.org/packages/Jaguar.Reporting.Html).

```bash
PM > Install-Package Jaguar.Reporting.Html -Pre
```

El paquete depende tanto de [#Mustache](https://github.com/jehugaleahsa/mustache-sharp) (en la versión .NET Core usa una [versión modificada](https://github.com/huchim/mustache-sharp) que funciona en .NET Core) y de [Reporting](https://github.com/huchim/reporting) para generar el resultado.



## Recomendaciones

Trabajar con los reportes, mantenerlos actualizados y organizados puede ser sencillo si se usa el [administrador de reportes](ReportManager) que permite leer la información de todos los reportes contenidos en un repositorio o carpeta.

```
src/
| -- myproject.csproj
| -- Reports
     | -- Report01
     	  | -- report.json
     	  | -- template.html
     | -- Report02
     | -- Report03
     | -- Report04  
```

La estructura anterior permite organizar cada reporte de tal manera que toda la información del reporte ([archivo de configuración](ReportConfiguration), rutinas sql, etc) se mantenga separados de los demás reportes.

## Cargar la configuración del reporte

Puede crear una instancia que le ayudará a recuperar la información de los reportes.

```csharp
// Recupero la ruta al directorio donde se encuentran los reportes.
var carpetaReportes = Server.MapPath("~/Reports/");

// .NET Core
// var carpetaReportes = Path.Combine(env.ContentRootPath, "Reports");

// Creo una instancia para administrar los reportes.
var administradorReportes = new ReportRepository(carpetaReportes);
```

En el ejemplo anterior hay un reporte dentro de la carpeta `Reports/Report01.json`(leer más sobre el [archivo de configuración](ReportConfiguration)) dicho reporte tendría una configuración parecida a esta.

```json
{
  "$schema": "https://raw.githubusercontent.com/huchim/schemas/master/Reports/reports.schema.json",
  "name": "NombreReporte1",
  "label": "Ejemplo de reporte",
  "icon": "list",
  "html.template": "template.html",
  "sql": [
    {
      "name": "data",
      "script": "SELECT Nombre, Apellido FROM Alumnos WHERE Maestro = @MaestroId",
      "required": [ "MaestroId" ]
    }
  ],
  "args": [
    {
      "description": "Identificador del maestro",
      "label": "Maestro",
      "name": "MaestroId",
      "type": "number"
    }
  ]
}
```

**Nota:** la variable `html.template` indica el archivo HTML que será usado para la plantilla.

Para poder utilizar el reporte dentro de nuestro código, debemos recuperarlo del repositorio.

```csharp
var configuracionReporte = administradorReportes.Reports.Single(x => x.Name == "NombreReporte1");
```

Toda la información del reporte ahora está disponible dentro de `configuracionReporte` y podemos trabajar con el. Nota: Utilizamos `Single` porque si no existe, generará un error, ya que el comportamiento esperado es que si exista.

## Consulta SQL

Un reporte puede tener varias consultas SQL. Cada consulta debe estar dentro de la colección `sql`del archivo de configuración.

```json
{
  "$schema": "https://raw.githubusercontent.com/huchim/schemas/master/Reports/reports.schema.json",
  "name": "NombreReporte1",
  "sql": [
    {
      "name": "data",
      "script": "SELECT Nombre, Apellido FROM Alumnos WHERE Maestro = @MaestroId",
      "required": [ "MaestroId" ]
    }
  ]
}
```

El nombre que le hemos asignado a esa consulta SQL es `data` y ella extrae una lista de alumnos, cuyo maestro es variable (`MaestroId`) .

[Reporting](https://github.com/huchim/reporting) se encargará de completar la consulta, pero antes debemos indicarle el tipo de datos de esa variable.

```json
{
  "$schema": "https://raw.githubusercontent.com/huchim/schemas/master/Reports/reports.schema.json",
  "name": "NombreReporte1",  
  "args": [
    {
      "description": "Identificador del maestro",
      "label": "Maestro",
      "name": "MaestroId",
      "type": "number"
    }
  ]
}
```

De esta manera durante la ejecución de la consulta, Reporting intentará asignar el tipo de valor correcto.

**Nota:** Si la consulta es muy larga, puede moverla a un archivo (ejemplo: scripts.sql) y utilizar la propiedad `file`en vez de `script`. El archivo es relativo al directorio del archivo `report.json`

## Ejecutar el reporte

El proceso de ejecutar el reporte es sencillo.

Una vez obtenido el reporte debemos crear una instancia de la clase `ReportManager`que se encargará de ejecutar la consulta y cederle el control al generador que será el encargado de darle el formato deseado a los datos.

Para que pueda ejecutar las consultas, debemos darle una conexión ([IDbConnection](https://msdn.microsoft.com/en-us/library/system.data.idbconnection(v=vs.110).aspx)). En el siguiente ejemplo usamos una clase llamada `DataContext`que extiende a `DbContext`

**Nota:** esta clase usa [EntityFramework](https://github.com/aspnet/EntityFramework6), pero puede usar cualquier conexión que implemente [IDbConnection](https://msdn.microsoft.com/en-us/library/system.data.idbconnection(v=vs.110).aspx).

```csharp
using (var db = new DataContext())
{
    // Crear la instancia de quién se encargará de realizar la consulta.
    // Le pasamos la conexión activa, para que ejecute la consulta.
    var gestorReportes = new ReportManager(db.Database.Connection);
  
    // El SQL tiene filtros que sólo se ejecutarán si existen estas variables.
    var variablesReporte = new Dictionary<string, object>();
  
    // Agregamos la variable del maestro
	gestorReportes.Add("MaestroId", 58);
  
    // Abrimos el reporte. "configuracionReporte" es el reporte del ejemplo de arriba.
    gestorReportes.Open(configuracionReporte, variablesReporte);
}
```

## Utilizar un "generador" para dar formato

Una vez que la consulta ha sido configurada, [Reporting](https://github.com/huchim/reporting) se encargará de ejecutarla, pero antes hay que indicarle el formato en el que queremos los resultados.

Puede agregar un generador usando`ReportManager.AddGenerator(IGeneratorEngine)` o indicarle el generador al momento de ejecutar el reporte.

Este generador puede generar reportes usando un formato HTML.

```csharp
using (var db = new DataContext())
{
    // Crear la instancia de quién se encargará de realizar la consulta.
    // Le pasamos la conexión activa, para que ejecute la consulta.
    var gestorReportes = new ReportManager(db.Database.Connection);
    ...
    gestorReportes.Open(configuracionReporte, variablesReporte);
	...
      
    // Recuperar el contenido en formato HTML.
  	var htmlOutput = reportManager.GetString(new HtmlGenerator());
```

Ahora htmlOutput contiene el HTML generado con los datos de la consulta SQL.

## Formato HTML

Este generador usa Mustache, puede obtener más información en su [manual](https://mustache.github.io/mustache.5.html).

Para generar una simple tabla podemos usar el siguiente ejemplo, donde `data`corresponde al nombre que le pusimos a la consulta SQL, y contiene un arreglo con todos los registros. Usaremos `{{#each data}}{{/each}}`para poder ir por cada registro.

**Nota:** Este archivo HTML se define en la variable `html.template`dentro del archivo `report.json`

```html
<div class="page-header">
    <h3>Reporte de alumnos</h3>
</div>

<table class="table table-striped">
    <thead>
        <tr>
            <th>Nombre</th>
            <th>Apellido</th>
        </tr>
    </thead>
    <tbody>
        {{#each data}}
        <tr>
            <td align="center">{{Nombre}}</td>
            <td class="visible-lg">{{Apellido}}</td>
        </tr>
        {{/each}}
    </tbody>
</table>

```

Como puede observar se recorre registro por registro dentro de la variable `data`.

Aunque no es la idea principal, se puede usar javascript para crear gráficas por ejemplo.

```
<span id="_ReportData" class="hidden" data-info='[{{#each data}}{"Nombre":{{Nombre}},"Apellido":"{{Apellido}}",{{/each}}]'></span>

<script type="text/javascript">
    /* 
    	Obtener la información del reporte e inicializar el elemento de la gráfica.
    	Debemos quitar la última coma para poder convertirlo a JSON por ejemplo.
    */
    var data = $("#_ReportData").attr("data-info").replace(",]", "]");
    var infoJson = $.parseJSON(data);
</script>
```

Nota: No se soporta comentarios Javascript usando `// ...`, solo comentarios tipo `/* ... */`



## Presentar resultados

Se puede presentar de distintas maneras la información, pero eso ya queda de lado de usted.

En casos como [Excel](https://github.com/huchim/reporting-excel) este no devuelve una cadena `GetString` como en el ejemplo, ya que devuelve el archivo en formato binario, y lo ideal sería presentarlo en pantalla o descargarlo.

En este caso de HTML, lo ideal sería ponerlo en una variable e imprimirla en la página. El siguiente ejemplo es para una vista MVC, pero funciona en WebForms.

```csharp
public ActionResult Index()
{
    ...
     
    // Generar los datos y recuperar el HTML.
	var htmlOutput = reportManager.GetString(new HtmlGenerator());  
}
```

En una vista imprimir la variable.

```html
@* Es necesario usar Raw, el contenido generado está en HTML. *@
@Html.Raw(ViewData["HtmlContent"])
```

## Conclusión.

Actualmente se tiene diferentes generadores como HTML, [Excel](https://github.com/huchim/reporting-excel), [Csv](https://github.com/huchim/reporting-csv), Json que permiten generar el reporte de manera sencilla.

Cada generador tiene un identificador único, de tal manera que los puede agregar y luego utilizar ese identificador desde una URL para recuperar el generador que desea el usuario.

Ejemplo para descargar un reporte.

```csharp
/// <summary>
/// Encargado de ejecutar los reportes.
/// </summary>
private readonly ReportManager reportManager;

/// <summary>
/// Administrador de reportes.
/// </summary>
private readonly ReportRepository reportRepository;

public ReportController(...)
{
  // Crear una instancia para administrar los reportes.
  this.reportRepository = new ReportRepository(Path.Combine(env.ContentRootPath, "Reports"));

  // Crear una instancia del administrador de reportes para poder listar los generadores.
  this.reportManager = new ReportManager(this.connection);

  this.reportManager.AddGenerator(new HtmlGenerator());
  this.reportManager.AddGenerator(new ExcelGenerator());
  this.reportManager.AddGenerator(new CsvGenerator());
}

public IActionResult Details(string id, Guid? type)
{
	// Recuperar el reporte.
	var configuracionReporte = this.reportRepository.Reports.SingleOrDefault(x => x.Name == id);

	// Abre el reporte, solo si se define un tipo de generador pasamos los datos de la query.
	this.OpenReport(configuracionReporte, type);
  
  	// Obtiene el resultado del reporte.
    try
    {
      var results = this.reportManager.Process(type);

      if (results == null)
      {
        return this.NotFound("No hay reporte disponible.");
      }

      return this.File(results.Data, results.MimeType, $"{id}{results.FileExtension}");
    }
    catch (NoDataException ex)
    {
      return this.RedirectToAction("Details", new { id = id, message = ex.Message });
    }
}
```



## Ayuda

Este repositorio acepta código y ayuda para documentar.

Muchas gracias.

Carlos Huchim Ahumada