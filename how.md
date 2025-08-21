docker build  --no-cache -t yellow444/workingcalendarserver:latest -f .\WorkingCalendar.Server\Dockerfile .

docker push yellow444/workingcalendarserver:latest

## XML Calendar Data

The project uses XML calendars sourced from
[`xmlcalendar/data`](https://github.com/xmlcalendar/data). The files under
`WorkingCalendar.Server/Data` are copied from that repository and are consumed by
`WorkingCalendar.Server` to populate the database with information about
weekends and holidays.

### Updating the Data

To refresh to the latest calendars, clone or pull the upstream repository and
replace the country folders (`ru`, `kz`, `by`, `ua`, `uz`, etc.) in
`WorkingCalendar.Server/Data`:

```bash
cd WorkingCalendar.Server/Data
git clone https://github.com/xmlcalendar/data tmp-data   # first time
# or, if already cloned
cd tmp-data && git pull

cp -r tmp-data/{ru,kz,by,ua,uz} .
```

### License

The calendars are provided by XMLCalendar (`https://xmlcalendar.ru`). The site
notes that the data is © xmlcalendar.ru and may require attribution. Review
their repository or website before redistributing the data.

