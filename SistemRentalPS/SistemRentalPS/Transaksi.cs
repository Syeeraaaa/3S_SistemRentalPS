using SistemRentalPS;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace SistemRentalPS
{
    public partial class Transaksi : Form
    {
        SqlConnection conn;
        //SqlCommand cmd;
        //SqlDataReader reader;
        SqlDataAdapter adapter;
        DataTable dt;

        string selectedId = "";

        string connString = "Data Source=DESKTOP-A1J1BDF\\SYEERA; Initial Catalog=SistemRental_PS; Integrated Security=True; Encrypt=False; TrustServerCertificate=True";
        public Transaksi()
        {
            InitializeComponent();
            conn = new SqlConnection(connString);

            
        }
        private void simpanLog(string pesan)
        {
            using(SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"insert into LogError values(getdate(), @pesan)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pesan", pesan);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void LoadData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // if (conn.State == ConnectionState.Closed)
                    conn.Open();

                    string query = "SELECT * FROM vwTransaksi ORDER BY id_transaksi DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    bindingSource1.DataSource = dt;
                    dgvTransaksi.DataSource = bindingSource1;

                    if (dgvTransaksi.Columns["id_transaksi"] != null)
                    {
                        dgvTransaksi.Columns["id_transaksi"].Visible = false;
                    }

                    if (bindingNavigator1 != null)
                    {
                        bindingNavigator1.BindingSource = bindingSource1;
                    }
                }

                //conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Load Data: " + ex.Message);
            }
        }

        private void dashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dashboardcs dasboard = new Dashboardcs();
            dasboard.Show();
            this.Hide();
        }

        private void cmbUnit_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cmbUnit.SelectedIndex == -1 || cmbUnit.DataSource == null) return;
            DataRowView row = (DataRowView)cmbUnit.SelectedItem;
            if (row != null)
            {
                int harga = Convert.ToInt32(row["harga_perjam"]);
                txtTotal.Text = "Rp " + harga.ToString("NO");

                HitungTotal(harga);
            }
        }

        private void HitungTotal(int hargaPerJam)
        {
            TimeSpan durasi = dtSelesai.Value - dtMulai.Value;
            int jam = (int)Math.Ceiling(durasi.TotalHours);
            if (jam <= 0) return;

            int total = jam * hargaPerJam;
            txtTotal.Text = total.ToString();
           
        }

        private void btnMulai_Click_1(object sender, EventArgs e)
        {
            if (cmbUnit.SelectedIndex == -1)
            {
                MessageBox.Show("Pilih Unit terlebih dahulu!");
                return;
            }

            DataRowView row = (DataRowView)cmbUnit.SelectedItem;
            int hargaPerJam = Convert.ToInt32(row["harga_perjam"]);

            TimeSpan durasi = dtSelesai.Value - dtMulai.Value;
            int jam = (int)Math.Ceiling(durasi.TotalHours);
            if (jam <= 0)
            {
                MessageBox.Show("Jam selesai harus lebih besar dari jam mulai");
                return;
            }
            int total = jam * hargaPerJam;
            txtTotal.Text = total.ToString();
            MessageBox.Show("Durasi: " + jam + " jam, Total: Rp " + total);
        }

        private void Transaksi_Load(object sender, EventArgs e)
        {
            dtMulai.Format = DateTimePickerFormat.Custom;
            dtMulai.CustomFormat = "dd/MM/yyyy HH:mm";
            dtMulai.ShowUpDown = true;

            dtSelesai.Format = DateTimePickerFormat.Custom;
            dtSelesai.CustomFormat = "dd/MM/yyyy HH:mm";
            dtSelesai.ShowUpDown = true;

            //LoadComboUnit();

            dgvTransaksi.AutoGenerateColumns = true;
            dgvTransaksi.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTransaksi.MultiSelect = false;
            dgvTransaksi.ReadOnly = true;
            dgvTransaksi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            LoadComboTipe();
            LoadData();
        }

        private void btnSelesai_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Total Bayar: Rp " + txtTotal.Text);
        }

        private void btnSimpan_Click_1(object sender, EventArgs e)
        {
          if (string.IsNullOrWhiteSpace(txtNama.Text) || txtNama.Text.Any(char.IsDigit))
          {
              MessageBox.Show("Nama harus diisi dan tidak boleh mengandung angka!");
              return;
          }
          if (string.IsNullOrWhiteSpace(txtNoHP.Text) || txtNoHP.Text.Any(char.IsLetter))
          {
              MessageBox.Show("No HP harus diisi, dan hanya boleh berisi Angka!");
              return;
          }
          if (cmbTipePS.SelectedIndex == -1)
          {
              MessageBox.Show("Pilih unit terlebih dahulu!");
              return;
          }
          if (cmbUnit.SelectedIndex == -1)
          {
              MessageBox.Show("Pilih Unit terlebih dahulu !");
              return;
          }
          if (String.IsNullOrWhiteSpace(txtTotal.Text))
          {
              MessageBox.Show("Klik tombol Mulai untuk menghitung");
              return;
          }
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        int id_unit = (int)cmbUnit.SelectedValue;

                        using (SqlCommand cmdT = new SqlCommand("sp_InsertTransaksi", conn,trans))
                        {
                            cmdT.CommandType = CommandType.StoredProcedure;
                            cmdT.Parameters.AddWithValue("@nama", txtNama.Text);
                            cmdT.Parameters.AddWithValue("@nohp", txtNoHP.Text);
                            cmdT.Parameters.AddWithValue("@id_unit", id_unit);
                            cmdT.Parameters.AddWithValue("@jam_mulai", dtMulai.Value);
                            cmdT.Parameters.AddWithValue("@jam_selesai", dtSelesai.Value);
                            cmdT.Parameters.AddWithValue("@total_bayar", int.Parse(txtTotal.Text));
                            cmdT.ExecuteNonQuery();


                        }
                        SqlCommand cmdLog = new SqlCommand(
                               @"insert into LogAktivitas (aktivitas, waktu) values (@aktivitas, GETDATE())", conn, trans);

                        cmdLog.Parameters.AddWithValue("@aktivitas", "INSERT TRANSAKSI: " + txtNama.Text);
                        cmdLog.ExecuteNonQuery();

                        trans.Commit();

                        MessageBox.Show("Data berhasil disimpan!");
                        txtNama.Text = "";
                        txtNoHP.Text = "";
                        txtTotal.Text = "";
                        cmbTipePS.SelectedIndex = -1;
                        cmbUnit.SelectedIndex = -1;

                        LoadComboTipe();
                        LoadData();
                    }
                    catch (SqlException ex)
                    {
                        trans.Rollback();

                        simpanLog("ROLLBACK INSERT : " + ex.Message);
                        MessageBox.Show("SQL ERROR : " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        simpanLog("GENERAL ERROR: " + ex.Message);
                        MessageBox.Show("ERROR SIMPAN: " + ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }


        }
        private void dgvTransaksi_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTransaksi.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvTransaksi.SelectedRows[0];

                selectedId = row.Cells["id_transaksi"].Value.ToString();
                txtNama.Text = row.Cells["Nama Pelanggan"].Value.ToString();
                txtNoHP.Text = row.Cells["No HP"].Value.ToString();
                dtMulai.Text = row.Cells["Jam Mulai"].Value.ToString();
                dtSelesai.Text = row.Cells["Jam Selesai"].Value.ToString();
                txtTotal.Text = row.Cells["Total Bayar"].Value.ToString();

                string tipe = row.Cells["Tipe PS"].Value.ToString();
                cmbTipePS.SelectedValue = tipe;

                LoadComboUnit(tipe);
                cmbUnit.Text = row.Cells["Unit"].Value.ToString();
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedId == "")
            {
                MessageBox.Show("Pilih data yang akan diupdate!");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNama.Text) || txtNama.Text.Any(char.IsDigit))
            {
                MessageBox.Show("Nama harus diisi dan tidak boleh mengandung ANGKA!!");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNoHP.Text) || txtNoHP.Text.Any(char.IsLetter))
            {
                MessageBox.Show("No HP harus diisi dan Hanya boleh berisi ANGKA!");
                return;
            }
            if (cmbUnit.SelectedIndex == -1)
            {
                MessageBox.Show("Pilih Unit terlebih dahulu!");
                return;
            }
            if (dtSelesai.Value <= dtMulai.Value)
            {
                MessageBox.Show("Jam selesai harus lebih besar dari jam mulai!");
                return;
            }
            try
            {

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    int id_unit = (int)cmbUnit.SelectedValue;

                    using (SqlCommand cmdT = new SqlCommand("sp_UpdateTransaksi", conn))
                    {
                        cmdT.CommandType = CommandType.StoredProcedure;
                        cmdT.Parameters.AddWithValue("@id_transaksi", selectedId);
                        cmdT.Parameters.AddWithValue("@id_unit", id_unit);
                        cmdT.Parameters.AddWithValue("@jam_mulai", dtMulai.Value);
                        cmdT.Parameters.AddWithValue("@jam_selesai", dtSelesai.Value);
                        cmdT.Parameters.AddWithValue("@total_bayar", txtTotal.Text == "" ? 0 : int.Parse(txtTotal.Text));
                        cmdT.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil diupdate!");
                conn.Close();

                LoadData();

                selectedId = "";
                txtNama.Text = "";
                txtNoHP.Text = "";
                txtTotal.Text = "";
                cmbUnit.SelectedIndex = -1;

            }
            catch (SqlException ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("SQL ERROR: " + ex.Message);
            }
            catch (Exception ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("Error Update: " + ex.Message);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand cmdT = new SqlCommand("sp_SearchTransaksi", conn))
                    {
                        cmdT.CommandType = CommandType.StoredProcedure;
                        cmdT.Parameters.AddWithValue("@nama", txtSearch.Text);



                        SqlDataAdapter searchAdapter = new SqlDataAdapter(cmdT);
                        DataTable searchDt = new DataTable();
                        searchAdapter.Fill(searchDt);

                        bindingSource1.DataSource = searchDt;
                        dgvTransaksi.DataSource = bindingSource1;

                        if (dgvTransaksi.Columns["id_transaksi"] != null)
                        {
                            dgvTransaksi.Columns["id_transaksi"].Visible = false;
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Search: " + ex.Message);
            }
        }

        private void LoadComboUnit(string tipe)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT id_unit, nama_unit,harga_perjam FROM UnitPS WHERE status = 'Tersedia' and tipe_ps = @tipe";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@tipe", tipe);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbUnit.DataSource = dt;
                cmbUnit.DisplayMember = "nama_unit";
                cmbUnit.ValueMember = "id_unit";
                cmbUnit.SelectedIndex = -1;
            }
        }

        private void LoadComboTipe()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT distinct tipe_ps FROM UnitPS WHERE status = 'Tersedia'";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbTipePS.DataSource = dt;
                cmbTipePS.DisplayMember = "tipe_ps";
                cmbTipePS.ValueMember = "tipe_ps";
                cmbTipePS.SelectedIndex = -1;
            }
        }private void cmbTipePS_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTipePS.SelectedIndex == -1) return;
            string tipe = cmbTipePS.SelectedValue.ToString();
            LoadComboUnit(tipe);

        }
    }
}