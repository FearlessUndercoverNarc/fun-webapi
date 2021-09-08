cd ../
export CONN_STR='Host=localhost;Port=5432;Database=FUN;Username=postgres;Password=root'
dotnet ef migrations add ProdMerge -o Data/Migrations --configuration Release
read -p "Press enter to continue"