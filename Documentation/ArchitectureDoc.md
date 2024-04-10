# Schriftliche Dokumentation der Architekturumsetzung für die Ereignisdiskreten Simulation in Unity

## Grundlegender Aufbau und Idee

In erster Linie ist Unity3d eine Game Engine und basiert deshalb auf einer Zeitdiskreten Architektur. Das bedeutet, die Laufzeit in mehr oder weniger gleich langen Zeitschritten abgearbeitet wird. In jedem dieser Zeitschritte (Update()) entwickelt sich die virtuelle Umgebung ein Stück weiter.
Für das effiziente Training des Agenten und der Simulation einer Produktionsanlage im generellen eignet sich die Ereignisdiskrete Simulation besser. Hierbei wird statt eines möglichst kleinen Zeitschrittes direkt zu einem eintretenden Ereignis gesprungen. So kann die vergangene Zwischenzeit gespart werden, was eine relativ zur Aktionsanzahl viel schnellere Simulation ermöglicht.
Um dies in Unity umzusetzen, wird eine an die bestehende Umgebung angepasste Architektur genutzt.  Kern des Systems ist ein Eventmanager, der in jedem Update-Zyklus mindestens ein Event abarbeitet, und in den von Modulen Events eingereiht werden können. Um optimale Performance gewährleisten zu können, ist die Aktualisierung des Models nach dem Eintreten eines Events vollständig automatisiert.

## Abfolge

Die Umgebungsanpassung folgt einem sich wiederholenden Muster:

-	Der Event Manager arbeitet ein Event ab. Das bedeutet, zuerst die Simulationszeit (Verwaltet vom Time Manager) anzupassen und die Zeiten der anderen, eingereihten Events zu verringern.
-	Das Modul, welches ein Event mit einem Zeitstempel (hier relativ) eingereiht hat, wird vom Event Manager über das Eintreten des Event Informiert (EventCallback). Je nach Modul bewirkt das eine bestimmte Aktion.
-	Die Aktion wird durchgeführt. Das kann eine signifikante Änderung des Systems zur Folge haben. 
-	Der Zustand muss wieder „ausgeglichen“ werden. Die Kernidee hierbei ist, dass das System sich so lange in einem instabilen Zustand befindet, wie eines der Module eine Aktion ausführen könnte. Der Ausgleich kann effizient durch ein kaskadierendes Updaten der Modulzustände erreicht werden. 
  >	- Zunächst wird der Zustand des Event-Modules aktualisiert, indem nach einem möglichen Nachfolger für eigene ausgangsfertige Ressourcen gesucht wird.
  >	- Existiert dieser, wird die Ressource verschoben
  >	- Das Modul ist nun in jedem Fall wieder verfügbar. Es wird nach Vorgängern gesucht, die bereit sind, eine Ressource in das Modul zu verschieben.
  >	- Existiert ein solcher, wird die Ressource verschoben
  >	- Bei jedem Modul, mit dem interagiert wurde, wird dieser Prozess selbst erneut ausgeführt.
  >	- Je nach Modul kann ein neues Event erstellt und im Event Manager eingereiht werden.
    
Durch diese Abfolge ist gewährleistet, dass das Modelsystem sich stets im korrekten Zustand befindet.
Da sich der Prozess unabhängig von der Engine-Iteration nutzen lässt, kann durch Batching eine sehr gute Performance erzielt werden.

## Architektur

Die Architektur hat einen Relativ simplen Aufbau. Das Ziel ist, die Erstellung von unterschiedlichen Simulationsumgebungen und die Einbindung von Agenten in diese so einfach wie möglich zu gestalten. Das Kernobjekt ist dabei das SimulationObject. Hier müssen nur Nachfolger definiert werden, Vorgänger werden dann selbst ermittelt. Das SimulationObject bietet die Basis für Modul und Agent.  Beide können gleich in das Produktionsnetz integriert werden, haben aber unterschiedliche Zwecke. Module sind alle diejenigen Nodes, welche Produktionsaufgaben übernehmen: Quellen, Buffer, Maschinen oder Senken zum Beispiel (Siehe Doku für Module). Agenten dagegen sind für die Steuerung ihrer Umgebung zuständig. Hier wird zwischen dem HeuristicAgent und dem RLAgent unterschieden (siehe Doku für Agent).

# Module

## Source
//……
::FOLGETASK:: 
