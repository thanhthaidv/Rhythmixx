# syntax=docker/dockerfile:1

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution + csproj files first for better layer caching
COPY Rhythmix.sln ./
COPY Rhythmix.API/Rhythmix.API.csproj Rhythmix.API/
COPY Rhythmix.Application/Rhythmix.Application.csproj Rhythmix.Application/
COPY Rhythmix.Domain/Rhythmix.Domain.csproj Rhythmix.Domain/
COPY Rhythmix.Infrastructure/Rhythmix.Infrastructure.csproj Rhythmix.Infrastructure/
COPY Rhythmix.Application.Tests/Rhythmix.Application.Tests.csproj Rhythmix.Application.Tests/ 
# Restore
RUN dotnet restore "Rhythmix.sln"

# Copy the rest of the source code
COPY . .

# Publish
RUN dotnet publish "Rhythmix.API/Rhythmix.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

# Container listens on 8080
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "Rhythmix.API.dll"]