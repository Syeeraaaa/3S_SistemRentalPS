using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemRentalPS
{
    public partial class Dashboardcs : Form
    {
        SqlConnection conn;
        SqlCommand cmd;
        private void Koneksi()
        {
            conn = new SqlConnection(
             "Data Source=DESKTOP-A1J1BDF\\SYEERA; Initial Catalog=SistemRental_PS; Integrated Security=True"
            );
        }
        public Dashboardcs()
        {
            InitializeComponent();
            
        }

        private void dtDashboard_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void Laporan_Click(object sender, EventArgs e)
        {
            Laporan laporan = new Laporan();
            laporan.Show();
            this.Hide();

        }

        private void btnTransaksi_Click(object sender, EventArgs e)
        {
            
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                Koneksi();
                conn.Close();
                conn.Close();

                Form1 form1 = new Form1();
                form1.Show();
                this.Hide();

            }
        }
    }
}
