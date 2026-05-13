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
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // if (conn.State == ConnectionState.Closed)
                    conn.Open();

                    string query = "SELECT * FROM vwTransaksi ORDER BY id_transaksi DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    dt = new DataTable();
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

            LoadComboUnit();

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
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    if (string.IsNullOrWhiteSpace(txtNama.Text) || txtNama.Text.Any(char.IsDigit))
                    {
                        MessageBox.Show("Nama harus diisi!");
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(txtNoHP.Text) || txtNoHP.Text.Any(char.IsLetter))
                    {
                        MessageBox.Show("No HP harus diisi, dan hanya boleh berisi ANGKA!");
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

                    int id_unit = (int)cmbUnit.SelectedValue;

                    using (SqlCommand cmdT = new SqlCommand("sp_InsertTransaksi", conn))
                    {
                        cmdT.CommandType = CommandType.StoredProcedure;
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
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    //if (conn.State == ConnectionState.Closed)
                    conn.Open();

                    string queryP = @"UPDATE Pelanggan 
                          SET nama_pelanggan = @nama, no_hp = @nohp 
                          WHERE id_pelanggan = (SELECT id_pelanggan FROM Transaksi WHERE id_transaksi = @id)";
                    SqlCommand cmdP = new SqlCommand(queryP, conn);
                    cmdP.Parameters.AddWithValue("@nama", txtNama.Text);
                    cmdP.Parameters.AddWithValue("@nohp", txtNoHP.Text);
                    cmdP.Parameters.AddWithValue("@id", selectedId);
                    cmdP.ExecuteNonQuery();

                    int id_unit = (int)cmbUnit.SelectedValue;

                    using (SqlCommand cmdT = new SqlCommand("sp_UpdateTransaksi", conn))
                    {
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

                using (SqlCommand cmdT = new SqlCommand("sp_SearchTransaksi", conn))
                {
                    cmdT.CommandType = CommandType.StoredProcedure;
                    cmdT.Parameters.AddWithValue("@search", txtSearch.Text);



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

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Search: " + ex.Message);
            }
        }

        private void LoadComboUnit()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT id_unit, nama_unit FROM UnitPS WHERE status = 'Tersedia'";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbUnit.DataSource = dt;
                cmbUnit.DisplayMember = "nama_unit";
                cmbUnit.ValueMember = "id_unit";
            }
        }
    }
}