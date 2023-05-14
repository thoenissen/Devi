#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Devi.ServiceHosts.Discord/Devi.ServiceHosts.Discord.csproj", "Devi.ServiceHosts.Discord/"]
COPY ["Devi.ServiceHosts.Clients/Devi.ServiceHosts.Clients.csproj", "Devi.ServiceHosts.Clients/"]
COPY ["Devi.ServiceHosts.DTOs/Devi.ServiceHosts.DTOs.csproj", "Devi.ServiceHosts.DTOs/"]
COPY ["Devi.ServiceHosts.Core/Devi.ServiceHosts.Core.csproj", "Devi.ServiceHosts.Core/"]
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