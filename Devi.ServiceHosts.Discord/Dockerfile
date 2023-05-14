#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Devi.ServiceHosts.Discord/Devi.ServiceHosts.Discord.csproj", "Devi.ServiceHosts.Discord/"]
RUN dotnet restore "Devi.ServiceHosts.Discord/Devi.ServiceHosts.Discord.csproj"
COPY . .
WORKDIR "/src/Devi.ServiceHosts.Discord"
RUN dotnet build "Devi.ServiceHosts.Discord.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Devi.ServiceHosts.Discord.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Devi.ServiceHosts.Discord.dll"]