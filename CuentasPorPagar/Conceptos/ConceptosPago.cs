using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CuentasXPagar_WinForms
{
    public partial class ConceptosPago : Form
    {
        public ConceptosPago()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos(string filtro = "")
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = "SELECT Id, Descripcion, Estado FROM Conceptos_De_Pago";
                if (!string.IsNullOrEmpty(filtro))
                {
                    query += " WHERE Descripcion LIKE @Filtro";
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
                    dgvConceptos.DataSource = dt;
                }
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarDatos(txtBuscar.Text);
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            AgregarEditarConcepto form = new AgregarEditarConcepto();
            if (form.ShowDialog() == DialogResult.OK)
            {
                CargarDatos();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvConceptos.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvConceptos.SelectedRows[0].Cells["Id"].Value);
                string descripcion = dgvConceptos.SelectedRows[0].Cells["Descripcion"].Value.ToString();
                string estado = dgvConceptos.SelectedRows[0].Cells["Estado"].Value.ToString();

                AgregarEditarConcepto form = new AgregarEditarConcepto(id, descripcion, estado);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    CargarDatos();
                }
            }
            else
            {
                MessageBox.Show("Seleccione un concepto para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvConceptos.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvConceptos.SelectedRows[0].Cells["Id"].Value);

                if (MessageBox.Show("¿Está seguro de eliminar este concepto?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (SqlConnection conexion = ConexionBD.ObtenerConexion())
                    {
                        string query = "DELETE FROM Conceptos_de_Pago WHERE Id = @Id";
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
                MessageBox.Show("Seleccione un concepto para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}