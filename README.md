![.NET](https://img.shields.io/badge/.NET-8.0-blue) ![C#](https://img.shields.io/badge/C%23-12.0-blue) ![License](https://img.shields.io/badge/License-MIT-green)

# Calendário Acadêmico API

## Descrição
A **Calendário Acadêmico API** é uma RESTful API desenvolvida para automatizar o gerenciamento do calendário acadêmico, eventos e portarias da universidade, centralizando operações de cadastro, consulta, atualização e desativação.  
> **Problema:** A Universidade carece de um sistema automatizado para o Calendário Acadêmico.  
> **Solução:** API REST para integrar os sistemas SCP, oferecendo endpoints para calendários, eventos e portarias.


## Índice
- [Funcionalidades](#funcionalidades)  
- [Regras de Negócio](#regras-de-negócio)  
- [Tech Stack](#tech-stack)  
- [Estrutura do Projeto](#estrutura-do-projeto)  
- [Instalação](#instalação)  
- [Uso](#uso)  
- [Contribuindo](#contribuindo)  
- [Licença](#licença)  

## Conjunto de Soluções
- **.NET 8** & **C# 12**  
- **Entity Framework Core 8**  
- **Mapster** & **Mapster.EFCore**  
- **LinqKit**  
- **Azure.Identity**  
- **ASP.NET Core Web API**  
- **Swashbuckle / Swagger**  
- **Razor Views** 
- **Blazor** 

## Estrutura do Projeto
```text
CalendarioAcademico/
├── CalendarioAcademico.Domain      # Modelos, DTOs, enums e validações
├── CalendarioAcademico.Data        # DbContext, Repositórios, UnitOfWork, Migrações
└── CalendarioAcademico.WebAPI      # Controllers, Services, Middleware, Program.cs
```

## Funcionalidades
- **Calendários** (`CAD_Calendario`)  
  - CRUD: criar, consultar (por ID, ano ou status), editar e desativar  
- **Eventos** (`EVNT_Evento`)  
  - CRUD: criar, consultar (por calendário, ano, mês ou período), editar e desativar  
- **Portarias** (`PORT_Portaria`)  
  - CRUD: criar, consultar, editar e desativar  
- **Associações Evento ⇄ Portaria** (`EVPT_Evento_Portaria`)  
  - Criar/editar/desativar vínculo entre eventos e portarias  
- **Filtros & Paginação**  
  - Listagens parametrizadas por status, datas, ordenação e páginas  

## Regras de Negócio
1. **Calendário**  
   - **Status**  
     - 0 – Aguardando Aprovação: permite criar/editar eventos  
     - 1 – Aprovado: não permite criar/editar eventos; permite associar portarias  
     - 2 – Desativado: somente consulta histórica  
   - **Transições**  
     - `PATCH /api/calendario/{id}/aprovar` (Administrador)  
     - `PATCH /api/calendario/{id}/desativar` (Administrador)  
2. **Evento**  
   - Só criado/editado quando o calendário está “Aguardando Aprovação”  
   - Desativação somente se calendário não estiver desativado  
3. **Portaria**  
   - CRUD livre, mas só pode editar/desativar antes de vinculá‑la a evento aprovado   
4. **Associação Evento–Portaria**  
   - Só quando calendário estiver com status de “Aprovado”  
   - Datas de vigência da portaria devem caber dentro do evento  
5. **Auditoria**  
   - Em todas as operações `Create/Update/Deactivate`, gravar usuário (`*_CD_Usuario`) e timestamp (`*_DT_DataAtualizacao = DateTime.Now`)  
6. **Transações**  
   - Criar/editar/desativar que envolvam múltiplas entidades devem usar `UnitOfWork.BeginTransactionAsync()` → `CommitAsync()` → `RollbackAsync()`  

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
