# ---------- БАЗОВЫЙ RUNTIME ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# ---------- SDK + NODE.JS (для сборки .NET и Vite) ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Node.js 22.x для Vite
RUN apt-get update \
 && apt-get install -y curl ca-certificates gnupg \
 && install -d -m 0755 /etc/apt/keyrings \
 && curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key \
    | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg \
 && echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_22.x nodistro main" \
    > /etc/apt/sources.list.d/nodesource.list \
 && apt-get update \
 && apt-get install -y nodejs \
 && node --version && npm --version \
 && apt-get clean && rm -rf /var/lib/apt/lists/*

# Копируем csproj, выполняем restore (кэшируем зависимости)
COPY WorkingCalendar.sln ./
COPY WorkingCalendar.Server/WorkingCalendar.Server.csproj WorkingCalendar.Server/
COPY WorkingCalendar.Domain/WorkingCalendar.Domain.csproj WorkingCalendar.Domain/
COPY WorkingCalendar.Application/WorkingCalendar.Application.csproj WorkingCalendar.Application/
COPY WorkingCalendar.Infrastructure/WorkingCalendar.Infrastructure.csproj WorkingCalendar.Infrastructure/
RUN dotnet restore WorkingCalendar.Server/WorkingCalendar.Server.csproj

# Копируем остальной код
COPY . .

# Сборка фронтенда (Vite)
WORKDIR /src/workingcalendar.client
RUN npm ci && npm run build

# Публикация бекенда
WORKDIR /src/WorkingCalendar.Server
RUN dotnet publish "WorkingCalendar.Server.csproj" -c Release -o /out /p:UseAppHost=false

# Кладём билд фронта в wwwroot
RUN mkdir -p /out/wwwroot && cp -r /src/workingcalendar.client/dist/* /out/wwwroot/

# ---------- ФИНАЛ ----------
FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "WorkingCalendar.Server.dll"]
