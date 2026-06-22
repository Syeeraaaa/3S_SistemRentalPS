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

        private SqlConnection Koneksi()
        {
            return new SqlConnection(connectionString);
        }
        public Game()
        {
            InitializeComponent();
        }


        private void dgvGamee_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvGamee.Rows[e.RowIndex];

                id_game = row.Cells["id_game"].Value.ToString();
                cmbPilihUnit.SelectedValue = row.Cells["id_unit"].Value;
                txtNamaGame.Text = row.Cells["nama_game"].Value.ToString();
                cmbGenre.Text = row.Cells["genre"].Value.ToString();

            }
        }

        private void btnTambahGame_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = Koneksi())
                {


                    if (conn.State == System.Data.ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    if (txtNamaGame.Text == "")
                    {
                        MessageBox.Show("Nama Game harus diisi");
                        txtNamaGame.Focus();
                        return;
                    }
                    if (cmbPilihUnit.SelectedValue == null)
                    {
                        MessageBox.Show("Pilih Unit terlebih dahulu!");
                        cmbPilihUnit.Focus();
                        return;
                    }
                    if (cmbGenre.Text == "")
                    {
                        MessageBox.Show("Pilih Genre Game terlebih dahulu!!");
                        cmbGenre.Focus();
                        return;
                    }

                    //string query = @"INSERT INTO Game
                    //(id_unit,nama_game,genre)
                    //VALUES
                    //(@id_unit,@nama_game,@genre)";
                    using (SqlCommand cmd = new SqlCommand("sp_InsertGamePS", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();

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
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }

        private void btnUpdateGame_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = Koneksi())
                {


                    if (conn.State == System.Data.ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    //string query = @"UPDATE Game
                    //SET 
                    //id_unit = @id_unit,
                    //nama_game = @nama_game,
                    //genre = @genre
                    //WHERE id_game = @id_game";
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateGamePS", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_game", int.Parse(id_game));
                        cmd.Parameters.AddWithValue("@id_unit", cmbPilihUnit.SelectedValue);
                        cmd.Parameters.AddWithValue("@nama_game", txtNamaGame.Text);
                        cmd.Parameters.AddWithValue("@genre", cmbGenre.Text);

                        int result = cmd.ExecuteNonQuery();
                        //MessageBox.Show("Rows affected: " + result);

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
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }

        private void btnHapusGame_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = Koneksi())
                {
                    conn.Open();

                    //if (conn.State == System.Data.ConnectionState.Closed)
                    //{
                    //conn.Open();
                    //}

                    DialogResult resultConfirm = MessageBox.Show(
                        "Apakah anda yakin menghapus data ini?",
                        "Konfirmasi",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (resultConfirm == DialogResult.Yes)
                    {
                        //string query = "DELETE FROM Game WHERE id_game = @id_game";

                        using (SqlCommand cmd = new SqlCommand("sp_DeleteGamePS", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            //cmd.Parameters.AddWithValue("@id_game", cmbPilihUnit.SelectedValue);
                            cmd.Parameters.AddWithValue("@id_game", id_game);

                            int result = cmd.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Data Unit PS berhasil dihapus");
                                ClearForm();
                                //btnHapus.PerformClick();
                                btnTampilGame_Click(sender, e);
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

        private void btnTampilGame_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = Koneksi())
                {


                    //Koneksi();
                    conn.Open();

                    string query = @"
                            SELECT 
                                g.id_game,
                                g.id_unit,
                                u.nama_unit,
                                g.nama_game,
                                g.genre
                            FROM Game g
                            LEFT JOIN UnitPS u ON g.id_unit = u.id_unit
                            ";
                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        // SqlDataAdapter da = new SqlDataAdapter(query, conn);

                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgvGamee.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
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
                    }
                }
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
            cmbGenre.SelectedIndex = -1;
            cmbPilihUnit.SelectedIndex = -1;
            cmb_Tipe_PS.SelectedIndex = = -1;
            txtNamaGame.Focus();
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
    }
}
