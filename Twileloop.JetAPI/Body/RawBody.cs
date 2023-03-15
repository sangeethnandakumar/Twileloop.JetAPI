using System.IO;
using System;
using System.Text.Json;
using System.Xml.Serialization;
using Twileloop.JetAPI.Types;

namespace Twileloop.JetAPI.Body {
    /// <summary>
    /// Represents the raw body of a HTTP request or response.
    /// </summary>
    public class RawBody {
        /// <summary>
        /// The content of the raw body.
        /// </summary>
        public string Content { get; }
        /// <summary>
        /// The content type of the raw body.
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawBody"/> class with the specified body type and content.
        /// </summary>
        /// <param name="bodyType">The type of the raw body.</param>
        /// <param name="content">The content of the raw body.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="content"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="bodyType"/> is not a valid <see cref="BodyType"/> value.</exception>
        public RawBody(BodyType bodyType, string content) {
            if (content == null) throw new ArgumentNullException(nameof(content));

            ContentType = bodyType switch {
                BodyType.Json => "application/json",
                BodyType.XML => "application/xml",
                BodyType.Text => "text/plain",
                BodyType.HTML => "text/html",
                BodyType.JavaScript => "application/javascript",
                _ => throw new ArgumentOutOfRangeException(nameof(bodyType), bodyType, null)
            };

            Content = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawBody"/> class with the specified body type and content.
        /// </summary>
        /// <param name="bodyType">The type of the raw body.</param>
        /// <param name="content">The content of the raw body.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="content"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="bodyType"/> is not a valid <see cref="BodyType"/> value.</exception>
        public RawBody(BodyType bodyType, object content) {
            if (content == null) throw new ArgumentNullException(nameof(content));

            ContentType = bodyType switch {
                BodyType.Json => "application/json",
                BodyType.XML => "application/xml",
                BodyType.Text => "text/plain",
                BodyType.HTML => "text/html",
                BodyType.JavaScript => "application/javascript",
                _ => throw new ArgumentOutOfRangeException(nameof(bodyType), bodyType, null)
            };

            switch (bodyType) {
                case BodyType.Json:
                    Content = JsonSerializer.Serialize(content);
                    break;
                case BodyType.XML:
                    var serializer = new XmlSerializer(content.GetType());
                    using (var writer = new StringWriter()) {
                        serializer.Serialize(writer, content);
                        Content = writer.ToString();
                    }
                    break;
                default:
                    Content = content.ToString();
                    break;
            }
        }
    }
}
