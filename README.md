# Projeto Calendario Acadêmico
Continuação do Projeto do Calendário Acadêmico utilizando as normas padrões da instituição

# Visão Geral

O arquivo `CAD_Calendario.cs` define a classe `CAD_Calendario`, que representa o modelo de um calendário acadêmico no domínio do sistema. Ele utiliza o **Entity Framework Core** para mapeamento de banco de dados e inclui propriedades e relacionamentos que refletem a estrutura e os requisitos do sistema.

## Estrutura da Classe

### Atributos Principais

A classe `CAD_Calendario` contém os seguintes atributos principais:

- **CAD_CD_Calendario**: Chave primária do calendário.
- **CAD_Ano**: Ano associado ao calendário (opcional).
- **CAD_Status**: Status do calendário, representado pelo enum `StatusCalendario`.
- **CAD_DS_Observacao**: Observações gerais sobre o calendário (opcional).
- **CAD_NumeroResolucao**: Número da resolução associada ao calendário (opcional, limitado a 50 caracteres).
- **CAD_DT_DataAtualizacao**: Data e hora da última atualização do calendário.
- **CAD_CD_Usuario**: Identificador do usuário responsável pela última atualização (opcional).
- **CAD_CD_Evento**: Identificador de um evento associado ao calendário (opcional).

### Relacionamentos

- **EVNT_Evento**: Relacionamento de **um-para-muitos** com a entidade `EVNT_Evento`. Um calendário pode conter múltiplos eventos.

### Anotações de Dados

- `[Key]`: Define a chave primária.
- `[Index]`: Cria um índice único na coluna `CAD_Ano`.
- `[StringLength(50)]`: Limita o comprimento da string para 50 caracteres.
- `[Unicode(false)]`: Define que a string não será armazenada como Unicode.
- `[Column(TypeName = "datetime")]`: Especifica o tipo de dado no banco como `datetime`.

### Enumeração Associada

A classe utiliza o enum `StatusCalendario` para representar o status do calendário. Os valores possíveis são:

- `Aguardando_Aprovacao` (0): Calendário aguardando aprovação.
- `Aprovado` (1): Calendário aprovado.
- `Desativado` (2): Calendário desativado.

## Relacionamentos com Outras Entidades

### EVNT_Evento

- Um calendário pode conter múltiplos eventos (`EVNT_Evento`).
- A propriedade `EVNT_Evento` é uma coleção que utiliza a anotação `[InverseProperty]` para mapear o relacionamento.

### Outras Entidades Relacionadas

A classe `EVNT_Evento` possui relacionamentos adicionais com a entidade `EVPT_Evento_Portaria`, que pode ser explorada para detalhar eventos específicos.

## Considerações Técnicas

1. **Índice Único**: O índice único em `CAD_Ano` garante que não existam calendários duplicados para o mesmo ano.
2. **Propriedades Opcionais**: Muitas propriedades são opcionais (nullable), o que oferece flexibilidade, mas pode exigir validações adicionais no código.
3. **Relacionamentos**: O uso de coleções e chaves estrangeiras está bem estruturado, permitindo navegação eficiente entre entidades.

## Sugestões de Melhoria

1. **Validação de Dados**: Implementar validações adicionais para garantir a consistência dos dados, como verificar se `CAD_Ano` é único antes de salvar.
2. **Documentação**: Adicionar comentários XML para descrever as propriedades e seus usos.
3. **Auditoria**: Considerar a implementação de um sistema de auditoria para rastrear alterações em `CAD_DT_DataAtualizacao` e `CAD_CD_Usuario`.
