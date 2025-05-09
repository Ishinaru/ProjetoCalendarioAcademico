# 📅 Calendário Acadêmico API

API RESTful desenvolvida para automatizar e gerenciar o calendário acadêmico da Universidade, com suporte completo a cadastros, consultas, atualizações e desativações de calendários, eventos e portarias.

## 🚀 Tecnologias Utilizadas

- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- C# 12
- Entity Framework Core 8
- ASP.NET Core Web API
- Mapster (mapeamento de DTOs)
- LinqKit (consultas dinâmicas)
- Azure.Identity (autenticação)
- Swagger / Swashbuckle (documentação interativa)
- Microsoft.Data.SqlClient (conexão com SQL Server)

## 🧩 Estrutura da Solução

A solução é modularizada nos seguintes projetos:

- `CalendarioAcademico.Domain`: Entidades, enums e DTOs.
- `CalendarioAcademico.Data`: Acesso a dados, repositórios e Unit of Work.
- `CalendarioAcademico.WebAPI`: Controllers, validações, middleware e serviços.
- Suporte opcional a Blazor WebAssembly para UI interativa.

## 🗃️ Modelagem de Dados

### Calendário (`CAD_Calendario`)
- Ano único, status (Aguardando, Aprovado, Desativado), número da resolução.
- Relacionamento 1:N com eventos.

### Evento (`EVNT_Evento`)
- Datas de início/fim, descrição, tipo de feriado, flags como importante e ativo.
- Relacionamento N:1 com calendário e 1:N com portarias.

### Portaria (`PORT_Portaria`)
- Número e ano da portaria, status ativo, observações.
- Associada a múltiplos eventos via tabela de junção.

### Evento-Portaria (`EVPT_Evento_Portaria`)
- Associação entre eventos e portarias com data de vigência e status.

## 📚 Funcionalidades Principais

- 📌 Cadastro, edição e desativação de calendários acadêmicos
- 📌 Consulta por ID, ano, status ou critérios dinâmicos
- 📌 Gerenciamento completo de eventos e portarias
- 📌 Filtros por período, mês, ano, status, tipo de feriado, etc.
- 📌 Paginação, ordenação e filtros dinâmicos
- 📌 Mapeamento entre objetos com Mapster
- 📌 Validações específicas por tipo de dado (ex: `DateOnly`)
- 📌 Middleware global para tratamento de exceções
- 📌 Suporte a autenticação com Azure Active Directory

## ⚙️ Regras de Negócio Implementadas

- Não permite calendários duplicados por ano
- Status "Aprovado" bloqueia edição posterior
- Desativação lógica de calendários, eventos e portarias
- Relacionamentos consistentes entre entidades
- Paginação eficiente para grandes volumes de dados
- Validações dinâmicas (ex.: ano válido)

## 🧪 Testes e Documentação

A documentação da API está disponível via Swagger no próprio projeto (`/swagger`). É possível testar todos os endpoints diretamente pela interface interativa.

---

## 📂 Como Rodar o Projeto

```bash
# Requisitos
- .NET 8 SDK
- SQL Server
- Visual Studio 2022 ou VS Code com extensões C#

# Clonar o repositório
git clone https://github.com/Ishinaru/ProjetoCalendarioAcademico.git

# Restaurar pacotes
dotnet restore

# Aplicar migrações e rodar
dotnet ef database update
dotnet run --project CalendarioAcademico.WebAPI
