cd ../
export CONN_STR='Host=localhost;Port=5432;Database=FUN;Username=postgres;Password=root'
dotnet run -c Release
read -p "Press enter to continue"