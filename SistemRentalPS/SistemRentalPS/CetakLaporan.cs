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
    public partial class CetakLaporan : Form
    {
        static string connectionString = "Data Source=DESKTOP-A1J1BDF\\SYEERA; Initial Catalog=SistemRental_PS; Integrated Security=True";
        SqlDataAdapter da;
        DataTable dtLaporan;

        RekapData listLaporan = new RekapData();

        private DateTime tglMulai;
        private DateTime tglSampai;
       
        public CetakLaporan(DateTime tglMulai, DateTime tglSampai)
        {
            InitializeComponent();
            this.tglMulai = tglMulai;
            this.tglSampai = tglSampai;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_LaporanTransaksi", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddWithValue("@tipe_ps", tipePS);
                        cmd.Parameters.AddWithValue("@tglMulai", tglMulai.Date);
                        cmd.Parameters.AddWithValue("@tglSampai", tglSampai.Date);

                        da = new SqlDataAdapter(cmd);
                        dtLaporan = new DataTable();
                        da.Fill(dtLaporan);
                    }
                }
                if (dtLaporan.Rows.Count == 0)
                {
                    MessageBox.Show("Tidak ada data untuk dicetak!", "info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                listLaporan.SetDataSource(dtLaporan);
                crystalReportViewer1.ReportSource = listLaporan;
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }

      
    }
}
