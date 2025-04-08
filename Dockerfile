# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia arquivos de solução e projetos primeiro (para cache de camadas)
COPY *.sln .
COPY SkillStack.API/*.csproj ./SkillStack.API/
COPY SkillStack.Application/*.csproj ./SkillStack.Application/
COPY SkillStack.Core/*.csproj ./SkillStack.Core/
COPY SkillStack.Domain/*.csproj ./SkillStack.Domain/
COPY SkillStack.Infrastructure/*.csproj ./SkillStack.Infrastructure/

# Restaura dependências
RUN dotnet restore "SkillStack.BackEnd.sln"

# Copia todo o código fonte
COPY . .

# Publica a aplicação
RUN dotnet publish "SkillStack.API/SkillStack.API.csproj" -c Release -o /app

# Estágio de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "SkillStack.API.dll"]