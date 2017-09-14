using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IMAP
{
    public partial class frmMain : Form
    {
        static System.Net.Sockets.TcpClient tcpc = null;
        static System.Net.Security.SslStream ssl = null;
        static byte[] dummy;
        static byte[] buffer;
        static int bytes = -1;
        static StringBuilder sb = new StringBuilder();

        public frmMain()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            //https://code.msdn.microsoft.com/windowsdesktop/Simple-IMAP-CLIENT-b249d2e6
            //http://www.4d.com/docs/CMU/CMU88858.HTM

            tcpc = new System.Net.Sockets.TcpClient("mail.datainn.co.nz", 993); ///

            ssl = new System.Net.Security.SslStream(tcpc.GetStream());
            ssl.AuthenticateAsClient("imap.gmail.com");
            receiveResponse("");

            receiveResponse("$ LOGIN " + "gregtichbon@gmail.com" + " " + "Linus007b" + "  \r\n");
            Console.Clear();

            receiveResponse("$ LIST " + "\"\"" + " \"*\"" + "\r\n");

            receiveResponse("$ SELECT INBOX\r\n");

            receiveResponse("$ STATUS INBOX (MESSAGES)\r\n");


            Console.WriteLine("enter the email number to fetch :");
            int number = int.Parse(Console.ReadLine());

            receiveResponse("$ FETCH " + number + " body[header]\r\n");
            receiveResponse("$ FETCH " + number + " body[text]\r\n");


            receiveResponse("$ LOGOUT\r\n");


        }
        static void receiveResponse(string command)
        {
            try
            {
                if (command != "")
                {
                    if (tcpc.Connected)
                    {
                        dummy = Encoding.ASCII.GetBytes(command);
                        ssl.Write(dummy, 0, dummy.Length);
                    }
                    else
                    {
                        throw new ApplicationException("TCP CONNECTION DISCONNECTED");
                    }
                }
                ssl.Flush();


                buffer = new byte[2048];
                bytes = ssl.Read(buffer, 0, 2048);
                sb.Append(Encoding.ASCII.GetString(buffer));


                Console.WriteLine(sb.ToString());
                sb = new StringBuilder();

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
