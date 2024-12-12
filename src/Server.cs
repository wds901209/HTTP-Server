using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// 1. 定義server端監聽 port number
int port = 4221;
TcpListener server = new TcpListener(IPAddress.Any, port);

// 2. 啟動 server
server.Start();
Console.WriteLine($"Server listenting on port {port}");

while (true)    // 讓 server 一直跑處理多個連接
{
    // 3. 等待並接收 client 端連接
    using (Socket socket = server.AcceptSocket())
    {
        Console.WriteLine("client 成功連接");

        // 4. 建立 HTTP 回應字串
        string responeString = "HTTP/1.1 200 OK\r\n\r\n";
        byte[] responseBytes = Encoding.UTF8.GetBytes(responeString);   

        // 5. 發送 HTTP 回應給client端
    }
}
//var socket = server.AcceptSocket(); // wait for client






