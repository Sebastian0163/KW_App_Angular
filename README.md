# KW_App_Angular
 Small Business KeysWine
->install docker  
--> Microsoft SQL Server
---> docker run --rm --name mssql-docker -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Pwd12345!"  -d -p 11433:1433 mcr.microsoft.com/mssql/server:2019-latest 
-> Migrations
-->dotnet ef
--->dotnet ef migrations add InitialCreate --context DbContext
---->dotnet ef database update
