-- =============================================================================
-- Reset diario dos dados de demonstracao.
--
-- Apaga as vendas (com seus acompanhamentos), os usuarios e as pizzas, e
-- repopula com 5 pizzas, 5 usuarios e 10 vendas sorteadas.
-- Executado pelo container pizzaria-seed (ver Scripts/AgendadorReset.sh).
--
-- Os cadastros de apoio ficam como estao: Status, Tipo_pizza, Borda_pizza e
-- Acompanhamentos nao sao apagados.
--
-- ATENCAO: este script destroi todos os dados. Ele existe porque o ambiente e
-- de demonstracao/aula. Nao habilite em um banco com dados reais.
-- =============================================================================

-- DELETE e nao TRUNCATE: TRUNCATE exige o privilegio DROP, e a conta da
-- aplicacao so tem SELECT, INSERT, UPDATE e DELETE. Por isso os Ids continuam
-- de onde pararam em vez de voltar a 1.
-- A ordem segue as chaves estrangeiras (filhas antes das maes), com as chaves
-- ligadas: VendaAcompanhamentos sai junto com Vendas, e nenhum item antigo
-- sobra apontando para uma venda que nao existe mais.
DELETE FROM VendaAcompanhamentos;
DELETE FROM Vendas;
DELETE FROM Usuarios;
DELETE FROM Pizzas;

-- 5 pizzas sorteadas de um catalogo de 15, com preco entre 30.00 e 60.00,
-- custo entre 25% e 40% do preco e 20 unidades em estoque.
-- O sorteio fica numa subconsulta com LIMIT, que o MySQL materializa: assim o
-- custo e calculado sobre o mesmo preco que vai para a tabela.
INSERT INTO Pizzas (Nome, Descricao, Preco, Custo, Obs, Qtd)
SELECT Nome, Descricao, Preco, ROUND(Preco * (0.25 + RAND() * 0.15), 2), '', 20
FROM (
SELECT Nome, Descricao, ROUND(30 + RAND() * 30, 2) AS Preco
FROM (
              SELECT 'Margherita'         AS Nome, 'Molho de tomate, mussarela e manjericão'        AS Descricao
    UNION ALL SELECT 'Calabresa',               'Calabresa fatiada, cebola e mussarela'
    UNION ALL SELECT 'Quatro Queijos',          'Mussarela, parmesão, gorgonzola e provolone'
    UNION ALL SELECT 'Portuguesa',              'Presunto, ovo, cebola, azeitona e mussarela'
    UNION ALL SELECT 'Frango com Catupiry',     'Frango desfiado, requeijão cremoso e milho'
    UNION ALL SELECT 'Pepperoni',               'Pepperoni italiano e mussarela'
    UNION ALL SELECT 'Vegetariana',             'Abobrinha, berinjela, pimentão e tomate seco'
    UNION ALL SELECT 'Napolitana',              'Mussarela, tomate em rodelas e parmesão'
    UNION ALL SELECT 'Atum',                    'Atum, cebola roxa e azeitonas'
    UNION ALL SELECT 'Bacon com Cheddar',       'Bacon crocante e cheddar derretido'
    UNION ALL SELECT 'Camarão',                 'Camarão ao alho e óleo com catupiry'
    UNION ALL SELECT 'Lombo com Abacaxi',       'Lombo canadense, abacaxi e mussarela'
    UNION ALL SELECT 'Brócolis com Bacon',      'Brócolis, bacon e requeijão'
    UNION ALL SELECT 'Marguerita Especial',     'Tomate italiano, búfala e pesto de manjericão'
    UNION ALL SELECT 'Toscana',                 'Linguiça toscana, cebola caramelizada e mussarela'
) AS catalogo
ORDER BY RAND()
LIMIT 5
) AS sorteadas;

-- 5 usuarios sorteados de uma lista de 15, com telefone aleatorio
INSERT INTO Usuarios (Nome, Email, Telefone)
SELECT Nome, Email, CONCAT('119', LPAD(FLOOR(RAND() * 100000000), 8, '0'))
FROM (
              SELECT 'João Silva'       AS Nome, 'joao.silva@email.com'      AS Email
    UNION ALL SELECT 'Maria Souza',           'maria.souza@email.com'
    UNION ALL SELECT 'Cecília Ramos',         'cecilia.ramos@email.com'
    UNION ALL SELECT 'Pedro Almeida',         'pedro.almeida@email.com'
    UNION ALL SELECT 'Ana Beatriz Lima',      'ana.lima@email.com'
    UNION ALL SELECT 'Rafael Nogueira',       'rafael.nogueira@email.com'
    UNION ALL SELECT 'Juliana Prado',         'juliana.prado@email.com'
    UNION ALL SELECT 'Thiago Mendes',         'thiago.mendes@email.com'
    UNION ALL SELECT 'Larissa Fontes',        'larissa.fontes@email.com'
    UNION ALL SELECT 'Bruno Carvalho',        'bruno.carvalho@email.com'
    UNION ALL SELECT 'Camila Duarte',         'camila.duarte@email.com'
    UNION ALL SELECT 'Eduardo Tavares',       'eduardo.tavares@email.com'
    UNION ALL SELECT 'Fernanda Rocha',        'fernanda.rocha@email.com'
    UNION ALL SELECT 'Gustavo Pinheiro',      'gustavo.pinheiro@email.com'
    UNION ALL SELECT 'Helena Barros',         'helena.barros@email.com'
) AS pessoas
ORDER BY RAND()
LIMIT 5;

-- 10 vendas sorteadas. O CROSS JOIN com o multiplicador permite que o mesmo par
-- (usuario, pizza) saia mais de uma vez -- um cliente pode repetir o pedido.
-- ValorTotal e CustoTotal sao calculados (Preco/Custo x Quantidade), nao
-- sorteados, para os totais e o lucro da aplicacao fecharem com o cardapio.
-- Todas saem como pizza do tipo de fator 1 (Normal), sem borda e com o status
-- padrao da coluna (1, Preparando).
INSERT INTO Vendas (UsuarioId, PizzaId, Quantidade, ValorTotal, CustoTotal, DataVenda, tipo_da_pizza)
SELECT UsuarioId, PizzaId, Quantidade, ROUND(Preco * Quantidade, 2), ROUND(Custo * Quantidade, 2), DataVenda,
       (SELECT Id FROM Tipo_pizza WHERE Calculo_valor = 1 ORDER BY Id LIMIT 1)
FROM (
    SELECT
        u.Id                                            AS UsuarioId,
        p.Id                                            AS PizzaId,
        p.Preco                                         AS Preco,
        p.Custo                                         AS Custo,
        1 + FLOOR(RAND() * 4)                           AS Quantidade,
        NOW() - INTERVAL FLOOR(RAND() * 43200) MINUTE   AS DataVenda
    FROM Usuarios u
    CROSS JOIN Pizzas p
    CROSS JOIN (SELECT 1 AS n UNION ALL SELECT 2 UNION ALL SELECT 3) AS multiplicador
    ORDER BY RAND()
    LIMIT 10
) AS sorteio;
