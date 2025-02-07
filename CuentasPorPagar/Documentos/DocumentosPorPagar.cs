using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CuentasXPagar_WinForms
{
    public partial class DocumentosPorPagar : Form
    {
        public DocumentosPorPagar()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos(string filtro = "")
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = "SELECT Id, NumeroDocumento, NumeroFactura, ConceptodePago, FechaDocumento, Monto, FechaRegistro, Proveedor, Estado FROM Documentos_Pagar";
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
                    dgvDocumentos.DataSource = dt;
                }
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarDatos(txtBuscar.Text);
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            AgregarEditarDocumento form = new AgregarEditarDocumento();
            if (form.ShowDialog() == DialogResult.OK)
            {
                CargarDatos();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvDocumentos.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvDocumentos.SelectedRows[0].Cells["Id"].Value);
                string numeroDocumento = dgvDocumentos.SelectedRows[0].Cells["NumeroDocumento"].Value.ToString();
                string numeroFactura = dgvDocumentos.SelectedRows[0].Cells["NumeroFactura"].Value.ToString();
                string conceptoPago = dgvDocumentos.SelectedRows[0].Cells["ConceptodePago"].Value.ToString();
                DateTime fechaDocumento = Convert.ToDateTime(dgvDocumentos.SelectedRows[0].Cells["FechaDocumento"].Value);
                decimal monto = Convert.ToDecimal(dgvDocumentos.SelectedRows[0].Cells["Monto"].Value);
                DateTime fechaRegistro = Convert.ToDateTime(dgvDocumentos.SelectedRows[0].Cells["FechaRegistro"].Value);
                string proveedor = dgvDocumentos.SelectedRows[0].Cells["Proveedor"].Value.ToString();
                string estado = dgvDocumentos.SelectedRows[0].Cells["Estado"].Value.ToString();

                AgregarEditarDocumento form = new AgregarEditarDocumento(id, numeroDocumento, numeroFactura, conceptoPago, fechaDocumento, monto, fechaRegistro, proveedor, estado);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    CargarDatos();
                }
            }
            else
            {
                MessageBox.Show("Seleccione un documento para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvDocumentos.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvDocumentos.SelectedRows[0].Cells["Id"].Value);

                if (MessageBox.Show("¿Está seguro de eliminar este documento?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (SqlConnection conexion = ConexionBD.ObtenerConexion())
                    {
                        string query = "DELETE FROM Documentos_Pagar WHERE Id = @Id";
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
                MessageBox.Show("Seleccione un documento para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}