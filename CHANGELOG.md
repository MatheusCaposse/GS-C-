# Changelog

Segue o [versionamento semântico](https://semver.org/lang/pt-BR/).

## [1.0.0] - 2026-11-12

### Adicionado
- Cadastro de paciente com validação de nome e idade.
- Cálculo de prioridade (Urgente, Alta, Normal) e geração de senha.
- Fila ordenada por prioridade e ordem de chegada.
- Chamar próximo e cancelar paciente.
- Indicadores: total na fila, atendidos e tempo médio de espera.
- Persistência em `dados/fila.json`.
- Tema escuro com ciano.
