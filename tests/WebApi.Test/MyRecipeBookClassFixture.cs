
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace WebApi.Test
{
    //classe custom para implementar a classe do .Net IClassFixture
    public class MyRecipeBookClassFixture : IClassFixture<CustomWebApplicationFactory>
    {
        //configurando o HttpClient do .Net para ser possível aplicar os tests de requisições da API
        private readonly HttpClient _httpClient;
        public MyRecipeBookClassFixture(CustomWebApplicationFactory factory) => _httpClient = factory.CreateClient();

        //função que será utilizada por outras classe que irão implementar a MyRecipeBookClassFixture, modificador de acesso protected permite o uso somente de classes que fizerem a implementação/herança desta
        // Realiza uma requisição POST com o corpo em JSON
        protected async Task<HttpResponseMessage> DoPost(
            string method,
            object request,
            string token = "",
            string culture = "en")
        {
            ChangeRequestCulture(culture);     // Define o idioma para a requisição
            AuthorizeRequest(token);           // Adiciona o token de autorização no header (se houver)

            return await _httpClient.PostAsJsonAsync(method, request); // Envia POST com JSON no corpo
        }

        // Realiza uma requisição POST com FormData (multipart), geralmente usado para arquivos
        protected async Task<HttpResponseMessage> DoPostFormData(
            string method,
            object request,
            string token,
            string culture = "en")
        {
            ChangeRequestCulture(culture);     // Define o idioma
            AuthorizeRequest(token);           // Adiciona o token no header

            var multipartContent = new MultipartFormDataContent(); // Cria o conteúdo multipart para aceitar o formato multipart/form-data para o IFormFile

            var requestProperties = request.GetType().GetProperties().ToList(); // Recupera as propriedades do objeto (nome da prop e valor)

            // Itera sobre cada propriedade do objeto
            foreach (var property in requestProperties)
            {
                var propertyValue = property.GetValue(request); // Obtém valor da propriedade

                if (string.IsNullOrWhiteSpace(propertyValue?.ToString()))
                    continue; // Ignora valores nulos ou em branco

                // verificação para as props que forem lista
                if (propertyValue is System.Collections.IList list)
                {
                    // Se a propriedade for uma lista, adiciona adequadamente ao multipart com o método privado abaixo
                    AddListToMultipartContent(multipartContent, property.Name, list);
                }
                else
                {
                    // Caso contrário, adiciona o valor como StringContent
                    multipartContent.Add(new StringContent(propertyValue.ToString()!), property.Name);
                }
            }

            return await _httpClient.PostAsync(method, multipartContent); // Envia POST com multipart
        }

        // Realiza uma requisição GET com cabeçalho de idioma e autorização
        protected async Task<HttpResponseMessage> DoGet(string method, string token = "", string culture = "en")
        {
            ChangeRequestCulture(culture);     // Define idioma
            AuthorizeRequest(token);           // Adiciona token

            return await _httpClient.GetAsync(method); // Realiza GET
        }

        // Executa múltiplas requisições GET com diferentes culturas e retorna todas as respostas
        protected async Task<List<HttpResponseMessage>> DoGetWithMultipleCultures(string method, string token = "", params string[] cultures)
        {
            var responses = new List<HttpResponseMessage>();

            foreach (var culture in cultures)
            {
                ChangeRequestCulture(culture);         // Muda idioma a cada iteração
                AuthorizeRequest(token);               // Adiciona token
                var response = await _httpClient.GetAsync(method); // Realiza GET
                responses.Add(response);               // Adiciona resposta à lista
            }

            return responses;
        }

        // Realiza requisição PUT com corpo JSON
        protected async Task<HttpResponseMessage> DoPut(string method, object request, string token, string culture = "en")
        {
            ChangeRequestCulture(culture);     // Define idioma
            AuthorizeRequest(token);           // Adiciona token

            return await _httpClient.PutAsJsonAsync(method, request); // PUT com JSON
        }

        // Realiza requisição DELETE
        protected async Task<HttpResponseMessage> DoDelete(string method, string token, string culture = "en")
        {
            ChangeRequestCulture(culture);     // Define idioma
            AuthorizeRequest(token);           // Adiciona token

            return await _httpClient.DeleteAsync(method); // DELETE
        }

        // Versão antiga de alteração de cultura via header string
        private void ChangeRequestCultureOld(string culture)
        {
            // Remove header anterior se existir
            if (_httpClient.DefaultRequestHeaders.Contains("Accept-Language"))
                _httpClient.DefaultRequestHeaders.Remove("Accept-Language");

            // Adiciona novo valor ao header
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", culture);
        }

        // Versão atualizada de alteração de cultura usando AcceptLanguage
        private void ChangeRequestCulture(string culture)
        {
            _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear(); // Limpa idiomas anteriores
            _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(culture)); // Define novo
        }

        // Adiciona o token no header da requisição se estiver presente
        private void AuthorizeRequest(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // Adiciona listas de propriedades ao FormData
        private static void AddListToMultipartContent(
            MultipartFormDataContent multipartContent,
            string propertyName,
            System.Collections.IList list)
        {
            var itemType = list.GetType().GetGenericArguments().Single(); // Tipo do item da lista

            // verificação para validar se a lista é uma lista de objetos (uma classe) pelo itemType e que seja diferente de string devido retornar true para string
            if (itemType.IsClass && itemType != typeof(string))
            {
                // Se for uma classe, adiciona usando estrutura complexa
                AddClassListToMultipartContent(multipartContent, propertyName, list);
            }
            else
            {
                // Se for primitivo ou string, adiciona diretamente
                foreach (var item in list)
                {
                    multipartContent.Add(new StringContent(item.ToString()!), propertyName);
                }
            }
        }

        // Adiciona listas de objetos ao FormData (ex: Ingredientes[] com propriedades)
        private static void AddClassListToMultipartContent(
            MultipartFormDataContent multipartContent,
            string propertyName,
            System.Collections.IList list)
        {
            var index = 0;

            foreach (var item in list)
            {
                var classPropertiesInfo = item.GetType().GetProperties().ToList();

                // Adiciona cada propriedade da classe com sintaxe: propertyName[index][propName]
                foreach (var prop in classPropertiesInfo)
                {
                    var value = prop.GetValue(item, null);
                    multipartContent.Add(new StringContent(value!.ToString()!), $"{propertyName}[{index}][{prop.Name}]"); // ex: instructions[0][step] ou instructions[0][text]
                }

                index++;
            }
        }
    }
}
