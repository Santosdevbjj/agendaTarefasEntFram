using Xunit;
using Moq;
using AgendaTarefas.Services;
using AgendaTarefas.Repositories;
using AgendaTarefas.DTOs;
using AgendaTarefas.Models;

namespace AgendaTarefas.Tests.Unit
{
    public class TarefaServiceTests
    {
        private readonly Mock<ITarefaRepository> _repoMock;
        private readonly TarefaService _service;

        public TarefaServiceTests()
        {
            _repoMock = new Mock<ITarefaRepository>();
            _service = new TarefaService(_repoMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnDto_WithGeneratedId()
        {
            var dto = new TarefaDto { Titulo = "Test", Descricao = "Desc", Data = DateTime.Now, Status = StatusTarefa.Pendente };
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Tarefa>()))
                     .ReturnsAsync((Tarefa t) => { t.Id = 1; return t; });

            var result = await _service.CreateAsync(dto);

            Assert.Equal(1, result.Id);
            Assert.Equal(dto.Titulo, result.Titulo);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Tarefa>()), Times.Once);
        }
    }
}
