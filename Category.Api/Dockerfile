FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["Category.Api/CategoryApi.csproj", "Category.Api/"]
RUN dotnet restore "Category.Api/CategoryApi.csproj"
COPY . .
WORKDIR "/src/Category.Api"
RUN dotnet build "CategoryApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CategoryApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CategoryApi.dll"]
