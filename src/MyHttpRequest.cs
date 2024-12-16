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

        public Dictionary<string, string>? Headers { get; set; }
        public string? Body {  get; set; }

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
                Version = requestParts.Version,
                Headers = GetHeaders(requestString),
                Body = GetBody(requestString)
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

            // Request Line（請求行）格式是: <Method> <Target> <Version>   分別為：HTTP 方法、目標路徑、版本
            return (requestParts[0].Trim(),  
                    requestParts[1].Trim(),  // 預設目標為根路徑 "/"(index.html) 
                    requestParts[2].Trim()); 
        }

        private static Dictionary<string, string> GetHeaders(string requestString)
        {
            string strHeaders = requestString.Substring(requestString.IndexOf("Host"), requestString.IndexOf("\r\n\r\n") - requestString.IndexOf("Host"));

            // 使用 "\r\n" 將 Headers 部分切割成多行
            // 每行形如 "Key: Value" 的格式，例如：
            // headers = [
            //     "Host: localhost:4221",
            //     "User-Agent: curl/8.9.1",
            //     "Accept: */*"
            // ];
            string[] headers = strHeaders.Split("\r\n");

            // 將每行解析成Key Value，並存入 Dictionary
            Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
            foreach (string header in headers)
            {
                // 按 ":" 分割每一行，並去掉多餘的空格 (Trim)
                // 例如："Host: localhost:4221" => key = "Host", value = "localhost:4221"
                var keyValues = header.Split(':');
                dicHeaders.Add(keyValues[0].Trim(), keyValues[1].Trim());
            }

            return dicHeaders;
        }

        private static string GetBody(string requestString)
        {
            return requestString.Substring(requestString.IndexOf("\r\n\r\n") + 2).Trim();
        }
    }
}
