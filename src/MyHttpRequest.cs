using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_http_server.src
{
    public class MyHttpRequest
    {
        // HTTP 請求的方法、目標路徑和版本
        public string? Method { get; set; }
        public string? Target { get; set; }
        public string? Version { get; set; }

        // 靜態方法，解析 socket 的 HTTP 請求
        public static MyHttpRequest? ParseRequest(Socket socket)
        {
            // 接收原始的請求資料
            string? requestString = Receive(socket);
            if (string.IsNullOrEmpty(requestString)) return null;

            // 解析請求內容（方法、路徑和版本）
            var requestParts = GetRequestPart(requestString);

            // 回傳 MyHttpRequest 物件，把解析出來的資料設為對應屬性
            return new MyHttpRequest
            {
                Method = requestParts.Method,
                Target = requestParts.Target,
                Version = requestParts.Version
            };
        }

        // 處理接收到的原始請求資料
        private static string? Receive(Socket socket)
        {
            string? data = "";
            while (true)
            {
                byte[] bytes = new byte[1024];
                int bytesReceivede = socket.Receive(bytes, bytes.Length, SocketFlags.None);

                // 將接收到的字節資料轉換為可讀的 ASCII 字符串
                data += Encoding.ASCII.GetString(bytes, 0, bytesReceivede);

                if (bytesReceivede <= 0 || socket.Available <= 0)
                {
                    break;
                }
            }
            Console.WriteLine($"Received data: {data}");
            return data;
        }

        // 解析原始請求字串，提取方法、目標和版本
        private static (string Method, string Target, string Version) GetRequestPart(string requestString)  // 使用 tuple 傳回多個值
        {
            string requestLine = requestString.Substring(0, requestString.IndexOf("Host"));
            Console.WriteLine($"Request Line: {requestLine}");

            var requestParts = requestLine.Split(' ');

            // 回傳三部分：HTTP 方法、目標路徑、版本
            return (requestParts[0].Trim(),  
                    requestParts[1].Trim(),  // 預設目標為根路徑 "/"(index.html) 
                    requestParts[2].Trim()); 
        }
    }
}
