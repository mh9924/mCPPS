using System;
using MySql.Data.MySqlClient;

namespace mattsCPPS {
	
	public class Database
	{
		protected static MySql.Data.MySqlClient.MySqlConnection conn;
		
		public bool connect(string host, string username, string password, string dbname){
			string connStr = "server="+host+";user="+username+";database="+dbname+";port=3306;password="+password;
			conn = new MySqlConnection(connStr);
			try {
				Console.WriteLine("Connecting to database...");
				conn.Open();
			} catch {
                Console.WriteLine("Failed to connect to the database. Check that you have properly installed and configured MySQL.");
				return false;
			}
			return true;
		}
		
		public void select(string sel, string tbl, string whr, string col) {
			
			
		}
		
	}
}