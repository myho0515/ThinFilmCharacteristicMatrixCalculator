# CLAUDE.md

## 程式碼風格與格式

### 語言與註解

* 以繁體中文編寫註解與回答，並採用臺灣用語。
* 每個類別（class）方法（method）加上用途說明。
* 增加的程式碼要加上：簡短註解

### 命名規範

比照目前專案風格

### 縮排與換行

* 使用四個空白縮排，每行不超過 100 字元。
* 所有 `if`、`else`、`for`、`while` 等區塊都必須加上大括號 `{}`。

###  錯誤處理

* 使用 `try-catch` 捕捉例外，並透過日誌記錄錯誤。
* 可能為 `null` 的物件，優先使用 `??` 或 `?.` 以避免 NullReferenceException。

## 設計原則與模式

* 遵循 SOLID 原則：單一職責（SRP）、開放封閉（OCP）、里氏替換（LSP）、介面隔離（ISP）、依賴反轉（DIP）。
* DRY（Don't Repeat Yourself）、KISS（Keep It Simple and Stupid）、YAGNI（You Aren't Gonna Need It）。
* 儘量使用組合（Composition），減少繼承（Inheritance）。

## 集合與 LINQ

* 優先使用 LINQ 方法：如 `.Where()`、`.Select()`、`.Any()`。
* 檢查集合是否有元素，使用 `.Any()`；避免 `.Count() > 0`。

## 非同步與執行緒

* 以非同步（async/await）及可取消（CancellationToken）方式實作耗時作業。
* UI 部分採用事件驅動更新，確保主執行緒（UI thread）穩定。

## KISS原則
* 程式碼盡量Keep it simple and stupid 

## 回答與提問流程

1. **回答時**：

   * 回答最後可提出「你可能還想知道」的延伸問題並直接解答。

2. **修改程式碼**：

   * 程式碼保持簡潔。

---

