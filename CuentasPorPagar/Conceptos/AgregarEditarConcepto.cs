using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CuentasXPagar_WinForms
{
    public partial class AgregarEditarConcepto : Form
    {
        private int? idConcepto = null;

        public AgregarEditarConcepto()
        {
            InitializeComponent();
            cbEstado.Items.AddRange(new string[] { "Activo", "Inactivo" });
            cbEstado.SelectedIndex = 0;
        }

        public AgregarEditarConcepto(int id, string descripcion, string estado)
        {
            InitializeComponent();
            idConcepto = id;
            txtDescripcion.Text = descripcion;
            cbEstado.Items.AddRange(new string[] { "Activo", "Inactivo" });
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string descripcion = txtDescripcion.Text.Trim();
            string estado = cbEstado.SelectedItem.ToString();

            if (string.IsNullOrEmpty(descripcion))
            {
                MessageBox.Show("Debe ingresar una descripción.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query;

                if (idConcepto == null)
                {
                    query = "INSERT INTO Conceptos_de_Pago (Descripcion, Estado) VALUES (@Descripcion, @Estado)";
                }
                else
                {
                    query = "UPDATE Conceptos_de_Pago SET Descripcion = @Descripcion, Estado = @Estado WHERE Id = @Id";
                }

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                    cmd.Parameters.AddWithValue("@Estado", estado);

                    if (idConcepto != null)
                        cmd.Parameters.AddWithValue("@Id", idConcepto);

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