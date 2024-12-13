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
    // 等待來自客戶端的連線，這將會建立一個 socket 物件來代表此連線
    var socket = server.AcceptSocket(); // 三次握手過程完成後，客戶端連接成功

    // 解析收到的 HTTP 請求
    var request = MyHttpRequest.ParseRequest(socket);
    // 如果請求有效，印出相關的 HTTP 請求資訊
    if (request != null)
    {
        Console.WriteLine($"Http Method: {request.Method}");
        Console.WriteLine($"Http Target: {request.Target}");
        Console.WriteLine($"Http Version: {request.Version}");
    }

    string responseString;

    // 判斷請求的目標路徑 (Target)
    if (request != null)
    {
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

    // 將回應字串轉換為 ASCII 編碼的位元組陣列
    Byte[] responseBytes = Encoding.ASCII.GetBytes(responseString);

    // 將回應發送給客戶端
    socket.Send(responseBytes);

    socket.Close();
}


