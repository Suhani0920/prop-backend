# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy the .csproj file and restore dependencies
COPY *.csproj .
RUN dotnet restore

# Copy the rest of the application source code
COPY . .
# Publish the application for release
RUN dotnet publish -c Release -o out

# Use the official ASP.NET runtime image for the final, smaller image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
# Copy the published output from the build stage
COPY --from=build /app/out .

# The command to start the application
ENTRYPOINT ["dotnet", "PropVivo.API.dll"]