#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["OTUS.HomeWork.AuthService/OTUS.HomeWork.AuthService.csproj", "OTUS.HomeWork.AuthService/"]
RUN dotnet restore "OTUS.HomeWork.AuthService/OTUS.HomeWork.AuthService.csproj"
COPY . .
WORKDIR "/src/OTUS.HomeWork.AuthService"
RUN dotnet build "OTUS.HomeWork.AuthService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OTUS.HomeWork.AuthService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OTUS.HomeWork.AuthService.dll"]