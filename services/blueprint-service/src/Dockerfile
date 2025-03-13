FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["BlueprintService.csproj", "./"]
RUN dotnet restore "BlueprintService.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "BlueprintService.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "BlueprintService.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user and switch to it for security
RUN adduser --disabled-password --gecos "" appuser
USER appuser

# Expose the port the app will run on
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "BlueprintService.dll"]
