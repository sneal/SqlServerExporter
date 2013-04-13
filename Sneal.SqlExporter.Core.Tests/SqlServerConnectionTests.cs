using NUnit.Framework;
using SharpTestsEx;

namespace Sneal.SqlExporter.Core.Tests
{
    [TestFixture]
    public class SqlServerConnectionTests
    {
        [Test]
        public void Can_connect_to_local_instance_with_integrated_auth()
        {
            Executing.This(() => new SqlServerConnection(".")).Should().NotThrow();
            Executing.This(() => new SqlServerConnection(".").Connect()).Should().NotThrow();
        }

        [Test]
        public void Can_list_dbs_on_local_instance()
        {
            var connection = new SqlServerConnection(".");
            connection.Connect();
            connection.GetDatabaseNames().Count.Should().Be.GreaterThan(0);
        }

        [Test, Explicit]
        public void Can_list_dbs_on_local_instance_using_sql_auth()
        {
            var connection = new SqlServerConnection(".", "sa", "password");
            connection.Connect();
            connection.GetDatabaseNames().Count.Should().Be.GreaterThan(0);
        }
    }
}
