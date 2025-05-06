using CalendarioAcademico.Data.DBContext.UnitOfWork;
using CalendarioAcademico.Domain.DTO.Evento;
using CalendarioAcademico.Domain.Models;
using CalendarioAcademico.Domain.Enum;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using CalendarioAcademico.Data.DBContext.Repositories.Evento;
using CalendarioAcademico.Data.DBContext.Repositories.Calendario;
using System.Globalization;
using CalendarioAcademico.WebAPI.Helpers;
using CalendarioAcademico.WebAPI.Services.Dropdown;
using Microsoft.AspNetCore.Mvc.Rendering;
using CalendarioAcademico.WebAPI.Helpers.Validators;
using CalendarioAcademico.Data.DBContext.Repositories.EventoPortaria;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq.Expressions;
using CalendarioAcademico.Domain.DTO.Pagination;
using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace CalendarioAcademico.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : ControllerBase
    {
        private readonly IUnitOfWork _context;
        private readonly IDropdownService _dropdownService;

        public EventoController(IUnitOfWork context_uow, IDropdownService dropdownService)
        {
            _context = context_uow;
            _dropdownService = dropdownService;
        }
        #region Cadastrar
        /// <summary>
        /// Consulta todos os eventos disponíveis.
        /// </summary>
        /// <returns code = "200">Eventos encontrados com sucesso.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EventoDTO_Details>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<EventoDTO_Details>>> Consultar()
        {
            var eventos = await _context.GetRepository<EventoRepository, EVNT_Evento>().ListarAsync();
            var eventos_dto = eventos.Adapt<IEnumerable<EventoDTO_Details>>();
            return Ok(eventos_dto);
        }

        /// <summary>
        /// Cadastra um novo evento associado a um calendário específico.
        /// </summary>
        /// <param name="idCalendario">Identificador do calendário ao qual o evento será associado.</param>
        /// <param name="cad_evento">Objeto contendo os dados para criação do evento.</param>
        /// <returns>Retorna 201 (Created) com o evento criado ou mensagens de erro caso contrário.</returns>
        /// <response code="201">Evento criado com sucesso.</response>
        /// <response code="400">Dados inválidos ou calendário não permite criação.</response>
        /// <response code="404">Calendário não encontrado.</response>
        /// <response code="500">Erro interno ao cadastrar o evento.</response>
        [HttpPost("calendario/{idCalendario}/eventos")]
        [ProducesResponseType(typeof(EventoDTO_Details), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<EventoDTO_Details>> Cadastrar(int idCalendario, [FromBody] EventoDTO_Cadastrar cad_evento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verifica o calendário associado
            var calendario = await _context.GetRepository<CalendarioRepository, CAD_Calendario>().BuscarPorIdAsync(idCalendario);
            if (calendario == null)
            {
                return NotFound($"Calendário com ID {idCalendario} não encontrado.");
            }
            if (calendario.CAD_Status == StatusCalendario.Desativado || calendario.CAD_Status == StatusCalendario.Aprovado)
            {
                return BadRequest("Não é possível criar um evento para um calendário desativado ou aprovado.");
            }

            if (cad_evento.DataFinal < cad_evento.DataInicio)
            {
                return BadRequest("A data final não pode ser anterior à data inicial.");
            }

            var evento = new EVNT_Evento
            {
                EVNT_DT_DataInicio = cad_evento.DataInicio,
                EVNT_DT_DataFinal = cad_evento.DataFinal,
                EVNT_DS_Descricao = cad_evento.Descricao,
                EVNT_UescFunciona = cad_evento.UescFunciona,
                EVNT_Importante = cad_evento.Importante,
                EVNT_TipoFeriado = (int?)cad_evento.TipoFeriado,
                EVNT_Ativo = true,
                EVNT_DT_DataAtualizacao = DateTime.Now,
                EVNT_CD_Calendario = idCalendario
            };

            await _context.BeginTransactionAsync();
            try
            {
                var resultado = await _context.GetRepository<EventoRepository, EVNT_Evento>().CadastrarAsync(evento);
                await _context.CommitAsync();
                var resultadoDto = resultado.Adapt<EventoDTO_Details>();
                return CreatedAtAction(nameof(Listar_Eventos_Por_Calendario), new { idCalendario = resultado.EVNT_CD_Calendario, id = resultado.EVNT_CD_Evento }, resultadoDto);
            }
            catch (Exception e)
            {
                await _context.RollbackAsync();
                return StatusCode(500, $"Erro ao cadastrar o evento: {e.Message}");
            }
        }
        #endregion
        #region Editar
        [HttpPut("editar/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Editar(int id, [FromBody] EventoDTO_Edit edit_dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var evento = await _context.GetRepository<EventoRepository, EVNT_Evento>().BuscarPorIdAsync(id);
            if (evento == null)
            {
                return NotFound("Evento não encontrado.");
            }

            var calendario = await _context.GetRepository<CalendarioRepository, CAD_Calendario>().BuscarPorIdAsync(evento.EVNT_CD_Calendario.Value);
            if (calendario == null)
            {
                return NotFound("Calendário associado não encontrado.");
            }

            // Verifica desativação
            if (edit_dto.Desativar.HasValue && edit_dto.Desativar.Value)
            {
                if (evento.EVNT_Ativo == false)
                {
                    return BadRequest("Evento já está desativado.");
                }
                evento.EVNT_Ativo = false;
            }
            else
            {
                // Só permite edição se o calendário estiver em Aguardando_Aprovacao
                if (calendario.CAD_Status != StatusCalendario.Aguardando_Aprovacao)
                {
                    return BadRequest("Não é possível editar um evento se o calendário não estiver em Aguardando Aprovação.");
                }
                if (evento.EVNT_Ativo == false)
                {
                    return BadRequest("Não é possível editar um evento desativado.");
                }

                if (edit_dto.DataFinal.HasValue && edit_dto.DataInicio.HasValue && edit_dto.DataFinal < edit_dto.DataInicio)
                {
                    return BadRequest("A data final não pode ser anterior à data inicial.");
                }

                evento.EVNT_DT_DataInicio = edit_dto.DataInicio ?? evento.EVNT_DT_DataInicio;
                evento.EVNT_DT_DataFinal = edit_dto.DataFinal ?? evento.EVNT_DT_DataFinal;
                evento.EVNT_DS_Descricao = edit_dto.Descricao ?? evento.EVNT_DS_Descricao;
                evento.EVNT_UescFunciona = edit_dto.UescFunciona ?? evento.EVNT_UescFunciona;
                evento.EVNT_Importante = edit_dto.Importante ?? evento.EVNT_Importante;
                evento.EVNT_TipoFeriado = edit_dto.TipoFeriado ?? evento.EVNT_TipoFeriado;
                evento.EVNT_DT_DataAtualizacao = DateTime.Now;
            }

            await _context.BeginTransactionAsync();
            try
            {
                await _context.GetRepository<EventoRepository, EVNT_Evento>().Atualizar(evento);
                await _context.CommitAsync();
                return NoContent();
            }
            catch (Exception e)
            {
                await _context.RollbackAsync();
                return StatusCode(500, $"Erro ao atualizar o evento: {e.Message}");
            }
        }
        #endregion
        #region Listar Eventos Por Calendário
        /// <summary>
        /// Lista todos os eventos associados a um calendário específico.
        /// </summary>
        /// <param name="idCalendario">Identificador do calendário cujos eventos serão listados.</param>
        /// <returns>Retorna 200 (Ok) com a lista de eventos ou mensagens de erro caso contrário.</returns>
        /// <response code="200">Eventos encontrados para o calendário especificado.</response>
        /// <response code="404">Calendário não encontrado ou nenhum evento associado.</response>
        /// <response code="500">Erro interno ao buscar os eventos.</response>
        [HttpGet("calendario/{idCalendario}/eventos")]
        [ProducesResponseType(typeof(IEnumerable<EventoDTO_Details>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<EventoDTO_Details>>> Listar_Eventos_Por_Calendario(int idCalendario)
        {
            // Verifica se o calendário existe
            var calendario = await _context.GetRepository<CalendarioRepository, CAD_Calendario>().BuscarPorIdAsync(idCalendario);
            if (calendario == null)
            {
                return NotFound($"Calendário com ID {idCalendario} não encontrado.");
            }

            try
            {
                // Busca os eventos associados ao calendário
                var eventos = await _context.GetRepository<EventoRepository, EVNT_Evento>()
                    .Listar_Por_Condicao(e => e.EVNT_CD_Calendario == idCalendario);

                if (!eventos.Any())
                {
                    return NotFound($"Nenhum evento encontrado para o calendário com ID {idCalendario}.");
                }

                // Mapeia para o DTO
                var eventosDto = eventos.Adapt<IEnumerable<EventoDTO_Details>>();
                return Ok(eventosDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erro ao buscar os eventos do calendário {idCalendario}: {e.Message}");
            }
        }
        #endregion
        #region Listar Eventos Por Ano do Calendário
        /// <summary>
        /// Lista todos os eventos associados a um calendário específico por ano.
        /// </summary>
        /// <param name="ano">Ano do calendário cujos eventos serão listados.</param>
        /// <returns>Retorna 200 (Ok) com a lista de eventos ou mensagens de erro caso contrário.</returns>
        /// <response code="200">Eventos encontrados para o calendário especificado.</response>
        /// <response code="404">Calendário não encontrado ou nenhum evento associado.</response>
        /// <response code="500">Erro interno ao buscar os eventos.</response>
        [HttpGet("calendario/{ano}/eventos_por_ano")]
        [ProducesResponseType(typeof(IEnumerable<EventoDTO_Details>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<EventoDTO_Details>>> Listar_Eventos_Por_AnoCalendario(int ano)
        {
            try
            {
                // Busca os eventos associados ao calendário
                var eventos = await _context.GetRepository<EventoRepository, EVNT_Evento>().Evento_Por_Ano(ano);


                if (!eventos.Any())
                {
                    return NotFound($"Nenhum evento encontrado para o calendário com o ano: {ano}.");
                }

                // Mapeia para o DTO
                var eventosDto = eventos.Adapt<IEnumerable<EventoDTO_Details>>();
                return Ok(eventosDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erro ao buscar os eventos do calendário {ano}: {e.Message}");
            }
        }
        #endregion
        #region Listar Eventos por Mês do Calendário
        /// <summary>
        /// Lista todos os eventos de um mês e ano específicos.
        /// </summary>
        /// <param name="mes">Mês do calendário (1 a 12)</param>
        /// <param name="ano">Ano do calendário cujos eventos serão listados.</param>
        /// <returns>Retorna 200 (Ok) com a lista de eventos ou mensagens de erro caso contrário.</returns>
        /// <response code="200">Eventos encontrados para o mês/ano especificado.</response>
        /// <response code = "400">Mês inválido.</response>
        /// <response code="404">Calendário não encontrado ou nenhum evento associado.</response>
        /// <response code="500">Erro interno ao buscar os eventos.</response>
        [HttpGet("calendario/{ano}/{mes}/eventos_por_mes")]
        [ProducesResponseType(typeof(IEnumerable<EventoDTO_Details>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<EventoDTO_Details>>> Listar_Eventos_Por_MesCalendario(int ano, int mes)
        {
            if (mes < 1 || mes > 12)
            {
                return BadRequest("O mês deve estar entre 1 e 12.");
            }

            var mes_format = MonthValidator.ObterNomeDoMes(mes);


                // Busca os eventos associados ao calendário
                var eventos = await _context.GetRepository<EventoRepository, EVNT_Evento>().Evento_Por_Mes(ano, mes);

                if (!eventos.Any())
                {
                    return NotFound($"Nenhum evento encontrado para o mês: {mes_format}.");
                }

                // Mapeia para o DTO
                var eventosDto = eventos.Adapt<IEnumerable<EventoDTO_Details>>();
                return Ok(eventosDto);

        }
        #endregion
        #region Listar Eventos Por Período
        /// <summary>
        /// Lista todos os eventos dentro de um período específico.
        /// </summary>
        /// <param name="dataInicio">Data de início do período (formato: dd/MM/yyyy).</param>
        /// <param name="dataFim">Data de fim do período (formato: dd/MM/yyyy).</param>
        /// <returns>Eventos do período ou mensagem de erro.</returns>
        [HttpGet("eventos_por_periodo")]
        [ProducesResponseType(typeof(IEnumerable<EventoDTO_Details>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<EventoDTO_Details>>> Listar_Eventos_Por_Periodo(
            [FromQuery][ModelBinder(typeof(DateOnlyModelBinder))] DateOnly dataInicio,
            [FromQuery][ModelBinder(typeof(DateOnlyModelBinder))] DateOnly dataFim)
        {
            // Valida o período
            if (dataInicio > dataFim)
            {
                return BadRequest("A data de início não pode ser posterior à data de fim.");
            }

            try
            {
                // Busca eventos do período
                var eventos = await _context.GetRepository<EventoRepository, EVNT_Evento>()
                    .Evento_Por_Periodo(dataInicio, dataFim);

                if (!eventos.Any())
                {
                    return NotFound($"Nenhum evento encontrado entre {dataInicio} e {dataFim}.");
                }

                return Ok(eventos.Adapt<IEnumerable<EventoDTO_Details>>());
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erro ao buscar eventos do período: {e.Message}");
            }
        }
        #endregion
        #region Obter Datas dos Eventos
        //Simular a view para mostrar as datas válidas mais recentes
        /// <summary>
        /// Obtém as datas efetivas de um evento, considerando alterações por portarias ativas.
        /// </summary>
        /// <remarks>
        /// Retorna as datas de início e fim de um evento, priorizando as datas definidas pela portaria ativa mais recente, se houver, ou as datas originais do evento. A origem das datas é indicada no campo "origem".
        /// Exemplo de resposta (com portaria ativa):
        /// ```json
        /// {
        ///   "idEvento": 6,
        ///   "descricao": "Descrição do Evento 6",
        ///   "dataInicio": "2025-04-16",
        ///   "dataFinal": "2025-04-24",
        ///   "origem": "Portaria 001/2025"
        /// }
        /// ```
        /// Exemplo de resposta (sem portaria ativa):
        /// ```json
        /// {
        ///   "idEvento": 6,
        ///   "descricao": "Descrição do Evento 6",
        ///   "dataInicio": "2025-01-01",
        ///   "dataFinal": "2025-01-02",
        ///   "origem": "Evento Original"
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">ID do evento a ser consultado.</param>
        /// <returns>Dados do evento com datas efetivas e origem.</returns>
        /// <response code="200">Datas do evento retornadas com sucesso.</response>
        /// <response code="404">Evento não encontrado.</response>
        /// <response code="500">Erro interno ao consultar as datas do evento.</response>
        [HttpGet("{id}/datas")]
        [SwaggerOperation(Summary = "Obtém as datas efetivas de um evento", Tags = new[] { "Eventos" })]
        [ProducesResponseType(typeof(EventoDTO_View), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<EventoDTO_View>> ObterDatasEvento(int id)
        {
            try
            {
                // Busca o evento
                var evento = await _context.GetRepository<EventoRepository, EVNT_Evento>()
                    .BuscarPorIdAsync(id);

                if (evento == null)
                {
                    return NotFound($"Evento com ID {id} não encontrado.");
                }

                // Busca a associação ativa mais recente (se houver)
                var associacaoAtiva = (await _context.GetRepository<EventoPortariaRepository, EVPT_Evento_Portaria>()
                            .Listar_Por_Condicao(
                                ep => ep.EVPT_CD_Evento == id &&
                                      ep.EVPT_Ativo &&
                                      (ep.EVPT_CD_PortariaNavigation.PORT_Ativo ?? false), // Trata o bool? com valor padrão false
                                ep => ep.EVPT_CD_PortariaNavigation
                            ))
                            .OrderByDescending(ep => ep.EVPT_CD_PortariaNavigation.PORT_DT_DataAtualizacao)
                            .FirstOrDefault();

                // Monta o DTO de resposta
                var resultado = new EventoDTO_View
                {
                    IdEvento = evento.EVNT_CD_Evento,
                    Descricao = evento.EVNT_DS_Descricao,
                    DataInicio = associacaoAtiva != null ? associacaoAtiva.EVPT_DT_DataInicio : evento.EVNT_DT_DataInicio,
                    DataFinal = associacaoAtiva != null ? associacaoAtiva.EVPT_DT_DataFinal : evento.EVNT_DT_DataFinal,
                    Origem = associacaoAtiva != null ? $"Portaria {associacaoAtiva.EVPT_CD_PortariaNavigation.PORT_NumPortaria}" : "Evento Original"
                };

                return Ok(resultado);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erro ao obter as datas do evento: {e.Message}");
            }
        }
        #endregion
        #region Paginação
        /// <summary>
        /// Lista eventos acadêmicos com suporte a paginação, filtros, ordenação e inclusão de dados relacionados.
        /// </summary>
        /// <remarks>
        /// Retorna uma lista paginada de eventos com metadados de paginação (total de registros, total de páginas, etc.).
        /// 
        /// **Parâmetros:**
        /// - `TamanhoPagina`: Quantidade de itens por página (máximo 50, padrão 10).
        /// - `NumeroPagina`: Número da página desejada (padrão 1).
        /// - `CalendarioId`: ID do calendário para filtrar eventos (ex.: 1).
        /// - `TipoFeriado`: Tipo de feriado para filtrar eventos (ex.: 0 = Não feriado, 1 = Municipal, 2 = Estadual, 3 = Nacional).
        /// - `OrdenarPor`: Campo para ordenação (dataInicio, dataFinal, descricao).
        /// - `Ordem`: Direção da ordenação (asc, desc).
        /// - `IncluirRelacionamentos`: Dados relacionados a incluir (ex.: calendario).
        /// </remarks>
        /// <param name="parametros">Parâmetros de filtro, ordenação e inclusão.</param>
        /// <returns>Lista paginada de eventos com metadados.</returns>
        /// <response code="200">Eventos retornados com sucesso.</response>
        /// <response code="400">Parâmetros inválidos (ex.: tamanhoPagina inválido, ordenarPor não suportado).</response>
        /// <response code="500">Erro interno ao consultar eventos.</response>
        [HttpGet("paginacao")]
        [SwaggerOperation(Summary = "Lista eventos com paginação", Tags = ["Eventos"])]
        [ProducesResponseType(typeof(PaginacaoDTO<EventoDTO_Paginacao_View>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PagedList<EventoDTO_Paginacao_View>>> Paginacao([FromQuery] Params_Paginacao_EventoDTO parametros)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Parâmetros inválidos",
                        Detail = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)),
                        Status = 400
                    });
                }

                // Contrução do filtro
                Expression<Func<EVNT_Evento, bool>> filtroExpression = null;
                if (parametros.CalendarioId.HasValue)
                {
                    var filtro = (Expression<Func<EVNT_Evento, bool>>)(e => e.EVNT_CD_Calendario == parametros.CalendarioId.Value);
                    filtroExpression = filtroExpression == null ? filtro : And(filtroExpression, filtro);
                }
                if (parametros.TipoFeriado.HasValue)
                {
                    var filtro = (Expression<Func<EVNT_Evento, bool>>)(e => e.EVNT_TipoFeriado == parametros.TipoFeriado.Value);
                    filtroExpression = filtroExpression == null ? filtro : And(filtroExpression, filtro);
                }

                //Construção da ordenação
                Func<IQueryable<EVNT_Evento>, IOrderedQueryable<EVNT_Evento>> ordenacaoExpression = null;
                if (!string.IsNullOrWhiteSpace(parametros.OrdenarPor))
                {
                    var descendente = !string.IsNullOrWhiteSpace(parametros.Ordem) && parametros.Ordem.ToLower() == "desc";
                    switch (parametros.OrdenarPor.ToLower())
                    {
                        case "datainicio":
                            ordenacaoExpression = descendente
                                ? q => q.OrderByDescending(e => e.EVNT_DT_DataInicio)
                                : q => q.OrderBy(e => e.EVNT_DT_DataInicio);
                            break;
                        case "datafinal":
                            ordenacaoExpression = descendente
                                ? q => q.OrderByDescending(e => e.EVNT_DT_DataFinal)
                                : q => q.OrderBy(e => e.EVNT_DT_DataFinal);
                            break;
                        case "descricao":
                            ordenacaoExpression = descendente
                                ? q => q.OrderByDescending(e => e.EVNT_DS_Descricao)
                                : q => q.OrderBy(e => e.EVNT_DS_Descricao);
                            break;
                        default:
                            return BadRequest(new ProblemDetails
                            {
                                Title = "Ordenação não suportada",
                                Detail = "Use: dataInicio, dataFinal ou descricao.",
                                Status = 400
                            });
                    }
                    
                }

                // Mapear relacionamentos para incluir
                string incluirPropriedades = "";
                if (!string.IsNullOrWhiteSpace(parametros.IncluirRelacionamentos))
                {
                    var relacionamentos = parametros.IncluirRelacionamentos.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(r => r.Trim().ToLower());
                    if (relacionamentos.Contains("calendario"))
                    {
                        incluirPropriedades = "EVNT_CD_CalendarioNavigation";
                    }
                    else
                    {
                        return BadRequest(new ProblemDetails
                        {
                            Title = "Relacionamento não suportado",
                            Detail = "Use: calendario.",
                            Status = 400
                        });
                    }
                }

                var resultadoPaginado = await _context.GetRepository<EventoRepository, EVNT_Evento>()
                    .Listar_Por_Paginacao(
                        filtro: filtroExpression,
                        orderBy: ordenacaoExpression,
                        incluirPropriedades: incluirPropriedades,
                        tamanhoPagina: parametros.TamanhoPagina,
                        numeroPagina: parametros.NumeroPagina
                    );
                // Mapear para DTO
                var eventosDto = resultadoPaginado.Itens.Adapt<List<EventoDTO_Paginacao_View>>();

                // Criar DTO de resposta
                var resposta = new PaginacaoDTO<EventoDTO_Paginacao_View>
                {
                    Itens = eventosDto,
                    TotalRegistros = resultadoPaginado.TotalRegistros,
                    TotalPaginas = resultadoPaginado.TotalPaginas,
                    PaginaAtual = resultadoPaginado.PaginaAtual,
                    TamanhoPagina = resultadoPaginado.TamanhoPagina
                };

                return Ok(resposta);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Erro interno",
                    Detail = $"Erro ao consultar eventos: {e.Message}",
                    Status = 500
                });
            }
        }

        // Método auxiliar para combinar expressões com AND
        private Expression<Func<T, bool>> And<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }

        #endregion
        #region Dropdown Lista de Feriados
        [HttpGet("dropdown_list_tipo_feriado")]
        public async Task<IActionResult> Dropdown_Tipo_Feriado()
        {
            var selectList = _dropdownService.Gerar_SelectList_Enum<TipoFeriado>("Selecione um tipo");
            return Ok(selectList);
        }
        #endregion
        #region Dropdown Meses
        // GET: api/evento/dropdown/meses
        [HttpGet("dropdown/meses")]
        public IActionResult GetDropdownMeses()
        {
            var meses = Enumerable.Range(1, 12).Select(m => new SelectListItem
            {
                Value = m.ToString(),
                Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m)
            }).OrderBy(m => m.Value);

            return Ok(new SelectList(meses, "Value", "Text"));
        }
        #endregion
    }
}
