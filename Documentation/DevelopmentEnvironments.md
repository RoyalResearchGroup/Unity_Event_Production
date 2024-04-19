# Plant Simulation
## Vorteile
* Masterarbeit bietet eine Grundlage
* Basisfunktionalitäten bereits vorhanden

## Nachteile
* Keine Integration mit Git möglich
  * Binärdateien können nicht zusammengefügt werden
* Agent der Masterarbeit ist nicht modular => viel Überarbeitung in Plant Simulation notwendig

# SimPy
## Vorteile
* Direkte Einbindung des Python-Agenten => keine Sockets notwendig
* diskrete Eventsimulation, Ressourcen, Prozesse bereits eingebaut

## Nachteile
* keine grafische Benutzeroberfläche => angewiesen auf Konsolenoutput
* Python-Projekt der Masterarbeit muss umgeschrieben werden
* Module aus Produktionssystemen müssen selbst implementiert werden

# Unity
Letztendlich haben wir uns für einen Wechsel zur Unity-Engine entschieden. Nachdem sich herausgestellt hatte, dass Plant Simulation mit Version-Control wie Git kaum nutzbar ist. Neben anderen Optionen wie SimPy zeichnet sich Unity durch mehrere Vorteile aus:
## Vorteile
* Starke Reinforcement Learning Bibliothek
  * Performance-Statistiken bereits vorhanden
  * praktisch kein eigener Entwicklungsaufwand nötig
  * Diverse Algorithmen bereits verfügbar
  * Bietet Möglichkeit, eigene Algorithmen effizient zu integrieren
  * PyTorch mit CUDA Beschleunigung verfügbar für schnelleres Training
* Vorerfahrung im Team reduziert Einarbeitungszeit
* C# als moderne, verbreitete Programmiersprache mit guter Quellenlage
* "Schauwert" mittels 3D, Animationen und flexiblem UI Framework für Erweiterungen

## Nachteile
* ereignisdiskrete Simulation muss selbst implementiert werden, bedeutet erblichen Zeitaufwand
* Module aus Produktionssystemen müssen selbst implementiert werden
