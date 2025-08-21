# WorkingCalendar

The "WorkingCalendar" table in the database is intended to store information about holidays and weekends in Russia. This is useful for applications that need to consider the schedule of work based on the calendar of holidays and weekends.

Here are some examples of why this might be useful:

    Employee Work Planning: Many companies and organizations consider holidays and weekends when planning employee schedules. This can be useful for automating the scheduling process and managing vacations.

    Salary Calculations: Holidays and weekends can affect employee salary calculations, especially if additional wage rates are provided on these days.

    Event Planning and Marketing Campaigns: Companies can use information about holidays and weekends to plan events, promotions, and advertising campaigns that may be related to holidays.

    Resource Planning: Some businesses, such as transportation companies or restaurants, may use information about holidays and weekends to plan resources such as staffing levels and inventory.

Thus, this table can be a useful tool for managing business processes that depend on the calendar of holidays and weekends in Russia.

## Port configuration

The backend listens on port **8080**. Docker and `launchSettings.json` expose the same port, and the Vite dev server proxies API calls to it. When running locally, set

```bash
ASPNETCORE_URLS=http://localhost:8080 dotnet run
```

or export `ASPNETCORE_HTTP_PORT=8080` so that both server and client target the same address.

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

## Checking migrations in CI

To verify that Entity Framework Core migrations apply to a clean database, run
the helper script `scripts/check-migrations.sh`. The script spins up a temporary
PostgreSQL container and executes `dotnet ef database update` against it. Use in
continuous integration to catch migration issues early:

```bash
./scripts/check-migrations.sh
```

The script exits with a non-zero status if the migrations fail.

## Helm deployment with database initialization

The Helm chart in `helm/` ships with a post-install hook that seeds PostgreSQL
using the SQL found in `init.pssql/sql.sql`.

### Database configuration

The chart can either provision its own PostgreSQL instance or connect to an
external database. Set `db.connectionString` to use an external PostgreSQL
server; when this value is empty and `postgres.enabled=true`, the chart
deploys a bundled Postgres and the application reads the generated connection
string from the `ConnectionStrings__Postgres` environment variable.

### Running the hook

Install the chart and enable the initializer:

```bash
helm install workingcalendar ./helm \
  --set initdb.enabled=true
```

To run a custom script instead, pass it at install time:

```bash
helm install workingcalendar ./helm \
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

### Post-install verification

After installing the chart you can run the built-in Helm test to make sure
the `workingcalendar` table schema matches expectations and contains sample
data:

```bash
helm test workingcalendar
```

The test pod queries the database and fails if the table structure differs or
no rows are present, allowing your deployment pipeline to stop on error.

## Развёртывание из папки k8s

Перед применением манифестов создайте секреты с параметрами приложения. Например:

```bash
kubectl create secret generic workingcalendar-secrets \
  --from-literal=ConnectionStrings__Postgres="postgres://user:pass@host:5432/workingcalendar"
```

Тот же секрет можно оформить YAML-манифестом:

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: workingcalendar-secrets
type: Opaque
stringData:
  ConnectionStrings__Postgres: "postgres://user:pass@host:5432/workingcalendar"
```

После создания секретов примените остальные файлы:

```bash
kubectl apply -f ./k8s/
```

Манифесты следует применять в таком порядке: сначала секреты, затем остальные ресурсы из `k8s/`.

