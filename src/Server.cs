using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

// 你可以使用 Console.WriteLine 來進行調試
Console.WriteLine("Logs from your program will appear here!");

// 1. 定義 server 端監聽的 port number
int port = 4221;
TcpListener server = new TcpListener(IPAddress.Any, port);

// 2. 啟動 server
server.Start();
Console.WriteLine($"Server listening on port {port}");

while (true)    // 讓 server 一直運行處理多個連接
{
    // 3. 等待並接收 client 端連接
    using (Socket socket = server.AcceptSocket())   // TCP 握手 (SYN-SYN+ACK-ACK)
    {
        Console.WriteLine("client 成功連接");

        // 4. 建立 HTTP 回應字串
        string responseString = @"HTTP/1.1 200 OK\r\n\r\n"; // 使用逐字字符串避免問題
        byte[] responseBytes = Encoding.UTF8.GetBytes(responseString);

        // 5. 發送 HTTP 回應給 client 端
        socket.Send(responseBytes);
        Console.WriteLine("已傳送 response");

        // 6. 關閉 client 連線 (這裡只處理單一 request)
        //socket.Close();
    }
}
