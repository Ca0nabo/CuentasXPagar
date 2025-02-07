using System;
using System.Data.SqlClient;

namespace CuentasXPagar_WinForms
{
    public class ConexionBD
    {
        private static string cadenaConexion = "Server=localhost;Database=CuentasXPagar;Trusted_Connection=True;";

        public static SqlConnection ObtenerConexion()
        {
            SqlConnection conexion = new SqlConnection(cadenaConexion);
            try
            {
                conexion.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("Error de conexión: " + ex.Message);
            }
            return conexion;
        }
    }
}