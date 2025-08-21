# ---------- Ѕј«ќ¬џ… RUNTIME —Ћќ… ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:6d40ae71165cb8a4427477ae3b447b05c080330cabae08b0dae672552bcd34d8 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# ---------- —Ѕќ– ј .NET + NODE.JS ƒЋя JS SDK ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:7fd287cec03e027e39be13fb06ca1347f1338c7a6ab4ad305d94d91852693f70 AS build
WORKDIR /src

# ”станавливаем Node.js 22 (нужен дл€ Microsoft.VisualStudio.JavaScript.Sdk)
RUN apt-get update \
 && apt-get install -y curl ca-certificates gnupg \
 && install -d -m 0755 /etc/apt/keyrings \
 && curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg \
 && echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_22.x nodistro main" > /etc/apt/sources.list.d/nodesource.list \
 && apt-get update \
 && apt-get install -y nodejs \
 && node --version && npm --version

#  опируем csprojТи Ч быстрый restore на чистом контексте
COPY WorkingCalendar.Server/WorkingCalendar.Server.csproj WorkingCalendar.Server/
COPY WorkingCalendar.Application/WorkingCalendar.Application.csproj WorkingCalendar.Application/
COPY WorkingCalendar.Infrastructure/WorkingCalendar.Infrastructure.csproj WorkingCalendar.Infrastructure/
COPY WorkingCalendar.Domain/WorkingCalendar.Domain.csproj WorkingCalendar.Domain/

# ≈сли у теб€ есть esproj (JS-проект) Ч тоже копируем, чтобы restore прошЄл
COPY workingcalendar.client/workingcalendar.client.esproj workingcalendar.client/

RUN dotnet restore "WorkingCalendar.Server/WorkingCalendar.Server.csproj"

#  опируем всЄ остальное исходники
COPY . .

# —обираем
WORKDIR /src/WorkingCalendar.Server
RUN dotnet build "WorkingCalendar.Server.csproj" -c Release -o /app/build

# ---------- ѕ”ЅЋ» ј÷»я ----------
FROM build AS publish
WORKDIR /src/WorkingCalendar.Server
RUN dotnet publish "WorkingCalendar.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---------- ‘»ЌјЋ№Ќџ… ќЅ–ј« ----------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkingCalendar.Server.dll"]
