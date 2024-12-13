using codecrafters_http_server.src;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;

// ��ܱҰʰT���A��{������ɡA���ծɥi�H�d�ݦ��T��
Console.WriteLine("Logs from your program will appear here!");

TcpListener server = null;
try
{
    // ��l�� TcpListener�A��ť�Ҧ����������d (IPAddress.Any) �� 4221 ��
    server = new TcpListener(IPAddress.Any, 4221);
    server.Start();

    while (true)
    {
        if (server.Pending())
        {
            Console.WriteLine($"Time of accepting the socket: {DateTime.Now}");
            Task.Run(() => ProcessRequest());
        }
    }
}
catch(SocketException e)
{
    Console.WriteLine("SocketException: {0}", e);
}
finally
{
    server?.Stop();
}



void ProcessRequest()
{
    // ���ݨӦ۫Ȥ�ݪ��s�u�A�o�N�|�إߤ@�� socket ����ӥN���s�u
    var socket = server.AcceptSocket(); // �T������L�{������A�Ȥ�ݳs�����\

    // �ѪR���쪺 HTTP �ШD
    var request = MyHttpRequest.ParseRequest(socket);
    // �p�G�ШD���ġA�L�X������ HTTP �ШD��T
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
        if (request.Target == "/")  // �p�G���|�O�ڥؿ�
        {
            responseString = "HTTP/1.1 200 OK\r\n\r\n";
        }
        else if (request.Target?.ToLower().StartsWith("/echo/") ?? false)    // Respond body
        {
            var target = request.Target;
            string content = target.Substring(6);
            responseString = $"HTTP/1.1 200 OK\r\n" +
                             $"Content-Type: text/plain\r\n" +
                             $"Content-Length: {content.Length}\r\n\r\n" +
                             $"{content}";
        }
        else if (request.Target?.ToLower().StartsWith("/user-agent") ?? false)  // Read header
        {
            // ���o Header(dic) �� "User-Agent" ���ȡA�p�G���s�b�N�]���Ŧr��
            string userAgentValue = request.Headers?["User-Agent"] ?? "";
            responseString = $"HTTP/1.1 200 OK\r\n" +
                             $"Content-Type: text/plain\r\n" +
                             $"Content-Length: {userAgentValue.Length}\r\n\r\n" +
                             $"{userAgentValue}";
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
    Console.WriteLine($"\r\nResponse: {responseString}");

    // �N�^���r���ഫ�� ASCII �s�X���줸�հ}�C
    Byte[] responseBytes = Encoding.ASCII.GetBytes(responseString);

    // �N�^���o�e���Ȥ��
    socket.Send(responseBytes);

    socket.Close();
}


