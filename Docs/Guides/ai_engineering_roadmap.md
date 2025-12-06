# 🚀 Roadmap de Funcionalidades MyVocaList (Engenharia de IA)

Este documento cataloga todas as funcionalidades de valor discutidas, separadas pelas áreas de foco (Eficiência, Performance e Social), servindo como base para as próximas etapas de planejamento e implementação da sua jornada como Engenheiro de IA.

---

### I. 🎤 Valor para a Performance do Cantor (Foco: DSP e Áudio)

Funcionalidades que transformam o app em uma ferramenta de aprimoramento vocal, aumentando o valor para o público focado em aprimoramento.

| Funcionalidade | Descrição | Foco em Engenharia de IA |
| :--- | :--- | :--- |
| **1. Pontuação de IA (Auto-Score)** | Análise de voz (via microfone) em tempo real ou pós-gravação para determinar a precisão da afinação (*pitch*) e do ritmo (*timing*), gerando uma pontuação objetiva. | **DSP** (Processamento de Sinais), Modelos de Classificação/Regressão. |
| **2. Críticas e Sugestões de Melhoria** | **LLM (Gemini) aplicado ao DSP:** Traduzir os dados de erro numérico de afinação em *feedback* textual útil e motivacional. | **Geração de Linguagem Natural (NLP),** Integração de LLM. |
| **3. Modo Treinamento (Visualização de Notas)** | Detecção de notas em tempo real para exibir a melodia de referência contra a voz do cantor em um gráfico, permitindo a prática vocal guiada. | **DSP** (Pitch Detection/FFT), Engenharia de Frontend Otimizada (MAUI). |
| **4. Detecção e Sincronização de Letras** | Buscar letras via catálogo interno ou API e sincronizar cada palavra com *timestamp* exato para o efeito "passar a cor" progressivamente. | **Engenharia de Conteúdo e Tempo,** Integração de APIs. |
| **5. Separação de Áudio Integrada** | Usar APIs (como Moises) para remover a voz de faixas originais sob demanda, permitindo que músicos criem *backing tracks* personalizados (útil no modo Bandokê). | **Engenharia de Backend**, Integração de APIs de Processamento de Áudio. |

---

### II. 🧠 Valor para a Eficiência do Host (Foco: Previsão e UX)

Funcionalidades que usam a IA e automação para otimizar a experiência do organizador do evento.

| Funcionalidade | Descrição | Foco em Engenharia de IA |
| :--- | :--- | :--- |
| **6. Estimativa Inteligente de Espera** | **Previsão de Regressão:** Estimar o tempo de espera para o próximo cantor ou a duração do round, baseando-se em dados históricos e no número de participantes pendentes. | **Modelagem de Regressão** (Time Series Forecasting), MLOps. |
| **7. Gerenciamento por Comando de Voz** | Permitir que o Host realize ações críticas na fila (ex: "Adicionar Marcos", "Próximo cantor") usando apenas comandos de voz. | **Processamento de Linguagem Natural** (ASR - Automatic Speech Recognition), Otimização de UX/Workflow. |
| **8. Sugestão Inteligente de Próxima Música** | Sugerir a próxima música ou gênero a ser cantado, evitando repetições ou garantindo a variedade ideal para manter a energia da multidão. | **Sistemas de Recomendação** (Filtro Baseado em Conteúdo), Análise de Dados. |

---

### III. 🧑‍🤝‍🧑 Valor para Engajamento Social (Foco: LLM e Comunidade)

Funcionalidades que cumprem o objetivo futuro de rede social, criando uma comunidade em torno do aplicativo.

| Funcionalidade | Descrição | Foco em Engenharia de IA |
| :--- | :--- | :--- |
| **9. Geração de Persona Karaokê (LLM)** | O cantor insere *tags* (ex: gênero, humor), e a **API Gemini** gera uma biografia ou "Persona de Karaokê" única e envolvente para o perfil. | **LLM (Gemini API) Integration,** Engenharia de Prompt (Prompt Engineering). |
| **10. Votação e Engajamento da Audiência** | Sistema de votação em tempo real que permite ao público interagir com o cantor e a performance (ex: 'Aplausos', 'Fogo'). | **Engenharia de Backend** (WebSockets/SignalR), Experiência de Usuário (UX) Social. |
| **11. Folha de Pontuação/Cifra para Bandas** | No modo Bandokê, gerar ou transpor automaticamente uma folha de cifras ou partituras digitais para os músicos da banda, com base na música e no tom escolhidos. | **Engenharia de Automação de Conteúdo,** Processamento de Dados Musicais. |

---

### IV. 🛠️ Valor para a Equipe de Desenvolvimento (Foco: AIA-D)

Funcionalidades que garantem que o desenvolvimento seja eficiente, econômico e com alta qualidade de código.

| Funcionalidade | Descrição | Foco em Engenharia de IA |
| :--- | :--- | :--- |
| **12. Agentes de Compliance MD3/Arquitetura** | Usar o Agente Gemini 3 Pro (Jules) para garantir que todas as telas sejam construídas seguindo 100% as diretrizes MD3 e a arquitetura .NET MAUI do `CLAUDE.md`. | **MLOps para Código,** Autonomia de Agente (Jules/Antigravity). |
| **13. Otimização de Tokens** | Gerenciamento inteligente do contexto do agente para minimizar o consumo de tokens e o custo do desenvolvimento assistido por IA. | **Eficiência de Custo/Recursos,** Engenharia de Prompt. |
