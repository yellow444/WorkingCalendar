# WorkingCalendar

The "WorkingCalendar" table in the database is intended to store information about holidays and weekends in Russia. This is useful for applications that need to consider the schedule of work based on the calendar of holidays and weekends.

Here are some examples of why this might be useful:

    Employee Work Planning: Many companies and organizations consider holidays and weekends when planning employee schedules. This can be useful for automating the scheduling process and managing vacations.

    Salary Calculations: Holidays and weekends can affect employee salary calculations, especially if additional wage rates are provided on these days.

    Event Planning and Marketing Campaigns: Companies can use information about holidays and weekends to plan events, promotions, and advertising campaigns that may be related to holidays.

    Resource Planning: Some businesses, such as transportation companies or restaurants, may use information about holidays and weekends to plan resources such as staffing levels and inventory.

Thus, this table can be a useful tool for managing business processes that depend on the calendar of holidays and weekends in Russia.

## XML Calendar Data Source

The project relies on official calendar data from the
[`xmlcalendar/data`](https://github.com/xmlcalendar/data) repository. Copies of
these XML files live under `WorkingCalendar.Server/Data` and are used by the
server to seed the database with holiday and weekend information.

### Updating the Calendar Data

To pull the latest calendars:

```bash
cd WorkingCalendar.Server/Data
git clone https://github.com/xmlcalendar/data tmp-data   # first time
# or update existing copy
cd tmp-data && git pull
cp -r tmp-data/{ru,kz,by,ua,uz} .
```

### License

XMLCalendar notes that the calendars are © xmlcalendar.ru. Review their
repository or website before redistributing the data and provide attribution
when required.

## Helm deployment with database initialization

The Helm chart in `k8s/` ships with a post-install hook that seeds PostgreSQL
using the SQL found in `init.pssql/sql.sql`.

### Running the hook

Install the chart and enable the initializer:

```bash
helm install workingcalendar ./k8s \
  --set initdb.enabled=true
```

To run a custom script instead, pass it at install time:

```bash
helm install workingcalendar ./k8s \
  --set initdb.enabled=true \
  --set-file initdb.script=/path/to/your.sql
```

### Troubleshooting

Check the Job status and logs if initialization fails:

```bash
kubectl get jobs
kubectl logs job/<release-name>-db-init
```

You can also verify that the table was created:

```bash
kubectl exec -it <postgres-pod> -- \
  psql -U workingcalendar -d workingcalendar -c '\dt'
```
