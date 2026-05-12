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
        SqlConnection conn;
        SqlCommand cmd;
        private void Koneksi()
        {
            conn = new SqlConnection(
             "Data Source=DESKTOP-A1J1BDF\\SYEERA; Initial Catalog=SistemRental_PS; Integrated Security=True"
            );
        }
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
                Koneksi();
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                dataGridView1.Columns.Add("Nama", "Nama");
                dataGridView1.Columns.Add("Unit", "Unit");
                dataGridView1.Columns.Add("Tanggal", "Tanggal");
                dataGridView1.Columns.Add("Mulai", "Mulai");
                dataGridView1.Columns.Add("Selesai", "Selesai");
                dataGridView1.Columns.Add("Total", "Total");

                string query = @"SELECT 
                                p.nama_pelanggan,
                                u.nama_unit,
                                t.tanggal,
                                t.jam_mulai,
                                t.jam_selesai,
                                t.total_bayar
                                FROM Transaksi t
                                JOIN Pelanggan p ON t.id_pelanggan = p.id_pelanggan
                                JOIN UnitPS u ON t.id_unit = u.id_unit
                                WHERE t.tanggal BETWEEN @tglMulai AND @tglSampai";

                cmd = new SqlCommand(query, conn);

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
                    reader["total_bayar"].ToString()
                    );
                }

                reader.Close();
                string queryTotal = "SELECT SUM(total_bayar) FROM cTransaksi WHERE tanggal BETWEEN @tglMulai AND @tglSampai";
                SqlCommand cmdTotal = new SqlCommand(queryTotal, conn);
                cmdTotal.Parameters.AddWithValue("@tglMulai", dtmDari.Value.Date);
                cmdTotal.Parameters.AddWithValue("@tglSampai", dtmSampai.Value.Date);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
         
        }

        private void label4_Click(object sender, EventArgs e)
        {
            //try
            //{
                //Koneksi();
                //conn.Open();

                //string query = "SELECT SUM(total_bayar) FROM Transaksi";
                //cmd = new SqlCommand(query, conn);
                //int jumlah = (int)cmd.ExecuteScalar();
                //label4.Text = jumlah.ToString();
                //conn.Close();
            //}
            //catch (Exception ex)
            //{
              //  MessageBox.Show(ex.Message);
            //}
        }

        
        private void TotalPendapatan() //BUAT UCP 2
        {
            try
            {

                Koneksi();  
                //using (SqlConnection conn = new SqlConnection(connectionString))
                //{
                    using (SqlCommand cmd = new SqlCommand("sp_TotalPendapatan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter outputParam = new SqlParameter("@Total", SqlDbType.Int);
                        outputParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outputParam);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        label4.Text = "Total Pendapatan:Rp. " + outputParam.Value.ToString();

                    }
                //}
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
                Koneksi();
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                string query = @"SELECT 
                            t.id_transaksi,
                            p.nama_pelanggan,
                            u.nama_unit,
                            t.tanggal,
                            t.jam_mulai,
                            t.jam_selesai,
                            t.total_bayar
                        FROM Transaksi t
                        JOIN Pelanggan p ON t.id_pelanggan = p.id_pelanggan
                        JOIN UnitPS u ON t.id_unit = u.id_unit
                        WHERE p.nama_pelanggan LIKE '%' + @search + '%'
                           OR u.nama_unit LIKE '%' + @search + '%'";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@search", txtCari.Text);

                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                dataGridView1.DataSource = dt;

                reader.Close();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Data tidak ditemukan!");
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
                Koneksi();
                conn.Open();

                TimeSpan selisih = dtmSampai.Value.Date - dtmDari.Value.Date;
                double totalHari = selisih.TotalDays;
                if (totalHari > 30)
                {
                    MessageBox.Show("Maksimal cek laporan 30 hari!", "Batas Laporan Maksimal melebihi 30 hari", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;

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
    }
}
