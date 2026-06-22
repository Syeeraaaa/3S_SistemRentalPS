using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SistemRentalPS
{
    public partial class DataPS : Form
    {
        private BindingSource bindingSource = new BindingSource(); 
        private DataTable dt = new DataTable();
        //SqlConnection conn;
        SqlCommand cmd;
        SqlDataReader reader;
        string id_unit;
        string id_game;
        SqlDataAdapter da;

        private readonly string connectionString =
    "Data Source=DESKTOP-A1J1BDF\\SYEERA; Initial Catalog=SistemRental_PS; Integrated Security=True";

        private SqlConnection Koneksi()
        {
            return new SqlConnection(connectionString);
        }

        public DataPS()
        {
            InitializeComponent();
        }



        private void ClearForm()
        {
            txtNamaUnit.Clear();
            txtTipePS.Clear();
            txtHargaJam.Clear();
            cmbStatus.SelectedIndex = -1;
            txtNamaUnit.Focus();
        }
        // ------------------------
        // BTN TAMPILKAN DI UNIT
        // ------------------------
        private void btnTampilkanUnit_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        // ------------------------
        // TOMBOL TAMBAHA BUAT UNIT
        // ------------------------
        private void btnTambah_Click_1(object sender, EventArgs e)
        {
                try
                {
                using (SqlConnection conn = Koneksi())
                {
                    conn.Open();
                 
                    if (string.IsNullOrWhiteSpace(txtNamaUnit.Text))
                    {
                        MessageBox.Show("Nama Unit harus diisi");
                        txtNamaUnit.Focus();
                        return;
                    }
                    if (txtTipePS.Text == "")
                    {
                        MessageBox.Show("Tipe PS harus diisi");
                        txtTipePS.Focus();
                        return;
                    }
                    if (txtHargaJam.Text == "" || txtHargaJam.Text.Any(char.IsLetter))
                    {
                        MessageBox.Show("Harga/Jam harus diisi dengan Angka!!");
                        txtHargaJam.Focus();
                        return;
                    }
                    if (cmbStatus.Text == "")
                    {
                        MessageBox.Show("Status harus diisi");
                        cmbStatus.Focus();
                        return;
                    }
                    using (SqlCommand cmd = new SqlCommand("sp_InsertUnitPS", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@nama_unit", txtNamaUnit.Text);
                        cmd.Parameters.AddWithValue("@tipe_ps", txtTipePS.Text);
                        cmd.Parameters.AddWithValue("@harga_perjam", txtHargaJam.Text);
                        cmd.Parameters.AddWithValue("@status", cmbStatus.Text);

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Data Berhasil ditambahkan");
                            ClearForm();
                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Data Gagal ditambahkan");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }
        // ------------------------
        // TOMBOL UPDATE BUAT UNIT
        // ------------------------
        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = Koneksi())
                {

                    //Koneksi();
                    
                    conn.Open();
                    if (string.IsNullOrEmpty(id_unit))
                    {
                        MessageBox.Show("Klik pada bagian baris yang ingin diupdate!");
                        return;
                    }
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateUnitPS", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddWithValue("@id_unit", txtIDUnit.Text);
                        cmd.Parameters.AddWithValue("@nama_unit", txtNamaUnit.Text);
                        cmd.Parameters.AddWithValue("@tipe_ps", txtTipePS.Text);
                        cmd.Parameters.AddWithValue("@harga_perjam", txtHargaJam.Text);
                        cmd.Parameters.AddWithValue("@status", cmbStatus.Text);
                        cmd.Parameters.AddWithValue("@id_unit", int.Parse(id_unit));

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Data Unit PS berhasil diupdate");
                            ClearForm();
                            LoadData();
                            id_unit = "";
                        }
                        else
                        {
                            MessageBox.Show("Data gagal diupdate");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }

        // ------------------------
        // TOMBOL HAPUS BUAT UNIT
        // ------------------------
        private void btnHapus_Click_1(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = Koneksi())
                {
                    conn.Open();
                    if (string.IsNullOrEmpty(id_unit))
                    {
                        MessageBox.Show("Pilih data dulu!");
                        return;
                    }

                    DialogResult resultConfirm = MessageBox.Show(
                        "Apakah anda yakin menghapus data ini?",
                        "Konfirmasi",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (resultConfirm == DialogResult.Yes)
                    {
                        string deleteGame = "DELETE FROM Game WHERE id_unit = @id_unit";
                        SqlCommand cmdGame = new SqlCommand(deleteGame, conn);
                        cmdGame.Parameters.AddWithValue("@id_unit", id_unit);
                        cmdGame.ExecuteNonQuery();

                        //string query = "DELETE FROM UnitPS WHERE id_unit = @id_unit";

                        using (SqlCommand cmd = new SqlCommand("sp_DeleteUnitPS", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@id_unit", int.Parse(id_unit));

                            int result = cmd.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Data Unit PS berhasil dihapus");
                                ClearForm();
                                //btnTampilkanUnit.PerformClick();
                                LoadData();
                                id_unit = "";
                            }
                            else
                            {
                                MessageBox.Show("Data gagal dihapus");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }

        private void dgvUnit_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvUnit.Rows[e.RowIndex];

                id_unit = row.Cells["id_unit"].Value.ToString();
                txtNamaUnit.Text = row.Cells[1].Value.ToString();
                txtTipePS.Text = row.Cells["tipe_ps"].Value.ToString();
                txtHargaJam.Text = row.Cells["harga_perjam"].Value.ToString();
                cmbStatus.Text = row.Cells["status"].Value.ToString();

            }

        }

        private void dashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dashboardcs dasboard = new Dashboardcs();
            dasboard.Show();
            this.Hide();
        }

        private void DataPS_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'sistemRental_PSDataSet2.Game' table. You can move, or remove it, as needed.
            this.gameTableAdapter.Fill(this.sistemRental_PSDataSet2.Game);
            // TODO: This line of code loads data into the 'sistemRental_PSDataSet.UnitPS' table. You can move, or remove it, as needed.
            this.unitPSTableAdapter.Fill(this.sistemRental_PSDataSet.UnitPS);
            cmbStatus.DataSource = new string[] { "Tersedia", "Dipakai", "Maintenance" };

            dgvUnit.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUnit.MultiSelect = false;
            dgvUnit.ReadOnly = true;
            dgvUnit.AllowUserToAddRows = false;
            dgvUnit.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            bindingNavigator1.BindingSource = bindingSource;

            
            LoadData();
        }

        private void LoadData()
        {
            using (SqlConnection conn = Koneksi())
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetUnit", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        dt = new DataTable();
                        da.Fill(dt);

                        bindingSource.DataSource = dt;
                        dgvUnit.DataSource = bindingSource;

                        BindControls();

                        if (dgvUnit.Columns["id_unit"] != null)
                            dgvUnit.Columns["id_unit"].Visible = false;
                    }
                }
            }
        }

        private void BindControls()
        {
            txtNamaUnit.DataBindings.Clear();
            txtTipePS.DataBindings.Clear();
            txtHargaJam.DataBindings.Clear();
            cmbStatus.DataBindings.Clear();

            txtNamaUnit.DataBindings.Add("Text", bindingSource, "nama_unit");
            txtTipePS.DataBindings.Add("Text", bindingSource, "tipe_ps");
            txtHargaJam.DataBindings.Add("Text", bindingSource, "harga_perjam");
            cmbStatus.DataBindings.Add("Text", bindingSource, "status");
        }

        private void btnResetData_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = Koneksi())
                {
                    conn.Open();

                    string query = @"
                        IF OBJECT_ID('dbo.UnitPS_Backup') IS NOT NULL
                        BEGIN
                            DELETE FROM dbo.Transaksi;
                            DELETE FROM dbo.Game;
                            DELETE FROM dbo.UnitPS

                            INSERT INTO dbo.UnitPS
                            (nama_unit, tipe_ps, harga_perjam, status)
                            
                            SELECT
                            nama_unit, tipe_ps, harga_perjam, status
                            FROM dbo.UnitPS_Backup;
                        END";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Data berhasil direset");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reset gagal: " + ex.Message);
            }
        }

        private void btnTestInjection_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = Koneksi())
                {
                    conn.Open();

                    string query =
                        "UPDATE UnitPS SET nama_unit='HACKED' WHERE tipe_ps='" +
                        txtTipePS.Text + "'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        int result = cmd.ExecuteNonQuery();
                        MessageBox.Show(result + " baris terupdate");
                    }
                }
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void unitPSBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }
    }
}

