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

        SqlConnection conn;
        SqlCommand cmd;
        SqlDataReader reader;
        string id_unit;

        private void Koneksi()
        {
            conn = new SqlConnection(
             "Data Source=DESKTOP-A1J1BDF\\SYEERA; Initial Catalog=SistemRental_PS; Integrated Security=True"
            );
        }

        public DataPS()
        {
            InitializeComponent();
        }
        private void btnTambah_Click(object sender, EventArgs e)
        {
            try
            {
                Koneksi();
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (txtNamaUnit.Text == "")
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
                if (txtHargaJam.Text == "")
                {
                    MessageBox.Show("Harga/Jam harus diisi");
                    txtHargaJam.Focus();
                    return;
                }
                if (cmbStatus.Text == "")
                {
                    MessageBox.Show("Status harus diisi");
                    cmbStatus.Focus();
                    return;
                }

                string query = @"INSERT INTO UnitPS
                          (id_unit, nama_unit,tipe_ps,harga_perjam,status)
                          VALUES
                          (@id_unit, @nama_unit,@tipe_ps,@harga_perjam,@status)";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id_unit", cmbPilihUnit.Text);
                cmd.Parameters.AddWithValue("@nama_unit", txtNamaUnit.Text);
                cmd.Parameters.AddWithValue("@tipe_ps", txtTipePS.Text);
                cmd.Parameters.AddWithValue("@harga_perjam", txtHargaJam.Text);
                cmd.Parameters.AddWithValue("@status", cmbStatus.Text);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Data Unit PS berhasil ditambahkan");
                    ClearForm();
                    btnTambah.PerformClick();
                }
                else
                {
                    MessageBox.Show("Data gagal ditambahkan");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                Koneksi();
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (dgvUnit.CurrentRow == null)
                {
                    MessageBox.Show("Klik paada bagian baris yang ingin diupdate!");
                    return;
                }

                string query = @"UPDATE UnitPS
                          SET nama_unit = @nama_unit,
                              tipe_ps = @tipe_ps,
                              harga_perjam = @harga_perjam,
                              status = @status
                          WHERE id_unit = @id";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@nama_unit", txtNamaUnit.Text);
                cmd.Parameters.AddWithValue("@tipe_ps", txtTipePS.Text);
                cmd.Parameters.AddWithValue("@harga_perjam", txtHargaJam.Text);
                cmd.Parameters.AddWithValue("@status", cmbStatus.Text);
                cmd.Parameters.AddWithValue("@id", dgvUnit.CurrentRow.Cells[0].Value);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Data Unit PS berhasil diupdate");
                    ClearForm();
                    btnTampilkanUnit.PerformClick();
                }
                else
                {
                    MessageBox.Show("Data gagal diupdate");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }
        private void btnHapus_Click(object sender, EventArgs e)
        {
            try
            {
                Koneksi();
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                DialogResult resultConfirm = MessageBox.Show(
                    "Apakah anda yakin menghapus data ini?",
                    "Konfirmasi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultConfirm == DialogResult.Yes)
                {
                    string query = "DELETE FROM RentalPS WHERE nama_unit = @nama_unit";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nama_unit", txtNamaUnit.Text);

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Data Unit PS berhasil dihapus");
                        ClearForm();
                        btnHapus.PerformClick();
                    }
                    else
                    {
                        MessageBox.Show("Data gagal dihapus");
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
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvUnit.Rows[e.RowIndex];

                id_unit = row.Cells[0].Value.ToString();
                txtNamaUnit.Text = row.Cells[1].Value.ToString();
                txtTipePS.Text = row.Cells["tipe_ps"].Value.ToString();
                txtHargaJam.Text = row.Cells["harga_perjam"].Value.ToString();
                cmbStatus.Text = row.Cells["status"].Value.ToString();
            }
        }
        private void ClearForm()
        {
            txtNamaUnit.Clear();
            txtTipePS.Clear();
            txtHargaJam.Clear();
            cmbStatus.SelectedIndex = -1;
            txtNamaUnit.Focus();
        }

        private void btnTambahGame_Click(object sender, EventArgs e)
        {
            try
            {
                Koneksi();
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

                string query = @"INSERT INTO Game
                          (id_unit,nama_game,genre)
                          VALUES
                          (@id_unit,@nama_game,@genre)";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id_unit", cmbPilihUnit.SelectedValue);
                cmd.Parameters.AddWithValue("@nama_game", txtNamaGame.Text);
                cmd.Parameters.AddWithValue("@genre", cmbGenre.Text);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Data Game berhasil ditambahkan");
                    ClearForm();

                }
                else
                {
                    MessageBox.Show("Data gagal ditambahkan");
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
                Koneksi();
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                string query = @"UPDATE Game
                          SET 
                              nama_game = @nama_game,
                              genre = @genre,
                          WHERE id_unit = @id_unit";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id_unit", cmbPilihUnit.SelectedValue);
                cmd.Parameters.AddWithValue("@nama_game", txtNamaGame.Text);
                cmd.Parameters.AddWithValue("@genre", cmbGenre.Text);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Data Unit PS berhasil diupdate");
                    ClearForm();
                    btnUpdateGame.PerformClick();
                }
                else
                {
                    MessageBox.Show("Data gagal diupdate");
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
                Koneksi();
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                DialogResult resultConfirm = MessageBox.Show(
                    "Apakah anda yakin menghapus data ini?",
                    "Konfirmasi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultConfirm == DialogResult.Yes)
                {
                    string query = "DELETE FROM Game WHERE id_unit = @id_unit";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id_unit", cmbPilihUnit.Text);

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Data Unit PS berhasil dihapus");
                        ClearForm();
                        btnHapus.PerformClick();
                    }
                    else
                    {
                        MessageBox.Show("Data gagal dihapus");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }

        private void btnTampilkanUnit_Click(object sender, EventArgs e)
        {
            try
            {
                Koneksi();
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                dgvUnit.Rows.Clear();
                dgvUnit.Columns.Clear();

                dgvUnit.Columns.Add("id_unit", "No");
                dgvUnit.Columns.Add("nama_unit", "Nama Unit");
                dgvUnit.Columns.Add("tipe_ps", "Tipe PS");
                dgvUnit.Columns.Add("harga_perjam", "Harga perjam");
                dgvUnit.Columns.Add("status", "Status");

                string query = "Select * from UnitPS";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dgvUnit.Rows.Add(
                        reader["id_unit"].ToString(),
                        reader["nama_unit"].ToString(),
                        reader["tipe_ps"].ToString(),
                        reader["harga_perjam"].ToString(),
                        reader["status"].ToString()
                        );
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("GAGAL MENAAMPILKAAN DATA: " + ex.Message);
            }
        }
        private void btnTampilkanGame_Click(object sender, EventArgs e)
        {
            Koneksi();
            conn.Open();

            string query = "Select * from Game";
            SqlDataAdapter da = new SqlDataAdapter(query, conn);

            DataTable dt = new DataTable();
            da.Fill(dt);

            dgvGamee.DataSource = dt;
        }
        private void dgvGamee_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvUnit.Rows[e.RowIndex];

                cmbPilihUnit.SelectedValue = row.Cells["id_unit"].Value.ToString();
                txtNamaGame.Text = row.Cells["nama_game"].Value.ToString();
                cmbGenre.Text = row.Cells["Genre"].Value.ToString();
            }

        }
        private void dgvUnit_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvUnit.Rows[e.RowIndex];


                txtNamaUnit.Text = row.Cells[1].Value.ToString();
                txtTipePS.Text = row.Cells[2].Value.ToString();
                txtHargaJam.Text = row.Cells[3].Value.ToString();
                cmbStatus.Text = row.Cells[4].Value.ToString();
            }
        }
    }
}

