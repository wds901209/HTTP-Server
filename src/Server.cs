using codecrafters_http_server.src;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;

// 顯示啟動訊息，當程式執行時，測試時可以查看此訊息
Console.WriteLine("Logs from your program will appear here!");

TcpListener server = null;
try
{
    // 初始化 TcpListener，監聽所有網路介面卡 (IPAddress.Any) 的 4221 埠
    server = new TcpListener(IPAddress.Any, 4221);
    server.Start();

    while (true)
    {
        if (server.Pending())
        {
            // 等待來自客戶端的連線，這將會建立一個 socket 物件來代表此連線
            var socket = server.AcceptSocket(); // 三次握手過程完成後，客戶端連接成功

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
    // 解析收到的 HTTP 請求
    var request = MyHttpRequest.ParseRequest(socket);
    // 如果請求有效，印出相關的 HTTP 請求資訊
    if (request != null)
    {
        Console.WriteLine($"Http Method: {request.Method}");
        Console.WriteLine($"Http Target: {request.Target}");
        Console.WriteLine($"Http Version: {request.Version}");
    }

    string responseString = "";

    // 判斷請求的目標路徑 (Target)
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

    // 將回應字串轉換為 ASCII 編碼的位元組陣列
    Byte[] responseBytes = Encoding.ASCII.GetBytes(responseString);

    // 將回應發送給客戶端
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

    if (request.Target == "/")  // 如果路徑是根目錄
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
        // 取得 Header(dic) 中 "User-Agent" 的值，如果不存在就設為空字串
        string userAgentValue = request.Headers?["User-Agent"] ?? "";
        responseString = $"HTTP/1.1 200 OK\r\n" +
                         $"Content-Type: text/plain\r\n" +
                         $"Content-Length: {userAgentValue.Length}\r\n\r\n" +
                         $"{userAgentValue}";
    }
    else if (request.Target?.ToLower().StartsWith("/files/") ?? false)  // Return file
    {
        // 取得啟動程式時的命令列引數（例如 `--directory` 參數），
        // 這會回傳一個字串陣列，包含所有啟動時傳入的引數。
        // 例如，如果啟動程式時傳入的是 `--directory /tmp/`，
        // 那麼 argv 會是 ["program", "--directory", "/tmp/"]。
        var argv = Environment.GetCommandLineArgs();
        string filePath = GetFilePath(argv);  // 定義檔案路徑，初始值為空字串

        

        // 構建完整的檔案路徑，將 --directory 指定的路徑和請求的檔案名稱結合
        // 當請求 GET /files/pear_pineapple_raspberry_orange 時，我們只想要 pear_pineapple_raspberry_orange
        // request.Target.Substring(7) 是因為我們需要去除 "/files/" 這個部分
        var fileName = $"{filePath}{request.Target.Substring(7)}";

        Console.WriteLine($"File path: {fileName}");

        if (File.Exists(fileName))
        {
            // 如果檔案存在，把檔案內容轉成string
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

    // 找 --directory 的引數
    if (argv != null)
    {
        // 迭代每一個引數來尋找 --directory，並將相對應的路徑賦值給 filePath
        for (int i = 0; i < argv.Length; i++)
        {
            // 如果當前引數是 --directory，則將下一個引數作為檔案路徑
            if (i > 0 && argv[i - 1] == "--directory")
            {
                filePath = argv[i];  // 取得檔案目錄路徑
                break;
            }
        }
    }

    return filePath;
}