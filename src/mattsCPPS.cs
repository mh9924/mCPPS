using System.Windows.Forms;
using System.Threading;
using System;

namespace mattsCPPS
{
    public static class mattsCPPS
	{
        private static mServer login;
        private static mServer game;

        public static void Main()
        {
            Application.Run(new mApp());
        }

        public static void StartServer(mApp parent)
        {
            mattsCPPS.login = new mServer("config.xml");
            mattsCPPS.login.startServ();

            Thread l = new Thread(() =>
            {
                while (mApp.started)
                {
                    try
                    {
                        mattsCPPS.login.acceptPenguins();
                    } catch
                    {
                        Console.WriteLine("[mCPPS] Server stopped.");
                        Console.WriteLine();
                        parent.serverStopEvent();
                        return;
                    }
                }
            });
            l.Start();
        }

        public static void StopServer()
        {
            mattsCPPS.login.stopServ();
        }

    }
}