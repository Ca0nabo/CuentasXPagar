using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Collections.Generic;

namespace CuentasXPagar_WinForms
{
    public partial class AgregarEditarSolicitud : Form
    {
        private int? idSolicitud = null;
        private Dictionary<string, string> documentosDict = new Dictionary<string, string>();

        public AgregarEditarSolicitud()
        {
            InitializeComponent();
            cbEstado.Items.AddRange(new string[] { "Pendiente", "Aprobado", "Rechazado" });
            cbEstado.SelectedIndex = 0;

            CargarDocumentos();
        }

        public AgregarEditarSolicitud(int id, string numeroDocumento, string numeroFactura, DateTime fechaSolicitud, decimal monto, string estado)
        {
            InitializeComponent();
            idSolicitud = id;
            cbEstado.Items.AddRange(new string[] { "Pendiente", "Aprobado", "Rechazado" });
            cbEstado.SelectedItem = estado;

            CargarDocumentos();

            cbNumeroDocumento.SelectedItem = numeroDocumento;
            cbNumeroFactura.SelectedItem = numeroFactura;
            dtpFechaSolicitud.Value = fechaSolicitud;
            txtMonto.Text = monto.ToString();
        }

        public AgregarEditarSolicitud(int documentoId)
        {
            InitializeComponent();
            cbEstado.Items.AddRange(new string[] { "Pendiente", "Aprobado", "Rechazado" });
            cbEstado.SelectedIndex = 0;

            CargarDocumentos();
            CargarDatosDesdeDocumento(documentoId);
        }

        private void CargarDocumentos()
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = "SELECT Id, NumeroDocumento, NumeroFactura FROM Documentos_Pagar";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string documentoId = reader["Id"].ToString();
                            string numeroDocumento = reader["NumeroDocumento"].ToString();
                            string numeroFactura = reader["NumeroFactura"].ToString();

                            cbNumeroDocumento.Items.Add(numeroDocumento);
                            cbNumeroFactura.Items.Add(numeroFactura);
                            documentosDict[numeroDocumento] = numeroFactura;
                        }
                    }
                }
            }
        }

        private void CargarDatosDesdeDocumento(int documentoId)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = "SELECT NumeroDocumento, NumeroFactura, Monto FROM DocumentosPagar WHERE Id = @DocumentoId";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@DocumentoId", documentoId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cbNumeroDocumento.SelectedItem = reader["NumeroDocumento"].ToString();
                            cbNumeroFactura.SelectedItem = reader["NumeroFactura"].ToString();
                            txtMonto.Text = reader["Monto"].ToString();
                        }
                    }
                }
            }
        }

        private void cbNumeroDocumento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (documentosDict.TryGetValue(cbNumeroDocumento.SelectedItem.ToString(), out string numeroFactura))
            {
                cbNumeroFactura.SelectedItem = numeroFactura;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cbNumeroDocumento.SelectedItem == null || cbNumeroFactura.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un número de documento y factura.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string numeroDocumento = cbNumeroDocumento.SelectedItem.ToString();
            string numeroFactura = cbNumeroFactura.SelectedItem.ToString();
            DateTime fechaSolicitud = dtpFechaSolicitud.Value;
            decimal montoPagar = decimal.TryParse(txtMonto.Text, out decimal m) ? m : 0;
            string estado = cbEstado.SelectedItem.ToString();

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query;

                if (idSolicitud == null)
                {
                    query = "INSERT INTO Solicitudes_de_Pago (NumeroDocumento, NumeroFactura, FechaSolicitud, MontoPagar, Estado) VALUES (@NumeroDocumento, @NumeroFactura, @FechaSolicitud, @MontoPagar, @Estado)";
                }
                else
                {
                    query = "UPDATE Solicitudes_de_Pago SET NumeroDocumento = @NumeroDocumento, NumeroFactura = @NumeroFactura, FechaSolicitud = @FechaSolicitud, MontoPagar = @MontoPagar, Estado = @Estado WHERE Id = @Id";
                }

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@NumeroDocumento", numeroDocumento);
                    cmd.Parameters.AddWithValue("@NumeroFactura", numeroFactura);
                    cmd.Parameters.AddWithValue("@FechaSolicitud", fechaSolicitud);
                    cmd.Parameters.AddWithValue("@MontoPagar", montoPagar);
                    cmd.Parameters.AddWithValue("@Estado", estado);

                    if (idSolicitud != null)
                        cmd.Parameters.AddWithValue("@Id", idSolicitud);

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