# Changelog - MyKaraoke

## Objetivo do App

O MyKaraoke é um aplicativo .NET MAUI para gerenciar filas de participantes em rodadas de karaokê. Permite criar novas filas, acompanhar o status das filas em andamento e consultar filas já encerradas, facilitando a organização dos eventos e a experiência dos participantes.

---

## Histórico de Mudanças

- **21/06/2025** - Implementado suporte completo a múltiplos idiomas (localização). Adicionados recursos de tradução para 11 idiomas: inglês (padrão), português do Brasil, espanhol, francês, alemão, chinês simplificado, japonês, coreano, árabe, russo e hindi.
- **21/06/2025** – Simplificada a SplashPage: removida a imagem filakaraoke.png e o texto "Gerenciamento inteligente". Adicionada a imagem mykaraokesplashpage.jpg ocupando toda a tela para uma experiência visual mais impactante.
- **20/06/2025** – Ajustada a SplashPage para que a imagem do título ocupe aproximadamente 2/3 da largura da tela, centralizada, e o texto "Gerenciamento inteligente" fique alinhado à esquerda da imagem, com tamanho de fonte menor e mais próximo do visual do preview.
- **20/06/2025** – Removido o código de gradiente em texto e adicionado a imagem 'filakaraoke.png' como título da SplashPage, conforme o preview visual desejado.
- **20/06/2025** – Removido todo o código relacionado ao preenchimento gradiente do texto do título na SplashPage. Agora o título é exibido como imagem, garantindo compatibilidade e visual fiel ao design.
- **20/06/2025** – Corrigido namespace e visibilidade da classe GradientTextDrawable e referência no Styles.xaml para garantir reconhecimento do recurso customizado no XAML.
- **20/06/2025** – Corrigido GradientTextDrawable.cs: ajustada a ordem dos parâmetros de GradientStop (Color, float) e uso explícito do namespace para Font, eliminando erros de compilação.
- **20/06/2025** – Corrigido erro de uso de LinearGradientBrush em Label.TextColor na SplashPage (não suportado pelo .NET MAUI 8). Agora o texto "FILA KARAOKÊ" com gradiente é exibido via GraphicsView e GradientTextDrawable. Ajustado o namespace do recurso em Styles.xaml.
- **20/06/2025** – Implementação da SplashPage com layout atualizado:
- **20/06/2025** – SplashPage atualizada: texto "FILA KARAOKÊ" centralizado com gradiente (#d5528a → #9047ac), texto "Gerenciamento inteligente" abaixo em branco, SplashPage ativada como tela inicial e removida a splashscreen azul padrão do .NET MAUI no Android.
- **20/06/2025** – Adicionado recurso de gradiente horizontal reutilizável (de #221b3c à esquerda até #331e6e à direita) para fundo de tela em toda a aplicação e aplicado na HomePage.
- **20/06/2025** – Criação do projeto inicial e definição do objetivo do aplicativo.