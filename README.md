![github-repository-share_shadow-monster](https://github.com/user-attachments/assets/fc0d4703-c9b5-4320-969f-c234c9d4b979)

## 👻 Caça aos Bugs 2025 - Desafio 08 - Shadow Monster

Oi, eu sou o Bruno e este é o espaço onde compartilho minha jornada de aprendizado durante o desafio **Caça aos Bugs 2025**, realizado pelo [balta.io](https://balta.io). 👻

Aqui você vai encontrar projetos, exercícios e códigos que estou desenvolvendo durante o desafio. O objetivo é colocar a mão na massa, testar ideias e registrar minha evolução no mundo da tecnologia.

### Sobre este desafio

No desafio **Shadow Monster** eu tive que fazer a publicação da aplicação utilizando o processo de CI/CD com GitHub Actions.
Neste processo eu aprendi:

-   ✅ Git e GitHub
-   ✅ CI/CD
-   ✅ Build e Test
-   ✅ GitHub Actions
-   ✅ Microsoft Azure

## REGRAS DO DESAFIO

-   [x] Realizar um fork do repositório do desafio
-   [x] Criar um Workflow para branch main
-   [x] Executar o build do projeto no Workflow
-   [x] Executar o teste (Unit Test) do projeto no Workflow
-   [x] Realizar o deployment automatizado da aplicação via CI/CD

---

# ✅ DESAFIO CONCLUÍDO

### 🚀 Deploy

-   Hospedagem: **Azure Container Apps**
-   Pipeline: **GitHub Actions**
-   Imagem: build + push para **Azure Container Registry (ACR)**
-   Atualização: `az containerapp update` com imagem e variáveis de ambiente
-   Segredos: configurados em **GitHub Actions Secrets** (`AZURE_*`, `DATABASE_URL`)

### 🔄 Fluxo do Pipeline

-   Em `push` ou `pull request` para `main`:
    -   Executa build e testes
    -   Se aprovado, faz build da imagem, push para o ACR e deploy no Azure Container Apps

## 🏗️ Arquitetura e Tecnologias

### **Padrões Arquiteturais**

-   **Clean Architecture** - Separação em camadas bem definidas
-   **CQRS** - Command Query Responsibility Segregation
-   **Mediator Pattern** - Desacoplamento entre componentes
-   **Domain-Driven Design** - Domínio rico com regras de negócio

### **Tecnologias Utilizadas**

-   **ASP.NET Core 9** - Framework web
-   **Entity Framework Core** - ORM e migrations
-   **Dapper** - Sql para consultas complexas
-   **PostgreSQL** - Banco de dados relacional
-   **Swagger/OpenAPI** - Documentação da API
-   **FluentValidation** - Validação de dados
-   **xUnit + Moq** - Testes unitários
-   **Bogus** - Geração de dados fake para testes
-   **SQLite In-Memory** - Testes de integração

### **Qualidade de Código**

-   **Tratamento Global de Exceções** - Filter customizado
-   **Exceções Customizadas** - Tipos específicos de erro
-   **Cobertura de Testes** - 138/138 testes passando (100%)

### Veja meu progresso no desafio

🔗 [Repositório central dos desafios](https://github.com/b01tech/balta-desafio-caca-aos-bugs-2025.git)
