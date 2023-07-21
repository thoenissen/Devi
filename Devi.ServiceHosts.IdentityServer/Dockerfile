#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Devi.ServiceHosts.IdentityServer/Devi.ServiceHosts.IdentityServer.csproj", "Devi.ServiceHosts.IdentityServer/"]
RUN dotnet restore "Devi.ServiceHosts.IdentityServer/Devi.ServiceHosts.IdentityServer.csproj"
COPY . .
WORKDIR "/src/Devi.ServiceHosts.IdentityServer"
RUN dotnet build "Devi.ServiceHosts.IdentityServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Devi.ServiceHosts.IdentityServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Devi.ServiceHosts.IdentityServer.dll"]