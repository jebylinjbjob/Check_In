# if you want to use linux shell please comment out the line below
set shell := ["powershell.exe", "-c"]

default:
    just --fmt --unstable 2> $null
    just --list --unsorted

# 執行專案
run:
    dotnet run --project JBpunch/JBpunch.csproj

# 建構專案
build:
    dotnet build

# 重新整理相依套件
refresh:
    dotnet restore

# 清除建構產物
clean:
    dotnet clean JBpunch/JBpunch.csproj

# 格式化程式碼
fmt:
    dotnet csharpier format .

# 檢查程式碼格式是否符合規範
fmt-check:
    dotnet csharpier check .

# 執行測試
test:
    # "test WIP"

# 執行整合測試
test-integration:
    # "integration test WIP"

restore_tool:
    dotnet tool restore

# CI 流程：重新整理相依套件、建構專案、檢查程式碼格式、執行測試
ci: restore_tool
    just fmt-check
    just refresh
    just build

#   just test
#    just test-integration
# === 資料庫相關 ===

# 新增 Migration
db-add name: restore_tool
    dotnet ef migrations add {{ name }} --project JBpunch/JBpunch.csproj

# 移除最後一個 Migration
db-remove: restore_tool
    dotnet ef migrations remove --project JBpunch/JBpunch.csproj

# 套用 Migration 到資料庫
db-update: restore_tool
    dotnet ef database update --project JBpunch/JBpunch.csproj

# 回滾到指定 Migration (傳入 Migration 名稱或 0 表示回滾全部)
db-rollback target="0": restore_tool
    dotnet ef database update {{ target }} --project JBpunch/JBpunch.csproj

# 列出所有 Migrations
db-list: restore_tool
    dotnet ef migrations list --project JBpunch/JBpunch.csproj

# 產生 SQL Script (從空白到最新)
db-script: restore_tool
    dotnet ef migrations script --project JBpunch/JBpunch.csproj --idempotent -o migrations.sql
