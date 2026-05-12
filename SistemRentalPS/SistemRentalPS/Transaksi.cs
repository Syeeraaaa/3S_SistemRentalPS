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
        SqlCommand cmd;
        SqlDataReader reader;
        SqlDataAdapter adapter;
        DataTable dt;

        string selectedId = "";

        string connString = "Data Source=DESKTOP-A1J1BDF\\SYEERA; Initial Catalog=SistemRental_PS; Integrated Security=True";

        public Transaksi()
        {
            InitializeComponent();
            conn = new SqlConnection(connString);

            this.dgvTransaksi.CellClick += new DataGridViewCellEventHandler(this.dgvTransaksi_CellContentClick);
        }

        private void LoadData()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                dgvTransaksi.Rows.Clear(); 

                string query = @"SELECT 
                    t.id_transaksi,
                    p.nama_pelanggan AS nama,
                    p.no_hp AS no_hp,
                    u.nama_unit AS unit,
                    t.jam_mulai,
                    t.jam_selesai,
                    t.total_bayar
                FROM Transaksi t
                JOIN Pelanggan p ON t.id_pelanggan = p.id_pelanggan
                JOIN UnitPS u ON t.id_unit = u.id_unit";

                adapter = new SqlDataAdapter(query, conn);
                dt = new DataTable();
                adapter.Fill(dt);



            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
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
            
            if (dtMulai.Value.TimeOfDay >= dtSelesai.Value.TimeOfDay)
            {
                MessageBox.Show("Jam Selesai harus LEBIH BESAR dari Jam Mulai!",
                                "Validasi Gagal",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            TimeSpan durasi = dtSelesai.Value - dtMulai.Value;
            int jam = (int)durasi.TotalHours;
            if (jam <= 0) jam = 1;
            if (durasi.TotalHours > 24)
            {
                MessageBox.Show("Durasi tidak boleh lebih dari 24 jam!", "Batas Durasi maximal",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                dtSelesai.Value = dtMulai.Value.AddHours(24);
                return;
            }


            int tarifPerJam = 10000;
            int total = jam * tarifPerJam;

            txtTotal.Text = total.ToString();
            MessageBox.Show("Transaksi dimulai. Durasi: " + jam + " jam");
        }

        private void Transaksi_Load(object sender, EventArgs e)
        {
            dtMulai.Format = DateTimePickerFormat.Custom;
            dtMulai.CustomFormat = "dd/MM/yyyy HH:mm";
            dtMulai.ShowUpDown = false;

            dtSelesai.Format = DateTimePickerFormat.Custom;
            dtSelesai.CustomFormat = "dd/MM/yyyy HH:mm";
            dtSelesai.ShowUpDown = false;

            cmbUnit.Items.Clear();
            cmbUnit.Items.Add("PS3");
            cmbUnit.Items.Add("PS4");
            cmbUnit.Items.Add("PS5");

           

            dgvTransaksi.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTransaksi.MultiSelect = false;
            dgvTransaksi.ReadOnly = true;
            dgvTransaksi.AllowUserToAddRows = false;
            dgvTransaksi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvTransaksi.Columns.Add("id_transaksi", "ID");
            dgvTransaksi.Columns["id_transaksi"].Visible = false;
            dgvTransaksi.Columns.Add("nama", "Nama Pelanggan");
            dgvTransaksi.Columns.Add("no_hp", "No HP");
            dgvTransaksi.Columns.Add("unit", "Unit");
            dgvTransaksi.Columns.Add("jam_mulai", "Jam Mulai");
            dgvTransaksi.Columns.Add("jam_selesai", "Jam Selesai");
            dgvTransaksi.Columns.Add("total_bayar", "Total Bayar");

            LoadData();
        }

        private void btnSelesai_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTotal.Text) || txtTotal.Text == "0")
            {
                MessageBox.Show("Tekan tombol MULAI terlebih dahulu!");
                return;
            }
            MessageBox.Show("Transaksi selesai. Total Bayar: Rp " + txtTotal.Text);
        }

        private void btnSimpan_Click_1(object sender, EventArgs e)
        {
           
            if (string.IsNullOrWhiteSpace(txtNama.Text) || txtNama.Text.Any(char.IsDigit))
            {
                MessageBox.Show("Nama Pelanggan harus diisi dan hanya boleh HURUF!");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNoHP.Text) || txtNoHP.Text.Any(char.IsLetter))
            {
                MessageBox.Show("No HP harus diisi dan hanya boleh ANGKA!");
                return;
            }

            if (string.IsNullOrWhiteSpace(cmbUnit.Text))
            {
                MessageBox.Show("Pilih Unit terlebih dahulu!");
                return;
            }

            if (string.IsNullOrEmpty(txtTotal.Text) || txtTotal.Text == "0")
            {
                MessageBox.Show("Hitung durasi dulu dengan tombol MULAI!");
                return;
            }

            
            if (dtMulai.Value.TimeOfDay >= dtSelesai.Value.TimeOfDay)
            {
                MessageBox.Show("Jam Selesai harus lebih besar dari Jam Mulai!");
                return;
            }

            try
            {
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

                
                string queryT = @"INSERT INTO Transaksi
                (id_pelanggan, id_unit, tanggal, jam_mulai, jam_selesai, total_bayar)
                VALUES (@id_pelanggan, @id_unit, @Tanggal, @Mulai, @Selesai, @Total)";

                SqlCommand cmdT = new SqlCommand(queryT, conn);
                cmdT.Parameters.AddWithValue("@id_pelanggan", id_pelanggan);
                cmdT.Parameters.AddWithValue("@id_unit", id_unit);
                cmdT.Parameters.AddWithValue("@Tanggal", DateTime.Now.Date);
                cmdT.Parameters.AddWithValue("@Mulai", dtMulai.Value.TimeOfDay);
                cmdT.Parameters.AddWithValue("@Selesai", dtSelesai.Value.TimeOfDay);
                cmdT.Parameters.AddWithValue("@Total", int.Parse(txtTotal.Text));

                cmdT.ExecuteNonQuery();

                MessageBox.Show("Data berhasil disimpan");

                conn.Close();

                
                txtNama.Clear();
                txtNoHP.Clear();
                txtTotal.Clear();
                cmbUnit.SelectedIndex = -1;
                dtMulai.Value = DateTime.Now;
                dtSelesai.Value = DateTime.Now.AddHours(1);

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void dgvTransaksi_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int i = e.RowIndex;

                selectedId = dgvTransaksi.Rows[i].Cells[0].Value.ToString();


                MessageBox.Show("ID keambil: " + selectedId);

                txtNama.Text = dgvTransaksi.Rows[i].Cells[1].Value.ToString();
                txtNoHP.Text = dgvTransaksi.Rows[i].Cells[2].Value.ToString();
                cmbUnit.Text = dgvTransaksi.Rows[i].Cells[3].Value.ToString();
                dtMulai.Value = DateTime.Parse(dgvTransaksi.Rows[i].Cells[4].Value.ToString());
                dtSelesai.Value = DateTime.Parse(dgvTransaksi.Rows[i].Cells[5].Value.ToString());
                txtTotal.Text = dgvTransaksi.Rows[i].Cells[6].Value.ToString();
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
                    MessageBox.Show("Pilih data dulu!");
                    return;
                }


                TimeSpan durasi = dtSelesai.Value - dtMulai.Value;
                int jam = (int)durasi.TotalHours;
                if (jam <= 0) jam = 1;

                int tarifPerJam = 10000;
                int total = jam * tarifPerJam;
                txtTotal.Text = total.ToString();

                conn.Open();


                string queryP = @"UPDATE Pelanggan 
                          SET nama_pelanggan=@nama, no_hp=@nohp 
                          WHERE id_pelanggan = (SELECT id_pelanggan FROM Transaksi WHERE id_transaksi=@id)";
                SqlCommand cmdP = new SqlCommand(queryP, conn);
                cmdP.Parameters.AddWithValue("@nama", txtNama.Text);
                cmdP.Parameters.AddWithValue("@nohp", txtNoHP.Text);
                cmdP.Parameters.AddWithValue("@id", selectedId);
                cmdP.ExecuteNonQuery();


                int id_unit = 1;
                if (cmbUnit.Text == "PS3") id_unit = 1;
                else if (cmbUnit.Text == "PS4") id_unit = 2;
                else if (cmbUnit.Text == "PS5") id_unit = 3;


                string queryT = @"UPDATE Transaksi 
                          SET id_unit=@unit, jam_mulai=@mulai, jam_selesai=@selesai, total_bayar=@total 
                          WHERE id_transaksi=@id";
                SqlCommand cmdT = new SqlCommand(queryT, conn);
                cmdT.Parameters.AddWithValue("@unit", id_unit);
                cmdT.Parameters.AddWithValue("@mulai", dtMulai.Value);
                cmdT.Parameters.AddWithValue("@selesai", dtSelesai.Value);
                cmdT.Parameters.AddWithValue("@total", total);
                cmdT.Parameters.AddWithValue("@id", selectedId);

                int result = cmdT.ExecuteNonQuery();
                MessageBox.Show("Row terupdate: " + result);
                MessageBox.Show("Data berhasil diupdate!");

                conn.Close();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}