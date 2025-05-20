-- select nome, HEX(digital_code) as digital from usuarios;

select nome, digital_code as digital from usuarios;

SELECT nome, CAST(digital_code AS CHAR(1000)) as digital FROM usuarios;

