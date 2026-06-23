using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemRentalPS
{
    public partial class Laporan : Form
    {
        SqlCommand cmd;
        static string connectionString ="Data Source=DESKTOP-A1J1BDF\\SYEERA; Initial Catalog=SistemRental_PS; Integrated Security=True";
        SqlDataAdapter da;
        DataTable dtTransaksi;
        DataTable dtPendapatan;
       
        public Laporan()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnTampil_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    dataGridView1.Rows.Clear();
                    dataGridView1.Columns.Clear();

                    dataGridView1.Columns.Add("Nama", "Nama");
                    dataGridView1.Columns.Add("Unit", "Unit");
                    dataGridView1.Columns.Add("Tanggal", "Tanggal");
                    dataGridView1.Columns.Add("Mulai", "Mulai");
                    dataGridView1.Columns.Add("Selesai", "Selesai");
                    dataGridView1.Columns.Add("Total", "Total");

                    string query = "SELECT * from vmLaporan WHERE tanggal BETWEEN @tglMulai AND @tglSampai";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@tglMulai", dtmDari.Value.Date);
                    cmd.Parameters.AddWithValue("@tglSampai", dtmSampai.Value.Date);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(
                            reader["nama_pelanggan"].ToString(),
                            reader["nama_unit"].ToString(),
                            Convert.ToDateTime(reader["tanggal"]).ToShortDateString(),
                        reader["jam_mulai"].ToString(),
                        reader["jam_selesai"].ToString(),
                        //reader["total_bayar"].ToString()
                        Convert.ToDecimal(reader["total_bayar"]).ToString("N0")
                        );
                    }

                    reader.Close();

                    string queryTotal = "SELECT SUM(total_bayar) FROM Transaksi WHERE tanggal BETWEEN @tglMulai AND @tglSampai";
                    SqlCommand cmdTotal = new SqlCommand(queryTotal, conn);
                    cmdTotal.Parameters.AddWithValue("@tglMulai", dtmDari.Value.Date);
                    cmdTotal.Parameters.AddWithValue("@tglSampai", dtmSampai.Value.Date);
                    TotalPendapatan();

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
         
        }

        private void label4_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_TotalPendapatan", conn))
                    {
                        cmd.Parameters.AddWithValue("@tglMulai", dtmDari.Value.Date);
                        cmd.Parameters.AddWithValue("@tglSampai", dtmSampai.Value.Date);
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter outputPram = new SqlParameter("@Total", SqlDbType.Int);
                        outputPram.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outputPram);
                        conn.Open();
                        //if (conn.State == ConnectionState.Closed) conn.Open();
                        cmd.ExecuteNonQuery();

                        label4.Text = "Total Pendapatan: " + outputPram.Value.ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menghitung total: " + ex.Message);
            }
        }

        
        private void TotalPendapatan() //BUAT UCP 2
        {
            try
            {  
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_TotalPendapatan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@tglMulai", dtmDari.Value.Date);
                    cmd.Parameters.AddWithValue("@tglSampai", dtmSampai.Value.Date);
                        SqlParameter outputParam = new SqlParameter("@Total", SqlDbType.Int);
                        outputParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outputParam);

                        if (conn.State == ConnectionState.Closed)
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        label4.Text = "Total Pendapatan:Rp. " + outputParam.Value.ToString();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal Menghitung Total Pendapatan: " + ex.Message);
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void dashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dashboardcs dasboard = new Dashboardcs();
            dasboard.Show();
            this.Hide();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        

        private void button1_Click(object sender, EventArgs e)
        {
        
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    dataGridView1.DataSource = null;
                    dataGridView1.Columns.Clear();
                    TotalPendapatan();
                    string query = "SELECT * FROM vmLaporan WHERE nama_pelanggan LIKE @nama";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        //SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@nama", "%" + txtCari.Text + "%");

                        //SqlDataReader reader = cmd.ExecuteReader();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        //dt.Load(reader);
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;

                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        //reader.Close();

                        if (dt.Rows.Count > 0)
                        {
                            //MessageBox.Show("Data tidak ditemukan!");
                            dataGridView1.Columns["nama_pelanggan"].HeaderText = "Nama Pelanggan";
                            dataGridView1.Columns["nama_unit"].HeaderText = "Unit";
                            dataGridView1.Columns["tanggal"].HeaderText = "Tanggal";
                            dataGridView1.Columns["jam_mulai"].HeaderText = "Mulai";
                            dataGridView1.Columns["jam_selesai"].HeaderText = "Selesai";
                            dataGridView1.Columns["total_bayar"].HeaderText = "Total";
                        }
                        else
                        {
                            MessageBox.Show("Data '" + txtCari.Text + "' tidak ditemuka!");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Laporan_Load(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    TimeSpan selisih = dtmSampai.Value.Date - dtmDari.Value.Date;
                    double totalHari = selisih.TotalDays;


                    if (totalHari > 30)
                    {
                        MessageBox.Show("Maksimal cek laporan 30 hari!", "Batas Laporan Maksimal melebihi 30 hari", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;

                    }
                    btnCetak.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi Kesalahan: " + ex.Message);
            }

            TotalPendapatan();
        }

        private void dtmDari_ValueChanged(object sender, EventArgs e)
        {
            dtmSampai.MaxDate = dtmDari.Value.AddDays(30);
        }

        private void dtmSampai_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void btnCetak_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Tidak ada data untuk dicetak!");
                return;
            }
            CetakLaporan frmCetak = new CetakLaporan(
                dtmDari.Value,
                dtmSampai.Value);
            frmCetak.Show();
            this.Hide();
        }
    }
}
