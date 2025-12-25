Hallo!

Das ist meine Media Rating Platform.

Bevor das Projekt gebuilded wird, müssen die docker\_db\_setup.bat (direkt im root verzeichnis) ausgeführt werden.
Alternativ kann auch vom root Verzeichnis 'docker compose -f "./DataAccess/db/docker-compose.yml" up -d mrp\_db' ausgeführt werden.

Github Link: https://github.com/david-lenz-fh/arp
Postman Collection: Im root Verzeichnis "postman_collection.json"



Ich hab probiert so gut wie ich kann die SOLID Prinzipien einzuhalten. Das Projekt ist unterteilt in API Layer, Business Layer und Data Access Layer.
Dabei kommunizieren DAL und API Layer nicht miteinander, sondern immer erst übers BL.
Die verschiedenen Layers sind dabei loosely coupled, das heißt sie sind nicht an die konkreten Implementierungen der anderen Layer abhängig,
sondern haben nur Interfaces des Layers.

Beim Login hab ich mich entschieden meinen eigenen Token zu erstellen. Dies mach ich in dem ich mit einem geheimen Schlüssel den Usernamen sowie ein Gültigkeitsende symmetrisch verschlüssele. Passwörter werden nur gehashed (SHA256) in der Datenbank abgespeichert.

Das Routing habe ich mit einem Tree gelöst, der die Path Variablen in einem Array abspeichert. Das würde theoretisch auch Routen mit mehreren Path Variablen erlauben.

Ich habe in diesem Semesterprojekt einiges über SOLID Prinzipien und dessen Umsetzung gelernt. Früher hätt ich dies nicht wirklich in meine Projektstruktur miteinfließen lassen, was dann auch das Unit Testen däutlich erschwert hätte.

Beim Unit testen war meine Rangehensweise so, dass ich die Service Klassen getestet habe, da im Business Layer die meiste Logik ist. Ich habe probiert alle Methoden die ich getestet habe, so genau zu testen wie möglich. Ich hab also möglichen Varianten getestet die pro Methode auftreten können. Beim Mocking hab ich mich entschieden, die Mock Klassen selber zu schreiben und kein Framework zu benutzen.

Die zeit hab ich nicht aktiv getracked, aber man hat anhand der commited lines of code in github eine ungefähre Aufwandseinschätzung.