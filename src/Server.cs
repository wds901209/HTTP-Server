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
            // ���ݨӦ۫Ȥ�ݪ��s�u�A�o�N�|�إߤ@�� socket ����ӥN���s�u
            var socket = server.AcceptSocket(); // �T������L�{������A�Ȥ�ݳs�����\

            Console.WriteLine($"Time of accepting the socket: {DateTime.Now}");

            Task.Run(() => ProcessRequest(socket));
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



void ProcessRequest(Socket socket)
{
    // �ѪR���쪺 HTTP �ШD
    var request = MyHttpRequest.ParseRequest(socket);
    // �p�G�ШD���ġA�L�X������ HTTP �ШD��T
    if (request != null)
    {
        Console.WriteLine($"Http Method: {request.Method}");
        Console.WriteLine($"Http Target: {request.Target}");
        Console.WriteLine($"Http Version: {request.Version}");
    }

    string responseString = "";

    // �P�_�ШD���ؼи��| (Target)
    if (request != null)
    {
        switch(request.Method)
        {
            case "GET":
                responseString = ProcessGet(request);
                break;
            case "POST":
                responseString = ProcessPost(request);
                break;
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

string ProcessPost(MyHttpRequest request)
{
    string responseString = string.Empty;

    if (request.Target?.ToLower().StartsWith("/files/") ?? false)  // Return file
    {
        var argv = Environment.GetCommandLineArgs();
        string filePath = GetFilePath(argv);
        var fileName = $"{filePath}{request.Target.Substring(7)}";
      
        int contentLength = int.Parse(request.Headers?["Content-Length"] ?? "0");
        string contentType = request.Headers?["Content-Type"] ?? "";
        
        string? body = request.Body;

        if(contentLength != body?.Length)
        {
            responseString = "HTTP/1.1 404 Bad Request\r\n\r\n";
        }
        else
        {
            File.WriteAllText(fileName, body);
            responseString = "HTTP/1.1 201 Created\r\n\r\n";
        }      
    }
    else
    {
        responseString = "HTTP/1.1 404 Not Found\r\n\r\n";
    }

    return responseString;
}

string ProcessGet(MyHttpRequest request)
{
    string responseString = "";

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
    else if (request.Target?.ToLower().StartsWith("/files/") ?? false)  // Return file
    {
        // ���o�Ұʵ{���ɪ��R�O�C�޼ơ]�Ҧp `--directory` �Ѽơ^�A
        // �o�|�^�Ǥ@�Ӧr��}�C�A�]�t�Ҧ��ҰʮɶǤJ���޼ơC
        // �Ҧp�A�p�G�Ұʵ{���ɶǤJ���O `--directory /tmp/`�A
        // ���� argv �|�O ["program", "--directory", "/tmp/"]�C
        var argv = Environment.GetCommandLineArgs();
        string filePath = GetFilePath(argv);  // �w�q�ɮ׸��|�A��l�Ȭ��Ŧr��

        

        // �c�ا��㪺�ɮ׸��|�A�N --directory ���w�����|�M�ШD���ɮצW�ٵ��X
        // ��ШD GET /files/pear_pineapple_raspberry_orange �ɡA�ڭ̥u�Q�n pear_pineapple_raspberry_orange
        // request.Target.Substring(7) �O�]���ڭ̻ݭn�h�� "/files/" �o�ӳ���
        var fileName = $"{filePath}{request.Target.Substring(7)}";

        Console.WriteLine($"File path: {fileName}");

        if (File.Exists(fileName))
        {
            // �p�G�ɮצs�b�A���ɮפ��e�নstring
            var content = File.ReadAllText(fileName);

            responseString = $"HTTP/1.1 200 OK\r\nContent-Type: application/octet-stream\r\nContent-Length: {content.Length}\r\n\r\n{content}";
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

    return responseString;
}

string GetFilePath(string[] argv)
{
    string filePath = string.Empty;

    // �� --directory ���޼�
    if (argv != null)
    {
        // ���N�C�@�Ӥ޼ƨӴM�� --directory�A�ñN�۹��������|��ȵ� filePath
        for (int i = 0; i < argv.Length; i++)
        {
            // �p�G��e�޼ƬO --directory�A�h�N�U�@�Ӥ޼Ƨ@���ɮ׸��|
            if (i > 0 && argv[i - 1] == "--directory")
            {
                filePath = argv[i];  // ���o�ɮץؿ����|
                break;
            }
        }
    }

    return filePath;
}