# if you want to use linux shell please comment out the line below
# set shell := ["powershell.exe", "-c"]

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
    dotnet tool restore;
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

# CI 流程：重新整理相依套件、建構專案、檢查程式碼格式、執行測試
ci:
    just refresh
    just build
    just fmt-check
#   just test
#    just test-integration
