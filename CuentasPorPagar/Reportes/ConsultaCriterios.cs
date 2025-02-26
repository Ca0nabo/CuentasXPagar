using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace CuentasXPagar_WinForms
{
    public partial class ConsultaCriterios : Form
    {
        public ConsultaCriterios()
        {
            InitializeComponent();
            CargarProveedores();
            CargarEstados();
        }

        private void CargarProveedores()
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                if (conexion.State == ConnectionState.Closed)
                    conexion.Open();

                string query = "SELECT DISTINCT NumeroDocumento FROM Solicitudes_de_Pago";
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    cbProveedor.Items.Clear();
                    cbProveedor.Items.Add("Todos");
                    while (reader.Read())
                    {
                        cbProveedor.Items.Add(reader["NumeroDocumento"].ToString());
                    }
                    cbProveedor.SelectedIndex = 0;
                }
            }
        }

        private void CargarEstados()
        {
            cbEstado.Items.Clear();
            cbEstado.Items.Add("Todos");
            cbEstado.Items.Add("Pendiente");
            cbEstado.Items.Add("Aprobado");
            cbEstado.Items.Add("Rechazado");
            cbEstado.SelectedIndex = 0;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                if (conexion.State == ConnectionState.Closed)
                    conexion.Open();

                string query = "SELECT NumeroDocumento, NumeroFactura, FechaSolicitud, MontoPagar, Estado FROM Solicitudes_de_Pago WHERE 1=1";
                SqlCommand cmd = new SqlCommand { Connection = conexion };

                if (cbProveedor.SelectedIndex > 0)
                {
                    query += " AND NumeroDocumento = @Proveedor";
                    cmd.Parameters.AddWithValue("@Proveedor", cbProveedor.SelectedItem.ToString());
                }

                if (cbEstado.SelectedIndex > 0)
                {
                    query += " AND Estado = @Estado";
                    cmd.Parameters.AddWithValue("@Estado", cbEstado.SelectedItem.ToString());
                }

                // Filtro por fecha (usando el único DateTimePicker: dtpFecha)
                query += " AND FechaSolicitud = @Fecha";
                cmd.Parameters.AddWithValue("@Fecha", dtpFecha.Value.Date);

                cmd.CommandText = query;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvResultados.DataSource = dt;

                // Validar y reemplazar valores nulos en el DataGridView
                foreach (DataGridViewRow row in dgvResultados.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value == null)
                        {
                            cell.Value = ""; // Asignar un valor vacío si es nulo
                        }
                    }
                }
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (dgvResultados.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Archivo Excel|*.xlsx",
                Title = "Guardar como",
                FileName = "Reporte.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        // Crear un DataTable a partir del DataGridView
                        DataTable dt = new DataTable();

                        // Agregar columnas al DataTable
                        foreach (DataGridViewColumn col in dgvResultados.Columns)
                        {
                            dt.Columns.Add(col.HeaderText);
                        }

                        // Agregar filas al DataTable
                        foreach (DataGridViewRow row in dgvResultados.Rows)
                        {
                            DataRow dr = dt.NewRow();
                            for (int i = 0; i < dgvResultados.Columns.Count; i++)
                            {
                                dr[i] = row.Cells[i].Value?.ToString() ?? "";
                            }
                            dt.Rows.Add(dr);
                        }

                        // Agregar el DataTable a una hoja de Excel
                        var ws = wb.Worksheets.Add(dt, "Reporte");

                        // Mejorar el diseño del archivo Excel
                        var headerRange = ws.Range(ws.Cell(1, 1), ws.Cell(1, dt.Columns.Count));

                        // Formato de los encabezados
                        headerRange.Style.Fill.BackgroundColor = XLColor.Green; // Fondo negro
                        headerRange.Style.Font.FontColor = XLColor.White; // Texto blanco
                        headerRange.Style.Font.Bold = true; // Texto en negrita
                        headerRange.Style.Font.FontSize = 12; // Tamaño de fuente
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Centrar texto

                        // Ajustar el ancho de las columnas al contenido
                        ws.Columns().AdjustToContents();

                        // Agregar bordes a todas las celdas
                        var dataRange = ws.Range(ws.Cell(1, 1), ws.Cell(dt.Rows.Count + 1, dt.Columns.Count));
                        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                        // Congelar la fila de encabezados
                        ws.SheetView.Freeze(1, 0);

                        // Guardar el archivo Excel
                        wb.SaveAs(saveFileDialog.FileName);
                    }

                    MessageBox.Show("Datos exportados correctamente en Excel.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar Excel: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}