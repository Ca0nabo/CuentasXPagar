using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CuentasXPagar_WinForms
{
    public partial class AgregarEditarDocumento : Form
    {
        private int? idDocumento = null;

        public AgregarEditarDocumento()
        {
            InitializeComponent();
            CargarConceptosDePago();
            CargarProveedores();

            cbEstado.Items.AddRange(new string[] { "Pendiente", "Pagado" });
            cbEstado.SelectedIndex = 0;
        }

        private void CargarConceptosDePago()
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = "SELECT Id, Descripcion FROM Conceptos_de_Pago";
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cbConceptoPago.DisplayMember = "Descripcion"; 
                    cbConceptoPago.ValueMember = "Id"; 
                    cbConceptoPago.DataSource = dt;
                }
            }
        }

        private void CargarProveedores()
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = "SELECT Id, Nombre FROM Proveedores";
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cbProveedor.DisplayMember = "Nombre";
                    cbProveedor.ValueMember = "Id";
                    cbProveedor.DataSource = dt;
                }
            }
        }

        public AgregarEditarDocumento(int id, string numeroDocumento, string numeroFactura, string conceptoPago, DateTime fechaDocumento, decimal monto, DateTime fechaRegistro, string proveedor, string estado)
        {
            InitializeComponent();
            CargarProveedores();
            CargarConceptosDePago();
            idDocumento = id;
            txtNumeroDocumento.Text = numeroDocumento;
            txtNumeroFactura.Text = numeroFactura;
            cbConceptoPago.Text = conceptoPago;
            dtpFechaDocumento.Value = fechaDocumento;
            txtMonto.Text = monto.ToString();
            dtpFechaRegistro.Value = fechaRegistro;
            cbProveedor.Text = proveedor;
            cbEstado.Items.AddRange(new string[] { "Pendiente", "Pagado" });
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string numeroDocumento = txtNumeroDocumento.Text.Trim();
            string numeroFactura = txtNumeroFactura.Text.Trim();
            string conceptoPago = cbConceptoPago.Text.Trim();
            DateTime fechaDocumento = dtpFechaDocumento.Value;
            decimal monto = decimal.TryParse(txtMonto.Text, out decimal m) ? m : 0;
            DateTime fechaRegistro = dtpFechaRegistro.Value;
            string proveedor = cbProveedor.Text.Trim();
            string estado = cbEstado.SelectedItem.ToString();

            if (string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(numeroFactura))
            {
                MessageBox.Show("Debe llenar todos los campos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query;

                if (idDocumento == null)
                {
                    query = "INSERT INTO Documentos_Pagar (NumeroDocumento, NumeroFactura, ConceptodePago, FechaDocumento, Monto, FechaRegistro, Proveedor, Estado) VALUES (@NumeroDocumento, @NumeroFactura, @ConceptodePago, @FechaDocumento, @Monto, @FechaRegistro, @Proveedor, @Estado)";
                }
                else
                {
                    query = "UPDATE Documentos_Pagar SET NumeroDocumento = @NumeroDocumento, NumeroFactura = @NumeroFactura, ConceptodePago = @ConceptodePago, FechaDocumento = @FechaDocumento, Monto = @Monto, FechaRegistro = @FechaRegistro, Proveedor = @Proveedor, Estado = @Estado WHERE Id = @Id";
                }

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@NumeroDocumento", numeroDocumento);
                    cmd.Parameters.AddWithValue("@NumeroFactura", numeroFactura);
                    cmd.Parameters.AddWithValue("@ConceptodePago", conceptoPago);
                    cmd.Parameters.AddWithValue("@FechaDocumento", fechaDocumento);
                    cmd.Parameters.AddWithValue("@Monto", monto);
                    cmd.Parameters.AddWithValue("@FechaRegistro", fechaRegistro);
                    cmd.Parameters.AddWithValue("@Proveedor", proveedor);
                    cmd.Parameters.AddWithValue("@Estado", estado);

                    if (idDocumento != null)
                        cmd.Parameters.AddWithValue("@Id", idDocumento);

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