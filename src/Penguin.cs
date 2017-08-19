using System.Net.Sockets;

namespace mattsCPPS {
	
	public class Penguin
	{
		public TcpClient conn;
		public mServer parent;
		public string randkey;
		
		public Penguin(TcpClient conn, mServer parent)
		{
			this.conn = conn;
			this.parent = parent;
		}
		
			
	}
	
}