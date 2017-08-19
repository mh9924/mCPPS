using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;
using System.Collections.Generic;

namespace mattsCPPS {
	
	public static class ServerGlobals
	{
		public static bool gstarted;
		public static int gcount;
	}
	
	public class mServer
	{
		public List<Penguin> penguins = new List<Penguin>();
		public Database db;
		public XmlDocument config;
		private TcpListener listener;
		
		public mServer(string config)
		{
			ServerGlobals.gcount++;
			if (!ServerGlobals.gstarted) {
				Console.WriteLine("MattsCPPS Source (c) 2012-2015, 2017");
				ServerGlobals.gstarted = true;
			} 
			else {
				Console.WriteLine("Starting CPPS...");
			}
			this.processConfig(config);
		}
		
		public void startServ()
		{
			this.db = new Database();
			if(!this.db.connect(
				this.getConfiguration("host"),
				this.getConfiguration("username"),
				this.getConfiguration("password"),
				this.getConfiguration("dbname")
				)){
					this.stopServ();
					return;
				}
				
			this.bindServ(IPAddress.Parse(this.getConfiguration("ip")), Convert.ToInt32(this.getConfiguration("port")));
			Console.WriteLine("[mCPPS] Server running!\n");
		}
		
		private void bindServ(IPAddress ip, int port)
		{
			this.listener = new TcpListener(ip, port);
			this.listener.Start();
		}
		
		public void acceptPenguins()
		{
			TcpClient client = listener.AcceptTcpClient();
			Console.Write("[mCPPS] Accepting new client \n");
			Console.WriteLine();
			Penguin penguin = new Penguin(client, this);
			this.penguins.Add(penguin);
			NetworkStream stream = penguin.conn.GetStream();
			var peng = new Thread(() => 
			{
				while(penguin.conn.Connected){
					byte[] data = new byte[256];
					int numBytesRead = stream.Read(data, 0, data.Length);
					if (numBytesRead == 0){
						this.removePenguin(penguin);
						return;
					}
					string packet = Encoding.UTF8.GetString(data).Substring(0, numBytesRead);
					String[] p = packet.Split('\0');
					foreach(string packett in p){
						if (packett.Length > 1){
							this.handleAPacket(packett, penguin);
						}
					}
				}
			});
			peng.Start();
		}
				
			
		public void stopServ(){
			if (this.listener != null)
			{
				this.listener.Server.Close();
				this.listener.Stop();
			}
		}
		
		private void handleAPacket(string packet, Penguin penguin)
		{
			Console.WriteLine("[RCVD] " + packet+" \n");
			string start = packet.Substring(0,1);
			switch(start) {
				case "<":
					this.handleXmlPacket(packet, penguin);
					break;
				case "%":
					this.handlePlayPacket(packet, penguin);
					break;
			}
		}
		
		private void handleXmlPacket(string packet, Penguin penguin)
		{
			switch(packet) {
				case "<policy-file-request/>":
					this.writeUser("<cross-domain-policy><allow-access-from domain='*' to-ports='*' /></cross-domain-policy>", penguin);
					return;
				case "<msg t='sys'><body action='verChk' r='0'><ver v='153' /></body></msg>":
					this.writeUser("<msg t='sys'><body action='apiOK' r='0'></body></msg>", penguin);
					return;
				case "<msg t='sys'><body action='rndK' r='-1'></body></msg>":
					penguin.randkey = this.generateRandomKey();
					this.writeUser("<msg t='sys'><body action='rndK' r='-1'><k>"+penguin.randkey+"</k></body></msg>", penguin);
					return;
			}
			if (packet.Substring(0,40) == "<msg t='sys'><body action='login' r='0'>"){
				this.handleLogin(packet, penguin);
			}
			
		}

		private string generateRandomKey()
		{
			Guid j = Guid.NewGuid();
			string k = Convert.ToBase64String(j.ToByteArray());
			k = k.Replace("=","").Replace("+","").Replace("/","");
			return k;
		}

		private void handlePlayPacket(string packet, Penguin penguin)
		{
		
		}
		
		private void handleLogin(string packet, Penguin penguin)
		{
			
		}
		
		private void writeUser(string packet, Penguin penguin)
		{
			if (packet.Substring(packet.Length-1,1) != "\0"){
				packet = packet+"\0";
			}
			byte[] bytes = Encoding.ASCII.GetBytes(packet);
			NetworkStream stream = penguin.conn.GetStream();
			stream.Write(bytes, 0, packet.Length);
			Console.WriteLine("[SENT] "+packet+"\n");
		}
			
		private void processConfig(string fileName)
		{
			if (!File.Exists(fileName)) {
				Console.WriteLine("Config File {} does not exist:", fileName);
				this.stopServ();
			}
			this.config = new XmlDocument();
			this.config.Load(fileName);
			Console.WriteLine("Processed configuration files!");
		}
		
		private string getConfiguration(string tag)
		{
			return this.config.GetElementsByTagName(tag)[0].InnerXml;
		}
		
		private void removePenguin(Penguin penguin)
		{
			Console.WriteLine("[mCPPS] Removing penguin\n");
			NetworkStream stream = penguin.conn.GetStream();
			stream.Close();  /// Probably unnecessary
			penguin.conn.Close();
			int i = 0;
			foreach(Penguin penguin2 in this.penguins){
				if (penguin2 == penguin){
					this.penguins.RemoveAt(i);
					return;
				}
				i++;
			}
		}
		
	}
	
}