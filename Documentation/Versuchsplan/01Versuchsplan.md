# Versuchsplan 1
## 1. Versuchsabsicht
## 2. Beschreibung des Simulationsmodells
### 2.1 Bild des Simulationsmodells
### 2.2 Grobes Konzept
1. Simulationsidee: *Bsp.: 2 Maschinen arbeiten 2 Blueprints schneller, die anderen..*
### 2.3 Verwendete Blueprints
1. Assembler Blueprints
    1. Resourcen:
    2. Produkt:
2. Station Blueprints
    1. Resourcen:
    2. Produkt:
## 3. Aufbau des Experiments 
**Wichtige Angabe**: Wie viele Experimente sollen für jeden Ansatz durchgeführt werden?
### 3.1 Leistungskriterien
### 3.X Grundansatz X
#### Simulationskonfiguration
2. Maschine X und Y
    1. Blueprints: 
    2. **Wichtig!** Besonderheit: 
#### Lernkonfiguration
## 4. Durchführung des Experiments 
### 4.1 Durchführung mit RL
### 4.2 Durchführung mit Heuristik
1. Heuristik: 
Das erste freie und valide Modul aus der übergebenen Liste wird ausgewählt.
2. Heuristik
    1. Stationswahl:
    Die beiden Stationen die am schnellsten das Produkt verarbeiten können werden priorisiert. Wenn beide verfügbar sind wird einer der beiden zufällig ausgewählt. Wenn nur eine der beiden Stationen frei ist wird diese ausgewählt. In dem Fall, in dem keine der beiden Stationen bereit ist, wird überprüft, ob die *beiden* anderen nicht priorisierten Stationen frei sind. *Nur dann* wird zufällig eine der beiden Stationen gewählt, um einen Deadlock zu verhindern. 
    2. Bufferwahl:
    Aus den bereiten zugleich und validen Optionen wird geschaut, ob ein Buffer dabei ist, der das Produkt anbietet, welches am schnellsten von der Station bearbeitet wird. Dieser wird dann ausgewählt.
## 5. Auswertung der Ergebnisse
## 6. Überprüfung und Anpassung für Folgeversuche
