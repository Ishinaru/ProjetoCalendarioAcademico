namespace CalendarioAcademico.WebAPI.Services.DiasLetivos
{
    public interface IDiasLetivosService
    {
        Task<int> Contar_Dias_Letivos_Ano(int ano);
        Task<int> Contar_Dias_Letivos_Mes(int ano, int mes);

    }
}
