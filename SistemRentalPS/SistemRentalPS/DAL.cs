using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemRentalPS
{
    internal class DAL
    {

        public static string GetConnectionString()
        {
            string connectionString = $"Data Source={GetLocalIPAddress()};Initial Catalog=SistemRental_PS;User ID=sa;Password=123;";
            return connectionString;
        }
        SqlConnection conn = new SqlConnection(GetConnectionString());
        public DataTable GetGame()
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand cmd = new SqlCommand("sp_GetGameByFilter", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }
        public void InsertGame(int id_unit, string nama_game, string genre)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlTransaction trans = conn.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_InsertGamePS", conn, trans);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_unit", id_unit);
                cmd.Parameters.AddWithValue("@nama_game", nama_game);
                cmd.Parameters.AddWithValue("@genre", genre);
                cmd.ExecuteNonQuery();

                trans.Commit();
            }
            catch (Exception)
            {
                trans.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
        }
        public void InsertLog(string message)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = "INSERT INTO LogError VALUES (GETDATE(), @pesan)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pesan", message);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public DataTable GetUnitByTipe(string tipe_ps)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_GetUnitByTipe", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tipe_ps", tipe_ps);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        public DataTable GetTipePS()
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = "SELECT DISTINCT tipe_ps FROM UnitPS";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        public static string GetLocalIPAddress()
        {
            string localIP = string.Empty;
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting local IP address: " + ex.Message);
            }
            return localIP;
        }
    }
}
