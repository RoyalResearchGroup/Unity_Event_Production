Alle unten erwähnten Technologien sind über die Config YAML konfigurierbar, keine Änderung auf Pythonseite notwendig.
***
* Unity hat Gym-Wrapper, *aber für uns nichts, weil nur auf 1 Agenten limitiert*
***
Mit Unity mögliche Trainingszenarien:
* Single Agent
* Simultaneous Single Agent: Was wir immer zum Training benutzt haben
* Adversarial Self-Play: Agent spielt 1v1 gegen sich selbst
* Cooperative Multi-Agent: Mehrere Agenten arbeiten gemeinsam an einem Ziel *(UNSERER USE CASE)*
* Competitive Multi-Agent: Team Sports
* Ecosystem: unabhängige Agents, die sich gegenseitig beeinflussen


*Idee: Competitive Multi-Agent könnten wir auch verwenden, indem wir mehrere Fabriken simulieren, welche kompetitiv das beste Ergebnis erzeugen sollen.*
***

### Arten von Reward Signals
* extrinsic: vom Environment erzeugt
* gail: siehe unten
* curiosity: siehe unten
* rnd: siehe unten
***
## RL-Algorithmen
* Proximal Policy Optimization (PPO)
default, stabil und generalisiert
* Soft Actor-Critic (SAC)
off policy, weniger samples nötig, daher gut geeignet für langsamere Lernumgebungen
***
### Sparse-reward Environments
Umgebungen, in denen extrinsische Belohnungen selten vorkommen. Dafür gibt es Methoden, wie der Agent mittels intrinsischen Rewards trotzdem lernen kann.
#### Curiosity
Zwei NNs werden verwendet.
1. Netz nimmt als Input die letzten beiden Beobachtungen und sagt vorher, welche Aktion dazwischen getroffen wurde.
2. Netz nimmt als Input die letzte Beobachtung und Aktion und sagt vorher, was die nächste Beobachtung sein wird

Der intrinsische Reward des Agenten ist die Differenz zwischen vorhergesagter Beobachtung und tatsächlicher Beobachtung des zweiten Netzes.

Mehr Infos [hier](https://pathak22.github.io/noreward-rl/)

#### RND
Mit RND soll der Agent dazu animiert werden, seltene Vorkommnisse zu exploren.

2 NNs werden verwendet:
1. Netz ist random initialisiert, nimmt Beobachtungen entgegen und liefert entsprechend random, aber konstanten Output
2. Netz wird trainiert, nimmt Beobachtungen entgegen und sagt die Outputs des ersten NNs vorher.

Der Reward errechnet sich proportional zum Fehler der Vorhersage des zweiten NNs. *Anders gesagt bedeutet das, wenn Beobachtungen ankommen, die selten vorkommen oder bisher selten vorkamen, macht das 2. NN eine schlechte Prediction, dementsprechend wird dies als positiver Reward dem Agenten angerechnet, um seine Exploration anzutreiben.*

Mehr Infos [hier](https://arxiv.org/pdf/1810.12894.pdf)
***
## Imitation Learning
Kein Trial-and-Error wie in RL, sondern der Agent lernt, ein in einem Datensatz vorgegebenes Verhalten zu imitieren

### Behavioral Cloning (BC)
Agent soll Verhalten genau nachahmen. Daher sollte der Input alle möglichen Szenarien aus der Umgebung abdecken.
*In unserem Fall uninteressant*

### Generative Adversarial Imitation Learning (GAIL)
Zstl. Netzwerk, welches Agenten-Aktion von demonstrierter Aktion unterscheiden lernt (discriminator)
* Gibt dem Agenten intrinsische Belohnungen je nach dem, wie ähnlich die Aktion der demonstrierten ist

Lernprozess: 
* Agent lernt immer besser, Verhalten nachzuahmen
* discriminator lernt, Aktionen zu unterscheiden, wird "strenger"

*Cooler Ansatz, aber einen vorgegebenen **optimalen** Datensatz zu erstellen könnte sich als schwierig herausstellen.*

***
### Self-Play in Competitive Multi-Agent Scenarios
PPO sollte eher verwendet werden als SAC.
*Wir haben nicht zwingend eine symmetrische Umgebung, d.h. eine in der mehrere Agenten dieselbe Policy verfolgen. Daher wird uns Self-Play nicht viel bringen. Es sei denn, wir setzen die oben erwähnte Idee durch, dann würde quasi eine Fabrik "gegen sich selbst" trainieren.*

***
## MA-POCA (Multiagent Posthumous Credit Assignment)
"Coach-Agent", welcher lernt, einen Gruppenreward richtig auf die einzelnen Agenten aufzuteilen.
Agenten müssen dasselbe Behaviour haben, das innovative an diesem Ansatz ist die Erweiterung, dass auch Agenten an der Belohnung teilhaben, die nach Trainingsstart erstellt werden oder vor Trainingsende zerstört werden.
*Auf Unity-Seite muss über einen Environment Controller o.ä. die Gruppe festgelegt werden.*
*Wegen oben genanntem nicht für unseren Use Case geeignet.*
***
## Sonstiges
* Curriculum Learning
Training beginnt mit einfacher Task, Schwierigkeit wird prozedural erhöht.

* Environment Parameter Randomization
Umgebungsparameter werden random verändert 

* Variable Length Observations using Attention Networks
Agents können beliebige Anzahl an observations annehmen, indem *attention layers* eingesetzt werden. Diese filtern die relevanten Daten aus den bereitgestellten heraus.

* LSTM for memory-based decision making

* Mehrere Unity-Instanzen fürs Training

* Side Channels: Datenaustausch zwischen Unity und Python außerhalb vom Trainingskontext, z.B. zur Definition von Umgebungsparametern.