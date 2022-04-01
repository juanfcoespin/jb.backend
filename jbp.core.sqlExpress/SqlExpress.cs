using System.Data.SqlClient;
namespace jbp.core.sqlExpress
{
    public class SqlExpress
    {
        private string connectionString;
        private SqlConnection sqlcnn;
        public SqlExpress() {
            this.setConnectionString();
        }

        private void setConnectionString()
        {
            this.connectionString =
                String.Format(@"Server={0};Database={1};User ID={2};Password={3};Trusted_Connection=False;",
                    conf.Default.ipserver,
                    conf.Default.dbName,
                    conf.Default.user,
                    conf.Default.pwd);
        }

        public string Connect() {
            try
            {
                this.sqlcnn = new SqlConnection(this.connectionString);
                this.sqlcnn.Open();
                return "ok";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public void Disconect() {
            if (this.sqlcnn.State == System.Data.ConnectionState.Open)
                this.sqlcnn.Close();
        }
    }
}