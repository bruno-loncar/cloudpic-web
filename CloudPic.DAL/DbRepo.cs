using NPoco;
using System.Data.SqlClient;

namespace CloudPic.DAL
{
    public abstract class DbRepo
    {
        protected IDatabase DBConn
        {
            get
            {
                using var db = new Database(
                    @"Data Source=.;Initial Catalog=CloudPic;Integrated Security=True;",
                    DatabaseType.SqlServer2012,
                    SqlClientFactory.Instance);
                return db;
            }
        }
    }
}