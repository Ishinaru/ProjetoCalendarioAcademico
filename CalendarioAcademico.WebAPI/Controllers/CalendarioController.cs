using Microsoft.AspNetCore.Mvc;
using CalendarioAcademico.Domain.Models;
using CalendarioAcademico.Data.DBContext.UnitOfWork;
using CalendarioAcademico.Domain.DTO.Calendario;
using CalendarioAcademico.Domain.Enum;
using Mapster;
using System.Net;
using CalendarioAcademico.Data.DBContext.Repositories.Calendario;
using CalendarioAcademico.WebAPI.Services.DiasLetivos;

namespace CalendarioAcademico.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarioController : ControllerBase
    {
        private readonly IUnitOfWork _context;
        private readonly IDiasLetivosService _diasLetivosService;

        public CalendarioController(IUnitOfWork context_uow, IDiasLetivosService diasLetivosService)
        {
            _context = context_uow;
            _diasLetivosService = diasLetivosService;
        }
        #region Consultar Calendários
        /// <summary>
        /// Consulta todos os calendários disponíveis.
        /// </summary>
        /// <returns>Uma lista de itens detalhados.</returns>
        /// <response code="200">Calendários encontrados.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CAD_Calendario>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CAD_Calendario>>> Consultar_Calendarios()
        {
            var calendarios = await _context.GetRepository<CalendarioRepository, CAD_Calendario>().ListarAsync();
            var calendario_dto = calendarios.Adapt<IEnumerable<CalendarioDTO_Details>>();
            return Ok(calendario_dto);
        }
        #endregion
        #region Consultar Calendários Aprovados
        /// <summary>
        /// Consultar todos os calendários com status de aprovado disponíveis.
        /// </summary>
        /// <returns>Retorna 200 (Ok) caso os calendários sejam encontrados ou mensagens de erros caso contrário.</returns>
        /// <response code = "200">Calendários aprovados encontrados.</response>
        /// <response code = "404">Calendários aprovados não foram encontrados.</response>
        /// <response code = "500">Erro interno ao buscar os calendários.</response>
        [HttpGet("calendarios_aprovados")]
        public async Task<ActionResult<IEnumerable<CAD_Calendario>>> Consultar_Calendarios_Aprovados()
        {
            var calendarioRepo = _context.GetRepository<CalendarioRepository, CAD_Calendario>();
            if (calendarioRepo == null)
            {
                return StatusCode(500, "Erro ao acessar o repositório de calendários.");
            }

            var calendarios = await calendarioRepo.Consultar_Aprovados();
            if (calendarios == null || !calendarios.Any())
            {
                return NotFound("Nenhum calendário aprovado encontrado no sistema.");
            }
            var calendarios_dto = calendarios.Adapt<IEnumerable<CalendarioDTO_Details>>();
            return Ok(calendarios_dto);
        }
        #endregion
        #region Editar
        /// <summary>
        /// Edita um calendário existente, permitindo atualizar a observação ou desativá-lo.
        /// </summary>
        /// <param name="id">Identificador do calendário a ser editado.</param>
        /// <param name="edit_dto">Objeto contendo os dados para a atualização do calendário.</param>
        /// <returns>Retorna 204 (No Content) caso o calendário seja criado com sucesso ou mensagens de erro caso contrário.</returns>
        /// <response code="204">Calendário criado com sucesso.</response>
        /// <response code="400">Status do calendário não permite edição ou dados inválidos.</response>
        /// <response code="404">Calendário não encontrado.</response>
        /// <response code="500">Erro interno ao atualizar o calendário.</response>
        [HttpPut("editar/{id}")]
        public async Task<ActionResult<CAD_Calendario>> Editar(int id, [FromBody] CalendarioDTO_Edit edit_dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var calendario = await _context.GetRepository<CalendarioRepository, CAD_Calendario>().BuscarPorIdAsync(id);
            if (calendario == null)
            {
                return NotFound("Calendário não encontrado.");
            }

            if (edit_dto.desativar.HasValue && edit_dto.desativar.Value)
            {
                if (calendario.CAD_Status == StatusCalendario.Desativado)
                {
                    return BadRequest("Calendário já possui o status de desativado.");
                }
                calendario.CAD_Status = StatusCalendario.Desativado;
            }
            else
            {
                if (calendario.CAD_Status != StatusCalendario.Aguardando_Aprovacao)
                {
                    return BadRequest("Não é possível editar um calendário com o status diferente ao de Aguardando Aprovação.");
                }
                calendario.CAD_DS_Observacao = edit_dto.observacao;
            }
            await _context.BeginTransactionAsync();
            try
            {
                await _context.GetRepository<CalendarioRepository, CAD_Calendario>().Atualizar(calendario);
                await _context.CommitAsync();
                return NoContent();
            }
            catch (Exception e)
            {
                await _context.RollbackAsync();
                return StatusCode(500, $"Erro ao atualizar a observação: {e.Message}");
            }
        }
        #endregion
        #region Cadastrar
        // POST: api/Calendario
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Cadastra um novo calendário.
        /// </summary>
        /// <param name="cad_calendario">Objeto contendo os dados para criação do calendário.</param>
        /// <returns>O calendário criado e seu respectivo ID.</returns>
        /// <response code="201">Calendário criado com sucesso.</response>
        /// <response code="400">Dados de entrada inválidos.</response>
        /// <response code="500">Erro interno ao cadastrar o calendário.</response>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<CAD_Calendario>> Cadastrar([FromBody] CalendarioDTO_Cadastrar cad_calendario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var calendario = new CAD_Calendario
            {
                CAD_Ano = cad_calendario.ano,
                CAD_Status = StatusCalendario.Aguardando_Aprovacao,
                CAD_DS_Observacao = cad_calendario.observacao,
                CAD_DT_DataAtualizacao = DateTime.Now
            };

            await _context.BeginTransactionAsync();
            try
            {
                var resultado = await _context.GetRepository<CalendarioRepository, CAD_Calendario>().CadastrarAsync(calendario);
                await _context.CommitAsync();
                return CreatedAtAction(nameof(Consultar_Calendarios), new { id = resultado.CAD_CD_Calendario });
            }
            catch (Exception e)
            {
                await _context.RollbackAsync();
                return StatusCode(500, $"Erro ao cadastrar o calendário: {e.Message}");
            }
        }
        #endregion
        #region Aprovar Calendário
        /// <summary>
        /// Aprova um calendário.
        /// </summary>
        /// <param name="id">Id do calendário a ser aprovado.</param>
        /// <returns>Retorna 204 (No Content) se concluída com sucesso ou mensagens de erro caso contrário.</returns>
        /// <response code = "204">Calendário aprovado com sucesso.</response>
        /// <response code = "400">Calendário já aprovado ou com status inválido para a aprovação.</response>
        /// <response code = "404">Calendário não encontrado ou desativado.</response>
        /// <response code = "500">Erro interno ao atualizar o calendário.</response>
        // Aprovar: api/Calendario/Aprovar/{id}
        [HttpPatch("aprovar/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Aprovar_Calendario(int id)
        {
            try
            {
                var calendario = await _context.GetRepository<CalendarioRepository, CAD_Calendario>().BuscarPorIdAsync(id);
                if (calendario == null || calendario.CAD_Status == StatusCalendario.Desativado)
                {
                    return NotFound("Calendário não encontrado.");
                }
                if (calendario.CAD_Status == StatusCalendario.Aprovado)
                {
                    return BadRequest("Calendário já está com o status de aprovado");
                }

                calendario.CAD_Status = StatusCalendario.Aprovado;

                await _context.BeginTransactionAsync();
                try
                {
                    await _context.GetRepository<CalendarioRepository, CAD_Calendario>().Atualizar(calendario);
                    await _context.CommitAsync();
                    return NoContent();
                }
                catch (Exception e)
                {
                    await _context.RollbackAsync();
                    return StatusCode(500, $"Erro ao aprovar calendário: {e.Message}.");
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erro ao buscar o calendário: {e.Message}.");
            }
        }
        #endregion
        #region Dias Letivos do Calendário por Ano
        /// <summary>
        /// Retorna a quantidade de dias letivos em um ano específico.
        /// </summary>
        /// <param name="ano">Ano do calendário para realizar a contagem de dias letivos.</param>
        /// <returns>Retorna a quantidade de dias letivos do ano ou mensagens de erro caso contrário.</returns>
        /// <response code="200">Retorna a quantidade de dias letivos com sucesso.</response>
        /// <response code="500">Erro interno ao processar a solicitação.</response>
        // Dias Letivos por Ano: api/Calendario/dias_letivos/ano/{ano}
        [HttpGet("dias_letivos/ano/{ano}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Dias_Letivos_Por_Ano(int ano)
        {
            try
            {
                int diasLetivos = await _diasLetivosService.Contar_Dias_Letivos_Ano(ano);
                return Ok(new { DiasLetivos = diasLetivos });
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Erro interno",
                    Detail = $"Erro ao contar dias letivos: {e.Message}",
                    Status = 500
                });
            }
        }
        #endregion
        #region Dias Letivos do Calendário por Mês
        /// <summary>
        /// Retorna a quantidade de dias letivos em um mês específico de um ano.
        /// </summary>
        /// <param name="ano">Ano do calendário para realizar a contagem de dias letivos.</param>
        /// <param name="mes">Mês do calendário para realizar a contagem de dias letivos (deve estar entre 1 e 12).</param>
        /// <returns>Retorna a quantidade de dias letivos do mês ou mensagens de erro caso contrário.</returns>
        /// <response code="200">Retorna a quantidade de dias letivos com sucesso.</response>
        /// <response code="400">Parâmetro inválido (mês fora do intervalo 1-12).</response>
        /// <response code="500">Erro interno ao processar a solicitação.</response>
        // Dias Letivos por Mês: api/Calendario/dias_letivos/ano/{ano}/mes/{mes}
        [HttpGet("dias_letivos/ano/{ano}/mes/{mes}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Dias_Letivos_Por_Mes(int ano, int mes)
        {
            try
            {
                int diasLetivos = await _diasLetivosService.Contar_Dias_Letivos_Mes(ano, mes);
                return Ok(new {DiasLetivos = diasLetivos});
            }
            catch (ArgumentException e)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Parâmetro inválido",
                    Detail = e.Message,
                    Status = 400
                });
            }
            catch(Exception e)
            {
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Erro interno",
                    Detail = $"Erro ao contar dias letivos: {e.Message}",
                    Status = 500
                });
            }
        }
        #endregion
        /*
                private async Task<bool> CalendarioExists(int id)
                {
                    var calendarioRepo = _context.GetRepository<CAD_Calendario>() as ICalendarioRepository;

                    return await calendarioRepo.BuscarPorId(id);
                }*/
    }
}
