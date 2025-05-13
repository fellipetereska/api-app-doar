using MySql.Data.MySqlClient;

namespace Api.AppDoar.PersistenciaDB
{
    public static class ConnectionDB
    {
        private static MySqlConnection conn;

        // Construtor - Toda vez que a classe for chamada ela automáticamente já se conecta no banco
        static ConnectionDB()
        {
            conn = new MySqlConnection("Server=localhost;Database=db_doar;Uid=root;Pwd=fte3009;");
            conn.Open();
        }

        // Método público para chamar a classe de conexão
        public static MySqlConnection GetConnection() { return conn; }
    }
}
