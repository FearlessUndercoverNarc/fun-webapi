cd ../
export CONN_STR='Host=localhost;Port=5432;Database=FUN;Username=postgres;Password=root'
export IS_PRODUCTION='1'
dotnet run -c Release
read -p "Press enter to continue"