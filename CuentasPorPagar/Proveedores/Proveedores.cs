using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CuentasXPagar_WinForms
{
    public partial class Proveedores : Form
    {
        public Proveedores()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos(string filtro = "")
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = "SELECT Id, Nombre, TipoPersona, CedulaRNC, Balance, Estado FROM Proveedores";
                if (!string.IsNullOrEmpty(filtro))
                {
                    query += " WHERE Nombre LIKE @Filtro";
                }

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    if (!string.IsNullOrEmpty(filtro))
                    {
                        cmd.Parameters.AddWithValue("@Filtro", "%" + filtro + "%");
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvProveedores.DataSource = dt;
                }
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarDatos(txtBuscar.Text);
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            AgregarEditarProveedor form = new AgregarEditarProveedor();
            if (form.ShowDialog() == DialogResult.OK)
            {
                CargarDatos();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvProveedores.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvProveedores.SelectedRows[0].Cells["Id"].Value);
                string nombre = dgvProveedores.SelectedRows[0].Cells["Nombre"].Value.ToString();
                string tipoPersona = dgvProveedores.SelectedRows[0].Cells["TipoPersona"].Value.ToString();
                string cedulaRNC = dgvProveedores.SelectedRows[0].Cells["CedulaRNC"].Value.ToString();
                decimal balance = Convert.ToDecimal(dgvProveedores.SelectedRows[0].Cells["Balance"].Value);
                string estado = dgvProveedores.SelectedRows[0].Cells["Estado"].Value.ToString();

                AgregarEditarProveedor form = new AgregarEditarProveedor(id, nombre, tipoPersona, cedulaRNC, balance, estado);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    CargarDatos();
                }
            }
            else
            {
                MessageBox.Show("Seleccione un proveedor para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProveedores.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvProveedores.SelectedRows[0].Cells["Id"].Value);

                if (MessageBox.Show("¿Está seguro de eliminar este proveedor?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (SqlConnection conexion = ConexionBD.ObtenerConexion())
                    {
                        string query = "DELETE FROM Proveedores WHERE Id = @Id";
                        using (SqlCommand cmd = new SqlCommand(query, conexion))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    CargarDatos();
                }
            }
            else
            {
                MessageBox.Show("Seleccione un proveedor para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}