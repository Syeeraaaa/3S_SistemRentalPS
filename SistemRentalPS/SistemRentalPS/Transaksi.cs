using SistemRentalPS;
using System;
using System.Data;
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

        private void LoadData()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string query = @"SELECT 
                                    t.id_transaksi,
                                    p.nama_pelanggan AS 'Nama Pelanggan',
                                    p.no_hp AS 'No HP',
                                    u.nama_unit AS 'Unit',
                                    t.jam_mulai AS 'Jam Mulai',
                                    t.jam_selesai AS 'Jam Selesai',
                                    t.total_bayar AS 'Total Bayar'
                                FROM Transaksi t
                                JOIN Pelanggan p ON t.id_pelanggan = p.id_pelanggan
                                JOIN UnitPS u ON t.id_unit = u.id_unit
                                ORDER BY t.id_transaksi DESC";

                adapter = new SqlDataAdapter(query, conn);
                dt = new DataTable();
                adapter.Fill(dt);

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

                conn.Close();
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
            
        }

        private void btnMulai_Click_1(object sender, EventArgs e)
        {

            TimeSpan durasi = dtSelesai.Value - dtMulai.Value;
            int jam = (int)Math.Ceiling(durasi.TotalHours);
            if (jam <= 0) jam = 1;
            int total = jam * 10000;
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

            cmbUnit.Items.Clear();
            cmbUnit.Items.Add("PS3");
            cmbUnit.Items.Add("PS4");
            cmbUnit.Items.Add("PS5");

            dgvTransaksi.AutoGenerateColumns = true;
            dgvTransaksi.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTransaksi.MultiSelect = false;
            dgvTransaksi.ReadOnly = true;
            dgvTransaksi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            LoadData();
        }

        private void btnSelesai_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Total Bayar: Rp " + txtTotal.Text);
        }

        private void btnSimpan_Click_1(object sender, EventArgs e)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(txtNama.Text))
                {
                    MessageBox.Show("Nama harus diisi!");
                    return;
                }

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string queryP = @"INSERT INTO Pelanggan (nama_pelanggan, no_hp)
                          OUTPUT INSERTED.id_pelanggan
                          VALUES (@nama, @nohp)";
                SqlCommand cmdP = new SqlCommand(queryP, conn);
                cmdP.Parameters.AddWithValue("@nama", txtNama.Text);
                cmdP.Parameters.AddWithValue("@nohp", txtNoHP.Text);
                int id_pelanggan = (int)cmdP.ExecuteScalar();

                int id_unit = 1;
                if (cmbUnit.Text == "PS3") id_unit = 1;
                else if (cmbUnit.Text == "PS4") id_unit = 2;
                else if (cmbUnit.Text == "PS5") id_unit = 3;

                using (SqlCommand cmdT = new SqlCommand("INSERT INTO Transaksi (id_pelanggan, id_unit, jam_mulai, jam_selesai, total_bayar) VALUES (@id_pelanggan, @id_unit, @jam_mulai, @jam_selesai, @total_bayar)", conn))
                {
                    cmdT.Parameters.AddWithValue("@id_pelanggan", id_pelanggan);
                    cmdT.Parameters.AddWithValue("@id_unit", id_unit);
                    cmdT.Parameters.AddWithValue("@jam_mulai", dtMulai.Value);
                    cmdT.Parameters.AddWithValue("@jam_selesai", dtSelesai.Value);
                    cmdT.Parameters.AddWithValue("@total_bayar", txtTotal.Text == "" ? 0 : int.Parse(txtTotal.Text));
                    cmdT.ExecuteNonQuery();
                }

                MessageBox.Show("Data berhasil disimpan!");
                conn.Close();

                txtNama.Text = "";
                txtNoHP.Text = "";
                txtTotal.Text = "";
                cmbUnit.SelectedIndex = -1;

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Simpan: " + ex.Message);
            }
        }

        private void dgvTransaksi_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvTransaksi.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvTransaksi.SelectedRows[0];

                selectedId = row.Cells[0].Value.ToString();
                txtNama.Text = row.Cells[1].Value.ToString();
                txtNoHP.Text = row.Cells[2].Value.ToString();
                cmbUnit.Text = row.Cells[3].Value.ToString();
                dtMulai.Text = row.Cells[4].Value.ToString();
                dtSelesai.Text = row.Cells[5].Value.ToString();
                txtTotal.Text = row.Cells[6].Value.ToString();
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
                cmbUnit.Text = row.Cells["Unit"].Value.ToString();
                dtMulai.Text = row.Cells["Jam Mulai"].Value.ToString();
                dtSelesai.Text = row.Cells["Jam Selesai"].Value.ToString();
                txtTotal.Text = row.Cells["Total Bayar"].Value.ToString();
            }
        }

        private void dtMulai_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtNoHP_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNama_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void txtNoHP_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedId == "")
                {
                    MessageBox.Show("Pilih data yang akan diupdate!");
                    return;
                }

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string queryP = @"UPDATE Pelanggan 
                          SET nama_pelanggan = @nama, no_hp = @nohp 
                          WHERE id_pelanggan = (SELECT id_pelanggan FROM Transaksi WHERE id_transaksi = @id)";
                SqlCommand cmdP = new SqlCommand(queryP, conn);
                cmdP.Parameters.AddWithValue("@nama", txtNama.Text);
                cmdP.Parameters.AddWithValue("@nohp", txtNoHP.Text);
                cmdP.Parameters.AddWithValue("@id", selectedId);
                cmdP.ExecuteNonQuery();

                int id_unit = 1;
                if (cmbUnit.Text == "PS3") id_unit = 1;
                else if (cmbUnit.Text == "PS4") id_unit = 2;
                else if (cmbUnit.Text == "PS5") id_unit = 3;

                using (SqlCommand cmdT = new SqlCommand("UPDATE Transaksi SET id_unit = @id_unit, jam_mulai = @jam_mulai, jam_selesai = @jam_selesai, total_bayar = @total_bayar WHERE id_transaksi = @id_transaksi", conn))
                {
                    cmdT.Parameters.AddWithValue("@id_transaksi", selectedId);
                    cmdT.Parameters.AddWithValue("@id_unit", id_unit);
                    cmdT.Parameters.AddWithValue("@jam_mulai", dtMulai.Value);
                    cmdT.Parameters.AddWithValue("@jam_selesai", dtSelesai.Value);
                    cmdT.Parameters.AddWithValue("@total_bayar", txtTotal.Text == "" ? 0 : int.Parse(txtTotal.Text));
                    cmdT.ExecuteNonQuery();
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
            catch (Exception ex)
            {
                MessageBox.Show("Error Update: " + ex.Message);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string query = @"SELECT 
                                    t.id_transaksi,
                                    p.nama_pelanggan AS 'Nama Pelanggan',
                                    p.no_hp AS 'No HP',
                                    u.nama_unit AS 'Unit',
                                    t.jam_mulai AS 'Jam Mulai',
                                    t.jam_selesai AS 'Jam Selesai',
                                    t.total_bayar AS 'Total Bayar'
                                FROM Transaksi t
                                JOIN Pelanggan p ON t.id_pelanggan = p.id_pelanggan
                                JOIN UnitPS u ON t.id_unit = u.id_unit
                                WHERE p.nama_pelanggan LIKE '%' + @search + '%'
                                ORDER BY t.id_transaksi DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@search", txtSearch.Text);

                SqlDataAdapter searchAdapter = new SqlDataAdapter(cmd);
                DataTable searchDt = new DataTable();
                searchAdapter.Fill(searchDt);

                bindingSource1.DataSource = searchDt;
                dgvTransaksi.DataSource = bindingSource1;

                if (dgvTransaksi.Columns["id_transaksi"] != null)
                {
                    dgvTransaksi.Columns["id_transaksi"].Visible = false;
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Search: " + ex.Message);
            }
        }
    }
}