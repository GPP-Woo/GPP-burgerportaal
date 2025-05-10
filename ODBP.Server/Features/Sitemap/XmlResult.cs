using System.IO.Hashing;
using System.Net.Mime;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace ODBP.Features.Sitemap
{
    public class XmlResult<T>(T model, XmlSerializerNamespaces? namespaces = null, (string, string)[]? schemas = null) : IActionResult
    {
        private static readonly XmlSerializer s_serializer = new(typeof(T));
        private static readonly XmlWriterSettings s_xmlWriterSettings = new() { Async = true, NamespaceHandling = NamespaceHandling.OmitDuplicates };
        private const int DefaultBufferSize = 4096;

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var token = context.HttpContext.RequestAborted;
            await using var tempFile = File.Create(Path.GetTempFileName(), DefaultBufferSize, FileOptions.DeleteOnClose);
            await using var writer = XmlWriter.Create(tempFile, s_xmlWriterSettings);

            s_serializer.Serialize(writer, model, namespaces);
            tempFile.Seek(0, SeekOrigin.Begin);

            if (schemas != null)
            {
                var errors = await ValidateAsync(tempFile, schemas);
                if (errors.Count > 0)
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<XmlResult<T>>>();
                    foreach (var item in errors.DistinctBy(x => (x.Severity, x.Message)))
                    {
                        logger.LogError("{Severity} while generating xml of type {Type}: {Message}", item.Severity, typeof(T).Name, item.Message);
                    }
                    var first = errors[0];
                    context.HttpContext.Response.Headers["X-Xml-Validation-Error"] = first.Message;
                }
                tempFile.Seek(0, SeekOrigin.Begin);
            }

            var etag = await GetEtag(tempFile, token);
            tempFile.Seek(0, SeekOrigin.Begin);

            context.HttpContext.Response.Headers.ContentType = MediaTypeNames.Text.Xml;
            context.HttpContext.Response.Headers.ContentLength = tempFile.Length;
            context.HttpContext.Response.Headers.ETag = etag;

            await tempFile.CopyToAsync(context.HttpContext.Response.Body, token);
        }

        private static async Task<string> GetEtag(Stream stream, CancellationToken token)
        {
            var hash = new XxHash3();
            await hash.AppendAsync(stream, token);
            return GetEtag(hash);
        }

        private static string GetEtag(XxHash3 hash)
        {
            const int HashSizeInBytes = 8;
            const int HashSizeInChars = 12;
            Span<byte> hashBytes = stackalloc byte[HashSizeInBytes];
            Span<char> chars = stackalloc char[HashSizeInChars];
            hash.GetCurrentHash(hashBytes);
            Convert.TryToBase64Chars(hashBytes, chars, out _);
            return $"\"{chars}\"";
        }

        private record ValidationError(XmlSeverityType Severity, string Message);

        private static async Task<IReadOnlyList<ValidationError>> ValidateAsync(Stream stream, (string, string)[] schemas)
        {
            List<ValidationError> messages = [];
            try
            {
                var readerSettings = new XmlReaderSettings();
                foreach (var (key, value) in schemas)
                {
                    readerSettings.Schemas.Add(key, value);
                }
                readerSettings.ValidationType = ValidationType.Schema;
                readerSettings.Async = true;
                readerSettings.ValidationEventHandler += (_, e) =>
                {
                    messages.Add(new(e.Severity, e.Message));
                };
                using var reader = XmlReader.Create(stream, readerSettings);
                while (await reader.ReadAsync()) { }
            }
            catch (Exception e)
            {
                messages.Add(new(XmlSeverityType.Error, e.Message));
            }
            return messages;
        }
    }
}
