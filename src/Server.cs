using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// 1. �w�qserver�ݺ�ť port number
int port = 4221;
TcpListener server = new TcpListener(IPAddress.Any, port);

// 2. �Ұ� server
server.Start();
Console.WriteLine($"Server listenting on port {port}");

while (true)    // �� server �@���]�B�z�h�ӳs��
{
    // 3. ���ݨñ��� client �ݳs��
    using (Socket socket = server.AcceptSocket())
    {
        Console.WriteLine("client ���\�s��");

        // 4. �إ� HTTP �^���r��
        string responeString = "HTTP/1.1 200 OK\r\n\r\n";
        byte[] responseBytes = Encoding.UTF8.GetBytes(responeString);   

        // 5. �o�e HTTP �^����client��
    }
}
//var socket = server.AcceptSocket(); // wait for client






