# HTTP Server 專案

這個專案是我在 Codecrafters 線上學習平台進行的一個練習，目的是實作一個簡單的 HTTP 伺服器，學習如何處理 HTTP 請求和回應。每個練習都包括了伺服器的不同功能，並且會通過自動測試來檢查實作是否正確。

## 專案目標

在這個專案中，我學會了以下內容：
- 如何解析和處理 HTTP 請求。
- 如何根據請求返回正確的 HTTP 回應（包括標頭和回應體）。
- 如何使用自動化測試來檢查我的伺服器是否符合要求。

## 專案結構

這個專案的結構包含以下檔案：
- **`Server.cs`**：主要伺服器邏輯，負責處理請求並返回對應回應。
- **`MyHttpRequest.cs`**：處理 HTTP 請求的解析邏輯。
- **`your_program.sh`**：啟動伺服器和運行測試的腳本。
- **`README.md`**：專案說明文件。

## 如何運行

### 1. 克隆專案

首先，克隆專案到本地：
```bash
git clone https://github.com/yourusername/codecrafters-http-server.git
cd codecrafters-http-server
