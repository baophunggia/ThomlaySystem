# 1. Môi trường Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy toàn bộ source code vào
COPY . .

# Restore và Build dự án
RUN dotnet restore src/Thomlay.Api/Thomlay.Api.csproj
RUN dotnet publish src/Thomlay.Api/Thomlay.Api.csproj -c Release -o /out

# 2. Môi trường Runtime (Siêu nhẹ)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# Mở cổng 8080 cho Render
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "Thomlay.Api.dll"]