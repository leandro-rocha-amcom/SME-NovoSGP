﻿ALTER TABLE objetivo ADD COLUMN pagina INT;
UPDATE objetivo SET pagina = 1 WHERE id = 5;
UPDATE objetivo SET pagina = 2 WHERE id IN (6,7,8,9);
UPDATE objetivo SET pagina = 3 WHERE id IN (10,11);
UPDATE objetivo SET pagina = 4 WHERE id IN (12) ;
UPDATE objetivo SET pagina = 5 WHERE id IN (13,14);
UPDATE objetivo SET pagina = 6 WHERE id IN (15);
UPDATE objetivo SET pagina = 7 WHERE id IN (16,17,18,19);
UPDATE objetivo SET pagina = 8 WHERE id IN (20,21,22,23);
UPDATE objetivo SET pagina = 9 WHERE id IN (24,25,26,27);