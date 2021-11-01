#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["IdentityWallet.SDK.Example/IdentityWallet.SDK.Example.csproj", "IdentityWallet.SDK.Example/"]
RUN dotnet restore "IdentityWallet.SDK.Example/IdentityWallet.SDK.Example.csproj"
COPY . .
WORKDIR "/src/IdentityWallet.SDK.Example"
RUN dotnet build "IdentityWallet.SDK.Example.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityWallet.SDK.Example.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "IdentityWallet.SDK.Example.dll", "--server.urls", "http://0.0.0.0:5000"]