build-api:
	dotnet build src/Api/Api.csproj

watch-api:
	dotnet watch --project src/Api/Api.csproj

migration:
	dotnet ef migrations add $(name) --context AppDbContext -o Infrastructure/Database/Migrations --project src/Api/Api.csproj

migration-remove:
	dotnet ef migrations remove --context AppDbContext --project src/Api/Api.csproj

migrate-up:
	dotnet ef database update --context AppDbContext --project src/Api/Api.csproj

migrate-down:
	dotnet ef database update --context AppDbContext $(target) --project src/Api/Api.csproj
	