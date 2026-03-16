@echo off

dotnet ef migrations add Init ^
  --project DataAccessLayer ^
  --startup-project PresentationLayer

dotnet ef database update ^
  --project DataAccessLayer ^
  --startup-project PresentationLayer

dotnet ef database drop ^
  --project DataAccessLayer ^
  --startup-project PresentationLayer

pause
