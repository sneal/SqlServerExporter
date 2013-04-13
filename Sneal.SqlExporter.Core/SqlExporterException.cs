using System;
using System.Runtime.Serialization;

namespace Sneal.SqlExporter.Core
{
    public class SqlExporterException : ApplicationException
    {
        public SqlExporterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SqlExporterException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SqlExporterException(string message) : base(message)
        {
        }

        public SqlExporterException()
        {
        }
    }
}