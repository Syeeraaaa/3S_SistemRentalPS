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

    }
}

