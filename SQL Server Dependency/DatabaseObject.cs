using System.Collections.Generic;

namespace SQL_Server_Dependency
{
    
    public class DatabaseObject
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _type;

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
        
        private string _database;

        public string Database
        {
            get { return _database; }
            set { _database = value; }
        }

        private string _instance;

        public string Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }

        public string DependentObj { get; set; }

        public string Text;

        public List<DatabaseObject> Dependence { get; set; }

        public DatabaseObject(string name, string type, string database, string instance, string text)
        {
            this.Name = name;
            this.Type = type;
            this.Database = database;
            this.Instance = instance;
            this.Text = text;
            this.Dependence = new List<DatabaseObject>();
            this.DependentObj = "";
        }

        public static bool operator ==(DatabaseObject x, DatabaseObject y)
        {
            return (x.Name == y.Name && x.Database == y.Database && x.Instance == y.Instance);
        }

        public static bool operator !=(DatabaseObject x, DatabaseObject y)
        {
            return !(x.Name == y.Name && x.Database == y.Database && x.Instance == y.Instance);
        }
    }
}
