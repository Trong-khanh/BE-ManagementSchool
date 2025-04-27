FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy csproj and restore dependencies
COPY ["ManagementSchool/ManagementSchool.csproj", "ManagementSchool/"]
RUN dotnet restore "ManagementSchool/ManagementSchool.csproj"

# Copy the rest of the project
COPY . .
WORKDIR "/app/ManagementSchool"
RUN dotnet build "ManagementSchool.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ManagementSchool.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ManagementSchool.dll"]