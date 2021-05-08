#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 5010

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["OTUS.HomeWork.PaymentGatewayService/OTUS.HomeWork.PaymentGatewayService.csproj", "OTUS.HomeWork.PaymentGatewayService/"]
RUN dotnet restore "OTUS.HomeWork.PaymentGatewayService/OTUS.HomeWork.PaymentGatewayService.csproj"
COPY . .
WORKDIR "/src/OTUS.HomeWork.PaymentGatewayService"
RUN dotnet build "OTUS.HomeWork.PaymentGatewayService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OTUS.HomeWork.PaymentGatewayService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OTUS.HomeWork.PaymentGatewayService.dll"]