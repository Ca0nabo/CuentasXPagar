using System;
using System.Windows.Forms;

namespace CuentasXPagar_WinForms
{
    public partial class MenuPrincipal : Form
    {
        public MenuPrincipal()
        {
            InitializeComponent();
        }

        private void btnConceptos_Click(object sender, EventArgs e)
        {
            ConceptosPago form = new ConceptosPago();
            form.Show();
        }

        private void btnProveedores_Click(object sender, EventArgs e)
        {
            Proveedores form = new Proveedores();
            form.Show();
        }

        private void btnDocumentos_Click(object sender, EventArgs e)
        {
            DocumentosPorPagar form = new DocumentosPorPagar();
            form.Show();
        }

        private void btnSolicitudes_Click(object sender, EventArgs e)
        {
            SolicitudesDePago form = new SolicitudesDePago();
            form.Show();
        }

        private void btnReportes_Click(object sender, EventArgs e)
        {
            ConsultaCriterios form = new ConsultaCriterios();
            form.Show();
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            this.Close();
            Login login = new Login();
            login.Show();
        }
    }
}