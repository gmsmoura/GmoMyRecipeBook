# criando imagem Microsoft .NET 8.0 com o projeto receitas e aplicando um alias
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# definindo o diretório de trabalho, criando o diretório caso não exista
WORKDIR /app

# copiando o projeto para a imagem, argumento 1 a origem (src, onde está o projeto) e argumento 2 o destino (/app) "container"
COPY src/ .

# dentro do diretório abaixo será feito o restore do projeto
# o comando restore baixa as dependências do projeto, o arquivo .csproj é o arquivo de configuração do projeto
WORKDIR Backend/MyRecipeBook.API

RUN dotnet restore

# compilação do projeto, gerando as dlls do projeto para a API executar e release será otimizado para produção
# o comando -o /app/out define o diretório de saída da compilação, onde as dlls serão geradas
# lembrando que o comando de Release seria para gerar com o appsettings development, para produção o comando seria com o EntryPoint
RUN dotnet publish -c Release -o /app/out

# todo caminho acima cria uma imagem temporária, agora vamos criar a imagem final
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# copiando a imagem da pasta /app/out da imagem build-env para a pasta /app da imagem final
COPY --from=build-env /app/out .

# executando o projeto à partir da dll principal do projeto, neste caso MyRecipeBook.API.dll
# para testar localmente, rodar o comando: dotnet MyRecipeBook.API.dll no terminal dentro do container (pasta app/out)
ENTRYPOINT ["dotnet", "MyRecipeBook.API.dll"]

