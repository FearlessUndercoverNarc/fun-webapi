ps aux | grep FunAPI | grep -v grep | awk '{print $2}' | sudo xargs kill