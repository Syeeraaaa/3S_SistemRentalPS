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
    }
}

