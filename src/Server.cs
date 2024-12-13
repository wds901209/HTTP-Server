using codecrafters_http_server.src;
using System.Net;
using System.Net.Sockets;
using System.Text;

// 顯示啟動訊息，當程式執行時，測試時可以查看此訊息
Console.WriteLine("Logs from your program will appear here!");

// 初始化 TcpListener，監聽所有網路介面卡 (IPAddress.Any) 的 4221 埠
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();

// 等待來自客戶端的連線，這將會建立一個 socket 物件來代表此連線
var socket = server.AcceptSocket(); // 三次握手過程完成後，客戶端連接成功

// 解析收到的 HTTP 請求
var request = MyHttpRequest.ParseRequest(socket);
// 如果請求有效，則印出相關的 HTTP 請求資訊
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
    if (request.Target == "/")  // 如果請求的路徑是根目錄
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

// 將回應字串轉換為 ASCII 編碼的位元組陣列
Byte[] responseBytes = Encoding.ASCII.GetBytes(responseString);

// 將回應發送給客戶端
socket.Send(responseBytes);
