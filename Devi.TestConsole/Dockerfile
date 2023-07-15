#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Devi.TestConsole/Devi.TestConsole.csproj", "Devi.TestConsole/"]
COPY ["Devi.EventQueue/Devi.EventQueue.csproj", "Devi.EventQueue/"]
COPY ["Devi.ServiceHosts.Core/Devi.ServiceHosts.Core.csproj", "Devi.ServiceHosts.Core/"]
COPY ["Devi.Core/Devi.Core.csproj", "Devi.Core/"]
RUN dotnet restore "Devi.TestConsole/Devi.TestConsole.csproj"
COPY . .
WORKDIR "/src/Devi.TestConsole"
RUN dotnet build "Devi.TestConsole.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Devi.TestConsole.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Devi.TestConsole.dll"]