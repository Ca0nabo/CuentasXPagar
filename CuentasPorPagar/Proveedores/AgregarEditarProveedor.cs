using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CuentasXPagar_WinForms
{
    public partial class AgregarEditarProveedor : Form
    {
        private int? idProveedor = null;

        public AgregarEditarProveedor()
        {
            InitializeComponent();
            cbTipoPersona.Items.AddRange(new string[] { "Física", "Jurídica" });
            cbEstado.Items.AddRange(new string[] { "Activo", "Inactivo" });
            cbTipoPersona.SelectedIndex = 0;
            cbEstado.SelectedIndex = 0;
        }

        public AgregarEditarProveedor(int id, string nombre, string tipoPersona, string cedulaRNC, decimal balance, string estado)
        {
            InitializeComponent();
            idProveedor = id;
            txtNombre.Text = nombre;
            cbTipoPersona.Items.AddRange(new string[] { "Física", "Jurídica" });
            txtCedulaRNC.Text = cedulaRNC;
            txtBalance.Text = balance.ToString();
            cbEstado.Items.AddRange(new string[] { "Activo", "Inactivo" });
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string tipoPersona = cbTipoPersona.SelectedItem.ToString();
            string cedulaRNC = txtCedulaRNC.Text.Trim();
            decimal balance = decimal.TryParse(txtBalance.Text, out decimal b) ? b : 0;
            string estado = cbEstado.SelectedItem.ToString();

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(cedulaRNC))
            {
                MessageBox.Show("Debe llenar todos los campos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query;

                if (idProveedor == null)
                {
                    query = "INSERT INTO Proveedores (Nombre, TipoPersona, CedulaRNC, Balance, Estado) VALUES (@Nombre, @TipoPersona, @CedulaRNC, @Balance, @Estado)";
                }
                else
                {
                    query = "UPDATE Proveedores SET Nombre = @Nombre, TipoPersona = @TipoPersona, CedulaRNC = @CedulaRNC, Balance = @Balance, Estado = @Estado WHERE Id = @Id";
                }

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@TipoPersona", tipoPersona);
                    cmd.Parameters.AddWithValue("@CedulaRNC", cedulaRNC);
                    cmd.Parameters.AddWithValue("@Balance", balance);
                    cmd.Parameters.AddWithValue("@Estado", estado);

                    if (idProveedor != null)
                        cmd.Parameters.AddWithValue("@Id", idProveedor);

                    cmd.ExecuteNonQuery();
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}