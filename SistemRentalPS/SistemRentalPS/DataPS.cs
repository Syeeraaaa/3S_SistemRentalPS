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

       string connectionString = "Data Source=DESKTOP-A1J1BDF\\SYEERA; Initial Catalog=SistemRental_PS; Integrated Security=True";

        

        public DataPS()
        {
            InitializeComponent();
        }

        private void simpanLogPS(string pesan)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"insert into LogError values(getdate(), @pesan)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Pesan", pesan);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        private void ClearForm()
        {
            id_unit = "";
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
            

            if (string.IsNullOrWhiteSpace(txtNamaUnit.Text))
            {
                MessageBox.Show("Nama Unit harus diisi");
                txtNamaUnit.Focus();
                return;
            }
            if (!txtTipePS.Text.StartsWith("PS") ||
                txtTipePS.Text.Length <= 2 ||
                !txtTipePS.Text.Substring(2).All(char.IsDigit))
            {
                MessageBox.Show("Tipe PS harus diawali PS dan diikuti angka!");
                txtTipePS.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtHargaJam.Text) ||
                !txtHargaJam.Text.All(char.IsDigit))
            {
                MessageBox.Show("Harga/Jam harus berupa angka!");
                txtHargaJam.Focus();
                return;
            }
            if (cmbStatus.Text == "")
            {
                MessageBox.Show("Status harus diisi");
                cmbStatus.Focus();
                return;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand cmd = new SqlCommand("sp_InsertUnitPS", conn, trans))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@nama_unit", txtNamaUnit.Text);
                                cmd.Parameters.AddWithValue("@tipe_ps", txtTipePS.Text);
                                cmd.Parameters.AddWithValue("@harga_perjam", int.Parse(txtHargaJam.Text));
                                cmd.Parameters.AddWithValue("@status", cmbStatus.Text);

                                int result = cmd.ExecuteNonQuery();

                                SqlCommand cmdLog = new SqlCommand(@"insert into LogAktivitas(aktivitas,waktu) values (@aktivitas, getdate())", conn, trans);
                                cmdLog.Parameters.AddWithValue("@aktivitas", "INSERT Data PS: " + txtTipePS.Text);
                                cmdLog.ExecuteNonQuery();

                                trans.Commit();

                                if (result < 0)
                                {
                                    MessageBox.Show("Data Unit PS Berhasil ditambahkan");
                                    ClearForm();
                                    LoadData();
                                }
                                else
                                {
                                    MessageBox.Show("Data Gagal ditambahkan!");
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            trans.Rollback();
                            simpanLogPS("ROLLBACK INSERT: " + ex.Message);
                            MessageBox.Show(ex.Message);
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
            if (string.IsNullOrEmpty(id_unit))
            {
                MessageBox.Show("Klik baris data yang ingin diupdate!");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNamaUnit.Text))
            {
                MessageBox.Show("Nama Unit harus diisi!");
                return;
            }
            if (!txtTipePS.Text.StartsWith("PS") ||
                txtTipePS.Text.Length <= 2 ||
                !txtTipePS.Text.Substring(2).All(char.IsDigit))
            {
                MessageBox.Show("Tipe PS harus diawali PS dan diikuti angka!");
                txtTipePS.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtHargaJam.Text) || txtHargaJam.Text.Any(char.IsLetter))
            {
                MessageBox.Show("Harga/Jam harus diisi dengan angka!");
                return;
            }
            if (cmbStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Setatus harus dipilih!");
                return;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateUnitPS", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_unit", int.Parse(id_unit));
                        cmd.Parameters.AddWithValue("@nama_unit", txtNamaUnit.Text);
                        cmd.Parameters.AddWithValue("@tipe_ps", txtTipePS.Text);
                        cmd.Parameters.AddWithValue("@harga_perjam", txtHargaJam.Text);
                        cmd.Parameters.AddWithValue("@status", cmbStatus.Text);


                        int result = cmd.ExecuteNonQuery();

                        if (result < 0)
                        {
                            MessageBox.Show("Data Unit PS berhasil diupdate");
                            ClearForm();
                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Data gagal diupdate");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                simpanLogPS(ex.Message);
                MessageBox.Show("SQL Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                simpanLogPS(ex.Message);
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }

        // ------------------------
        // TOMBOL HAPUS BUAT UNIT
        // ------------------------
        private void btnHapus_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(id_unit))
            {
                MessageBox.Show("Pilih data yang ingin dihapus!!");
                return;
            }
            DialogResult confirm = MessageBox.Show(
                "Yaking ingin menghapus unit ini?? Data transaksi terkait juga akan terhapus!",
                "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning
                );
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string deleteGame = "Delete from Game where id_unit = @id_unit";
                        SqlCommand cmdGame = new SqlCommand(deleteGame, conn);
                        cmdGame.Parameters.AddWithValue("@id_unit", int.Parse(id_unit));
                        cmdGame.ExecuteNonQuery();

                        string deleteTransaksi = "delete from Transaksi where id_unit = @id_unit";
                        SqlCommand cmdTransaksi = new SqlCommand(deleteTransaksi, conn);
                        cmdTransaksi.Parameters.AddWithValue("@id_unit", int.Parse(id_unit));
                        cmdTransaksi.ExecuteNonQuery();

                        using (SqlCommand cmd = new SqlCommand("sp_deleteUnitPS", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@id_unit", int.Parse(id_unit));

                            int result = cmd.ExecuteNonQuery();
                            if (result < 0)
                            {
                                MessageBox.Show("Data Unit PS Berhasil dihapus!");
                                ClearForm();
                                LoadData();
                            }
                            else
                            {
                                MessageBox.Show("Data gagal dihapus!!");
                            }
                        }

                    }
                }
                catch (SqlException ex)
                {
                    simpanLogPS(ex.Message);
                    MessageBox.Show("SQL ERROR:  " + ex.Message);
                }
                catch (Exception ex)
                {
                    simpanLogPS(ex.Message);
                    MessageBox.Show("Terjadi Kesalahan: " + ex.Message);
                }
            }

        }

        private void dgvUnit_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvUnit.Rows[e.RowIndex];

                id_unit = row.Cells["id_unit"].Value.ToString();
                //txtNamaUnit.Text = row.Cells[1].Value.ToString();
                //txtTipePS.Text = row.Cells["tipe_ps"].Value.ToString();
                //txtHargaJam.Text = row.Cells["harga_perjam"].Value.ToString();
                //cmbStatus.Text = row.Cells["status"].Value.ToString();

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
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
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
                using (SqlConnection conn = new SqlConnection(connectionString))
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
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    

                    string query =
                        "UPDATE UnitPS SET nama_unit='" + txtNamaUnit.Text +"' WHERE tipe_ps='" +
                        txtTipePS.Text + "'";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Update Berhasil");
                    
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

        private void keloaDataGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Game frmGame = new Game();
            frmGame.Show();
            this.Hide();
        }
        
    }
}

