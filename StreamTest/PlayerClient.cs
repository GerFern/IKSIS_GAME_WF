using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NotifyTest
{
    public class PlayerClient : StreamWRHandler
    {
        private Socket _handler;
        private Thread _thread;
        public Guid PrivateID;
        public int PublicID;
        public string Name;
        NetworkStream Stream;

        public PlayerClient(Socket socket) : base()
        {
            
            _handler = socket;
            Stream = new NetworkStream(socket);
            Reader = new BinaryReaderE(Stream);
            Writer = new BinaryWriterE(Stream);
            _thread = new Thread(Listner);
            _thread.IsBackground = true;
            _thread.Start();
        }
        //public string UserName
        //{
        //    get { return _userName; }
        //}
        private void Listner()
        {
            while (true)
            {
                try
                {
                    Recieve();
                }
                catch {
                    //Program.Server.EndClient(this);
                    return;
                }
            }
        }
        public void End()
        {
            try
            {
                _handler.Close();
                try
                {
                    _thread.Abort();
                }
                catch { } // г
            }
            catch (Exception exp) { Console.WriteLine("Error with end: {0}.", exp.Message); }
        }

        /// <summary>
        /// Обработка команд
        /// </summary>
        /// <param name="data"></param>
      
        public void Send(object value)
        {
            try
            {
                SendObject(value);
                //if (bytesSent > 0) Console.WriteLine($"Success_{bytesSent}");
            }
            catch (Exception exp)
            {
                //Console.WriteLine("Error with send command: {0}.", exp.Message);
                //Program.Server.EndClient(this);
            }
        }
    }

}
