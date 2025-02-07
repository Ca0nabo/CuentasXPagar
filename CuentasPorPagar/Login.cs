using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CuentasXPagar_WinForms
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text;
            string contraseña = txtContraseña.Text;

            if (ValidarUsuario(usuario, contraseña))
            {
                MessageBox.Show("Acceso concedido", "Bienvenido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                MenuPrincipal menu = new MenuPrincipal();
                menu.Show();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarUsuario(string usuario, string contraseña)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = "SELECT Contraseña FROM Usuarios WHERE Usuario = @Usuario";
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Usuario", usuario);
                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null)
                    {
                        string contraseñaAlmacenada = (string)resultado;

                        return contraseñaAlmacenada == contraseña;
                    }
                }
            }
            return false;
        }
    }
}