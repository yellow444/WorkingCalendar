# ---------- БАЗОВЫЙ RUNTIME ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# ---------- SDK + NODE.JS (для JS SDK и сборки Vite) ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Node.js для Microsoft.VisualStudio.JavaScript.Sdk и сборки клиента
RUN apt-get update \
 && apt-get install -y curl ca-certificates gnupg \
 && install -d -m 0755 /etc/apt/keyrings \
 && curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg \
 && echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_22.x nodistro main" > /etc/apt/sources.list.d/nodesource.list \
 && apt-get update \
 && apt-get install -y nodejs \
 && node --version && npm --version

# Копируем csproj / esproj для точечного restore
COPY WorkingCalendar.Server/WorkingCalendar.Server.csproj WorkingCalendar.Server/
COPY WorkingCalendar.Application/WorkingCalendar.Application.csproj WorkingCalendar.Application/
COPY WorkingCalendar.Infrastructure/WorkingCalendar.Infrastructure.csproj WorkingCalendar.Infrastructure/
COPY WorkingCalendar.Domain/WorkingCalendar.Domain.csproj WorkingCalendar.Domain/
# если есть esproj
COPY workingcalendar.client/workingcalendar.client.esproj workingcalendar.client/

RUN dotnet restore "WorkingCalendar.Server/WorkingCalendar.Server.csproj"

# Копируем все исходники
COPY . .

# --- СБОРКА Vite клиента ---
WORKDIR /src/workingcalendar.client
# если используется npm ci — замени команду
RUN npm ci || npm install
RUN npm run build

# --- СБОРКА .NET ---
WORKDIR /src/WorkingCalendar.Server
RUN dotnet build "WorkingCalendar.Server.csproj" -c Release -o /app/build

# ---------- ПУБЛИКАЦИЯ .NET + КОПИЯ VITE В wwwroot ----------
FROM build AS publish
WORKDIR /src/WorkingCalendar.Server
RUN dotnet publish "WorkingCalendar.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Кладём собранный фронт в wwwroot
RUN mkdir -p /app/publish/wwwroot
COPY --from=build /src/workingcalendar.client/dist/ /app/publish/wwwroot/

# ---------- ФИНАЛ ----------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkingCalendar.Server.dll"]
