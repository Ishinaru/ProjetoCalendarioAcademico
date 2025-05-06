using CalendarioAcademico.Domain.DTO.Calendario;
using CalendarioAcademico.Domain.DTO.Evento;
using CalendarioAcademico.Domain.DTO.EventoPortaria;
using CalendarioAcademico.Domain.DTO.Portaria;
using CalendarioAcademico.Domain.Models;
using Mapster;

namespace CalendarioAcademico.WebAPI.DTOs
{
    public static class MappingConfiguration
    {
        public static void RegisterMaps(this IServiceCollection services)
        {
            #region CAD_Calendario<>CalendarioDTO
            TypeAdapterConfig<CAD_Calendario, CalendarioDTO_Details>
                .NewConfig()
                .Map(dest => dest.id, source: source => source.CAD_CD_Calendario)
                .Map(dest => dest.ano, source: source => source.CAD_Ano)
                .Map(dest => dest.observacao, source: source => source.CAD_DS_Observacao)
                .Map(dest => dest.data_atualizacao, source: source => source.CAD_DT_DataAtualizacao)
                .Map(dest => dest.status, source: source => source.CAD_Status)
                .Map(dest => dest.num_resolucao, source => source.CAD_NumeroResolucao);


            TypeAdapterConfig<CAD_Calendario, CalendarioDTO_Edit>
                .NewConfig()
                .Map(dest => dest.observacao, source: source => source.CAD_DS_Observacao);

            #endregion

            #region CAD_Calendario<>CalendarioDTO_View_Evento
            TypeAdapterConfig<CAD_Calendario, CalendarioDTO_View_Evento>
                .NewConfig()
                .Map(dest => dest.IdCalendario, source => source.CAD_CD_Calendario)
                .Map(dest => dest.Observacao, source => source.CAD_DS_Observacao)
                .Map(dest => dest.Ano, source => source.CAD_Ano)
                .Map(dest => dest.Status, source => source.CAD_Status)
                .Map(dest => dest.NumResolucao, source => source.CAD_NumeroResolucao)
                ;
            #endregion

            #region EVNT_Evento<>EventoDTO
            TypeAdapterConfig<EVNT_Evento, EventoDTO_Details>
                .NewConfig()
                .Map(dest => dest.id, source: source => source.EVNT_CD_Evento)
                .Map(dest => dest.data_inicio, source: source => source.EVNT_DT_DataInicio)
                .Map(dest => dest.data_final, source: source => source.EVNT_DT_DataFinal)
                .Map(dest => dest.ativo, source: source => source.EVNT_Ativo)
                .Map(dest => dest.descricao, source: source => source.EVNT_DS_Descricao)
                .Map(dest => dest.eh_importante, source: source => source.EVNT_Importante)
                .Map(dest => dest.id_calendario, source: source => source.EVNT_CD_Calendario)
                .Map(dest => dest.tipo_de_feriado, source: source => source.EVNT_TipoFeriado)
                .Map(dest => dest.uesc_funciona, source: source => source.EVNT_UescFunciona)
                .Map(dest => dest.data_atualizacao, source: source => source.EVNT_DT_DataAtualizacao)
                ;
            #endregion

            #region EVNT_Evento<>EventoDTO_Paginacao_View
            TypeAdapterConfig<EVNT_Evento, EventoDTO_Paginacao_View>
                .NewConfig()
                .Map(dest => dest.IdEvento, source => source.EVNT_CD_Evento)
                .Map(dest => dest.Descricao, source => source.EVNT_DS_Descricao)
                .Map(dest => dest.DataInicio, source => source.EVNT_DT_DataInicio)
                .Map(dest => dest.DataFinal, source => source.EVNT_DT_DataFinal)
                .Map(dest => dest.Ativo, source => source.EVNT_Ativo)
                .Map(dest => dest.Calendario, source => source.EVNT_CD_CalendarioNavigation)
                ;
            #endregion

            #region PORT_Portaria<>PortariaDTO
            TypeAdapterConfig<PORT_Portaria, PortariaDTO_Details>
                .NewConfig()
                .Map(dest => dest.id, source: source => source.PORT_CD_Portaria)
                .Map(dest => dest.ano, source: source => source.PORT_Ano)
                .Map(dest => dest.num_portaria, source: source => source.PORT_NumPortaria)
                .Map(dest => dest.is_ativo, source: source => source.PORT_Ativo)
                .Map(dest => dest.observacao, source: source => source.PORT_Observacao)
                .Map(dest => dest.data_atualizacao, source: source => source.PORT_DT_DataAtualizacao)
                .Map(dest => dest.eventos, source: source => source.EVPT_Evento_Portaria)
                ;
            #endregion

            #region EVNT_Portaria<>EventoPortariaDTO
            TypeAdapterConfig<EVPT_Evento_Portaria, EventoPortariaDTO>
                .NewConfig()
                .Map(dest => dest.id_evento, source: source => source.EVPT_CD_Evento)
                .Map(dest => dest.nova_data_inicio, source => source.EVPT_DT_DataInicio)
                .Map(dest => dest.nova_data_final, source => source.EVPT_DT_DataFinal)
                ;
            #endregion
        }
    }
}