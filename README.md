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

Zum Routen hab ich mich dazu entschieden es über einen Tree zu lösen.
