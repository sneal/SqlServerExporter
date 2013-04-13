using System;
using System.Runtime.Serialization;

namespace Sneal.SqlExporter.Core
{
    public class SqlExporterConnectionException : SqlExporterException
    {
        public SqlExporterConnectionException()
        {
        }

        public SqlExporterConnectionException(string message) : base(message)
        {
        }

        public SqlExporterConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SqlExporterConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}