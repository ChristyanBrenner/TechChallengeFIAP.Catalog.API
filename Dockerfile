# Etapa 1 — build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln ./
COPY GameStore.API/*.csproj ./GameStore.API/
COPY Middleware/*.csproj ./Middleware/
COPY Repositories/*.csproj ./Repositories/
COPY Services/*.csproj ./Services/
COPY Utils/*.csproj ./Utils/
COPY Domain/*.csproj ./Domain/
COPY Consumers/*.csproj ./Consumers/

# ---------------------------
# Mock: ignora restore
# RUN dotnet restore
# ---------------------------

COPY . .

WORKDIR /src/GameStore.API
# RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["sleep", "infinity"]