$ErrorActionPreference = "Stop"
$root = "C:\Users\truon\Desktop\TuneVault"
Set-Location $root

"=== dotnet restore ===" | Out-File build.log
dotnet restore TuneVault.sln 2>&1 | Tee-Object -FilePath build.log -Append

"=== dotnet build ===" | Tee-Object -FilePath build.log -Append
dotnet build TuneVault.sln 2>&1 | Tee-Object -FilePath build.log -Append

"=== dotnet ef migrations ===" | Tee-Object -FilePath build.log -Append
dotnet ef migrations add InitialCreate --project backend\TuneVault.Infrastructure --startup-project backend\TuneVault.API --output-dir Persistence\Migrations 2>&1 | Tee-Object -FilePath build.log -Append
dotnet ef migrations add AddIndexes --project backend\TuneVault.Infrastructure --startup-project backend\TuneVault.API --output-dir Persistence\Migrations 2>&1 | Tee-Object -FilePath build.log -Append

"=== DONE ===" | Tee-Object -FilePath build.log -Append
