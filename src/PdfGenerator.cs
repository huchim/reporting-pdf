// -----------------------------------------------------------------------
// <copyright file="PdfGenerator.cs" company="Carlos Huchim Ahumada">
// Este código se libera bajo los términos de licencia especificados.
// </copyright>
// -----------------------------------------------------------------------
namespace Jaguar.Reporting.Generators
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
#if !NETCOREAPP1_1
    using iTextSharp.text;
    using iTextSharp.text.pdf;
#endif
    using Jaguar.Reporting.Common;
    using Jaguar.Reporting.Html;

    /// <summary>
    /// Crea un nuevo generador basado en <see cref="HtmlGenerator"/> para obtener primero el HTML
    /// y poder generar el PDF posteriormente.
    /// </summary>
    public class PdfGenerator : IGeneratorEngine
    {
        /// <summary>
        /// Encargado de generar el HTML.
        /// </summary>
        private readonly HtmlGenerator _html;

        public PdfGenerator()
        {
            // Crea la instancia que se encargará de generar el HTML.
            _html = new HtmlGenerator();
        }

        /// <inheritdoc/>
        public Guid Id => new Guid("0fddb63c-e4b2-4c33-ac4c-de894715a74a");

        /// <inheritdoc/>
        public string Name => "Html To PDF";

        /// <inheritdoc/>
        public string MimeType => "application/pdf";

        /// <inheritdoc/>
        public string FileExtension => ".pdf";

        /// <inheritdoc/>
        public bool IsEmbed => false;

        /// <summary>
        /// Carga el texto de la plantilla.
        /// </summary>
        /// <returns></returns>
        private string LoadTemplateFile(string workingDirectory, string htmlTemplate)
        {
            if (string.IsNullOrEmpty(workingDirectory))
            {
                throw new ArgumentNullException(nameof(workingDirectory));
            }

            if (string.IsNullOrEmpty(htmlTemplate))
            {
                throw new ArgumentNullException(nameof(htmlTemplate));
            }

            var templateFile = Path.Combine(workingDirectory, htmlTemplate);
            
            return File.ReadAllText(templateFile);
        }

        /// <inheritdoc/>
        public byte[] GetAllBytes(ReportHandler report, List<DataTable> data, Dictionary<string, object> variables)
        {
#if NETCOREAPP1_1
            throw new NotImplementedException("No hay un renderizador disponible en PDF para .NET Core.");
#else
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }

            if (report.Options == null)
            {
                throw new ArgumentNullException(nameof(report.Options));
            }

            if (!report.Options.ContainsKey("html.template"))
            {
                throw new ArgumentNullException("html.template no se encuentra definido.");
            }

            // Inicializar lista de variables en caso de que no existan.
            variables = variables ?? new Dictionary<string, object>();
            
            // Obtener el HTML generado con los datos.
            var htmlOutput = _html.GetString(report, data, variables);

            if (string.IsNullOrEmpty(htmlOutput))
            {
                throw new EmptyHtmlException();
            }

            var cssExtractor = new CssResources {
                WorkingDirectory = report.WorkDirectory
            };

            // Recuperar la lista de recursos CSS tanto externos como internos.
            var cssResourceList = _html.GetResources(cssExtractor, htmlOutput);
            byte[] cssContent = null;

            if (cssResourceList.Count > 0)
            {
                // Unir todos los archivos externos y bloques "style"
                cssContent = _html.MergeResources(cssResourceList);
            }

            // Create a stream that we can write to, in this case a MemoryStream
            using (var ms = new MemoryStream())
            {

                //Create an iTextSharp Document which is an abstraction of a PDF but **NOT** a PDF
                using (var doc = new Document())
                {
                    using (var writer = PdfWriter.GetInstance(doc, ms))
                    {
                        //Open the document for writing
                        doc.Open();

                        if (cssContent == null)
                        {
                            using (var srHtml = new StringReader(htmlOutput))
                            {
                                //Parse the HTML without CSS
                                iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, srHtml);
                            }                            
                        }
                        else
                        {
                            using (var cssStream = new MemoryStream(cssContent))
                            {
                                using (var srHtml = new MemoryStream(Encoding.UTF8.GetBytes(htmlOutput)))
                                {
                                    //Parse the HTML without CSS
                                    iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, srHtml, cssStream);
                                }
                            }                            
                        }

                        doc.Close();
                    }
                }

                return ms.ToArray();
            }
#endif
}

        /// <inheritdoc/>
        public string GetString(ReportHandler report, List<DataTable> data, Dictionary<string, object> variables)
        {
            throw new NotImplementedException("El contenido del PDF no puede ser devuelto en una cadena de texto. Use GetAllBytes().");
        }
    }
}