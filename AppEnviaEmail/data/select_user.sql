-- select nome, HEX(digital_code) as digital from usuarios;
select nome, template from usuarios;

SELECT COUNT(nome) FROM usuarios WHERE nome = "Caio";
