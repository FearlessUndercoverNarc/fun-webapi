cd /home/memcards/fun/webapi
export CONN_STR='Host=localhost;Port=5432;Database=FUN;Username=postgres;Password=root'
export ASPNETCORE_ENVIRONMENT='Development'
dotnet FunAPI.dll &>asp_log.txt