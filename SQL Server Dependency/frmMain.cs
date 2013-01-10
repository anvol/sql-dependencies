using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SQL_Server_Dependency
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void buttonGetObjects_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            var objects = GetAndShowObjects(checkBoxExcludeSysDBs.Checked);
            AnalyzeObjects(objects, textBoxSearchTerm.Text);
            Cursor = Cursors.Default; // ux
        }

        private List<DatabaseObject> GetAndShowObjects(bool bExcludeSysDBs)
        {
            labelStatus.Text = "Loading... Please wait";
            Application.DoEvents();
            var list = new List<DatabaseObject>();
            foreach (var instance in textBoxConnectionString.Text.Split(';'))
            {
                var dbHelper = new DbHelper(instance.Trim());
                var t = dbHelper.GetAllObjects(bExcludeSysDBs);
                t.ForEach(item => item.Instance = instance.Trim());
                list.AddRange(t);
            }
            labelStatus.Text = string.Format("Ready. Total: {0} objects loaded.", list.Count);
            Application.DoEvents();
            return list;
        }

        private void AnalyzeObjects(List<DatabaseObject> list, string term)
        {
            labelStatus.Text = "Analyze... Please wait";
            Application.DoEvents();
            var dependency = list.FindAll(item => item.Text.IndexOf(term) > -1);
            
            // iterating...
            var count = dependency.Count;
            for (var i = 1; i <= numericUpDown1.Value; i++)
            {
                labelStatus.Text = string.Format("Analyze... Iteraction #{0}. Please wait", i);
                Application.DoEvents(); // statusBar Update
                dependency.AddRange(Depends(list, dependency)); // Add dependant objects
                if (count != dependency.Count) count = dependency.Count;
                else break;
            }
            
            dataGrid.DataSource = dependency;
            dataGrid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            labelStatus.Text = string.Format("Ready. Total: {0} objects loaded.", dependency.Count);
        }

        private IEnumerable<DatabaseObject> Depends(List<DatabaseObject> list, List<DatabaseObject> find)
        {
            var names = find.Select(i=>i.Name).ToArray<string>();
            var ret = new List<DatabaseObject>();
            foreach (var name in names)
            {
                // Find all objects that has at least one mention of any object from list
                var t = list.FindAll(item => !ret.Exists(i => i == item) && !find.Exists(i => i == item) && item.Text.IndexOf(name) > -1);
                t.ForEach(item => item.DependentObj = name);
                ret.AddRange(t);
            }
            
            return ret;
        }

        private void dataGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var list = dataGrid.DataSource as List<DatabaseObject>;
            var el = list[e.RowIndex];
            new frmSQLContent(el.Database + ": " + el.Name, el.Text).Show();
        }
    }
}
