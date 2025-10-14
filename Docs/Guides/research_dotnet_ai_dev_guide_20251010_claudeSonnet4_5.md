# 🚀 Guia Definitivo: Desenvolvimento .NET Turbinado por IA (2025)

## Maximizando Eficiência, Qualidade e Valor como Desenvolvedor .NET com 26 Anos de Experiência

---

## 📊 RESUMO EXECUTIVO - ROI E VALOR

### Dados de Mercado 2025
- **90% dos profissionais de desenvolvimento** já utilizam IA (aumento de 14% vs 2024)
- **80%+ reportam ganhos de produtividade** com ferramentas de IA
- **59% reportam melhoria na qualidade do código**
- **Ganhos realistas de produtividade: 10-30%** em estudos controlados

### Seu ROI Potencial
**Investimento:** $44-79/mês em ferramentas premium
**Retorno:** 15-40% de aumento de produtividade = **mais projetos, entregas mais rápidas, maior valor agregado**

**Meta:** Transformar-se no desenvolvedor "10x" que entrega soluções escaláveis, de alta qualidade, em 1/3 do tempo tradicional.

---

## 🎯 PARTE 1: ECOSSISTEMA DE FERRAMENTAS IA - ESCOLHA ESTRATÉGICA

### 1.1 Paradigmas Fundamentais: Compreendendo Como as Ferramentas Operam

Antes de escolher ferramentas, é crucial entender os **dois paradigmas operacionais** que definem como a IA se integra ao desenvolvimento. Esta distinção é o **framework mental** que organiza todas as decisões sobre ferramentas.

#### **Paradigma 1: Programador em Par (Pair Programmer)**

**Características:**
- Assistência em tempo real, integrada ao IDE
- Sugere código enquanto você digita
- Responde perguntas contextuais instantaneamente
- **Metáfora:** Colega sentado ao seu lado ajudando linha por linha

**Ferramentas neste paradigma:**
- GitHub Copilot
- Gemini Code Assist

**Quando usar:**
- Você **JÁ SABE** o que quer implementar
- Precisa acelerar a escrita de código
- Quer boilerplate e padrões rapidamente
- Está no "fluxo" de codificação

**Exemplo típico:**
```csharp
// Você digita o comentário:
// Método para calcular desconto baseado em quantidade

// Copilot sugere instantaneamente:
public decimal CalculateDiscount(int quantity, decimal price)
{
    if (quantity >= 100) return price * 0.20m;
    if (quantity >= 50) return price * 0.10m;
    if (quantity >= 10) return price * 0.05m;
    return 0;
}
```

---

#### **Paradigma 2: Agente Autônomo (Autonomous Agent)**

**Características:**
- Executa tarefas completas com supervisão mínima
- Recebe instruções de alto nível e planeja autonomamente
- Modifica múltiplos arquivos, executa comandos, roda testes
- **Metáfora:** Desenvolvedor júnior competente a quem você delega tarefas

**Ferramentas neste paradigma:**
- Claude Code

**Quando usar:**
- Você **DEFINE O QUE**, a IA define **COMO**
- Tarefas envolvem múltiplos arquivos
- Refatorações complexas
- Análise de codebase grande

**Exemplo típico:**
```bash
# Você dá uma instrução de alto nível:
> "Refatore a classe OrderProcessor para aplicar o padrão Strategy.
   Separe a lógica de cálculo de frete em estratégias individuais.
   Crie testes xUnit para cada estratégia."

# Claude:
# 1. Analisa OrderProcessor.cs
# 2. Cria IShippingStrategy.cs
# 3. Cria StandardShipping.cs, ExpressShipping.cs, OvernightShipping.cs
# 4. Refatora OrderProcessor.cs
# 5. Cria OrderProcessorTests.cs com todos os cenários
# 6. Mostra diff de TODAS as mudanças para sua aprovação
```

---

#### **A Maestria: Orquestração de Ambos Paradigmas**

**Princípio Fundamental:** Não é "Copilot OU Claude" - é "Copilot E Claude".

A verdadeira eficiência vem de **quando** usar cada um:

| Situação | Ferramenta | Razão |
|----------|------------|-------|
| Escrevendo um novo método simples | Copilot | Assistência em tempo real é mais rápida |
| Implementando uma feature completa (múltiplos arquivos) | Claude Code | Visão holística e execução autônoma |
| Debugando erro pontual | Copilot Chat | Contexto local, resposta imediata |
| Entendendo arquitetura de sistema legado | Claude Code | Janela de contexto gigante (200K tokens) |
| Escrevendo testes para uma classe | Copilot /tests | Comando especializado otimizado |
| Refatorando para aplicar SOLID em múltiplas classes | Claude Code | Mudanças estruturais cross-file |

**Implicação Prática:** Você se torna um **"Arquiteto-Diretor"**:
- Usa Copilot como **assistente pessoal** no dia a dia
- Delega tarefas maiores ao Claude como **desenvolvedor júnior**

---

### 1.2 Claude (Anthropic) - Seu Assistente Principal para .NET

**Por que Claude é superior para .NET em 2025:**
- Contexto de 200K tokens (processa codebases inteiras)
- Melhor compreensão de arquitetura e padrões SOLID
- Excelente para explicar conceitos complexos
- Artifacts para prototipagem rápida

**Como Usar Claude Eficientemente:**

#### A) Claude Chat (Web/Desktop/Mobile)
**Casos de Uso:**
- Arquitetura e planejamento de sistemas
- Code review detalhado
- Debugging complexo
- Geração de documentação técnica
- Análise de trade-offs arquiteturais

**Técnica do Projeto Claude:**
1. Crie um Projeto para cada aplicação/módulo
2. Faça upload de arquivos-chave: `CLAUDE.md`, documentos de arquitetura, schemas
3. Use como "memória do projeto" - Claude mantém contexto entre conversas

**CLAUDE.md - Arquivo Fundamental:**
```markdown
# Contexto do Projeto
- Stack: .NET 8, ASP.NET Core, Entity Framework Core
- Arquitetura: Clean Architecture + CQRS + Event Sourcing
- Padrões: Repository, Unit of Work, Mediator
- Banco: PostgreSQL + Redis
- Mensageria: RabbitMQ
- Testes: xUnit, Moq, FluentAssertions

# Comandos Comuns
- `dotnet build`
- `dotnet test --logger "console;verbosity=detailed"`
- `docker-compose up -d`

# Convenções de Código
- Usar async/await para operações I/O
- Injeção de dependência via constructor
- Validação com FluentValidation
- Mapeamento com Mapster (não AutoMapper)
- DTOs para todas as respostas de API

# Estrutura de Pastas
- src/Domain: Entidades e interfaces
- src/Application: Use cases e DTOs
- src/Infrastructure: Implementações concretas
- src/API: Controllers e configuração
```

#### B) Claude Code (CLI) - Para Desenvolvimento Ativo
**Claude Code é uma ferramenta de linha de comando para codificação agêntica**

**Casos de Uso Ideais:**
- Refatoração de múltiplos arquivos
- Implementação de features completas
- Migrações de código (ex: .NET Framework → .NET 8)
- Criação de testes automatizados
- Setup inicial de projetos

**Workflow Recomendado:**
```bash
# 1. Sempre criar branch antes
git checkout -b feature/nova-funcionalidade

# 2. Iniciar Claude Code no diretório do projeto
claude-code

# 3. Workflow de 3 passos (CRÍTICO)
> Step 1: Research - "Analise o código existente e entenda como implementar [feature]"
> Step 2: Plan - "Crie um plano de implementação passo-a-passo"
> Step 3: Implement - "Implemente o plano, começando pelos testes"

# 4. Usar TDD quando possível
> "Escreva os testes para [feature] primeiro, confirme que falham, depois implemente"
```

**Melhores Práticas Claude Code:**
- **Use tab-completion** para referenciar arquivos rapidamente
- **Cole URLs de documentação** relevante (ex: docs.microsoft.com)
- **Execute múltiplas instâncias** em diferentes terminais para trabalho paralelo
- **Adicione imagens/screenshots** (cmd+ctrl+shift+4 no Mac) para UI

---

### 1.3 IDEs com IA - Sua Escolha Estratégica

#### **OPÇÃO 1: Cursor (Recomendado para Desenvolvedores .NET Experientes)**

**Prós:**
- Melhor para prototipagem rápida e iterações rápidas
- Composer: edições em múltiplos arquivos simultaneamente
- Integração perfeita com VS Code (fork oficial)
- Auto-importa símbolos TypeScript/Python automaticamente
- Feedback loop extremamente rápido

**Contras:**
- Pode ter problemas com contexto em projetos muito grandes (100K+ linhas)
- $20/mês (mais caro que alternativas)

**Quando Usar Cursor:**
- Projetos novos e greenfield
- Prototipagem de MVPs
- Refatorações localizadas
- Desenvolvimento solo ou times pequenos (<5 pessoas)

**Setup Cursor para .NET:**
```json
// .cursor/rules.json
{
  "rules": [
    "Sempre usar async/await para operações I/O",
    "Preferir record types para DTOs",
    "Usar minimal APIs quando apropriado",
    "Implementar health checks para todos os serviços",
    "Adicionar logging estruturado com Serilog"
  ]
}
```

#### **OPÇÃO 2: Windsurf (Melhor para Codebases Grandes e Times)**

**Prós:**
- **Cascade:** Melhor compreensão de projetos grandes e multi-módulo
- Excelente para navegação em serviços grandes e monorepos
- UI mais limpa e refinada que Cursor
- $15/mês (25% mais barato que Cursor)
- Melhor em projetos complexos - teste MVP mostrou maior taxa de sucesso

**Contras:**
- Sugestões mais lentas que Cursor
- Curva de aprendizado um pouco maior

**Quando Usar Windsurf:**
- Projetos enterprise com múltiplos serviços
- Monorepos com vários microserviços
- Onboarding de novos desenvolvedores
- Quando precisar de consciência arquitetural profunda

#### **OPÇÃO 3: GitHub Copilot (Melhor para Integração Enterprise)**

**Prós:**
- $10/mês - melhor custo-benefício
- Funciona em VS Code, Visual Studio, JetBrains, Vim
- 78% das Fortune 500 usam (SOC 2 compliant)
- Integração nativa com GitHub Actions
- Tier gratuito disponível (12K completions/mês)

**Contras:**
- Contexto de projeto mais limitado que Cursor/Windsurf
- Recursos de agente menos maduros

**Quando Usar Copilot:**
- Ambientes enterprise com requerimentos de compliance
- Times distribuídos usando IDEs variadas
- Orçamento limitado
- Integração forte com GitHub necessária

#### **OPÇÃO 4: Lovable AI (Para Protótipos Web Full-Stack Rápidos)**

**O que é:**
- Plataforma que gera aplicações full-stack completas a partir de prompts em linguagem natural
- Gera React + Node.js + Supabase automaticamente
- Promete desenvolvimento "20x mais rápido"

**Prós:**
- Exporta código editável para GitHub
- Perfeito para MVPs e POCs rapidíssimos
- Integração com Supabase para auth e database

**Contras:**
- **CRÍTICO:** Vulnerabilidade de segurança identificada (VibeScamming) - pode gerar sites de phishing facilmente
- Limitado para lógica de negócio complexa
- Focado em web, não em APIs .NET

**Quando Usar:**
- Protótipos descartáveis para demonstração
- Frontends rápidos para testar APIs .NET
- Landing pages e ferramentas internas simples
- ⚠️ **NÃO usar para produção sem revisão de segurança completa**

**Integração com .NET:**
```bash
# Use Lovable para gerar frontend React rapidamente
# Conecte com sua API .NET via configuração de backend

# No Lovable prompt:
"Crie um dashboard React que consome uma API REST em https://api.meuservico.com
- Autenticação JWT
- Exibir lista de produtos
- Formulário de cadastro com validação"

# Depois, exporte o código e ajuste para sua API .NET
```

---

### 1.4 Workflow: Inner Loop vs Outer Loop - O Framework de Decisão

Esta distinção é **fundamental** para usar IA eficientemente. Cada tipo de ciclo exige uma ferramenta diferente.

#### **Inner Loop (Ciclo Interno) - Micro-Tarefas**

```
Codificar → Compilar → Testar → Depurar
(ciclo rápido, repetitivo, segundos a minutos)
```

**Características:**
- ⚡ **Velocidade é crítica**
- 🔄 Iterações frequentes (segundos/minutos)
- 🎯 Foco em linha/método/classe individual
- 💭 Desenvolvedor já sabe **O QUE** implementar

**Ferramenta ideal:** GitHub Copilot
- Autocomplete inteligente
- Geração de boilerplate
- Sugestões contextuais instantâneas
- Comando /tests para testes rápidos

**Exemplos .NET no Inner Loop:**
- Escrever propriedades de uma entidade EF Core
- Implementar método CRUD em controller
- Adicionar validações com Data Annotations
- Gerar construtor com injeção de dependências
- Escrever queries LINQ simples

---

#### **Outer Loop (Ciclo Externo) - Macro-Tarefas**

```
Planejar feature → Implementar múltiplos arquivos → Integrar → Revisar
(ciclo mais longo, estratégico, horas a dias)
```

**Características:**
- 🧠 **Compreensão é crítica**
- 📁 Múltiplos arquivos envolvidos
- 🏗️ Mudanças arquiteturais
- 💡 Desenvolvedor define **O QUE**, IA define **COMO**

**Ferramenta ideal:** Claude Code
- Análise de codebase completa
- Refatorações multi-arquivo
- Implementação de features end-to-end
- Janela de contexto massiva (200K tokens)

**Exemplos .NET no Outer Loop:**
- Refatorar serviço para aplicar SOLID
- Implementar CQRS em um módulo
- Migrar .NET Framework → .NET 8
- Adicionar logging estruturado em todo o sistema
- Criar camada de testes de integração

---

#### **Tabela de Decisão: Tarefa → Ferramenta**

| Tarefa de Desenvolvimento .NET | Ciclo | Ferramenta Ótima | Racional |
|-------------------------------|-------|------------------|----------|
| **Escrever nova entidade EF Core** | Inner | GitHub Copilot | Boilerplate e padrões. Copilot sugere propriedades, anotações, construtores baseado em exemplos. |
| **Implementar método de controller REST** | Inner | GitHub Copilot | Assistência em tempo real crucial. Sugere implementação baseada em assinatura e serviços injetados. |
| **Refatorar classe para Strategy pattern** | Outer | Claude Code | Requer compreensão holística. Claude analisa classe inteira e reestrutura em interface + implementações. |
| **Compreender método legado de 1000 linhas** | Outer | Claude Code | Janela de contexto essencial. Claude "lê" método inteiro e fornece resumo + estratégia de refatoração. |
| **Gerar testes xUnit para controller** | Inner | Copilot Chat | Comando /tests otimizado. Gera testes direcionados com mocks básicos no IDE. |
| **Atualizar projeto .NET Framework → .NET 9** | Outer | Copilot @modernize | Tarefa especializada multi-passos. Agente analisa dependências e corrige compatibilidade. |
| **Adicionar endpoint REST com validações** | Inner | Copilot | Padrão comum. Copilot sugere estrutura completa baseada em endpoints existentes. |
| **Implementar Event Sourcing em módulo** | Outer | Claude Code | Arquitetura complexa. Exige criação de múltiplas classes (Events, Handlers, Projections). |
| **Escrever query LINQ complexa** | Inner | Copilot | Sugestão contextual. Copilot entende entidades e relações EF Core. |
| **Adicionar observability (logs, metrics, traces)** | Outer | Claude Code | Cross-cutting concern. Requer mudanças em múltiplos arquivos e camadas. |

---

#### **Workflow Simbiótico em Ação - Exemplo Real**

**Cenário:** Implementar feature "Aplicar Cupom de Desconto ao Pedido"

**Outer Loop (Claude Code):**
```bash
> "Implemente feature de cupom de desconto:
   1. Crie entidade Coupon (EF Core)
   2. Adicione propriedade CouponId em Order
   3. Crie método ApplyCoupon no OrderService
   4. Adicione endpoint POST /orders/{id}/apply-coupon
   5. Crie testes de integração
   
   Regras: Cupom pode ser % ou valor fixo, validar expiração."
```

Claude cria estrutura completa: migrations, DTOs, validações, testes.

**Inner Loop (Copilot):**
Depois, você abre `OrderService.cs` e começa a adicionar lógica de negócio específica:

```csharp
// Você digita:
// Validar se cupom já foi usado pelo cliente

// Copilot autocompleta:
if (await _context.OrderCoupons
    .AnyAsync(oc => oc.CouponId == couponId && oc.CustomerId == customerId))
{
    throw new BusinessException("Cupom já utilizado por este cliente");
}
```

**Resultado:** Claude fez o "trabalho pesado" (arquitetura), Copilot acelerou os detalhes.

---

### 1.5 Comparação Rápida - Escolha Sua Ferramenta

| Ferramenta | Preço/Mês | Melhor Para | Limitações |
|------------|-----------|-------------|------------|
| **Claude Chat** | $20 (Pro) | Arquitetura, planejamento, code review, debugging | Sem integração direta com IDE |
| **Claude Code** | Incluído no Pro | Implementação de features, refatoração, TDD | CLI apenas, curva de aprendizado |
| **Cursor** | $20 | Prototipagem rápida, projetos pequenos/médios | Contexto limitado em projetos grandes |
| **Windsurf** | $15 | Codebases enterprise, monorepos, times | Mais lento que Cursor |
| **GitHub Copilot** | $10 | Enterprise, compliance, times distribuídos | Recursos de agente limitados |
| **Lovable** | $25-30 | Protótipos web full-stack ultra-rápidos | ⚠️ Problemas de segurança, não para .NET backend |

**Recomendação para Desenvolvedor .NET com 26 Anos de Experiência:**

**Stack Ideal = Claude Pro ($20) + Windsurf ($15) = $35/mês**

**Por quê?**
- Claude Pro: Para arquitetura, planejamento, e Claude Code CLI
- Windsurf: Para desenvolvimento diário em codebases .NET complexas
- Total: $35/mês para máxima produtividade

**Alternativa Budget = Claude Pro ($20) + Copilot ($10) = $30/mês**

---

## 🎓 PARTE 2: PROMPT ENGINEERING PARA .NET - TÉCNICAS AVANÇADAS

### 2.1 Framework de Prompt para Desenvolvimento .NET

**Estrutura de 4 Componentes:**

1. **Persona:** Define o papel do AI
2. **Contexto:** Fornece informações relevantes
3. **Task:** Descreve claramente o que precisa fazer
4. **Format:** Especifica o formato da resposta

**Exemplo Prático:**

```
PERSONA: Você é um arquiteto de software sênior .NET especializado em microservices e clean architecture.

CONTEXTO: Estou desenvolvendo um serviço de pedidos em uma arquitetura de microservices. O serviço precisa se comunicar com os serviços de Pagamento e Estoque. Stack atual: .NET 8, RabbitMQ, PostgreSQL, Docker.

TASK: Projete a arquitetura deste serviço seguindo:
- Clean Architecture (Domain, Application, Infrastructure, API)
- CQRS pattern
- Event-driven communication com RabbitMQ
- Resilience patterns (Circuit Breaker, Retry, Timeout)
- Health checks e observability

FORMAT: Forneça:
1. Diagrama em texto da arquitetura
2. Estrutura de pastas e responsabilidades
3. Exemplos de código para:
   - Command Handler de criação de pedido
   - Event publisher para OrderCreated
   - Circuit Breaker configuration
4. Recomendações de libraries (Polly, MassTransit, etc)
```

### 2.2 Prompts Específicos por Caso de Uso

#### A) Code Review de Qualidade Enterprise

```
PERSONA: Você é um lead developer .NET focado em qualidade de código e boas práticas.

CONTEXTO: [cole o código aqui]

TASK: Realize um code review completo identificando:
1. Violações de SOLID
2. Problemas de performance
3. Vulnerabilidades de segurança (OWASP Top 10)
4. Questões de thread-safety
5. Oportunidades de refatoração
6. Testes ausentes

FORMAT: Tabela com colunas:
| Issue | Severidade (1-5) | Linha | Explicação | Solução Sugerida | Exemplo de Código Corrigido |
```

#### B) Geração de Testes Automatizados

```
PERSONA: Você é um especialista em testes automatizados .NET (unit, integration, e2e).

CONTEXTO: [cole o código da classe/método]

TASK: Gere uma suíte de testes completa incluindo:
- Unit tests (xUnit + Moq)
- Test cases para cenários:
  * Happy path
  * Edge cases (valores nulos, vazios, limites)
  * Error handling
  * Concorrência (se aplicável)
- Integration tests se necessário
- Arrange-Act-Assert pattern
- Naming: MethodName_Scenario_ExpectedResult

FORMAT: Código completo de testes com:
1. Setup e fixtures necessários
2. Todos os test cases
3. Comentários explicando cenários não-óbvios
4. Coverage esperado: >85%
```

#### C) Otimização de Performance

```
PERSONA: Você é um especialista em performance .NET e otimização de sistemas de alta escala.

CONTEXTO: Este código está apresentando lentidão em produção:
[cole código + métricas se tiver: tempo resposta, memory usage, CPU]

TASK: Analise e otimize para:
1. Tempo de resposta <200ms
2. Redução de alocações de memória
3. Minimizar queries ao banco (N+1 problem)
4. Uso adequado de async/await
5. Caching strategy

FORMAT:
1. Lista de problemas identificados com impacto estimado
2. Código otimizado com comentários explicando mudanças
3. Benchmarks esperados (antes/depois)
4. Recommendations adicionais (indexação de banco, etc)
```

#### D) Implementação de Feature Completa (TDD)

```
PERSONA: Você é um desenvolvedor .NET experiente que pratica TDD rigoroso.

CONTEXTO: Preciso implementar uma feature de "Carrinho de Compras" com:
- Adicionar/remover produtos
- Calcular total com descontos
- Validações de estoque
- Persistência no Redis
- API REST
- Arquitetura: Clean Architecture + CQRS

TASK: Implementação completa usando TDD:
1. Primeiro, escreva TODOS os testes (red phase)
2. Depois, implemente o código para passar os testes (green phase)
3. Por fim, refatore mantendo testes verdes (refactor phase)

FORMAT:
## Fase 1: Testes (RED)
[Todos os testes falhando]

## Fase 2: Implementação (GREEN)
[Código mínimo para passar testes]

## Fase 3: Refatoração
[Código final otimizado]

## Resultado:
- Cobertura de testes
- Decisões arquiteturais tomadas
- Trade-offs considerados
```

### 2.3 Técnicas Avançadas de Prompting

#### A) Chain-of-Thought (Raciocínio Passo-a-Passo)

**Técnica:** Force o modelo a explicar seu raciocínio

```
"Antes de implementar, explique seu raciocínio passo-a-passo:
1. Qual padrão de design é mais apropriado?
2. Por que esse padrão e não alternativas?
3. Quais são os trade-offs?
4. Como isso impacta testabilidade?
5. Quais dependências serão necessárias?

Só depois disso, forneça a implementação."
```

#### B) Few-Shot Learning (Exemplos)

```
"Aqui estão exemplos do nosso estilo de código:

EXEMPLO 1 - Command Handler:
[cole exemplo existente]

EXEMPLO 2 - Event Publisher:
[cole exemplo existente]

Agora crie um novo Command Handler para ProcessPayment seguindo exatamente este estilo e convenções."
```

#### C) Self-Reflection (Auto-Crítica)

```
"Após gerar o código, faça uma análise crítica:
1. Este código está production-ready?
2. O que pode dar errado?
3. Quais edge cases não foram cobertos?
4. Como isso escala para 1000 requisições/segundo?
5. Onde estão os potenciais bottlenecks?

Então, melhore o código baseado nesta análise."
```

---

### 2.4 Manual de Prompts Arquiteturais - Templates Reutilizáveis

Esta seção fornece um **arsenal de prompts copy-paste ready** para automatizar tarefas arquiteturais de alto valor. Cada template é projetado para gerar código de nível profissional aderente a padrões estabelecidos.

#### **Princípios dos Prompts Arquiteturais**

1. **Contexto é Rei:** Qualidade de saída = qualidade de entrada
   - Inclua: domínio do problema, objetivos de negócio, restrições técnicas
   - Exemplo: "Sistema de gerenciamento de inventário para varejo com alta concorrência"

2. **Padrão de Persona:** Instrua a IA a adotar expertise específica
   - Exemplo: "Aja como arquiteto de software sênior especializado em aplicações .NET nativas da nuvem"

3. **Saída Estruturada:** Solicite formatos específicos
   - Tabelas Markdown, JSON, código em estrutura de pastas específica
   - Força organização e facilita parsing programático

4. **Chain-of-Thought:** Pedir para "pensar passo a passo"
   - Leva a resultados mais precisos e verificáveis
   - Força decomposição do problema

---

#### **Tabela de Templates Prontos para Uso**

| Tarefa Arquitetural | Template de Prompt | Variáveis Chave | Saída Esperada |
|---------------------|-------------------|----------------|----------------|
| **Scaffolding Clean Architecture** | `Aja como arquiteto .NET especialista. Projete solução C# com Arquitetura Limpa para Web API de '<DOMÍNIO>'. Inclua projetos: Domain, Application, Infrastructure, Presentation (ASP.NET Core). Defina pastas e interfaces chave como <INTERFACE_EXEMPLO> no Domain e implementação na Infrastructure usando <TECNOLOGIA>.` | `<DOMÍNIO>`, `<INTERFACE_EXEMPLO>`, `<TECNOLOGIA>` | Estrutura de arquivos + snippets de código iniciais para solução .NET seguindo Clean Architecture |
| **Refatoração SOLID** | `Analise a classe C# '<CLASSE>'. Refatore para aderir a <PRINCÍPIO_SOLID>. Explique violações no código original e justifique alterações. [Cole código C# aqui]` | `<CLASSE>`, `<PRINCÍPIO_SOLID>` | Código refatorado + explicação detalhada das melhorias |
| **Geração de Testes Unitários** | `Gere suíte de testes xUnit para classe C# '<CLASSE>'. Cubra: casos de sucesso, falha, edge cases. Use Moq para simular '<DEPENDÊNCIA>'. Estruture com padrão Arrange-Act-Assert. [Cole código da classe e interface]` | `<CLASSE>`, `<DEPENDÊNCIA>` | Arquivo de teste C# completo com testes xUnit e configurações Moq |
| **Implementação de Design Pattern** | `Tenho classe C# que <DESCRIÇÃO_PROBLEMA>. Refatore para usar padrão <PADRÃO> (ex: Strategy, Factory, Repository). Crie interfaces e classes necessárias. [Cole código C# relevante]` | `<DESCRIÇÃO_PROBLEMA>`, `<PADRÃO>` | Código C# refatorado implementando o padrão, com estrutura de classes apropriada |
| **Análise de Trade-Offs** | `Crie matriz de trade-offs em Markdown comparando <OPÇÃO_1> vs <OPÇÃO_2> para <CONTEXTO_PROJETO>. Avalie baseado em: <CRITÉRIO_1>, <CRITÉRIO_2>, <CRITÉRIO_3>.` | Opções arquiteturais, Contexto, Lista de critérios | Tabela Markdown clara comparando abordagens, facilitando decisão |
| **Migração de Framework** | `Analise projeto .NET Framework <VERSÃO_ORIGEM> e crie plano de migração para .NET <VERSÃO_DESTINO>. Identifique: dependências incompatíveis, APIs obsoletas, mudanças necessárias. Priorize por risco e esforço.` | `<VERSÃO_ORIGEM>`, `<VERSÃO_DESTINO>` | Plano de migração estruturado com checklist e estimativas |
| **API REST RESTful** | `Projete API REST para domínio <DOMÍNIO>. Defina: endpoints (verbos HTTP corretos), DTOs, status codes, versionamento, autenticação JWT. Siga Richardson Maturity Model nível 2+.` | `<DOMÍNIO>` | Especificação OpenAPI/Swagger + DTOs C# + exemplos de controllers |
| **Event-Driven Architecture** | `Desenhe arquitetura orientada a eventos para <CASO_DE_USO>. Defina: eventos de domínio, handlers, estratégia de persistência (Event Sourcing ou Event Notification). Use <BROKER> (RabbitMQ/Kafka).` | `<CASO_DE_USO>`, `<BROKER>` | Diagrama de eventos + classes C# (Events, Handlers, Configuration) |

---

#### **Prompts Completos - Exemplos Detalhados**

##### **1. O "Scaffolder de Arquitetura Limpa"**

```
PERSONA: Você é um arquiteto de software sênior .NET especializado em Clean Architecture e DDD.

CONTEXTO: Preciso criar uma API para gerenciamento de produtos de e-commerce. 
Requisitos: CRUD de produtos, categorias, controle de estoque, integração com ERP externo.

TASK: Projete uma solução C# completa com Arquitetura Limpa incluindo:
1. Projetos: Domain, Application, Infrastructure, Presentation (ASP.NET Core Web API)
2. Estrutura de pastas dentro de cada projeto
3. Interfaces principais (ex: IProductRepository, IUnitOfWork)
4. Exemplo de entidade Product com Value Objects
5. Exemplo de Use Case (Command/Query) usando MediatR

TECNOLOGIAS: Entity Framework Core, PostgreSQL, MediatR, AutoMapper

FORMAT: 
- Estrutura de diretórios em formato de árvore
- Código C# para classes chave com comentários explicativos
- Justificativa das decisões arquiteturais
```

**Quando usar:** Início de projeto greenfield, documentação de arquitetura existente.

---

##### **2. O "Especialista em Refatoração SOLID"**

```
PERSONA: Você é um especialista em princípios SOLID e refatoração de código .NET.

CONTEXTO: A classe abaixo viola múltiplos princípios SOLID e precisa ser refatorada.

TASK: 
1. Identifique TODAS as violações SOLID no código atual
2. Refatore aplicando:
   - Single Responsibility Principle (SRP)
   - Dependency Inversion Principle (DIP)
   - Open/Closed Principle (OCP)
3. Explique CADA violação original
4. Justifique CADA mudança na versão refatorada

CODE:
[Cole aqui sua classe OrderProcessor.cs que faz tudo: validação, cálculo, envio email, log, acesso DB direto]

FORMAT:
## Violações Identificadas
(lista numerada com linha de código)

## Código Refatorado
(código completo com comentários)

## Justificativas
(explicação decisão por decisão)
```

**Quando usar:** Code review, melhoria de código legado, treinamento de equipe.

---

##### **3. O "Gerador de Suíte de Testes Completa"**

```
PERSONA: Você é um especialista em testes automatizados .NET (TDD practitioner).

CONTEXTO: Preciso de cobertura de testes completa para a classe de serviço abaixo.

TASK: Gere suíte de testes xUnit incluindo:
1. Testes de casos de sucesso (happy path)
2. Testes de casos de falha (exceções esperadas)
3. Testes de edge cases (null, vazio, limites)
4. Mocks com Moq para todas as dependências
5. Arrange-Act-Assert pattern rigoroso
6. Naming: MethodName_Scenario_ExpectedBehavior

DEPENDÊNCIAS A MOCKAR:
- IProductRepository
- IEmailService
- ILogger<ProductService>

CODE:
[Cole ProductService.cs + IProductRepository.cs]

FORMAT: Arquivo completo ProductServiceTests.cs executável, target 90%+ coverage
```

**Quando usar:** Criar testes para código novo, aumentar cobertura de legado.

---

##### **4. O "Implementador de Padrões de Projeto"**

```
PERSONA: Você é um especialista em Design Patterns GoF aplicados a C#/.NET.

CONTEXTO: Tenho código com lógica condicional complexa que deve ser refatorado para um padrão.

PROBLEMA ATUAL:
Classe ShippingCalculator com bloco if-else-if gigante para calcular frete:
- Standard: $5 fixo
- Express: $15 fixo
- Overnight: $25 + $2 por kg
- International: $50 + $10 por kg + taxa alfandegária

TASK:
1. Refatore usando padrão Strategy
2. Crie interface IShippingStrategy
3. Implemente classe concreta para cada método de envio
4. Modifique ShippingCalculator para usar o padrão
5. Demonstre como adicionar novo método (SameDay) SEM modificar código existente (OCP)

CODE: [Cole ShippingCalculator.cs atual]

FORMAT:
- IShippingStrategy.cs
- StandardShippingStrategy.cs (e outras)
- ShippingCalculator.cs (refatorado)
- Exemplo de uso
- Diagrama UML em texto (opcional mas valorizado)
```

**Quando usar:** Eliminar code smells, preparar código para extensibilidade.

---

##### **5. O "Analisador de Trade-Offs Arquiteturais"**

```
PERSONA: Você é um arquiteto de sistemas .NET com experiência em avaliar decisões técnicas.

CONTEXTO: Estamos decidindo a arquitetura para nova plataforma de e-commerce .NET.

TASK: Crie matriz comparativa entre:

OPÇÃO 1: Monólito Modular
- Single deployment
- Módulos bem definidos (Catalog, Orders, Payments, Shipping)
- Database compartilhado com schemas separados

OPÇÃO 2: Microserviços
- Serviços independentes
- Database per service
- API Gateway + Event Bus (RabbitMQ)

CRITÉRIOS DE AVALIAÇÃO:
1. Velocidade de Desenvolvimento Inicial (0-6 meses)
2. Escalabilidade (100 → 10.000 req/s)
3. Complexidade Operacional (deploy, monitoring, debugging)
4. Autonomia da Equipe (3 times de 5 devs cada)
5. Consistência de Dados (transações, eventual consistency)
6. Custo de Infraestrutura (1º ano)
7. Time to Market (nova feature)

FORMAT: 
Tabela Markdown com colunas:
| Critério | Monólito Modular | Microserviços | Vencedor | Observações |

Após a tabela, forneça:
- Recomendação final com justificativa
- Quando mudar de uma abordagem para outra (evolutivamente)
```

**Quando usar:** Reuniões de decisão arquitetural, documentação de ADRs.

---

#### **Biblioteca Pessoal de Prompts - Como Criar**

1. **Salve seus melhores prompts** em repositório Git privado
2. **Parametrize** com `<VARIÁVEIS>` claras
3. **Documente** quando usar cada um
4. **Itere**: melhore baseado em resultados reais
5. **Compartilhe** com time (após validar qualidade)

**Estrutura sugerida:**
```
/prompts-library
  /architecture
    - clean-architecture-scaffold.md
    - trade-offs-analyzer.md
  /refactoring
    - solid-refactor.md
    - design-pattern-implementer.md
  /testing
    - unit-test-generator.md
    - integration-test-scaffold.md
  /api-design
    - rest-api-designer.md
    - graphql-schema-generator.md
```

---

## �️ PARTE 3: ARQUITETURAS ESCALÁVEIS .NET - PADRÕES E PRÁTICAS

### 3.1 Clean Architecture + Microservices - Template de Referência

**Microservices resolvem:** escalabilidade, fault tolerance, ciclos de desenvolvimento mais rápidos, flexibilidade tecnológica

**Estrutura de Diretórios:**

```
src/
├── Services/
│   ├── Orders/
│   │   ├── Orders.Domain/           # Entidades, Value Objects, Interfaces
│   │   ├── Orders.Application/      # Use Cases, DTOs, CQRS Handlers
│   │   ├── Orders.Infrastructure/   # EF Core, RabbitMQ, Redis
│   │   └── Orders.API/              # Controllers, Minimal APIs
│   ├── Payments/
│   └── Inventory/
├── BuildingBlocks/                   # Shared Kernel
│   ├── EventBus/
│   ├── Logging/
│   └── HealthChecks/
├── ApiGateway/                      # Ocelot ou YARP
└── docker-compose.yml

tests/
├── Orders.UnitTests/
├── Orders.IntegrationTests/
└── Orders.E2ETests/
```

### 3.2 Padrões Essenciais para Cada Serviço

#### A) API Gateway Pattern

**Função:** Roteamento, agregação, autenticação centralizada

**Implementação com Ocelot:**

```csharp
// ocelot.json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/orders/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        { "Host": "orders-service", "Port": 80 }
      ],
      "UpstreamPathTemplate": "/orders/{everything}",
      "UpstreamHttpMethod": [ "GET", "POST", "PUT", "DELETE" ],
      "RateLimitOptions": {
        "EnableRateLimiting": true,
        "Period": "1s",
        "PeriodTimespan": 1,
        "Limit": 10
      }
    }
  ],
  "GlobalConfiguration": {
    "BaseUrl": "https://api.meuservico.com"
  }
}

// Program.cs
builder.Services.AddOcelot()
  .AddCacheManager(x => x.WithDictionaryHandle());
```

#### B) Circuit Breaker Pattern

**Função:** Evitar cascata de falhas, melhorar resiliência

**Implementação com Polly:**

```csharp
services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>()
  .AddPolicyHandler(GetCircuitBreakerPolicy())
  .AddPolicyHandler(GetRetryPolicy());

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30),
            onBreak: (outcome, duration) =>
            {
                Log.Warning($"Circuit breaker opened for {duration.TotalSeconds}s");
            },
            onReset: () => Log.Information("Circuit breaker reset"));
}

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryCount, context) =>
            {
                Log.Warning($"Retry {retryCount} after {timespan.TotalSeconds}s");
            });
}
```

#### C) CQRS + Event Sourcing

**Command Handler (Write):**

```csharp
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _repository;
    private readonly IEventBus _eventBus;
    
    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        // 1. Validação
        var order = Order.Create(request.CustomerId, request.Items);
        
        // 2. Persistência
        await _repository.AddAsync(order, ct);
        await _repository.UnitOfWork.SaveChangesAsync(ct);
        
        // 3. Publicar evento
        var @event = new OrderCreatedEvent(order.Id, order.CustomerId, order.TotalAmount);
        await _eventBus.PublishAsync(@event, ct);
        
        return order.ToDto();
    }
}
```

**Query Handler (Read):**

```csharp
public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderDto>
{
    private readonly IReadOnlyDbContext _dbContext;
    private readonly IDistributedCache _cache;
    
    public async Task<OrderDto> Handle(GetOrderQuery request, CancellationToken ct)
    {
        // 1. Tentar cache primeiro
        var cacheKey = $"order:{request.OrderId}";
        var cached = await _cache.GetStringAsync(cacheKey, ct);
        if (cached != null)
            return JsonSerializer.Deserialize<OrderDto>(cached);
        
        // 2. Query otimizada (projeção direta para DTO)
        var order = await _dbContext.Orders
            .AsNoTracking()
            .Where(o => o.Id == request.OrderId)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerName = o.Customer.Name,
                Items = o.Items.Select(i => new OrderItemDto { ... }).ToList(),
                TotalAmount = o.TotalAmount
            })
            .FirstOrDefaultAsync(ct);
        
        // 3. Cachear resultado
        await _cache.SetStringAsync(
            cacheKey, 
            JsonSerializer.Serialize(order),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            ct);
        
        return order;
    }
}
```

#### D) Database Per Service + Saga Pattern

**Problema:** Transações distribuídas são complexas
**Solução:** Saga pattern com consistência eventual

**Saga de Criação de Pedido:**

```csharp
public class CreateOrderSaga : ISaga
{
    public async Task Execute(CreateOrderCommand command)
    {
        var sagaId = Guid.NewGuid();
        
        try
        {
            // Step 1: Criar pedido
            var order = await CreateOrder(command);
            
            // Step 2: Reservar estoque (compensável)
            await ReserveInventory(order.Items, sagaId);
            
            // Step 3: Processar pagamento (compensável)
            await ProcessPayment(order.TotalAmount, command.PaymentMethod, sagaId);
            
            // Step 4: Confirmar pedido
            await ConfirmOrder(order.Id);
            
            // Step 5: Publicar evento de sucesso
            await PublishOrderCompletedEvent(order);
        }
        catch (Exception ex)
        {
            // Compensações em ordem reversa
            await CompensatePayment(sagaId);
            await CompensateInventory(sagaId);
            await CancelOrder(sagaId);
            
            throw new SagaFailedException($"Order saga failed: {ex.Message}", ex);
        }
    }
}
```

### 3.3 Estratégias de Escalabilidade e Performance

#### A) Caching em Múltiplas Camadas

```csharp
// 1. In-Memory Cache (L1) - Para dados frequentes e pequenos
services.AddMemoryCache();

// 2. Distributed Cache (L2) - Redis para dados compartilhados
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "redis:6379";
    options.InstanceName = "OrdersService:";
});

// 3. HTTP Cache - Para APIs públicas
services.AddResponseCaching();
app.UseResponseCaching();

// Uso em código com fallback automático
public class CachedOrderRepository : IOrderRepository
{
    private readonly IMemoryCache _l1Cache;
    private readonly IDistributedCache _l2Cache;
    private readonly IOrderRepository _repository;
    
    public async Task<Order> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var key = $"order:{id}";
        
        // L1: Memory cache (mais rápido)
        if (_l1Cache.TryGetValue(key, out Order? order))
            return order!;
        
        // L2: Redis cache
        var cached = await _l2Cache.GetStringAsync(key, ct);
        if (cached != null)
        {
            order = JsonSerializer.Deserialize<Order>(cached)!;
            _l1Cache.Set(key, order, TimeSpan.FromMinutes(5));
            return order;
        }
        
        // L3: Database
        order = await _repository.GetByIdAsync(id, ct);
        
        // Populate caches
        await _l2Cache.SetStringAsync(key, JsonSerializer.Serialize(order), ct);
        _l1Cache.Set(key, order, TimeSpan.FromMinutes(5));
        
        return order;
    }
}
```

#### B) Otimização de Queries com EF Core

```csharp
// ❌ MAU - N+1 Problem
var orders = await _dbContext.Orders.ToListAsync();
foreach (var order in orders)
{
    var customer = await _dbContext.Customers.FindAsync(order.CustomerId);
    // ... 100 queries adicionais!
}

// ✅ BOM - Eager Loading
var orders = await _dbContext.Orders
    .Include(o => o.Customer)
    .Include(o => o.Items)
        .ThenInclude(i => i.Product)
    .AsNoTracking() // Read-only, mais performático
    .AsSplitQuery() // Evita cartesian explosion
    .ToListAsync();

// ✅ ÓTIMO - Projeção Direta (sem materializar entidades)
var ordersDto = await _dbContext.Orders
    .Where(o => o.Status == OrderStatus.Active)
    .Select(o => new OrderListDto
    {
        Id = o.Id,
        CustomerName = o.Customer.Name,
        ItemCount = o.Items.Count,
        TotalAmount = o.TotalAmount,
        CreatedAt = o.CreatedAt
    })
    .ToListAsync();
```

#### C) Mensageria Assíncrona com RabbitMQ/MassTransit

```csharp
// Configuração
services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        
        cfg.ReceiveEndpoint("order-created-queue", e =>
        {
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
            
            // Retry policy
            e.UseMessageRetry(r => r.Exponential(
                retryLimit: 5,
                minInterval: TimeSpan.FromSeconds(1),
                maxInterval: TimeSpan.FromMinutes(5),
                intervalDelta: TimeSpan.FromSeconds(2)));
            
            // Circuit breaker
            e.UseCircuitBreaker(cb =>
            {
                cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                cb.TripThreshold = 15;
                cb.ActiveThreshold = 10;
                cb.ResetInterval = TimeSpan.FromMinutes(5);
            });
            
            // Concorrência
            e.PrefetchCount = 16;
            e.UseConcurrencyLimit(10);
        });
    });
});

// Consumer
public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IPaymentService _paymentService;
    
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var order = context.Message;
        
        try
        {
            await _paymentService.ProcessPaymentAsync(order.OrderId, order.Amount);
            
            // Publicar evento de sucesso
            await context.Publish(new PaymentProcessedEvent(order.OrderId));
        }
        catch (Exception ex)
        {
            // Será retentado automaticamente pela policy
            throw;
        }
    }
}
```

#### D) Health Checks e Observability

```csharp
// Startup
services.AddHealthChecks()
    .AddDbContextCheck<OrdersDbContext>()
    .AddRedis("redis:6379")
    .AddRabbitMQ()
    .AddCheck<ExternalApiHealthCheck>("payment-api")
    .AddCheck<DiskSpaceHealthCheck>("disk-space");

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

// Logging Estruturado com Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProperty("Application", "OrdersService")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.Seq("http://seq:5341")
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://elasticsearch:9200"))
    {
        AutoRegisterTemplate = true,
        IndexFormat = "orders-service-{0:yyyy.MM.dd}"
    })
    .CreateLogger();

// Uso em código
_logger.LogInformation(
    "Order {OrderId} created for customer {CustomerId} with {ItemCount} items totaling {Amount:C}",
    order.Id,
    order.CustomerId,
    order.Items.Count,
    order.TotalAmount);
```

---

## 🚀 PARTE 4: WORKFLOW COMPLETO - DO ZERO AO DEPLOY

### 4.1 Setup Inicial de Projeto com IA

**1. Planejamento com Claude Chat**

```
PERSONA: Você é um arquiteto de software .NET com experiência em sistemas de alta escala.

CONTEXTO: Preciso criar um sistema de e-commerce com:
- Catálogo de produtos
- Carrinho de compras
- Checkout e pagamento
- Gestão de pedidos
- Notificações
- Expectativa: 10K req/s no pico
- Time: 5 desenvolvedores
- Budget: Cloud AWS

TASK: Projete a arquitetura completa incluindo:
1. Decisão: Monolito modular vs Microservices
2. Stack tecnológico (.NET, bancos, mensageria, cache)
3. Estrutura de serviços/módulos
4. Estratégia de deployment
5. Custos estimados mensais AWS
6. Roadmap de implementação (sprints de 2 semanas)

FORMAT: Documento executivo com diagramas em texto, justificativas técnicas, e trade-offs.
```

**2. Geração de Código Base com Claude Code**

```bash
# Terminal 1: Setup de infraestrutura
claude-code

> "Crie a estrutura de pastas completa para um microserviço Orders seguindo clean architecture. Inclua:
- Solution (.sln)
- Projects: Domain, Application, Infrastructure, API
- Dockerfile e docker-compose.yml
- GitHub Actions para CI/CD
- .editorconfig com conventions C#"

# Terminal 2: Implementação de domain layer
claude-code

> "Implemente as entidades do domínio para Orders:
- Order (aggregate root)
- OrderItem (entity)
- OrderStatus (value object)
- Eventos de domínio: OrderCreated, OrderCancelled
- Validações de negócio
- Testes de unidade para todas as regras"

# Terminal 3: Setup de testes
claude-code

> "Configure o projeto de testes com:
- xUnit
- FluentAssertions
- Moq
- Bogus (para fake data)
- Testcontainers (para integration tests)
- Coverage report com Coverlet"
```

### 4.2 Desenvolvimento de Feature Completa (Exemplo Prático)

**Cenário: Implementar "Aplicar Cupom de Desconto ao Pedido"**

**PASSO 1: Análise e Planejamento (Claude Chat)**

```
Preciso implementar a funcionalidade de aplicar cupom de desconto a um pedido.

Regras de negócio:
- Cupom pode ser percentual (10%) ou valor fixo ($50)
- Cupom pode ter quantidade limitada de usos
- Cupom pode ter data de validade
- Um pedido pode ter apenas um cupom
- Desconto não pode exceder o valor do pedido

Analyze e me dê:
1. Impacto em entidades existentes
2. Novos componentes necessários
3. Testes que precisam ser criados
4. Sequência de implementação (TDD)
```

**PASSO 2: Implementação TDD (Claude Code/Cursor)**

```bash
# Em Cursor ou Claude Code:

# Prompt 1: Testes primeiro
> "Crie os testes de unidade para a feature de aplicar cupom. Inclua:
- Aplicar cupom percentual válido
- Aplicar cupom valor fixo válido
- Rejeitar cupom expirado
- Rejeitar cupom sem usos restantes
- Rejeitar desconto maior que total do pedido
- Rejeitar aplicar mais de um cupom

Não implemente ainda, apenas os testes (fase RED)."

# Prompt 2: Implementação mínima
> "Agora implemente o código mínimo necessário para fazer todos os testes passarem."

# Prompt 3: Refatoração
> "Refatore o código para melhorar:
- Separar validações em validator class
- Extrair cálculo de desconto para método privado
- Adicionar logging
- Melhorar naming"
```

**PASSO 3: Integration Tests (Cursor)**

```
> "Crie integration tests usando Testcontainers para:
- Aplicar cupom via API endpoint
- Validar que desconto é persistido corretamente no banco
- Validar que uso do cupom é decrementado
- Validar que evento CouponApplied é publicado"
```

**PASSO 4: Code Review com IA (Claude Chat)**

```
[Cole o código gerado]

Faça um code review rigoroso verificando:
1. SOLID violations
2. Performance issues
3. Security vulnerabilities
4. Missing error handling
5. Test coverage gaps
6. Documentation needs

Seja extremamente crítico e sugira melhorias concretas.
```

### 4.3 Otimização de Performance - Caso Real

**Cenário: API endpoint lento (5 segundos)**

**PASSO 1: Profiling com Claude**

```
Este endpoint está levando 5 segundos:

[cole código do controller/service]

Analise e identifique:
1. N+1 queries
2. Operações síncronas que deveriam ser async
3. Falta de caching
4. Queries não otimizadas
5. Alocações desnecessárias de memória
```

**PASSO 2: Implementação de Fixes (Cursor Composer)**

```
> "Refatore este código aplicando todas as otimizações identificadas.
Use:
- EF Core Include() para eager loading
- Redis cache para dados frequentes
- Async/await corretamente
- Projeção direta para DTOs
- Índices necessários no banco (migrações)"
```

**PASSO 3: Benchmarking**

```
> "Crie benchmarks com BenchmarkDotNet para comparar:
- Versão original
- Versão otimizada
- Versão com cache

Inclua métricas de:
- Tempo de execução
- Alocações de memória
- GC collections"
```

### 4.4 Deploy e Monitoramento

**PASSO 1: Containerização (Claude Code)**

```bash
> "Crie Dockerfile otimizado para produção com:
- Multi-stage build
- Imagem Alpine Linux
- Non-root user
- Health check
- Minimal layers"

> "Crie docker-compose.yml para ambiente de desenvolvimento local com:
- API service
- PostgreSQL
- Redis
- RabbitMQ
- Seq (logging)
- Volumes para persistência"
```

**PASSO 2: CI/CD Pipeline (GitHub Actions)**

```
> "Crie pipeline GitHub Actions completo com:

Build Stage:
- Restore packages
- Build solution
- Run unit tests
- Run code coverage
- SonarQube analysis
- Build Docker image

Test Stage:
- Run integration tests
- Run security scan (Snyk)
- Performance tests

Deploy Stage:
- Push image to ECR
- Deploy to EKS (blue-green)
- Run smoke tests
- Rollback automático se smoke tests falharem"
```

**PASSO 3: Observability (Claude Chat)**

```
> "Configure observability completa usando:
- OpenTelemetry para distributed tracing
- Prometheus para métricas
- Grafana dashboards para visualização
- Alertas críticos (PagerDuty):
  * Error rate > 1%
  * Latency p99 > 500ms
  * Memory > 80%
  * Circuit breaker opened

Forneça código de configuração e exemplos de queries."
```

---

## 📈 PARTE 5: MEDINDO RESULTADOS E ROI

### 5.1 Métricas de Produtividade para Acompanhar

**ANTES da IA (Baseline):**
- Lines of Code por dia: ~200-300
- Features por sprint: 2-3
- Bugs em produção por sprint: 5-8
- Code review time: 2-3 horas por PR
- Setup de novo projeto: 1-2 dias

**COM IA (Target após 3 meses):**
- LoC gerado por dia: ~800-1200 (mas qualidade mantida)
- Features por sprint: 4-6 (aumento de 60-100%)
- Bugs em produção: 2-3 (redução de 50%)
- Code review time: 30-60 min por PR (redução de 70%)
- Setup novo projeto: 2-4 horas (redução de 80%)

### 5.2 Fórmula de ROI

```
ROI = (Ganho - Investimento) / Investimento × 100

Investimento Mensal:
- Ferramentas: $35 (Claude Pro + Windsurf)
- Tempo de aprendizado: 10-20 horas (primeiros 2 meses)

Ganho Mensal (estimativa conservadora para dev sênior):
- 20% mais produtivo = 8 horas/semana economizadas
- 8h × 4 semanas × $100/hora (custo dev) = $3.200/mês
- Menos bugs = $1.000/mês economizados
- Total: $4.200/mês

ROI = (4200 - 35) / 35 × 100 = 11,900%

**Payback period: < 1 semana**
```

### 5.3 Indicadores de Sucesso (OKRs)

**Q1 2025:**
- **Objetivo:** Dominar ferramentas IA para desenvolvimento .NET
  - KR1: Completar 100% dos projetos usando Claude/Cursor
  - KR2: Reduzir tempo de setup inicial em 70%
  - KR3: Aumentar cobertura de testes de 60% para 85%+

**Q2 2025:**
- **Objetivo:** Tornar-se referência em desenvolvimento .NET com IA no time/empresa
  - KR1: Treinar 3+ desenvolvedores nas técnicas
  - KR2: Criar 5+ templates/playbooks reutilizáveis
  - KR3: Apresentar resultados em tech talk interno

**Q3-Q4 2025:**
- **Objetivo:** Escalar impacto e multiplicar valor
  - KR1: Implementar 2+ sistemas greenfield com nova stack
  - KR2: Refatorar 1 sistema legado para arquitetura moderna
  - KR3: Contribuir com open-source (templates, tools)

---

### 5.4 Framework Estruturado para Demonstração de ROI aos Sponsors

Para justificar investimento em ferramentas e treinamento de IA, é fundamental demonstrar retorno mensurável. Este framework transforma dados em linguagem de negócio.

#### **Métricas Quantitativas - Dados Concretos de Mercado**

| Fonte | Métrica | Valor | Aplicação |
|-------|---------|-------|-----------|
| **GitHub Research** | Redução no tempo de conclusão de tarefas | **55%** | Modelar economia de tempo em sprints |
| **Bain & Company** | Ganho de produtividade geral | **25-35%** | Calcular aceleração de entregas |
| **Bain & Company** | Aumento em geração de testes unitários | **40-50%** | Justificar melhoria de qualidade |
| **Microsoft Research** | Desenvolvedores "no fluxo" (flow state) | **73%** | Benefício qualitativo (retenção) |
| **Microsoft Research** | Preservação de esforço mental | **87%** | Redução de burnout |

#### **Métricas Qualitativas - Impacto na Experiência do Desenvolvedor**

Valor da IA vai além da velocidade:
- **Satisfação no trabalho:** Desenvolvedores relatam maior fulfillment
- **Redução de burnout:** Menos tempo em tarefas repetitivas
- **Retenção de talentos:** Profissionais querem trabalhar com IA moderna
- **Atração de talentos:** Candidatos valorizam stack tecnológica avançada

#### **Framework de 4 Passos para Apresentar Valor**

##### **Passo 1: Estabelecer a Linha de Base**

**ANTES da implementação, medir:**

| Métrica | Como Medir | Exemplo Baseline |
|---------|-----------|------------------|
| **Tempo de ciclo para nova feature** | Story points / dias até produção | Feature CRUD: 8 dias |
| **Densidade de defeitos** | Bugs em produção / 1000 linhas | 12 bugs / 1K LOC |
| **Tempo gasto escrevendo testes** | Horas / sprint dedicadas a testes | 16h / sprint de 80h |
| **Cobertura de testes** | % código coberto por testes | 62% |
| **Satisfação do desenvolvedor** | Survey escala 1-10 | 6.8 / 10 |
| **Onboarding de novo dev** | Dias até primeiro commit produtivo | 14 dias |

**Ferramenta sugerida:** Azure DevOps Analytics, SonarQube, DORA metrics

---

##### **Passo 2: Implementar o Piloto**

**Selecionar:**
- **Equipe piloto:** 3-5 desenvolvedores representativos
- **Projeto representativo:** Nem muito simples, nem muito complexo
- **Duração:** 2-3 sprints (4-6 semanas)
- **Ferramentas:** Claude Pro + Windsurf/Cursor (setup completo)

**Durante o piloto:**
- Treinamento inicial: 4 horas (workshop + prática guiada)
- Daily check-ins: Coleta de feedback e ajustes
- Métricas contínuas: Mesmas métricas da baseline

---

##### **Passo 3: Medir o Impacto**

**Rastrear MESMAS métricas da baseline:**

| Métrica | Baseline | Piloto (após 6 semanas) | Delta |
|---------|----------|-------------------------|-------|
| Tempo de ciclo feature CRUD | 8 dias | **4.5 dias** | **-44%** |
| Densidade de defeitos | 12 bugs / 1K LOC | **7 bugs / 1K LOC** | **-42%** |
| Tempo escrevendo testes | 16h / sprint | **8h / sprint** | **-50%** |
| Cobertura de testes | 62% | **81%** | **+31%** |
| Satisfação desenvolvedor | 6.8 / 10 | **8.4 / 10** | **+24%** |
| Onboarding novo dev | 14 dias | **8 dias** | **-43%** |

**Coleta de casos qualitativos:**
- "Consegui implementar CQRS completo em 2 dias vs 1 semana antes"
- "Refatorar para Clean Architecture levou 3 dias vs estimativa de 2 semanas"
- "Geração de testes aumentou de 20 testes/sprint para 80 testes/sprint"

---

##### **Passo 4: Extrapolar o Valor e Apresentar**

**Cálculo do ROI em Linguagem de Negócio:**

```
INVESTIMENTO:
- Ferramentas: $35/dev/mês × 20 devs × 12 meses = $8.400/ano
- Treinamento: 4h/dev × 20 devs × $100/h = $8.000 (uma vez)
- Total Ano 1: $16.400

RETORNO (baseado em redução de 30% no tempo de desenvolvimento):
- Salário médio dev: $80K/ano
- 20 devs × $80K × 30% ganho produtividade = $480.000/ano em "capacidade liberada"

OU (em projetos):
- Projeto típico: $500K, 6 meses
- Com 30% redução: mesmo projeto em 4.2 meses
- Antecipação de receita: 1.8 meses = $150K em valor presente
- Capacidade para 1.4 projetos extras/ano = +$700K receita

ROI = ($480K - $16.4K) / $16.4K × 100 = 2.827% ROI
Payback period: 12.5 dias
```

#### **Template de Apresentação para Sponsors (Slide Deck)**

**Slide 1: O Problema**
- Tempo de desenvolvimento está aumentando
- Dificuldade em reter/atrair talentos
- Pressão por entregas mais rápidas

**Slide 2: A Solução - IA no Desenvolvimento**
- Claude + Copilot: Aumentam produtividade em 25-35%
- Dados de mercado: GitHub (55% redução tempo), Bain (40-50% em testes)

**Slide 3: Nosso Piloto - Resultados**
- [Gráfico de barras: Baseline vs Piloto]
- Destaque: 44% redução em tempo de feature, 50% menos tempo em testes
- Quote de desenvolvedor: "Refatoração que levaria 2 semanas, fiz em 3 dias"

**Slide 4: Projeção Organizacional**
- Investimento: $16.4K/ano
- Retorno: $480K em capacidade liberada (ou $700K em receita adicional)
- ROI: 2.827%
- Payback: 12.5 dias

**Slide 5: Benefícios Adicionais**
- Qualidade: +31% cobertura de testes, -42% defeitos
- Pessoas: +24% satisfação, -43% tempo onboarding
- Competitividade: Stack moderna atrai talentos

**Slide 6: Plano de Rollout**
- Fase 1 (Q1): 5 devs (piloto validado)
- Fase 2 (Q2): 15 devs (70% do time)
- Fase 3 (Q3): 20 devs (100% do time)
- Custo total ano 1: $16.4K
- Retorno esperado: $480K - $700K

**Slide 7: Próximos Passos**
- Aprovação para expansão
- Budget Q1: $X para ferramentas + treinamento
- Sponsor: [Nome do Sponsor Executivo]

---

## 🔐 PARTE 6: DEVSECOPS COM IA - SEGURANÇA E QUALIDADE

Esta seção crítica aborda um tema ausente em muitos guias: **como garantir que código gerado por IA seja seguro e de qualidade produtiva**. A IA é uma faca de dois gumes - pode encontrar E introduzir vulnerabilidades.

### 6.1 O Problema: IA Pode Gerar Código Vulnerável

#### **Dados Alarmantes**

- **≈40% das sugestões de IA podem conter vulnerabilidades de segurança** (estudos acadêmicos)
- Problemas comuns em código gerado:
  - Injeção SQL (uso de string interpolation em queries)
  - Regex excessivamente permissivos (ReDoS - Regex Denial of Service)
  - Blocos catch vazios (exceções engolidas)
  - Criptografia fraca (algoritmos deprecados)
  - Validação insuficiente de input
  - Hard-coded secrets

#### **Por Que Isso Acontece?**

1. **Treinamento em código público:** Modelos aprendem de repositórios open-source que contém código ruim
2. **Falta de contexto de segurança:** IA não "entende" implicações de segurança
3. **Otimização para funcionalidade:** Foco em "fazer funcionar" vs "fazer seguro"

**Princípio Fundamental:** **TODO código gerado por IA deve ser tratado como código de desenvolvedor júnior não auditado.**

---

### 6.2 Ceticismo Profissional - Mentalidade Obrigatória

#### **Checklist de Revisão para Código Gerado por IA**

Ao revisar código sugerido por IA, SEMPRE verificar:

**Categoria SQL/Database:**
- [ ] Queries usam parâmetros (SqlParameter) ao invés de concatenação?
- [ ] Não há string interpolation ($"SELECT * FROM Users WHERE Id = {userId}")?
- [ ] Entity Framework usa corretamente LINQ (não raw SQL quando evitável)?

**Categoria Input Validation:**
- [ ] Todos os inputs de usuário são validados?
- [ ] Data Annotations corretas ([Required], [StringLength], [EmailAddress])?
- [ ] Validação de range para números?
- [ ] Sanitização de HTML se aplicável?

**Categoria Autenticação/Autorização:**
- [ ] Endpoints têm [Authorize] quando necessário?
- [ ] Não há hard-coded credentials?
- [ ] Secrets vêm de variáveis de ambiente ou Azure Key Vault?
- [ ] JWT são validados corretamente?

**Categoria Criptografia:**
- [ ] Usa algoritmos modernos (AES-256, SHA-256+)?
- [ ] NÃO usa MD5, SHA1, ou DES?
- [ ] Salts aleatórios para hashing de senhas?
- [ ] HTTPS é enforçado?

**Categoria Exception Handling:**
- [ ] Catch blocks não estão vazios?
- [ ] Exceções são logadas adequadamente?
- [ ] Mensagens de erro não expõem stack traces ao cliente?
- [ ] Tipos de exceção específicos quando possível?

**Categoria Performance:**
- [ ] Não há N+1 query problems?
- [ ] Async/await usado corretamente (não .Result ou .Wait())?
- [ ] Operações I/O são realmente assíncronas?

---

### 6.3 Portões de Qualidade Conscientes da IA (AI-Aware Quality Gates)

Pipelines de CI/CD modernos devem incluir análise especializada em código gerado por IA.

#### **SAST (Static Application Security Testing) para IA**

Ferramentas SAST modernas estão desenvolvendo "postura consciente de IA" para detectar padrões problemáticos comuns em código gerado por LLMs.

**Ferramentas Recomendadas:**

| Ferramenta | Foco | Integração .NET |
|------------|------|-----------------|
| **SonarQube** | Vulnerabilidades, code smells, cobertura | Excelente (MSBuild/dotnet) |
| **Snyk Code** | Vulnerabilidades de segurança, dependências | Boa (CLI, IDE, CI/CD) |
| **GitHub Advanced Security** | SAST + dependências + secrets | Nativa (GitHub Actions) |
| **Checkmarx** | SAST enterprise, compliance | Boa (integração Azure DevOps) |

**Configuração Exemplo - SonarQube no Pipeline:**

```yaml
# .github/workflows/ci.yml
name: CI with Security Analysis

on: [push, pull_request]

jobs:
  build-and-analyze:
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v3
        with:
          fetch-depth: 0  # Importante para análise completa
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: SonarQube Begin Analysis
        run: |
          dotnet tool install --global dotnet-sonarscanner
          dotnet sonarscanner begin \
            /k:"meu-projeto" \
            /d:sonar.host.url="${{ secrets.SONAR_HOST_URL }}" \
            /d:sonar.login="${{ secrets.SONAR_TOKEN }}" \
            /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml"
      
      - name: Build
        run: dotnet build --no-restore
      
      - name: Test with Coverage
        run: dotnet test --no-build --verbosity normal \
          /p:CollectCoverage=true \
          /p:CoverletOutputFormat=opencover
      
      - name: SonarQube End Analysis
        run: dotnet sonarscanner end /d:sonar.login="${{ secrets.SONAR_TOKEN }}"
      
      - name: Quality Gate Check
        run: |
          # Falha o build se Quality Gate não passar
          curl -s -u ${{ secrets.SONAR_TOKEN }}: \
            "${{ secrets.SONAR_HOST_URL }}/api/qualitygates/project_status?projectKey=meu-projeto" \
            | jq -e '.projectStatus.status == "OK"'
```

**Quality Gates Recomendados:**

| Métrica | Threshold Mínimo | Razão |
|---------|------------------|-------|
| **Cobertura de testes** | 80% | Código gerado precisa de testes |
| **Vulnerabilidades (blocker/critical)** | 0 | Zero tolerância para críticos |
| **Code Smells** | < 100 | Manutenibilidade |
| **Duplicação de código** | < 3% | IA tende a repetir padrões |
| **Complexidade ciclomática** | < 15 por método | IA pode gerar métodos complexos |

---

### 6.4 Análise Automatizada com Claude API em CI/CD

Além de SAST tradicional, podemos usar a **própria IA para revisar código gerado por IA**. Meta? Sim. Efetivo? Muito.

#### **Exemplo Prático: AI Code Reviewer no Pull Request**

```yaml
# .github/workflows/ai-code-review.yml
name: AI Code Review

on:
  pull_request:
    types: [opened, synchronize]

jobs:
  ai-review:
    runs-on: ubuntu-latest
    permissions:
      pull-requests: write  # Necessário para postar comentários
    
    steps:
      - uses: actions/checkout@v3
        with:
          fetch-depth: 0
      
      - name: Get PR Diff
        id: diff
        run: |
          git fetch origin ${{ github.base_ref }}
          DIFF=$(git diff origin/${{ github.base_ref }}...HEAD)
          # Escapar para JSON
          DIFF_ESCAPED=$(echo "$DIFF" | jq -Rs .)
          echo "diff=$DIFF_ESCAPED" >> $GITHUB_OUTPUT
      
      - name: Review with Claude
        id: claude_review
        run: |
          RESPONSE=$(curl -s -X POST https://api.anthropic.com/v1/messages \
            -H "x-api-key: ${{ secrets.CLAUDE_API_KEY }}" \
            -H "anthropic-version: 2023-06-01" \
            -H "Content-Type: application/json" \
            -d '{
              "model": "claude-sonnet-4-20250514",
              "max_tokens": 4000,
              "messages": [{
                "role": "user",
                "content": "Você é um revisor de código sênior especializado em segurança .NET. Analise o diff abaixo e identifique:\n\n1. **Vulnerabilidades de Segurança** (OWASP Top 10: SQL Injection, XSS, autenticação, etc)\n2. **Problemas de Performance** (N+1 queries, uso incorreto de async/await, memory leaks)\n3. **Violações de Código Limpo** (SOLID, naming, complexidade)\n4. **Riscos de Produção** (exception handling inadequado, logging faltando)\n\nFormate a resposta em Markdown com seções claras. Para cada issue:\n- Severidade: 🔴 Crítico / 🟡 Médio / 🟢 Baixo\n- Localização: arquivo:linha\n- Descrição do problema\n- Sugestão de correção\n\nDiff:\n```\n'${{ steps.diff.outputs.diff }}'\n```"
              }]
            }')
          
          REVIEW=$(echo $RESPONSE | jq -r '.content[0].text')
          # Escapar para comentário do GitHub
          REVIEW_ESCAPED=$(echo "$REVIEW" | jq -Rs .)
          echo "review=$REVIEW_ESCAPED" >> $GITHUB_OUTPUT
      
      - name: Post Review Comment
        uses: actions/github-script@v7
        with:
          script: |
            const review = JSON.parse(${{ steps.claude_review.outputs.review }});
            
            await github.rest.issues.createComment({
              issue_number: context.issue.number,
              owner: context.repo.owner,
              repo: context.repo.repo,
              body: `## 🤖 AI Code Review (Claude)

${review}

---
*This review was generated automatically. Always validate suggestions before applying.*`
            });
      
      - name: Check for Critical Issues
        run: |
          # Falha o workflow se houver issues críticos (🔴)
          REVIEW="${{ steps.claude_review.outputs.review }}"
          if echo "$REVIEW" | grep -q "🔴 Crítico"; then
            echo "❌ Critical security issues found!"
            exit 1
          fi
```

**Resultado:** Cada PR recebe análise automatizada de:
- Vulnerabilidades de segurança
- Performance issues
- Code smells
- Riscos de produção

**Custo:** ~$0.01-0.05 por PR (dependendo do tamanho do diff)

---

### 6.5 Privacidade e Código Proprietário

#### **Requisitos Enterprise para Uso de IA**

Ao trabalhar com código proprietário, use **APENAS** soluções com garantias contratuais:

| Ferramenta | Plano Enterprise | Garantia de Privacidade |
|------------|------------------|-------------------------|
| **GitHub Copilot** | Copilot for Business | ✅ Código não usado para treinamento |
| **Claude** | Claude Pro / Team / Enterprise | ✅ Dados não persistidos para treinamento |
| **Cursor** | Cursor Business | ✅ Privacy mode disponível |
| **Windsurf** | Windsurf Team | ✅ On-premise deployment option |

#### **Checklist de Compliance**

Antes de adotar ferramenta de IA em empresa:

- [ ] Contrato garante não-uso de código para treinamento?
- [ ] Dados são criptografados em trânsito e em repouso?
- [ ] Há opção de self-hosted / on-premise?
- [ ] Compliance com GDPR / LGPD?
- [ ] Auditoria de logs disponível?
- [ ] SLA de uptime adequado (99.9%+)?
- [ ] Data residency configurável (dados ficam na região)?

#### **Configuração de Privacy no Copilot (Exemplo)**

```json
// .vscode/settings.json (ou organização-wide)
{
  "github.copilot.enable": {
    "*": true
  },
  "github.copilot.advanced": {
    "telemetry": false,  // Desabilita telemetria
    "inlineSuggestEnable": true
  },
  // Bloquear sugestões de código de repos públicos específicos
  "github.copilot.excludedRepos": [
    "public-repo/*"
  ]
}
```

---

### 6.6 Melhores Práticas - Resumo Executivo

**✅ DO (Faça):**
1. Trate código IA como código de júnior não-auditado
2. Implemente Quality Gates com SAST
3. Use análise automatizada (Claude API) em PRs
4. Revise manualmente código crítico (auth, pagamento, dados sensíveis)
5. Use planos enterprise com garantias de privacidade
6. Treine equipe em identificação de vulnerabilidades comuns
7. Mantenha checklist de revisão visível no PR template

**❌ DON'T (Não Faça):**
1. Confiar cegamente em código gerado
2. Pular revisão manual em código de segurança
3. Usar ferramentas gratuitas com código proprietário
4. Desabilitar análises de segurança por "lentidão"
5. Ignorar alertas de SAST como "falsos positivos" sem verificar
6. Commitar secrets ou credentials (mesmo que IA sugira)
7. Aceitar sugestões de IA que você não entende completamente

---

## 🎯 PRÓXIMOS PASSOS - SEU PLANO DE 30 DIAS

### Semana 1: Setup e Fundamentos
- [ ] Dia 1-2: Assinar Claude Pro + escolher IDE (Windsurf ou Cursor)
- [ ] Dia 3-4: Estudar este guia completo + compreender Paradigmas (Programador em Par vs Agente Autônomo) e Workflow (Inner/Outer Loop)
- [ ] Dia 5-7: Criar primeiro projeto usando Claude Code + testar workflows de ciclo interno e externo

### Semana 2: Prática Intensiva
- [ ] Dia 8-10: Implementar 3 features reais usando estratégia Inner/Outer Loop
- [ ] Dia 11-12: Criar biblioteca pessoal de prompts usando templates da seção 2.4
- [ ] Dia 13-14: Fazer code review com IA de projetos existentes + aplicar checklist de segurança

### Semana 3: Otimização e Segurança
- [ ] Dia 15-17: Identificar gargalos em código existente e otimizar com IA
- [ ] Dia 18-19: Implementar pipeline CI/CD com análise de segurança (SonarQube + Claude API)
- [ ] Dia 20-21: Configurar Quality Gates e testar em projeto piloto

### Semana 4: Consolidação e Demonstração de Valor
- [ ] Dia 22-24: Criar templates e documentação interna (usar Manual de Prompts como base)
- [ ] Dia 25-26: Coletar métricas e preparar apresentação de ROI para sponsors (usar Framework 4 Passos)
- [ ] Dia 27-28: Compartilhar conhecimento com time + definir plano de expansão
- [ ] Dia 29-30: Apresentar resultados e planejar próximos projetos

### Recursos Adicionais Críticos

**Documentação Oficial:**
- Claude: https://docs.anthropic.com
- Claude Code: https://docs.claude.com/en/docs/claude-code
- Cursor: https://cursor.sh/docs
- Windsurf: https://windsurf.com/docs
- GitHub Copilot: https://docs.github.com/copilot

**Comunidades:**
- r/ClaudeAI
- r/ChatGPTCoding
- Discord: Cursor Community
- Discord: Windsurf Users

**Cursos Recomendados:**
- Prompt Engineering Guide (promptingguide.ai)
- .NET Microservices (Microsoft Learn)
- Clean Architecture (Jason Taylor - Clean Architecture template)
- DevSecOps for .NET (Pluralsight)

---

## 🎁 BÔNUS: PROMPTS PRONTOS PARA COPIAR

### 1. Geração de Arquitetura Completa

```
PERSONA: Você é um arquiteto de software .NET experiente especializado em [domínio].

CONTEXTO: Preciso criar um sistema [descrição] com requisitos:
- [Requisito funcional 1]
- [Requisito funcional 2]
- [Requisito não-funcional 1]
- Stack: .NET 8, PostgreSQL, Redis, Docker

TASK: Projete arquitetura completa incluindo:
1. Decisão de estilo arquitetural (justificada)
2. Estrutura de projetos/camadas
3. Diagramas C4 (Context, Container, Component)
4. Padrões de design aplicáveis
5. Estratégia de testes
6. Estratégia de deploy e escalabilidade

FORMAT: Documento markdown estruturado com código de exemplo para componentes-chave.
```

### 2. Revisão de Código Automática

```
PERSONA: Lead developer .NET focado em qualidade e segurança.

CONTEXTO: [COLE SEU CÓDIGO AQUI]

TASK: Code review completo verificando:
✓ SOLID principles
✓ Performance (N+1, async/await, caching)
✓ Security (OWASP Top 10, SQL injection, XSS)
✓ Error handling
✓ Testability
✓ Documentation
✓ Naming conventions

FORMAT: Tabela markdown:
| Categoria | Issue | Severidade | Linha | Fix Sugerido |
```


### 3. Geração de Testes Completos

```
PERSONA: Especialista em testes automatizados .NET (TDD practitioner).

CONTEXTO: [COLE CÓDIGO DA CLASSE/MÉTODO]

TASK: Gere testes xUnit completos com:
- Setup usando AutoFixture
- Mocks com Moq
- Assertions com FluentAssertions
- Cenários: happy path, edge cases, exceptions
- Naming: MethodName_Scenario_ExpectedBehavior
- Arrange-Act-Assert pattern

FORMAT: Código completo executável. Target: 90%+ coverage.
```

### 4. Implementação de Feature (TDD)

```
PERSONA: Desenvolvedor .NET sênior adepto de TDD.

CONTEXTO: Feature [nome] com regras:
- [Regra 1]
- [Regra 2]
- [Regra 3]

TASK: Implementação TDD completa:

FASE RED:
1. Escreva TODOS os testes primeiro
2. Confirme que todos falham
3. Commit dos testes

FASE GREEN:
4. Implemente código mínimo
5. Todos os testes devem passar
6. Commit da implementação

FASE REFACTOR:
7. Melhore código mantendo testes verdes
8. Commit final

FORMAT: 3 blocos de código separados (Red/Green/Refactor) com explicações.
```

### 5. Otimização de Performance

```
PERSONA: Especialista em performance .NET e profiling.

CONTEXTO: Este código está lento:
[COLE CÓDIGO + MÉTRICAS]

TASK: Otimize para:
- Latência p99 < 200ms
- Throughput > 1000 req/s
- Memory footprint reduzido em 30%

Aplique:
✓ Query optimization (no N+1)
✓ Async/await proper usage
✓ Caching strategy (multi-layer)
✓ Connection pooling
✓ Object pooling quando aplicável

FORMAT:
1. Análise de problemas
2. Código otimizado com comentários
3. Benchmarks esperados (antes/depois)
4. Recomendações adicionais
```

### 6. Análise de Segurança (DevSecOps)

```
PERSONA: Especialista em segurança de aplicações .NET.

CONTEXTO: [COLE CÓDIGO PARA ANÁLISE]

TASK: Análise de segurança completa verificando:
1. OWASP Top 10 (Injection, Auth, XSS, etc)
2. Validação de input
3. Gestão de secrets
4. Criptografia adequada
5. Exception handling seguro (não vaza stack trace)
6. Logging de eventos de segurança

FORMAT:
## Vulnerabilidades Encontradas
(lista com severidade 🔴🟡🟢)

## Código Corrigido
(versão segura com explicações)

## Recomendações Adicionais
(práticas de segurança)
```

---

## 🏆 CONCLUSÃO

Você agora tem um sistema completo para se tornar um desenvolvedor .NET 10x usando IA:

✅ **Paradigmas fundamentais:** Entende quando usar Programador em Par vs Agente Autônomo
✅ **Workflow otimizado:** Domina Inner Loop (Copilot) e Outer Loop (Claude)
✅ **Ferramentas certas:** Claude Pro + Windsurf/Cursor ($35/mês) com estratégia clara de uso
✅ **Técnicas comprovadas:** Manual de Prompts Arquiteturais com templates copy-paste
✅ **Arquiteturas escaláveis:** Clean Architecture + Microservices + CQRS + Event Sourcing
✅ **DevSecOps robusto:** Quality Gates, SAST, análise automatizada com Claude API
✅ **ROI mensurável:** Framework de 4 passos para demonstrar valor aos sponsors (25-55% ganho)

**O diferencial competitivo:**
- **Não é apenas usar IA** - é orquestrar ferramentas certas nos momentos certos
- **Não é apenas gerar código** - é gerar código seguro, testado e arquitetonicamente sólido
- **Não é apenas produtividade** - é demonstrar valor mensurável que justifica investimento

**Próximo passo:** Comece HOJE com o plano de 30 dias. Não espere perfeição, comece experimentando com o Framework Inner/Outer Loop em uma tarefa real.

**Lembre-se:** A IA é uma assistente poderosa, não uma substituta do arquiteto. Você com 26 anos de experiência + poder da IA moderna = combinação imbatível no mercado.

**Sua vantagem:** Experiência para tomar decisões arquiteturais corretas + IA para executá-las 10x mais rápido.

---

**Última atualização:** Outubro 2025  
**Versão:** 2.0 (incluindo Paradigmas Fundamentais, Workflow Inner/Outer Loop, Manual de Prompts Arquiteturais, DevSecOps, Framework ROI Estruturado)  
**Criado especialmente para:** Arquitetos e desenvolvedores .NET sêniores que querem maximizar impacto e valor