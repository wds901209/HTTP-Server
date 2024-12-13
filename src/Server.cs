using codecrafters_http_server.src;
using System.Net;
using System.Net.Sockets;
using System.Text;

// ��ܱҰʰT���A��{������ɡA���ծɥi�H�d�ݦ��T��
Console.WriteLine("Logs from your program will appear here!");

// ��l�� TcpListener�A��ť�Ҧ����������d (IPAddress.Any) �� 4221 ��
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();

// ���ݨӦ۫Ȥ�ݪ��s�u�A�o�N�|�إߤ@�� socket ����ӥN���s�u
var socket = server.AcceptSocket(); // �T������L�{������A�Ȥ�ݳs�����\

// �ѪR���쪺 HTTP �ШD
var request = MyHttpRequest.ParseRequest(socket);
// �p�G�ШD���ġA�h�L�X������ HTTP �ШD��T
if (request != null)
{
    Console.WriteLine($"Http Method: {request.Method}");
    Console.WriteLine($"Http Target: {request.Target}");
    Console.WriteLine($"Http Version: {request.Version}");
}

string responseString;

// �P�_�ШD���ؼи��| (Target)
if (request != null)
{
    if (request.Target == "/")  // �p�G�ШD�����|�O�ڥؿ�
    {
        responseString = "HTTP/1.1 200 OK\r\n\r\n";
    }
    else
    {
        responseString = "HTTP/1.1 404 Not Found\r\n\r\n";
    }
}
else
{
    responseString = "HTTP/1.1 404 Not Found\r\n\r\n";
}
Console.WriteLine($"Response: {responseString}");

// �N�^���r���ഫ�� ASCII �s�X���줸�հ}�C
Byte[] responseBytes = Encoding.ASCII.GetBytes(responseString);

// �N�^���o�e���Ȥ��
socket.Send(responseBytes);
