using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExcelDataReader;

namespace SistemRentalPS
{
    
    public partial class Game : Form
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

        DAL dbLogic = new DAL();

        private SqlConnection Koneksi()
        {
            return new SqlConnection(connectionString);
        }
        public Game()
        {
            InitializeComponent();
        }

        private void simpanLog(string pesan)
        {
            dbLogic.InsertLog(pesan);
            
        }
        private void btnTambahGame_Click(object sender, EventArgs e)
        {
            if (cmb_Tipe_PS.SelectedIndex == -1)
            {
                MessageBox.Show("Pilih Tipe PS terlebih dahulu!");
                return;
            }
            if (cmbPilihUnit.SelectedValue == null)
            {
                MessageBox.Show("Pilih Unit terlebih dahulu!");
                cmbPilihUnit.Focus();
                return;
            }
            if (txtNamaGame.Text == "")
            {
                MessageBox.Show("Nama Game harus diisi");
                txtNamaGame.Focus();
                return;
            }
            if (cmbGenre.Text == "")
            {
                MessageBox.Show("Pilih Genre Game terlebih dahulu!!");
                cmbGenre.Focus();
                return;
            }
            try
            {
                using (SqlConnection conn = Koneksi())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertGamePS", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_unit", cmbPilihUnit.SelectedValue);
                        cmd.Parameters.AddWithValue("@nama_game", txtNamaGame.Text);
                        cmd.Parameters.AddWithValue("@genre", cmbGenre.Text);

                        int result = cmd.ExecuteNonQuery();

                        if (result < 0)
                        {
                            MessageBox.Show("Data Game berhasil ditambahkan");
                            ClearForm();
                            LoadComboUnit();
                        }
                        else
                        {
                            MessageBox.Show("Data gagal ditambahkan");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("SQL ERROR: " +ex.Message);
            }
            catch (Exception ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("GENERAL ERROR: "+ ex.Message);
            }
        }

        private void btnUpdateGame_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(id_game))
            {
                MessageBox.Show("Pilih data game yang akan diupdate!");
                return;
            }
            if (cmbPilihUnit.SelectedIndex == -1)
            {
                MessageBox.Show("Pilih Unit terlebih dahulu!");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNamaGame.Text))
            {
                MessageBox.Show("Nama Game harus diisi!");
                return;
            }
            if (cmbGenre.SelectedIndex == -1)
            {
                MessageBox.Show("Pilih Genre terebih dahulu!");
                return;
            }
            try
            {
                using (SqlConnection conn = Koneksi())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateGamePS", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_game", int.Parse(id_game));
                        cmd.Parameters.AddWithValue("@id_unit", cmbPilihUnit.SelectedValue);
                        cmd.Parameters.AddWithValue("@nama_game", txtNamaGame.Text);
                        cmd.Parameters.AddWithValue("@genre", cmbGenre.Text);

                        int result = cmd.ExecuteNonQuery();

                        if (result < 0)
                        {
                            MessageBox.Show("Data Unit PS berhasil diupdate");
                            ClearForm();
                            btnTampilGame.PerformClick();
                            
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
                simpanLog(ex.Message);
                MessageBox.Show("SQL ERROR: " + ex.Message);
            }
            catch (Exception ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }

        private void btnHapusGame_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(id_game))
            {
                MessageBox.Show("Piih data Game yang akan dihapus!");
                return;
            }
            DialogResult resultConfirm = MessageBox.Show(
             "Apakah anda yakin menghapus data ini?",
             "Konfirmasi",
             MessageBoxButtons.YesNo,
             MessageBoxIcon.Question);

            if (resultConfirm == DialogResult.Yes)
            {
                
                try
                {
                    using (SqlConnection conn = Koneksi())
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("sp_DeleteGamePS", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@id_game", id_game);

                            int result = cmd.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Data Unit PS berhasil dihapus");
                                ClearForm();
                                btnTampilGame.PerformClick();
                               
                            }
                            else
                            {
                                MessageBox.Show("Data gagal dihapus");
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    simpanLog(ex.Message);
                    MessageBox.Show("SQL ERROR: " + ex.Message);
                }
                catch (Exception ex)
                {
                    simpanLog(ex.Message);
                    MessageBox.Show("Terjadi kesalahan: " + ex.Message);
                }
            }
        }

        private void btnTampilGame_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = Koneksi())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_GetGameByFilter", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);

                        dgvGamee.DataSource = dt;
                    }
                }
            }
            catch (SqlException ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("SQL ERROR: " + ex.Message);
            }
            catch (Exception ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }
        private void LoadComboUnit()
        {

            using (SqlConnection conn = Koneksi())
            {
                conn.Open();
                string query = "Select distinct tipe_ps from UnitPS";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmb_Tipe_PS.DataSource = dt;
                cmb_Tipe_PS.DisplayMember = "tipe_ps";
                cmb_Tipe_PS.ValueMember = "tipe_ps";
                cmb_Tipe_PS.SelectedIndex = -1;
            }
        }
        private void ClearForm()
        {
            id_game = "";
            txtNamaGame.Clear();
            cmb_Tipe_PS.SelectedIndex = -1;
            cmbPilihUnit.DataSource = null;
            cmbGenre.SelectedIndex = -1;
        }


        private void Game_Load(object sender, EventArgs e)
        {
            dgvGamee.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvGamee.MultiSelect = false;
            dgvGamee.ReadOnly = true;
            dgvGamee.AllowUserToAddRows = false;
            dgvGamee.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            LoadComboUnit();
        }

        private void cmb_Tipe_PS_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_Tipe_PS.SelectedIndex == -1)
            {
                cmbPilihUnit.DataSource = null;
                return;
            }
            LoadComboUnitByTipe(cmb_Tipe_PS.SelectedValue.ToString());
        }

       private void LoadComboUnitByTipe(string tipe)
        {
            using (SqlConnection conn = Koneksi())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetUnitByTipe", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@tipe_ps", tipe);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbPilihUnit.DataSource = dt;
                    cmbPilihUnit.DisplayMember = "nama_unit";
                    cmbPilihUnit.ValueMember = "id_unit";
                    cmbPilihUnit.SelectedIndex = -1;
                }
            }
        }

        private void dashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dashboardcs fdashboard = new Dashboardcs();
            fdashboard.Show();
            this.Hide();
        }

        private void kelolaDataPSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataPS frmDataPS = new DataPS();
            frmDataPS.Show();
            this.Hide();
        }

        
        private void dgvGamee_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvGamee.Rows[e.RowIndex];

                id_game = row.Cells[0].Value.ToString();
                cmbPilihUnit.Text = row.Cells["Nama Unit"].Value.ToString();
                cmb_Tipe_PS.Text = row.Cells["Tipe PS"].Value.ToString();
                txtNamaGame.Text = row.Cells["Nama Game"].Value.ToString();
                cmbGenre.Text = row.Cells["Genre"].Value.ToString();

            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Excel Workbook|*.xlsx" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string filepath = ofd.FileName;

                    using (var stream = System.IO.File.Open(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true
                                }
                            });
                            DataTable dt = result.Tables[0];
                            dgvGamee.DataSource = dt;

                            btnDataBase.Enabled = true;
                            btnTambahGame.Enabled = false;
                            btnUpdateGame.Enabled = false;
                            btnHapusGame.Enabled = false;
                        }
                    }
                }
            }
        }

        private void btnDataBase_Click(object sender, EventArgs e)
        {
            DataTable dt = dgvGamee.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("Tidak ada data untuk diimportkan. Klik import dari Excel dulu ya!");
                return;
            }
            int sukses = 0;
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    string namaGame = row["Nama Game"].ToString().Trim();
                    string genre = row["genre"].ToString().Trim();
                    string idUnitStr = row["id_unit"].ToString().Trim();

                    if (string.IsNullOrEmpty(namaGame) || string.IsNullOrEmpty(idUnitStr))
                        continue;

                    if (!int.TryParse(idUnitStr, out int id_unit))
                        continue;

                    dbLogic.InsertGame(id_unit, namaGame, genre);
                    sukses++;
                }
                MessageBox.Show("Data game berhasil ditambahkan");

                btnTambahGame.Enabled = true;
                btnUpdateGame.Enabled = true;
                btnHapusGame.Enabled = true;
                btnDataBase.Enabled = false;

                btnTampilGame.PerformClick();
            }
            catch (SqlException ex)
            {
                dbLogic.InsertLog("SQL ERROR Imprt Game: " + ex.Message);
                MessageBox.Show("SQL ERROR: " + ex.Message);
            }
            catch (Exception ex)
            {
                dbLogic.InsertLog("GENERAL ERROR Import Game: " + ex.Message);
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void dgvGamee_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }
}
