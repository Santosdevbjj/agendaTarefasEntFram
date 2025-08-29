## Construindo um Sistema de Agendamento de Tarefas com Entity Framework.

![GFTNet001](https://github.com/user-attachments/assets/0c07fdcb-9e4c-457c-ab73-0ca31b495868)


**Bootcamp GFT Start #7 .NET**

---


**AgendaTarefas — README (detalhado)**

**Este README.md** explica cada arquivo gerado no projeto, sua funcionalidade, e dá um passo-a-passo didático para rodar localmente, executar migrations, rodar testes (unitários e de integração) e configurar CI/CD (Azure Pipelines com estágios Dev/Prod). Também inclui dicas de troubleshooting e referências oficiais.

> **Nota rápida:** este projeto usa .NET 8, Entity Framework Core com SQLite, xUnit para testes e Azure Pipelines (YAML) para CI/CD.

> Para testes de integração usamos WebApplicationFactory<TEntryPoint> (o Program do projeto deve ser partial para compatibilidade com o factory — veja seção Pronto para testes de integração).

Documentação oficial importante citada ao longo do README: EF Migrations, EF SQLite provider, Azure Pipelines multistage e templates, AzureWebApp task, WebApplicationFactory e xUnit. 




---

**Sumário rápido**

**Arquivos principais:** AgendaTarefas.csproj, Program.cs, appsettings.json, AppDbContext.cs

**Migrations:** 20250829_InitialCreate.cs, AgendaTarefasModelSnapshot.cs

**Debug:** launchSettings.json

**CI/CD:** azure-pipelines.yml, .template/variables-template.yml, .template/stage-template.yml

**Testes:** TarefaServiceTests.cs (unit), TarefaControllerIntegrationTests.cs (integration)



---

**O que cada arquivo faz (explicação detalhada)**

> **Cada item abaixo descreve função e comportamento esperado do arquivo.**



**AgendaTarefas.csproj**

Arquivo de projeto (.NET SDK).

Define TargetFramework (net8.0), Nullable, ImplicitUsings e as dependências NuGet (EF Core provider para SQLite, EF Core Design, Swashbuckle/Swagger, etc.).

Quando você rodar dotnet restore o NuGet instalará os pacotes citados aqui.


**Program.cs**

Ponto de entrada da aplicação (.NET 8 minimal hosting).

Registra serviços no DI container:

AddControllers() — habilita controllers.

AddDbContext<AppDbContext>(options => options.UseSqlite(...)) — conecta EF Core à base SQLite via connection string do appsettings.json.

AddSwaggerGen() / UseSwagger() / UseSwaggerUI() — habilita documentação Swagger (UI).

Registra ITarefaRepository / ITarefaService (Scoped).


Mapeia controllers com app.MapControllers() e roda a aplicação com app.Run().

Importante para testes de integração: adicione public partial class Program { } (ver seção Pronto para testes de integração). A documentação sobre testes com WebApplicationFactory recomenda que o Program seja detectável como entrypoint. 


**appsettings.json**

Configurações da aplicação (em especial ConnectionStrings).

No projeto adaptado para SQLite: "DefaultConnection": "Data Source=agendaTarefas.db".

Em desenvolvimento você pode sobrepor com appsettings.Development.json (não comitar credenciais sensíveis).

Quando rodar, o DB SQLite será criado no arquivo agendaTarefas.db na pasta do projeto (se ainda não existir).


**Data/AppDbContext.cs**

Classe AppDbContext : DbContext.

Registra DbSet<Tarefa> que representa a tabela Tarefas.

Em OnModelCreating define constraints (ex.: Titulo required, MaxLength) e a conversão do enum StatusTarefa para string no banco via EnumToStringConverter (legibilidade no BD).

É a peça central usada pelo EF para criar queries, migrations e persistência.


**Data/Migrations/20250829_InitialCreate.cs**

Migration inicial (arquivo Migration gerado manualmente neste projeto).

Up — cria a tabela Tarefas com colunas: Id (INTEGER autoincrement), Titulo, Descricao, Data, Status (string).

Down — reverte a criação (drop table).

Observação: em um fluxo típico você geraria essa migration localmente com dotnet ef migrations add InitialCreate (ou usaria o arquivo já incluído aqui). A funcionalidade Migrations do EF permite evoluir o schema do DB conforme o modelo muda. 


**Data/Migrations/AgendaTarefasModelSnapshot.cs**

Model snapshot (usado pelo EF para comparar modelo atual vs. último snapshot — base para gerar novas migrations).

Representa a “imagem” do modelo naquele momento.

Mantê-lo no repositório ajuda equipes a versionarem o histórico de schema.


**Properties/launchSettings.json**

Config de execução local (Visual Studio / dotnet run).

Define URLs locais (https://localhost:5001;http://localhost:5000) e a variável ASPNETCORE_ENVIRONMENT (ex.: Development).

É útil durante desenvolvimento para abrir automaticamente o browser com a URL correta.


**azure-pipelines.yml (na raiz)**

Pipeline multistage (CI + CD):

Trigger: branches que disparam o pipeline (ex.: dev, main).

Stage Build: compila, executa unit & integration tests, publica artefatos.

Stage Dev/Prod: fazem deploy (via template .template/stage-template.yml) usando variable groups e environment (para aprovações manuais no Prod).

Observação: um azure-pipelines.yml no root do repo é a prática padrão para Azure DevOps — o serviço procura lá por padrão. Multistage pipelines e templates são a forma recomendada de organizar CI/CD no Azure DevOps. 


**.template/variables-template.yml**

Template YAML com variáveis comuns (ex.: vmImage).

Utilizado pelo pipeline principal para centralizar valores reutilizáveis (evita duplicação).


**.template/stage-template.yml**

Template que define um stage parametrizável (usado para Dev e Prod).

Parâmetros:

stageName (Dev/Prod)

variableGroup (ex.: DevVariables, ProdVariables)

azureSubscription (service connection)

azureWebAppName (nome do App Service)


Implementa um deployment job que baixa artefato e usa a task AzureWebApp@1 para deploy.

Usa environment: <stageName> para associar o deployment a um Environment do Azure DevOps — isso permite configurar approvers/checks via UI. 


**Tests/AgendaTarefas.Tests.Unit/TarefaServiceTests.cs**

Unit tests (xUnit) cobrindo a camada TarefaService.

Usa mocks (ex.: Moq) no repositório para isolar a lógica de negócio e validar comportamento (criação, update, validação de retorno).

Executa via dotnet test <project> ou dotnet test --filter Category=Unit.


**Tests/AgendaTarefas.Tests.Integration/TarefaControllerIntegrationTests.cs**

Testes de integração (xUnit + WebApplicationFactory<Program>).

Cria um TestServer em memória, executa requests HTTP reais contra os endpoints (POST /Tarefa, GET /Tarefa/{id}) e valida fluxo end-to-end.

Requer que o Program seja detectável (ver Pronto para testes de integração). 



---

**Passo-a-passo para rodar localmente (didático, linha-a-linha)**

**1) Pré-requisitos (instalar)**

1. .NET 8 SDK (verifique com dotnet --version — deve ser >= 8.0).


**2. (Opcional) dotnet-ef CLI para manipular migrations:**

dotnet tool install --global dotnet-ef

ou, se já existe um dotnet-tools.json, dotnet tool restore. (EF CLI docs). 


**3. Git (para clonar o repositório).**


**4. (Opcional) SQLite browser/DB client se quiser inspecionar o arquivo .db.**




---

**2) Clonar o repositório**

git clone https://github.com/Santosdevbjj/agendaTarefasEntFram
cd agendaTarefasEntFram


---

**3) Restaurar e build**

dotnet restore
dotnet build --configuration Release


---

**4) (Opcional) Gerar migrations localmente**

Se você quiser gerar a migration em seu ambiente (recomendado em times, para timestamps locais), execute:

dotnet ef migrations add InitialCreate

> Caso você já tenha a migration **Data/Migrations/20250829_InitialCreate.cs** incluída, pode pular esse passo. A documentação de Migrations do EF explica as melhores práticas. 




---

**5) Aplicar migrations (criar ou atualizar o DB)**

Use a migration existente ou gerada para criar o banco SQLite:

**dotnet ef database update**

Isso criará (ou atualizará) o arquivo **agendaTarefas.db** usando o script Up() das migrations. Para produção, considere gerar scripts SQL revisáveis (opção no EF) antes de aplicar a DB em produção. 


---

**6) Rodar a API localmente**

dotnet run --project AgendaTarefas.csproj

Ao iniciar, o console exibirá a URL (ex.: https://localhost:5001).

Abra https://localhost:5001/swagger/index.html para acessar a documentação Swagger e testar os endpoints.



---

**7) Testes Unitários e de Integração (local)**

Unit tests

dotnet test ./Tests/AgendaTarefas.Tests.Unit/AgendaTarefas.Tests.Unit.csproj --configuration Release

Integration tests

dotnet test ./Tests/AgendaTarefas.Tests.Integration/AgendaTarefas.Tests.Integration.csproj --configuration Release

> **Observação:** Integration tests usam WebApplicationFactory<Program> e criarão um servidor in-memory. Certifique-se de que o Program seja partial (veja próximo ponto). 




---

**8) Pronto para testes de integração — alteração necessária**

Para o WebApplicationFactory<Program> encontrar o entrypoint, adicione no final do seu Program.cs:

// Após app.Run();
public partial class Program { }

Isso permite que WebApplicationFactory<Program> crie a aplicação de teste usando o mesmo Program como entrypoint. (Padrão recomendado na documentaçao MS). 


---

**CI/CD (Azure Pipelines) — como usar o template / configuração**

Onde colocar os arquivos

Coloque azure-pipelines.yml na raiz do repositório (padrão do Azure DevOps).

Mantenha .template/variables-template.yml e .template/stage-template.yml na pasta .template/ (ou ajuste o path se preferir). A documentação de templates mostra como reutilizar stages e parâmetros. 


**O que configurar no Azure DevOps (passo-a-passo)**

**1. Service connection (Project Settings → Service connections):** configure a conexão à sua assinatura Azure (usada pelo AzureWebApp@1 para fazer deploy).


**2. Variable Groups (Pipelines → Library):** crie DevVariables e ProdVariables com variáveis sensíveis (ex.: DevWebApp, DevSubscription, DevConnectionString, ProdWebApp, ProdSubscription etc.). Marque “Allow access to all pipelines” se apropriado. 


**3. Environments (Pipelines → Environments):** crie Dev e Prod (mesmos nomes usados no template). Em Prod adicione Approvals & checks (pessoas ou grupos aprovadores) para exigir aprovação manual antes de rodar o stage de produção. 


**4. Commit do azure-pipelines.yml e observe o pipeline.**



**Como o pipeline funciona (visão geral)**

**Build:** compila, roda testes unitários e de integração, publica artefato (drop).

**Dev stage:** baixa o artifact e faz deploy automático ao App Service do Dev.

**Prod stage:** depende de Dev; rodará apenas se Build/Dev tiverem sucesso e (se habilitado) após aprovação manual (via Environment checks). O deploy é feito com a task AzureWebApp@1. 



---

**Notas técnicas e troubleshooting (erros comuns)**

**“dotnet ef migrations add” falha:** verifique se o projeto compila (dotnet build) e se Microsoft.EntityFrameworkCore.Design está instalado; se o DbContext está em outro projeto, use os parâmetros --project e --startup-project. (Docs EF CLI). 

**SQLite e limitações:** SQLite tem limitações (algumas operações DDL não suportadas, tipos como DateTimeOffset podem ter problemas). Em caso de migrations complexas, EF pode precisar recriar tabelas; avalie esses impactos para produção. Use SQLite para desenvolvimento/testes; em produção, prefira SQL Server/Azure SQL se precisar de features avançadas. 

**Deploy via AzureWebApp@1:** verifique o azureSubscription (service connection) tem as permissões corretas e que o appName existe. Logs do task mostram causas de falha. 

**Aprovações não aparecendo:** confira se você criou o Environment com o mesmo nome usado no environment: do YAML e se adicionou Approvals and checks. 



---

**Sugestões de boas práticas aplicáveis aqui**

Versione as migrations no Git e coordene alterações entre a equipe (merge conflicts em migrations podem acontecer). 

Para testes de integração estáveis, prefira usar um banco isolado (SQLite in-memory ou teste com DB por CI em containers). A documentação do ASP.NET sugere WebApplicationFactory<TEntryPoint>. 

Proteja secrets com Variable Groups e Key Vault (se possível). 



---

**Referências (documentação oficial e leituras recomendadas)**

EF Core — Migrations (overview). 

EF Core — Applying migrations / strategies. 

EF Core — SQLite provider & limitations. 

Azure DevOps — Create a multistage pipeline (Build/Test/Deploy). 

Azure DevOps — YAML templates & parameters. 

Azure Pipelines — AzureWebApp@1 task (deploy to App Service). 

Azure DevOps — Variable Groups & Library. 

Azure DevOps — Approvals & checks (Environments). 

ASP.NET Core docs — Integration tests with WebApplicationFactory<TEntryPoint>. 

xUnit official site (test runner and docs). 



---

**Final — checklist prático (para rodar em ordem)**

1. Instalar .NET 8 e dotnet-ef (se necessário).


2. git clone ... → cd agendaTarefasEntFram.


3. dotnet restore → dotnet build.


4. (Opcional) dotnet ef migrations add InitialCreate — somente se quiser recriar timestamp.


5. dotnet ef database update — aplica migrations e cria agendaTarefas.db.


6. (adicionar public partial class Program { } em Program.cs) para integração.


7. dotnet run --project AgendaTarefas.csproj → acessar https://localhost:5001/swagger/index.html.


8. dotnet test para rodar Unit e Integration tests.


9. Subir azure-pipelines.yml para a raiz; criar Service Connections / Variable Groups / Environments no Azure DevOps; observar pipeline.




---



