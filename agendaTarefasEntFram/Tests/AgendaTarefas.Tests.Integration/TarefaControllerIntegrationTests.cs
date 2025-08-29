using Xunit;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using AgendaTarefas;
using System.Net;
using System.Text;
using System.Text.Json;
using AgendaTarefas.DTOs;

namespace AgendaTarefas.Tests.Integration
{
    public class TarefaControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TarefaControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostAndGetTarefa_ShouldReturnCreatedThenOk()
        {
            var dto = new TarefaDto { Titulo = "Integration Test", Descricao = "Teste", Data = DateTime.Now, Status = StatusTarefa.Pendente };
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            var postResponse = await _client.PostAsync("/Tarefa", content);
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var createdDto = JsonSerializer.Deserialize<TarefaDto>(await postResponse.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            var getResponse = await _client.GetAsync($"/Tarefa/{createdDto.Id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        }
    }
}
