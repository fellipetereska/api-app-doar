﻿using MySql.Data.MySqlClient;

namespace Api.AppDoar.PersistenciaDB
{
    public static class ConnectionDB
    {
        private static readonly string connectionString = "Server=193.203.175.155;Database=u271392345_db_doar;Uid=u271392345_admin_doar;Pwd=fte3009Aa.;";

        public static MySqlConnection GetConnection()
        {
            var conn = new MySqlConnection(connectionString);
            conn.Open();
            return conn;
        }
    }
}
