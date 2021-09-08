# !!! WARNING !!!
# NEVER DO THIS UNLESS YOU REALLY UNDERSTAND WHAT YOU ARE DOING

# https://stackoverflow.com/a/31938017

echo -n "YOU ARE ABOUT TO DROP PRODUCTION DATABASE"
echo -n "PLEASE VERIFY IT'S REALLY NECESSARY"
DEFAULT="y"
read -e -p "Proceed [Y/n/q]: " PROCEED
# adopt the default, if 'enter' given
PROCEED="${PROCEED:-${DEFAULT}}"
# change to lower case to simplify following if
PROCEED="${PROCEED,,}"
# condition for specific letter
if [ "${PROCEED}" == "q" ] ; then
  echo "Quitting"
  exit
# condition for non specific letter (ie anything other than q/y)
# if you want to have the active 'y' code in the last section
elif [ "${PROCEED}" != "y" ] ; then
  echo "Not Proceeding"
else
  echo "Proceeding"
  # do proceeding code in here
  cd ../
  export CONN_STR='Host=localhost;Port=55432;Database=FUN;Username=postgres;Password=root'
  echo y | dotnet ef database drop --configuration Release
  read -p "Press enter to continue"
fi