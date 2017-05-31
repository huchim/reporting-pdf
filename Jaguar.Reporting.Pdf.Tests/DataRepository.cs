using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Jaguar.Reporting;
using Jaguar.Reporting.Common;

namespace tests
{
    public abstract class DataRepository
    {
        public static ReportHandler GetReport()
        {
            var workingDirectory = Directory.GetCurrentDirectory();

            if (string.IsNullOrEmpty(workingDirectory))
            {
                throw new Exception("No se puede asignar el directorio del ensamblado al reporte.");
            }            

            return new ReportHandler
            {
                // Configurar la ruta al directorio de pruebas.
                WorkDirectory = workingDirectory,

                // Asignar un archivo de plantilla.
                Options = new Dictionary<string, object> { { "html.template", "template.html" } },
            };
        }

        public static List<DataTable> GetDummyData(string[] tableNames)
        {
            var data = new List<DataTable>();

            foreach (string tableName in tableNames)
            {
                var table = new DataTable(tableName);

                for (int i = 1; i <= 5; i++)
                {
                    var col = new DataColumn($"Col{i}", typeof(string));
                    table.Columns.Add(col);
                }

                for (int i = 0; i < 100; i++)
                {
                    var row = new DataRow();

                    for (int c = 0; c <= 5; c++)
                    {
                        row.Add($"Col{c}", $"Row {i} at column index {c}", typeof(string));
                    }

                    table.Add(row);
                }

                data.Add(table);
            }

            return data;
        }
    }
}
