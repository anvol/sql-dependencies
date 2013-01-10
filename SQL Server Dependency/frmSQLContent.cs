using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Server_Dependency
{
    public partial class frmSQLContent : Form
    {
        public frmSQLContent(string title, string sql)
        {
            InitializeComponent();
            Text = title;
            richTextBox.Text = sql;
        }
    }
}
