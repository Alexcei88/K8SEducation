#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["OTUS.HomeWork.RestAPI/OTUS.HomeWork.RestAPI.csproj", "OTUS.HomeWork.RestAPI/"]
RUN dotnet restore "OTUS.HomeWork.RestAPI/OTUS.HomeWork.RestAPI.csproj"
COPY . .
WORKDIR "/src/OTUS.HomeWork.RestAPI"
RUN dotnet build "OTUS.HomeWork.RestAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OTUS.HomeWork.RestAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OTUS.HomeWork.RestAPI.dll"]