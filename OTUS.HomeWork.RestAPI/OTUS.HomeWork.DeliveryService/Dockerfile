#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["OTUS.HomeWork.DeliveryService/OTUS.HomeWork.DeliveryService.csproj", "OTUS.HomeWork.DeliveryService/"]
RUN dotnet restore "OTUS.HomeWork.DeliveryService/OTUS.HomeWork.DeliveryService.csproj"
COPY . .
WORKDIR "/src/OTUS.HomeWork.DeliveryService"
RUN dotnet build "OTUS.HomeWork.DeliveryService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OTUS.HomeWork.DeliveryService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OTUS.HomeWork.DeliveryService.dll"]