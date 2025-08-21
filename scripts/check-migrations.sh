#!/usr/bin/env bash
set -euo pipefail

container="wc-migrations-test"

docker run -d --rm --name "$container" -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=wc_migrations_test -p 55432:5432 postgres:16 >/dev/null

until docker exec "$container" pg_isready -U postgres >/dev/null 2>&1; do
  sleep 1
done

if ! command -v dotnet-ef >/dev/null 2>&1; then
  dotnet tool install --global dotnet-ef --version 8.0.0 >/dev/null
  export PATH="$PATH:$HOME/.dotnet/tools"
fi

CONNECTION="Host=localhost;Port=55432;Database=wc_migrations_test;Username=postgres;Password=postgres"

dotnet ef database update --project WorkingCalendar.Infrastructure --startup-project WorkingCalendar.Server --connection "$CONNECTION" --no-build

docker stop "$container" >/dev/null
