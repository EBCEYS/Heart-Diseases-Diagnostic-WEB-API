#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Get_Requests_From_Client_For_Project_Test.csproj", "."]
RUN dotnet restore "./Get_Requests_From_Client_For_Project_Test.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Get_Requests_From_Client_For_Project_Test.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Get_Requests_From_Client_For_Project_Test.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Get_Requests_From_Client_For_Project_Test.dll"]