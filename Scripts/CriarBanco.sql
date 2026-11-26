-- =============================================
-- Pizzaria - Schema MySQL 8.4
--
-- Espelha o banco de producao (conferido em 25/11/2026). Todas as tabelas usam
-- utf8mb4_unicode_ci: misturar collations faz comparacao de texto entre
-- tabelas falhar com "Illegal mix of collations".
--
-- A API NAO executa este arquivo: a conta dela no banco so le e grava dados.
--   * Na sua maquina, o container do MySQL roda o script sozinho na primeira
--     subida de um volume vazio (ver docker-compose.yml).
--   * Num banco novo de producao, rode uma vez com uma conta administrativa.
-- Ele e idempotente: CREATE TABLE IF NOT EXISTS nao altera tabela que ja
-- existe, e cada bloco de dados so insere quando a tabela esta vazia.
--
-- Um banco local criado com a versao antiga deste script NAO ganha as colunas
-- novas: apague o volume (docker compose down -v) e suba de novo.
-- =============================================

-- Tabela de Pizzas
CREATE TABLE IF NOT EXISTS Pizzas (
    Id          INT           NOT NULL AUTO_INCREMENT,
    Nome        VARCHAR(200)  NOT NULL,
    Descricao   VARCHAR(400)  NOT NULL,
    Preco       DECIMAL(10,2) NOT NULL,
    Custo       DECIMAL(10,2) NOT NULL,
    Obs         VARCHAR(200)  DEFAULT NULL,
    Qtd         INT           DEFAULT 10,
    PRIMARY KEY (Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabela de Usuarios
CREATE TABLE IF NOT EXISTS Usuarios (
    Id          INT          NOT NULL AUTO_INCREMENT,
    Nome        VARCHAR(200) NOT NULL,
    Email       VARCHAR(200) NOT NULL,
    Telefone    VARCHAR(20)  NOT NULL,
    PRIMARY KEY (Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Status da venda (Preparando, A caminho, Entregue)
CREATE TABLE IF NOT EXISTS Status (
    StatusId    INT          NOT NULL AUTO_INCREMENT,
    Nome        VARCHAR(50)  NOT NULL,
    Cor         CHAR(7)      NOT NULL,
    PRIMARY KEY (StatusId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tipo da pizza. Calculo_valor e o fator aplicado ao preco (Broto = 0.5)
CREATE TABLE IF NOT EXISTS Tipo_pizza (
    Id            INT          NOT NULL AUTO_INCREMENT,
    Nome          VARCHAR(50)  NOT NULL,
    Calculo_valor FLOAT        NOT NULL,
    PRIMARY KEY (Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Bordas recheadas e seus precos
CREATE TABLE IF NOT EXISTS Borda_pizza (
    id          INT          NOT NULL AUTO_INCREMENT,
    Nome        VARCHAR(50)  NOT NULL,
    Valor       FLOAT        NOT NULL,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Acompanhamentos vendidos junto com a pizza
CREATE TABLE IF NOT EXISTS Acompanhamentos (
    ID          INT           NOT NULL AUTO_INCREMENT,
    Nome        VARCHAR(45)   DEFAULT NULL,
    Descricao   VARCHAR(100)  DEFAULT NULL,
    Preco       DECIMAL(10,2) DEFAULT NULL,
    Categoria   VARCHAR(50)   DEFAULT NULL,
    PRIMARY KEY (ID)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabela de Vendas
-- borda e tipo_da_pizza apontam para Borda_pizza e Tipo_pizza, e StatusId para
-- Status, mas sem chave estrangeira (igual a producao).
CREATE TABLE IF NOT EXISTS Vendas (
    Id               INT           NOT NULL AUTO_INCREMENT,
    UsuarioId        INT           NOT NULL,
    PizzaId          INT           NOT NULL,
    Quantidade       INT           NOT NULL,
    CustoTotal       DECIMAL(10,2) DEFAULT NULL,
    ValorTotal       DECIMAL(10,2) NOT NULL,
    DataVenda        DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    borda            INT           DEFAULT NULL,
    tipo_da_pizza    INT           DEFAULT NULL,
    StatusId         INT           NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    KEY IX_Vendas_UsuarioId (UsuarioId),
    KEY IX_Vendas_PizzaId (PizzaId),
    CONSTRAINT FK_Vendas_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios (Id),
    CONSTRAINT FK_Vendas_Pizzas   FOREIGN KEY (PizzaId)   REFERENCES Pizzas (Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Itens de acompanhamento de cada venda
CREATE TABLE IF NOT EXISTS VendaAcompanhamentos (
    Id               INT           NOT NULL AUTO_INCREMENT,
    VendaId          INT           NOT NULL,
    AcompanhamentoId INT           NOT NULL,
    Quantidade       INT           NOT NULL,
    PrecoUnitario    DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (Id),
    KEY VendaId (VendaId),
    KEY AcompanhamentoId (AcompanhamentoId),
    CONSTRAINT VendaAcompanhamentos_ibfk_1 FOREIGN KEY (VendaId)          REFERENCES Vendas (Id),
    CONSTRAINT VendaAcompanhamentos_ibfk_2 FOREIGN KEY (AcompanhamentoId) REFERENCES Acompanhamentos (ID)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Administradores (ainda sem uso na API)
CREATE TABLE IF NOT EXISTS Admins (
    Id          INT          NOT NULL AUTO_INCREMENT,
    Usuario     VARCHAR(100) NOT NULL,
    SenhaHash   CHAR(64)     NOT NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY UQ_Admins_Usuario (Usuario)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Dados. Cada bloco só insere quando a tabela está vazia, ou seja:
-- popula um banco novo e nunca ressuscita um registro que alguém apagou pela API.
-- =============================================

-- Cadastros de apoio, com os mesmos Ids de producao
INSERT IGNORE INTO Status (StatusId, Nome, Cor)
SELECT * FROM (
              SELECT 1 AS StatusId, 'Preparando' AS Nome, '#FF0000' AS Cor
    UNION ALL SELECT 2,             'A caminho',         '#C4FF0E'
    UNION ALL SELECT 3,             'Entregue',          '#008000'
) AS seed
WHERE NOT EXISTS (SELECT 1 FROM Status);

INSERT IGNORE INTO Tipo_pizza (Id, Nome, Calculo_valor)
SELECT * FROM (
              SELECT 1 AS Id, 'Broto' AS Nome, 0.5 AS Calculo_valor
    UNION ALL SELECT 2,       'Normal',        1
) AS seed
WHERE NOT EXISTS (SELECT 1 FROM Tipo_pizza);

INSERT IGNORE INTO Borda_pizza (id, Nome, Valor)
SELECT * FROM (
              SELECT 4 AS id, 'Catupiry' AS Nome, 15 AS Valor
    UNION ALL SELECT 5,       'Chocolate',        16
    UNION ALL SELECT 6,       'Doce de leite',    16
    UNION ALL SELECT 7,       'Kinder Bueno',     20
    UNION ALL SELECT 8,       'Cheddar',          12
    UNION ALL SELECT 9,       'Cream cheese',     16
    UNION ALL SELECT 10,      'Normal',           0
) AS seed
WHERE NOT EXISTS (SELECT 1 FROM Borda_pizza);

INSERT IGNORE INTO Acompanhamentos (ID, Nome, Descricao, Preco, Categoria)
SELECT * FROM (
              SELECT 4 AS ID, 'Batata frita' AS Nome, 'Porção de batata'          AS Descricao, 35.00 AS Preco, 'Porcoes'       AS Categoria
    UNION ALL SELECT 5,       'Coca',                 'Lata de Coca-Cola 350 ml',                10.00,          'Refrigerantes'
    UNION ALL SELECT 7,       'Guaraná',              'Lata de guaraná 350 ml',                  10.00,          'Refrigerantes'
) AS seed
WHERE NOT EXISTS (SELECT 1 FROM Acompanhamentos);

-- Dados de exemplo
INSERT IGNORE INTO Pizzas (Id, Nome, Descricao, Preco, Custo, Obs, Qtd)
SELECT * FROM (
              SELECT 1 AS Id, 'Margherita'     AS Nome, 'Molho de tomate, mussarela e manjericao'     AS Descricao, 35.00 AS Preco, 10.00 AS Custo, '' AS Obs, 10 AS Qtd
    UNION ALL SELECT 2,       'Calabresa',            'Calabresa, cebola e mussarela',                      38.00,          7.00,          '',        10
    UNION ALL SELECT 3,       'Quatro Queijos',       'Mussarela, parmesao, gorgonzola e provolone',        42.00,          5.00,          '',        10
    UNION ALL SELECT 4,       'Portuguesa',           'Presunto, ovo, cebola, azeitona e mussarela',        40.00,          15.00,         '',        10
) AS seed
WHERE NOT EXISTS (SELECT 1 FROM Pizzas);

INSERT IGNORE INTO Usuarios (Id, Nome, Email, Telefone)
SELECT * FROM (
              SELECT 1 AS Id, 'Joao Silva'  AS Nome, 'joao@email.com'  AS Email, '11999990001' AS Telefone
    UNION ALL SELECT 2,       'Maria Souza',       'maria@email.com',        '11999990002'
) AS seed
WHERE NOT EXISTS (SELECT 1 FROM Usuarios);

-- As duas últimas condições evitam erro de foreign key caso as pizzas ou os
-- usuários de exemplo já tenham sido apagados.
INSERT IGNORE INTO Vendas (Id, UsuarioId, PizzaId, Quantidade, ValorTotal, CustoTotal, DataVenda, tipo_da_pizza, StatusId)
SELECT * FROM (
              SELECT 1 AS Id, 1 AS UsuarioId, 1 AS PizzaId, 2 AS Quantidade, 70.00 AS ValorTotal, 20.00 AS CustoTotal, NOW() AS DataVenda, 2 AS tipo_da_pizza, 1 AS StatusId
    UNION ALL SELECT 2,       2,              3,             1,              42.00,                5.00,                NOW(),             2,                   1
) AS seed
WHERE NOT EXISTS (SELECT 1 FROM Vendas)
  AND EXISTS (SELECT 1 FROM Usuarios WHERE Id = seed.UsuarioId)
  AND EXISTS (SELECT 1 FROM Pizzas   WHERE Id = seed.PizzaId);
