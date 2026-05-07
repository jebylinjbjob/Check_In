# 開發筆記

協作者在開發過程中的筆記、決策紀錄與注意事項。

---
## 常用指令

> 以下指令皆透過 [justfile](justfile) 執行，需先安裝 [`just`](https://github.com/casey/just)。

### 開發

| 指令             | 說明                                 |
| ---------------- | ------------------------------------ |
| `just run`       | 啟動開發伺服器                       |
| `just build`     | 建構專案                             |
| `just clean`     | 清除建構產物                         |
| `just refresh`   | 重新整理相依套件（`dotnet restore`） |
| `just fmt`       | 格式化程式碼                         |
| `just fmt-check` | 檢查程式碼格式是否符合規範           |

### 測試

| 指令                    | 說明                                                                   |
| ----------------------- | ---------------------------------------------------------------------- |
| `just test`             | 執行單元測試                                                           |
| `just test-integration` | 執行整合測試                                                           |
| `just ci`               | CI 流程：restore → build → format check → unit test → integration test |
