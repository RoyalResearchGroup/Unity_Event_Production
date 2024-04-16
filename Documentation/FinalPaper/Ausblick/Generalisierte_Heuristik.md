# Problematik mit der Heuristik
* Verhindert keinen Deadlock
* Nicht unbedingt effizient bei gewissen Konstellationen -> Kein Aussagekräftiger Vergleich mit ML-Agent

## Problem Behebung
* Simplere Heuristik angepasst an das Modell -> Nach "Vorgabe" von Völker
    * Grundprinzipien sollten für unterschiedliche Modelle übernommen werden
    * Nur die Anpassung an die Simulation/Blueprint vornehmen
* Limitierung auf wenige aber aussagekräftige Beispiel Simulationen

## Angepassten Heuristik
### Pro
+ Aussagekräftiger Vergleichswert
### Contra
- Mehraufwand

### Idee
#### Konstruktor
1. Verhältnis von Blueprints im agenten durch Serializable angeben mit insgesamt Anzahl (> 1)
    1. Dictonary für alle Blueprints der 2. Ebene und deren Verhältnis (Original)   
#### Jeder Durchgang
2. Check Übergabe **Produkt der 0. Ebene**
    1. Erstelle Liste mit möglichen Produkten der 1. Ebene
    2. Erstelle Liste mit Blueprints der 2. Ebene, die das Produkt der 1. Ebene nutzen könnten
3. **Wahl des Blueprints:** Umformen der Verhältnisse in rangedictonaries (Wahrscheinlichkeitstabellen) für **Blueprints der 2. Ebene**
    1. Umformen in ein Rangedictonary für Blueprints der 2. Ebene:
    * **Key:** Range von x - "Verhältnis * FaktorZurGesamtVerteilungAufDieRange * (FaktorFürReadyMaschinen/FaktorFürNichtReadyMaschinen)" 
    * **Value:** Blueprint
        1. **FaktorFürReadyMaschinen:** Verhältnis ready/allen maschinen der 1. Ebene und später multipliziert als Faktor mit Range der Blueprints der 2. Ebene die das Produkt **verwerten** können
        2. **FaktorFürNichtReadyMaschinen:** Verhältnis nicht ready/allen maschinen der 1. Ebene und später multipliziert als Faktor mit Range der Blueprints der 2. Ebene die das Produkt **nicht verwerten** können
    2. Auswahl eines Blueprints der nachfolgenden Maschinen durch random aus der range in der Wahrscheinlichkeitstabelle der Blueprints der 2. Ebene
    3. Überprüfen ob Blueprint das Produkt verwenden kann
        1. **True:** Die range des gewählten Blueprints wird in der Wahrscheinlichkeitstabelle der 2. Ebene um 1/"Anzahl der benötigten Resourcen für das Blueprint" verringert
        2. **False:** Abbruch
4. **Wahl des Produkts:** Umformen der Verhältnisse in rangedictonaries (Wahrscheinlichkeitstabellen) für **Produkte der 1. Ebene**
    1. Check welche Produkte in der 1. Ebene wären machbar für das Blueprint der 2. Ebene anhand der freien maschinen
    2. Resourcen des gewählten Blueprints und deren Verhältnisse in rangedictionaries wie folgt einfügen:
        * **Key:** Range von x - "Verhältnis im Blueprint * FaktorZurGesamtVerteilungAufDieRange * (FaktorFürMachbar/FaktorFürMachbar)" 
        * **Value:** Resource
            1. **FaktorFürReadyMaschinen:** Verhältnis ready/allen maschinen der 1. Ebene und multipliziert mit Range der Produkte der 1. Ebene die von Maschinen gerade **hergestellt** werden können
            2. **FaktorFürNichtReadyMaschinen:** Verhältnis nicht ready/allen maschinen der 1. Ebene und multipliziert mit Range der Produkte der 1. Ebene die von Maschinen gerade **nicht hergestellt** werden können
            3. Auswahl eines Produkts der 1. Ebene random aus der range in der Wahrscheinlichkeitstabelle der Produkte der 1. Ebene
            4. Überprüfen ob das Produkt produziert werden kann
                1. **True:** Die range des gewählten Produkts wird in der Wahrscheinlichkeitstabelle der 2. Ebene um "Verhältnis der Resource im Blueprint"/"Anzahl der benötigten Resourcen für das Blueprint" verringert
                2. **False:** Abbruch
5. **Wahl der Maschine**
    1. Liste der möglichen Maschinen der 1. Ebene für das gewählte Produkt erstellen
    2. Aus der Liste der möglichen Maschinen auswählen
        1. Random eine Maschine auswählen
        2. Maschine mit der niedrigsten Produktionszeit für das Produkt wählen

### Setup in der Simulation
* Blueprints der 2. Ebene für den Agenten 

### Legende
* Agenten sind bei den Ebenen ausgenommen
    * 0. Ebene: Direkte Vorgänger vom Agenten
    * 1. Ebene: Direkte Nachfolger vom Agenten
    * 2. Ebene: Direkte Nachfolger nach der Ebene  