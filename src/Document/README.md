# 使用 `TcpListener` 接受連線

在 C# 中，`TcpListener` 類別用於建立和管理 TCP 連線。`AcceptSocket` 和 `AcceptTcpClient` 方法是用來等待並接受進來的連線。這些方法會阻塞執行，直到有連線請求進來。

## 方法說明

- `AcceptSocket()`：接受一個傳入的 TCP 連線，並返回一個 `Socket` 物件，表示這個連線的終端。
- `AcceptTcpClient()`：接受一個傳入的 TCP 連線，並返回一個 `TcpClient` 物件，表示這個連線的終端。

### 參考文件

- [AcceptTcpClient 方法](https://learn.microsoft.com/zh-tw/dotnet/api/system.net.sockets.tcplistener.accepttcpclient?view=net-9.0#system-net-sockets-tcplistener-accepttcpclient)