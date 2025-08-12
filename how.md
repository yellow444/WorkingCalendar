docker build  --no-cache -t yellow444/workingcalendarserver:latest -f .\WorkingCalendar.Server\Dockerfile .

docker push yellow444/workingcalendarserver:latest
