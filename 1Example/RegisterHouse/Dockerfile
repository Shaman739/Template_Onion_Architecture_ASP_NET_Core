#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80,443,1433

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["../../", "RegisterHouse/"]
RUN dotnet restore "RegisterHouse/1Example/RegisterHouse/RegisterHouse.csproj"
COPY . .
WORKDIR "/src/RegisterHouse"
RUN dotnet build "1Example/RegisterHouse/RegisterHouse.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "1Example/RegisterHouse/RegisterHouse.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RegisterHouse.dll"]