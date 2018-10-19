FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["travelling-deliveryman.csproj", ""]
RUN dotnet restore "travelling-deliveryman.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "travelling-deliveryman.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "travelling-deliveryman.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "travelling-deliveryman.dll"]