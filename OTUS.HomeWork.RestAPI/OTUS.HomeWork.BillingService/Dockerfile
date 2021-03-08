#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["OTUS.HomeWork.BillingService/OTUS.HomeWork.BillingService.csproj", "OTUS.HomeWork.BillingService/"]
RUN dotnet restore "OTUS.HomeWork.BillingService/OTUS.HomeWork.BillingService.csproj"
COPY . .
WORKDIR "/src/OTUS.HomeWork.BillingService"
RUN dotnet build "OTUS.HomeWork.BillingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OTUS.HomeWork.BillingService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OTUS.HomeWork.BillingService.dll"]