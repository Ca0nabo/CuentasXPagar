using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CuentasXPagar_WinForms
{
    public partial class SolicitudesDePago : Form
    {
        public SolicitudesDePago()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos(string filtro = "")
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = "SELECT Id, NumeroDocumento, NumeroFactura, FechaSolicitud, MontoPagar, Estado FROM Solicitudes_de_Pago";
                if (!string.IsNullOrEmpty(filtro))
                {
                    query += " WHERE NumeroDocumento LIKE @Filtro";
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
                    dgvSolicitudes.DataSource = dt;
                }
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarDatos(txtBuscar.Text);
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            AgregarEditarSolicitud form = new AgregarEditarSolicitud();
            if (form.ShowDialog() == DialogResult.OK)
            {
                CargarDatos();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvSolicitudes.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvSolicitudes.SelectedRows[0].Cells["Id"].Value);
                string numeroDocumento = dgvSolicitudes.SelectedRows[0].Cells["NumeroDocumento"].Value.ToString();
                string numeroFactura = dgvSolicitudes.SelectedRows[0].Cells["NumeroFactura"].Value.ToString();
                DateTime fechaSolicitud = Convert.ToDateTime(dgvSolicitudes.SelectedRows[0].Cells["FechaSolicitud"].Value);
                decimal montoPagar = Convert.ToDecimal(dgvSolicitudes.SelectedRows[0].Cells["MontoPagar"].Value);
                string estado = dgvSolicitudes.SelectedRows[0].Cells["Estado"].Value.ToString();

                AgregarEditarSolicitud form = new AgregarEditarSolicitud(id, numeroDocumento, numeroFactura, fechaSolicitud, montoPagar, estado);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    CargarDatos();
                }
            }
            else
            {
                MessageBox.Show("Seleccione una solicitud para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvSolicitudes.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvSolicitudes.SelectedRows[0].Cells["Id"].Value);

                if (MessageBox.Show("¿Está seguro de eliminar esta solicitud de pago?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (SqlConnection conexion = ConexionBD.ObtenerConexion())
                    {
                        string query = "DELETE FROM Solicitudes_de_Pago WHERE Id = @Id";
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
                MessageBox.Show("Seleccione una solicitud para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}