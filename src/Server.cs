using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

// �A�i�H�ϥ� Console.WriteLine �Ӷi��ո�
Console.WriteLine("Logs from your program will appear here!");

// 1. �w�q server �ݺ�ť�� port number
int port = 4221;
TcpListener server = new TcpListener(IPAddress.Any, port);

// 2. �Ұ� server
server.Start();
Console.WriteLine($"Server listening on port {port}");

while (true)    // �� server �@���B��B�z�h�ӳs��
{
    // 3. ���ݨñ��� client �ݳs��
    using (Socket socket = server.AcceptSocket())   // TCP ���� (SYN-SYN+ACK-ACK)
    {
        Console.WriteLine("client ���\�s��");

        // 4. �إ� HTTP �^���r��
        string responseString = @"HTTP/1.1 200 OK\r\n\r\n"; // �ϥγv�r�r�Ŧ��קK���D
        byte[] responseBytes = Encoding.UTF8.GetBytes(responseString);

        // 5. �o�e HTTP �^���� client ��
        socket.Send(responseBytes);
        Console.WriteLine("�w�ǰe response");

        // 6. ���� client �s�u (�o�̥u�B�z��@ request)
        //socket.Close();
    }
}
