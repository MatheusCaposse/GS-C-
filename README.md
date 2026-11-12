# Moving Health

Global Solution 2026.2 · FIAP · CitizenTech · C# Software Development

## Ficha da CitizenTech

| Campo | Resposta |
|---|---|
| **Nome da CitizenTech** | Moving Health |
| **Equipe** | Equipe Moving Health |
| **Cidade e bairro** | [Paulista] |
| **Cidadão beneficiado** | Idosos e pessoas com deficiência (PcD) atendidos em uma UBS. Pessoa real: [PREENCHER: quem foi entrevistado, sem dados pessoais reais] |
| **Problema, em uma frase** | Em postos de saúde, idosos e PcD esperam em pé em filas desorganizadas, sem que a prioridade seja controlada. |
| **Como funciona** | Um terminal desktop (Windows Forms) cadastra o paciente, calcula a prioridade, gera uma senha e chama sempre o próximo da fila correta. |
| **O que cada parte faz** | Esta parte (C#): terminal de atendimento da recepção. As demais disciplinas compõem a mesma CitizenTech. |
| **Link do repositório** | [PREENCHER: link do GitHub] |
| **Indicador de impacto** | Tempo médio de espera (em minutos) dos pacientes atendidos, exibido na tela. |
| **ODS conectados** | 3, 10 e 11 (detalhes abaixo) |

## Integrantes

| RM | Nome |
|---|---|
| RM559483 | Matheus Ikeda |
| RM560723 | Davis Júnior |
| RM560340 | Matheus Caposse |

## Problema e pessoa beneficiada

Em muitas UBS, o atendimento é por ordem de chegada ou por organização informal. Idosos, pessoas com deficiência, gestantes e quem chega com sinais de alerta acabam esperando muito tempo, às vezes em pé.

A solução é um terminal simples, para ser usado pelo operador da recepção, que organiza a fila automaticamente e deixa o critério de prioridade claro para todos.

## Regras da fila

| Prioridade | Quando | Senha |
|---|---|---|
| Urgente | Sinais de alerta marcados (falta de ar, dor forte, febre alta) | U001 |
| Alta | 60 anos ou mais, pessoa com deficiência ou gestante | A002 |
| Normal | Demais casos | N003 |

A fila é ordenada por prioridade e, dentro da mesma prioridade, por ordem de chegada.

## Requisitos da disciplina atendidos

- **Lógica e algoritmos:** cálculo de prioridade, ordenação da fila, validações (nome, idade) e cálculo do tempo médio de espera.
- **Persistência básica:** a fila é salva em `dados/fila.json` e recarregada ao abrir o programa.
- **POO:** classes `Paciente`, `FilaService`, `Validador` e `MainForm`, com encapsulamento (nome e idade validados nos *setters*) e separação em camadas (`Models`, `Services`, `Forms`).
- **Interface gráfica:** Windows Forms com campos, caixas de seleção, tabela e botões grandes, fundo escuro e contraste alto em ciano.

## Tecnologias

- C# 12 / .NET 8
- Windows Forms
- `System.Text.Json` para persistência

## Como executar

Requisitos: Windows e o [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
git clone [https://github.com/MatheusCaposse/GS-C-]
cd MovingHealth-GS
dotnet run --project MovingHealth
```

Ou abra a pasta `MovingHealth` no Visual Studio 2022 e pressione F5.

Como usar:
1. Preencha nome e idade, marque as opções que se aplicam e clique em **Gerar senha**.
2. Clique em **Chamar próximo** para atender o primeiro da fila.
3. Selecione um paciente e clique em **Cancelar selecionado** para removê-lo.

## ODS

- **ODS 3 (Saúde e bem-estar):** prioriza quem tem maior necessidade de atendimento e reduz a espera.
- **ODS 10 (Redução das desigualdades):** garante tratamento prioritário a idosos e PcD.
- **ODS 11 (Cidades e comunidades sustentáveis):** melhora o acesso a um serviço público essencial.

## Dados

Nenhum dado pessoal real é usado. Todos os nomes usados em testes são simulados.

## Versionamento semântico e pull request

O projeto segue o [versionamento semântico](https://semver.org/lang/pt-BR/) (`MAJOR.MINOR.PATCH`). As mudanças ficam registradas no [CHANGELOG.md](CHANGELOG.md). Fluxo usado:

```bash
git checkout -b feature/fila-prioritaria
git add .
git commit -m "feat: terminal de fila prioritária Moving Health"
git push -u origin feature/fila-prioritaria
# abrir o Pull Request para a main no GitHub e fazer o merge
git checkout main && git pull
git tag v1.0.0
git push origin v1.0.0
```

## Uso de IA

A IA foi usada como copiloto. Os prompts estão registrados em [prompts.md](prompts.md).
