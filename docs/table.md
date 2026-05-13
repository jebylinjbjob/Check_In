資料表架構

### ClockDatas（打卡資料表）

**表名**: `ClockData`

| 欄位名稱 | 資料型別 | 必填 | 說明 |
|---------|---------|------|------|
| PunchTime | DateTime | ✓ | 打卡時間 |
| IdentityUserId | Guid? | | 使用者識別碼（外鍵） |
| GpsId | Guid? | | GPS 打卡識別碼 |


### GpsPunches（GPS 打卡記錄表）

**表名**: `GpsPunche`

| 欄位名稱 | 資料型別 | 必填 | 說明 |
|---------|---------|------|------|
| Latitude | double? | | 緯度 |
| Longitude | double? | | 經度 |
| SysGpsPunche | Guid? | | 系統 GPS 打卡點識別碼 |

## 通用欄位說明

- **Id** (Guid) - 主鍵
- **TenantId** (Guid?) - 多租戶識別碼
- **CreationTime** (DateTime) - 建立時間
- **CreatorId** (Guid?) - 建立者識別碼
- **LastModificationTime** (DateTime?) - 最後修改時間
- **LastModifierId** (Guid?) - 最後修改者識別碼
- **IsDeleted** (bool) - 是否已刪除（軟刪除）
- **DeleterId** (Guid?) - 刪除者識別碼
- **DeletionTime** (DateTime?) - 刪除時間通用欄位
Id (Guid) - 主鍵
TenantId (Guid?) - 多租戶識別碼
CreationTime (DateTime) - 建立時間
CreatorId (Guid?) - 建立者識別碼
LastModificationTime (DateTime?) - 最後修改時間
LastModifierId (Guid?) - 最後修改者識別碼
IsDeleted (bool) - 是否已刪除（軟刪除）
DeleterId (Guid?) - 刪除者識別碼
DeletionTime (DateTime?) - 刪除時間