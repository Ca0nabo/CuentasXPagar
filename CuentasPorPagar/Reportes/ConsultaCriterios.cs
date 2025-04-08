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
            CargarListaEstados();
        }

        private void CargarListaEstados()
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

                string consultaSQL = "SELECT NumeroFactura, FechaSolicitud, MontoPagar, Estado FROM Solicitudes_de_Pago WHERE 1=1";
                SqlCommand comando = new SqlCommand { Connection = conexion };

                if (cbEstado.SelectedIndex > 0)
                {
                    consultaSQL += " AND Estado = @Estado";
                    comando.Parameters.AddWithValue("@Estado", cbEstado.SelectedItem.ToString());
                }

                consultaSQL += " AND FechaSolicitud = @Fecha";
                comando.Parameters.AddWithValue("@Fecha", dtpFecha.Value.Date);

                comando.CommandText = consultaSQL;
                SqlDataAdapter adaptador = new SqlDataAdapter(comando);
                DataTable tablaResultados = new DataTable();
                adaptador.Fill(tablaResultados);
                dgvResultados.DataSource = tablaResultados;

                foreach (DataGridViewRow fila in dgvResultados.Rows)
                {
                    foreach (DataGridViewCell celda in fila.Cells)
                    {
                        if (celda.Value == null)
                        {
                            celda.Value = "";
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

            SaveFileDialog dialogoGuardar = new SaveFileDialog
            {
                Filter = "Archivo Excel|*.xlsx",
                Title = "Guardar como",
                FileName = "Reporte.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (dialogoGuardar.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (XLWorkbook libroExcel = new XLWorkbook())
                    {
                        DataTable tablaDatos = new DataTable();
                        foreach (DataGridViewColumn columna in dgvResultados.Columns)
                        {
                            tablaDatos.Columns.Add(columna.HeaderText);
                        }

                        foreach (DataGridViewRow fila in dgvResultados.Rows)
                        {
                            DataRow nuevaFila = tablaDatos.NewRow();
                            for (int i = 0; i < dgvResultados.Columns.Count; i++)
                            {
                                nuevaFila[i] = fila.Cells[i].Value?.ToString() ?? "";
                            }
                            tablaDatos.Rows.Add(nuevaFila);
                        }

                        var hojaExcel = libroExcel.Worksheets.Add(tablaDatos, "Reporte");
                        var rangoEncabezado = hojaExcel.Range(hojaExcel.Cell(1, 1), hojaExcel.Cell(1, tablaDatos.Columns.Count));

                        rangoEncabezado.Style.Fill.BackgroundColor = XLColor.Green;
                        rangoEncabezado.Style.Font.FontColor = XLColor.White;
                        rangoEncabezado.Style.Font.Bold = true;
                        rangoEncabezado.Style.Font.FontSize = 12;
                        rangoEncabezado.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        hojaExcel.Columns().AdjustToContents();

                        var rangoDatos = hojaExcel.Range(hojaExcel.Cell(1, 1), hojaExcel.Cell(tablaDatos.Rows.Count + 1, tablaDatos.Columns.Count));
                        rangoDatos.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        rangoDatos.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                        hojaExcel.SheetView.Freeze(1, 0);

                        libroExcel.SaveAs(dialogoGuardar.FileName);
                    }

                    MessageBox.Show("Datos exportados correctamente en Excel.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception error)
                {
                    MessageBox.Show("Error al exportar Excel: " + error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}