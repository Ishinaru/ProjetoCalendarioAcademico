using CalendarioAcademico.Data.DBContext.Repositories.Calendario;
using CalendarioAcademico.Data.DBContext.Repositories.Evento;
using CalendarioAcademico.Data.DBContext.Repositories.EventoPortaria;
using CalendarioAcademico.Data.DBContext.Repositories.Portaria;
using CalendarioAcademico.Data.DBContext.UnitOfWork;
using CalendarioAcademico.Domain.DTO.EventoPortaria;
using CalendarioAcademico.Domain.DTO.Portaria;
using CalendarioAcademico.Domain.Enum;
using CalendarioAcademico.Domain.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Net;


namespace CalendarioAcademico.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortariaController : ControllerBase
    {
        private readonly IUnitOfWork _context;
        public PortariaController(IUnitOfWork context_uow)
        {
            _context = context_uow;
        }

        /// <summary>
        /// Consulta todas as portarias disponíveis.
        /// </summary>
        /// <returns>Uma lista de portarias detalhadas.</returns>
        /// <response code="200">Portarias encontradas.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PortariaDTO_Details>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<PortariaDTO_Details>>> Consultar()
        {
            var portarias = await _context.GetRepository<PortariaRepository, PORT_Portaria>().ListarAsync(p=>p.EVPT_Evento_Portaria);
            var portariasDto = portarias.Adapt<IEnumerable<PortariaDTO_Details>>();
            return Ok(portariasDto);
        }
        /// <summary>
        /// Cadastra uma nova portaria associada a um calendário, modificando datas de eventos.
        /// </summary>
        /// <param name="cad_portaria">Objeto contendo os dados da portaria e os eventos a serem modificados.</param>
        /// <returns>Retorna 201 (Created) com a portaria criada ou mensagens de erro caso contrário.</returns>
        /// <response code="201">Portaria criada com sucesso.</response>
        /// <response code="400">Dados inválidos ou calendário não permite modificação.</response>
        /// <response code="404">Calendário ou eventos não encontrados.</response>
        /// <response code="500">Erro interno ao cadastrar a portaria.</response>
        [HttpPost]
        [ProducesResponseType(typeof(PortariaDTO_Details), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PortariaDTO_Details>> Cadastrar([FromBody] PortariaDTO_Cadastrar cad_portaria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verifica o calendário associado
            var calendario = await _context.GetRepository<CalendarioRepository, CAD_Calendario>().BuscarPorIdAsync(cad_portaria.id_calendario);
            if (calendario == null)
            {
                return NotFound($"Calendário com ID {cad_portaria.id_calendario} não encontrado.");
            }
            if (calendario.CAD_Status != StatusCalendario.Aprovado)
            {
                return BadRequest("Portarias só podem ser criadas para calendários aprovados.");
            }

            // Verifica os eventos fornecidos
            if (cad_portaria.eventos == null || cad_portaria.eventos.Count == 0)
            {
                return BadRequest("Nenhum evento fornecido para modificação pela portaria.");
            }

            var eventos = new List<EVNT_Evento>();
            foreach (var eventoDto in cad_portaria.eventos)
            {
                var evento = await _context.GetRepository<EventoRepository, EVNT_Evento>().BuscarPorIdAsync((int)eventoDto.id_evento);
                if (evento == null)
                {
                    return NotFound($"Evento com ID {eventoDto.id_evento} não encontrado.");
                }
                if (evento.EVNT_CD_Calendario != cad_portaria.id_calendario)
                {
                    return BadRequest($"O evento com ID {eventoDto.id_evento} não pertence ao calendário com ID {cad_portaria.id_calendario}.");
                }
                if (!(bool)evento.EVNT_Ativo)
                {
                    return BadRequest($"O evento com ID {eventoDto.id_evento} está desativado e não pode ser modificado.");
                }
                if (eventoDto.nova_data_final.HasValue && eventoDto.nova_data_inicio.HasValue && eventoDto.nova_data_final < eventoDto.nova_data_inicio)
                {
                    return BadRequest($"A nova data final do evento {eventoDto.id_evento} não pode ser anterior à nova data inicial.");
                }

                eventos.Add(evento);
            }

            // Cria a portaria
            var portaria = new PORT_Portaria
            {
                PORT_NumPortaria = cad_portaria.num_portaria + $"/{calendario.CAD_Ano}",
                PORT_Ano = calendario.CAD_Ano,
                PORT_Ativo = true,
                PORT_Observacao = cad_portaria.observacao,
                PORT_DT_DataAtualizacao = DateTime.Now
            };

            await _context.BeginTransactionAsync();
            try
            {
                // Cadastra a portaria e salva para gerar o ID
                var portariaCriada = await _context.GetRepository<PortariaRepository, PORT_Portaria>().CadastrarAsync(portaria);
                await _context.SaveChangesAsync(); // Garante que o PORT_CD_Portaria seja gerado

                if (eventos.Count != cad_portaria.eventos.Count)
                {
                    await _context.RollbackAsync();
                    return BadRequest("Um ou mais eventos não encontrados ou não estão ativos.");
                }

                foreach (var eventoDto in cad_portaria.eventos)
                {
                    var evento = eventos.FirstOrDefault(e => e.EVNT_CD_Evento == eventoDto.id_evento);
                    if (evento == null) continue; 

                    var associacoesAnteriores = await _context.GetRepository<EventoPortariaRepository, EVPT_Evento_Portaria>()
                        .Listar_Por_Condicao(ep => ep.EVPT_CD_Evento == evento.EVNT_CD_Evento && ep.EVPT_Ativo == true);
                    foreach (var assoc in associacoesAnteriores)
                    {
                        assoc.EVPT_Ativo = false;
                        await _context.GetRepository<EventoPortariaRepository, EVPT_Evento_Portaria>().Atualizar(assoc);
                    }

                    var eventoPortaria = new EVPT_Evento_Portaria
                    {
                        EVPT_CD_Portaria = portariaCriada.PORT_CD_Portaria,
                        EVPT_CD_Evento = evento.EVNT_CD_Evento,
                        EVPT_DT_DataInicio = eventoDto.nova_data_inicio,
                        EVPT_DT_DataFinal = eventoDto.nova_data_final,
                        EVPT_Ativo = true
                    };

                    await _context.GetRepository<EventoPortariaRepository, EVPT_Evento_Portaria>().CadastrarAsync(eventoPortaria);
                }

                await _context.CommitAsync();

                // Monta o DTO de resposta
                var resultadoDto = portariaCriada.Adapt<PortariaDTO_Details>();
                resultadoDto.eventos = cad_portaria.eventos;

                return CreatedAtAction(nameof(Consultar), new { id = portariaCriada.PORT_CD_Portaria }, resultadoDto);
            }
            catch (Exception e)
            {
                await _context.RollbackAsync();
                return StatusCode(500, $"Erro ao cadastrar a portaria: {e.Message}");
            }
        }

        /// <summary>
        /// Edita uma portaria existente e os eventos associados.
        /// </summary>
        /// <param name="id">ID da portaria a ser editada.</param>
        /// <param name="edit_portaria">Objeto contendo os dados atualizados da portaria e eventos.</param>
        /// <returns>Retorna 200 (OK) com a portaria atualizada ou mensagens de erro.</returns>
        /// <response code="200">Portaria editada com sucesso.</response>
        /// <response code="400">Dados inválidos ou calendário não permite edição.</response>
        /// <response code="404">Portaria ou eventos não encontrados.</response>
        /// <response code="500">Erro interno ao editar a portaria.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PortariaDTO_Details), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PortariaDTO_Details>> Editar(int id, [FromBody] PortariaDTO_Edit edit_portaria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Busca a portaria existente com os eventos associados
            var portaria = (await _context.GetRepository<PortariaRepository, PORT_Portaria>()
                .ListarAsync(p => p.EVPT_Evento_Portaria))
                .FirstOrDefault(p => p.PORT_CD_Portaria == id);

            if (portaria == null)
            {
                return NotFound($"Portaria com ID {id} não encontrada.");
            }

            // Verifica o calendário associado aos eventos existentes
            var eventosExistentes = portaria.EVPT_Evento_Portaria.Select(ep => ep.EVPT_CD_Evento).ToList();
            var calendarioIds = (await _context.GetRepository<EventoRepository, EVNT_Evento>()
                .ListarAsync())
                .Where(e => eventosExistentes.Contains(e.EVNT_CD_Evento))
                .Select(e => e.EVNT_CD_Calendario)
                .Distinct()
                .ToList();

            if (calendarioIds.Count != 1)
            {
                return BadRequest("Os eventos associados à portaria pertencem a calendários diferentes, o que não é permitido.");
            }
            int? calendarioId = calendarioIds.First();
            if (!calendarioId.HasValue)
            {
                return BadRequest("O ID do calendário associado é inválido ou nulo.");
            }

            var calendario = await _context.GetRepository<CalendarioRepository, CAD_Calendario>()
                .BuscarPorIdAsync(calendarioId.Value);

            if (calendario == null)
            {
                return NotFound("Calendário associado não encontrado.");
            }
            if (calendario.CAD_Status != StatusCalendario.Aprovado)
            {
                return BadRequest("Não é possível editar uma portaria associada a um calendário que não está aprovado.");
            }

            // Valida os novos eventos fornecidos, se houver
            var eventosAtualizados = new List<EVNT_Evento>();
            if (edit_portaria.eventos != null && edit_portaria.eventos.Any())
            {
                foreach (var eventoDto in edit_portaria.eventos)
                {
                    if (!eventoDto.id_evento.HasValue)
                    {
                        continue;
                    }
                    var evento = await _context.GetRepository<EventoRepository, EVNT_Evento>().BuscarPorIdAsync(eventoDto.id_evento.Value);

                    if (evento == null)
                    {
                        return NotFound($"Evento com ID {eventoDto.id_evento} não encontrado.");
                    }
                    if (evento.EVNT_CD_Calendario != calendario.CAD_CD_Calendario)
                    {
                        return BadRequest($"O evento com ID {eventoDto.id_evento} não pertence ao calendário associado à portaria.");
                    }
                    if (!(bool)evento.EVNT_Ativo)
                    {
                        return BadRequest($"O evento com ID {eventoDto.id_evento} está desativado e não pode ser modificado.");
                    }
                    if (eventoDto.nova_data_final.HasValue && eventoDto.nova_data_inicio.HasValue && eventoDto.nova_data_final < eventoDto.nova_data_inicio)
                    {
                        return BadRequest($"A nova data final do evento {eventoDto.id_evento} não pode ser anterior à nova data inicial.");
                    }

                    eventosAtualizados.Add(evento);
                }
            }

            await _context.BeginTransactionAsync();
            try
            {
                // Atualiza os campos da portaria apenas se os valores fornecidos não forem nulos ou vazios
                if (!string.IsNullOrWhiteSpace(edit_portaria.num_portaria))
                {
                    portaria.PORT_NumPortaria = edit_portaria.num_portaria;
                }
                if (!string.IsNullOrWhiteSpace(edit_portaria.observacao))
                {
                    portaria.PORT_Observacao = edit_portaria.observacao;
                }
                if (edit_portaria.data_atualizacao.HasValue)
                {
                    portaria.PORT_DT_DataAtualizacao = edit_portaria.data_atualizacao.Value;
                }

                if (edit_portaria.desativar.HasValue)
                {
                    portaria.PORT_Ativo = !edit_portaria.desativar.Value;

                    // Se a portaria for desativada, marcar todas as associações como inativas
                    if (!(portaria.PORT_Ativo ?? false))
                    {
                        var associacoes = await _context.GetRepository<EventoPortariaRepository, EVPT_Evento_Portaria>()
                            .Listar_Por_Condicao(ep => ep.EVPT_CD_Portaria == portaria.PORT_CD_Portaria && ep.EVPT_Ativo);

                        foreach (var assoc in associacoes)
                        {
                            assoc.EVPT_Ativo = false;
                            await _context.GetRepository<EventoPortariaRepository, EVPT_Evento_Portaria>().Atualizar(assoc);
                        }
                    }
                }

                await _context.GetRepository<PortariaRepository, PORT_Portaria>().Atualizar(portaria);

                // Atualiza os eventos associados em EVPT_Evento_Portaria
                if (edit_portaria.eventos != null && edit_portaria.eventos.Any())
                {
                    // Remove os registros antigos de EVPT_Evento_Portaria
                    var eventosPortariaExistentes = portaria.EVPT_Evento_Portaria.ToList();
                    foreach (var ep in eventosPortariaExistentes)
                    {
                        await _context.GetRepository<EventoPortariaRepository, EVPT_Evento_Portaria>().Deletar(ep);
                    }

                    // Adiciona os novos registros
                    foreach (var eventoDto in edit_portaria.eventos)
                    {
                        if (!eventoDto.id_evento.HasValue)
                        {
                            continue;
                        }

                        var evento = eventosAtualizados.First(e => e.EVNT_CD_Evento == eventoDto.id_evento);

                        // Desativa associações ativas em outras portarias para este evento
                        var outrasAssociacoes = await _context.GetRepository<EventoPortariaRepository, EVPT_Evento_Portaria>()
                            .Listar_Por_Condicao(
                                ep => ep.EVPT_CD_Evento == evento.EVNT_CD_Evento &&
                                      ep.EVPT_Ativo &&
                                      ep.EVPT_CD_Portaria != portaria.PORT_CD_Portaria &&
                                      (ep.EVPT_CD_PortariaNavigation.PORT_Ativo ?? false),
                                ep => ep.EVPT_CD_PortariaNavigation
                            );

                        foreach (var assoc in outrasAssociacoes)
                        {
                            assoc.EVPT_Ativo = false;
                            await _context.GetRepository<EventoPortariaRepository, EVPT_Evento_Portaria>().Atualizar(assoc);
                        }

                        // Cria nova associação
                        var eventoPortaria = new EVPT_Evento_Portaria
                        {
                            EVPT_CD_Portaria = portaria.PORT_CD_Portaria,
                            EVPT_CD_Evento = evento.EVNT_CD_Evento,
                            EVPT_DT_DataInicio = eventoDto.nova_data_inicio,
                            EVPT_DT_DataFinal = eventoDto.nova_data_final,
                            EVPT_Ativo = true
                        };

                        await _context.GetRepository<EventoPortariaRepository, EVPT_Evento_Portaria>().CadastrarAsync(eventoPortaria);
                    }
                }

                await _context.CommitAsync();

                // Monta o DTO de resposta
                var resultadoDto = portaria.Adapt<PortariaDTO_Details>();
                resultadoDto.eventos = edit_portaria.eventos ?? new List<EventoPortariaDTO>();

                return Ok(resultadoDto);
            }
            catch (Exception e)
            {
                await _context.RollbackAsync();
                return StatusCode(500, $"Erro ao editar a portaria: {e.Message}");
            }
        }
    }
}


