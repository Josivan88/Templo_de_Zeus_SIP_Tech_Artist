Este é um projeto de Tech Art desenvolvido na Unity URP (Universal Render Pipeline), focado em performance para plataformas de recursos limitados. A cena convida a uma experiência imersiva no Templo de Zeus, onde cada elemento visual serve como prova de domínio técnico. O projeto apresenta 12 shaders customizados (incluindo materiais holográficos) e sistemas de VFX complexos, todos otimizados por meio de técnicas avançadas como Pooling de objetos, Bake de iluminação e Vertex Colors para garantir a máxima qualidade visual com framerate fluido.

![Screenshot do Templo de Zeus](https://i.imgur.com/MT4sWo1.jpeg)
![Screenshot do Templo de Zeus](https://i.imgur.com/NDrmxHs.jpeg)
![Screenshot do Templo de Zeus](https://i.imgur.com/JQbUlWi.jpeg)
![Screenshot do Templo de Zeus](https://i.imgur.com/qyNSXDN.jpeg)

**Versão da Unity e Pipeline Utilizada**
---

Unity: 6000.2.9f1
Pipeline Gráfico: URP
Plataformas Alvo: PC / Windows 64 bits

**Shaders e Efeitos Visuais Criados**

**Shaders**
---

Shader 1: BrightDissolve
Função: Efeito de dissolução, permite controle da direção, cor, cor da borda, evolução, intensidade, influência do ruido (frequência e velocidade)
Observações: se baseia na criação de uma máscara que define a opacidade final, um pequeno offset gera uma borda que pode destacar o efeito de dissolução

Shader 2: DirectionalWindEffect
Função: Permite a criação de efeitos de ventania ou nuvens em sentido controlado, permite controle da cor, difuso, tiling, velocidade em x e em y, brilho e máscara

Shader 3: DynamicSmoke
Função: Permite a criação de efeitos de neblina em constante movimento aleatório, permite o controle da cor, velocidade, suavidade da superfície, escala, opacidade, máscara e blend com o ambiente
Observações: usa ruido padrão do unity, diminui a carga na memória

Shader 4: Fire
Função: Permite a criação de efeitos de fogo, permite controle da cor, textura, brilho, tiling, velocidade em x e em turbulência (habilitado ou não, Velocidade, escala e influência), máscara
Observações: performance positiva em relação ao usar partículas, e mais realista

Shader 5: FlapWings
Função: Gera um vertex offset para simular bater de asas de animais, A seleção das partes móveis é otimizada via Vertex Color Channel (Canal RGB do Vértice). Isso evita a necessidade de texturas de máscara adicionais e minimiza o overhead do shader.
permite controlar a cor, difuso, amplitude do bater das asas, velocidade, e se o movimento é intermitente ou não
Observações: Necessita que a malha tenha cores atribuídas ao vértices para definir qual se move ou não

Shader 6: HolographicCard
Função: gera efeitos de cards holográficos, permite definir uma textura, brilho, tiling, máscara de efeito, cubemap para efeito de textura de profundidade, brilho do cubemap, velocidade de movimento do efeito holográfico, textura de conteúdo do card
Observações: permite diversas aplicações, com características que podem ser expandidas

Shader 7: RayBright
Função: Gera efeitos de raios dinâmicos, permite controlar a cor, intensidade, evolução (propagação) definir a textura do raio, controle sobre a textura do raio (UV), perturbação de turbulência (velocidade, influência e repetição)
Observações: aliado a malhas com bones é possível controlar direção, ponto inicial e final do raio

Shader 8: SimpleBright
Função: para efeitos sijmples de brilho controlado, permite controle de cor, intensidade, evolução, e textura com transparência definida pelo canal alfa

Shader 9: TransparentLeafs
Função: Permite a criação de folhas realistas para vegetação, permite controlar o difuso por uma textura que o alfa define a transparência (cutout), especular, suavidade, máscara de movimento de vértices, influência da cor dos vértices na oclusão de ambiente do material, 
frequência do vento, intensidade de vento, o quanto o material é emissivo, e qual o valor de corte do alfa (alpha clip)
Observações: tem impacto razoável no desempenho usar com moderação, lembrar e usar malhas com ambiente occlusion gravado nos vértices, ajuda no realismo e define quais partes se movem mais, ou menos.

Shader 10: ThreeMapsVertexColor
Função: shader triplanar para distribuir diversas texturas diferentes sobre uma superfície, aplicado principalmente em terrenos, definido pelas cores dos vértices, que podem ser pintados, resumidamente ele permite a definição de mpas de textura e normal que são controlados pelos vétices, são ao total 4 mapas difusos e 2 mapas de normal, 
devido a questões de desempenho o especular é calculado em função da combinação de cores do canal difuso
Observações: tem impacto razoável no desempenho,  usar com moderação

Shader 11: Tronco
Função: similar ao TransparentLeafs porém não gera movimento dos vértices, permite modificar o difuso, o tiling do difuso, a suavidade por valor, e a influência das cores dos vértices na oclusão do ambiente.
Observações: menos impacto no desempenho do que o TransparentLeafs

Shader 12: UVPanTransparency
Função: Similar ao DirectionalWindEffect, mas voltado para efeitos realistassem brilho próprio, permite controlar cor, texturas de nuvens, repetição (tiling) velocidade em x e em y


**Efeitos Visuais (VFX)**
---

Fora do templo:

VFX 1: [nevoeiro móvel: (Particle Fog) na entrada do templo, para dar a impressão de lugar alto acima das nuvens, constituido por partículas com um material usando o shader dynamic smoke, número baixo de partículas para evitar overdraw, porém as particulas cobrem vários metros de área]

VFX 2: [partículas de poeira brilhante: (BrightParticles) garantem um tom místico, cobrem uma grande área, externamente ao templo elas se movem rápido e semi-aleatoriamente, indicando vento, dentro do tempo elas são mais lentas]

VFX 3: [Folhas caindo das árvores: efeito natural, que garante realismo e imersão]

VFX 4: [Pássaros voando: (BirdsSystem) acima do templo existe um sistema totalmente produzido por mim, que simula a revoada de pássaros, um script gerencia a posição e direção enquanto um material com o shader flapwings gera o bater das asas]

Dentro do templo:

VFX 5: [Fagulhas de fogo: (SparksParticles) O fogo usa um shader que fiz (Fire) mas para adicionar realismo adicionei fagulhas que saem naturalmente das chamas]

VFX 6: [Raios de Zeus: (prefabs) saem do dedo dele é um sistema criado completamente por mim que se baseia no shader que criei (RayBright), nele uma textura ondula e se move dando a impressão de um raio dinâmico, enquanto isso o percurso do raio é definido através de raycasts, que posicionam bones do raio, 
gerando um destino, uma origem e uma curvatura central, usam pooling para evitar instanciar um grande número de elementos por frame]

VFX 7: [Efeito de revelação do card: (RevelationEffect) é uma demonstração de alto impacto de VFX, orquestrado via C#, que sincroniza dissolve no cilindro, espirais de vento e iluminação, entregando um feedback de alto valor de produção.]

VFX 8: [Fumaça constante atrás da carta: (InverseHemisphere) Uma semi esfera sempre apontada para a câmera e com um material com o shader DirectionalWindEffect dá a impressão de fumaça sempre atrás da carta]


**Tema Escolhido e Justificativa Artística**
---

**Tema: Zeus**

Motivação Artística:
Pretendi passar uma atmosfera divina, iluminada e positiva ao mesmo tempo realista e mística, usei azul e dourado na maioria dos elementos principais, procurei definir um ambiente sempre influenciado por nuvens.
Criei efeitos condizentes com o principal deus do olimpo, envolvendo raios, luzes e poder.

**Técnicas de Otimização Aplicadas**
---

Pooling de Objetos: reutilização de raios no dedo de Zeus e na revelação da carta.
Shaders otimizados: redução de instruções, nós simplificados.
LOD (Level of Detail): aplicado às árvores para simplificação de modelos distantes.
Bake de Iluminação: para diminuir o impacto do uso de vários efeitos luminosos.
Compressão de Texturas: sempre que possível, com resolução mais baixa possível, mas garantindo qualidade visual, resolução sempre que possível em potências de dois.
Redução de Draw Calls: usando o mesmo material e texturas para diversos objetos, como as pedras.
Evitar Overdraw Excessivo em partículas: diminui sempre que possível o número de partículas para apenas o essencial.
Occlusion culling: para ocultação de objetos não visíveis.

**Controles Básicos**
---

Movimentação: [WASD se move na direção desejada, Shift: corre, Espaço: pula]
Interações: [Mouse: olha ao redor]
Habilidades / Ações especiais: [botão direito do mouse: zoom]

**Como Executar o Projeto**
---

**via executável:**
certifique-se de que tem um PC com windows 10 ou 11 de 64 bits
CPU: processador de 6 núcleus ou mais
GPU: GTX 1060 ou superior
baixe https://drive.google.com/drive/folders/1aqeH-CxilTq41tW0ORfh5N35469YYvE5?usp=sharing
clique em TemploDeZeus_TesteSIP.exe

**Via projeto:**
https://github.com/Josivan88/Templo_de_Zeus_SIP_Tech_Artist
Abra o projeto na Unity versão indicada:
Unity 6000.2.9f1
Certifique-se de que o pipeline utilizado está instalado.
Abra a cena inicial:
Assets/Scenes/Opening.unity
Pressione Play.
