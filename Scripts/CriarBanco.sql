-- =============================================
-- Pizzaria - Schema MySQL 8.4
-- =============================================

-- Tabela de Pizzas
CREATE TABLE IF NOT EXISTS Pizzas (
    Id          INT           NOT NULL AUTO_INCREMENT,
    Nome        VARCHAR(200)  NOT NULL,
    Descricao   VARCHAR(400)  NOT NULL,
    Preco       DECIMAL(10,2) NOT NULL,
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

-- Tabela de Vendas
CREATE TABLE IF NOT EXISTS Vendas (
    Id          INT           NOT NULL AUTO_INCREMENT,
    UsuarioId   INT           NOT NULL,
    PizzaId     INT           NOT NULL,
    Quantidade  INT           NOT NULL,
    ValorTotal  DECIMAL(10,2) NOT NULL,
    DataVenda   DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    KEY IX_Vendas_UsuarioId (UsuarioId),
    KEY IX_Vendas_PizzaId (PizzaId),
    CONSTRAINT FK_Vendas_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios (Id),
    CONSTRAINT FK_Vendas_Pizzas   FOREIGN KEY (PizzaId)   REFERENCES Pizzas (Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Dados de exemplo. Cada bloco só insere quando a tabela está vazia, ou seja:
-- popula um banco novo e nunca ressuscita um registro que alguém apagou pela API.
INSERT IGNORE INTO Pizzas (Id, Nome, Descricao, Preco)
SELECT * FROM (
              SELECT 1 AS Id, 'Margherita'     AS Nome, 'Molho de tomate, mussarela e manjericao'     AS Descricao, 35.00 AS Preco
    UNION ALL SELECT 2,       'Calabresa',            'Calabresa, cebola e mussarela',                      38.00
    UNION ALL SELECT 3,       'Quatro Queijos',       'Mussarela, parmesao, gorgonzola e provolone',        42.00
    UNION ALL SELECT 4,       'Portuguesa',           'Presunto, ovo, cebola, azeitona e mussarela',        40.00
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
INSERT IGNORE INTO Vendas (Id, UsuarioId, PizzaId, Quantidade, ValorTotal, DataVenda)
SELECT * FROM (
              SELECT 1 AS Id, 1 AS UsuarioId, 1 AS PizzaId, 2 AS Quantidade, 70.00 AS ValorTotal, NOW() AS DataVenda
    UNION ALL SELECT 2,       2,              3,             1,              42.00,                NOW()
) AS seed
WHERE NOT EXISTS (SELECT 1 FROM Vendas)
  AND EXISTS (SELECT 1 FROM Usuarios WHERE Id = seed.UsuarioId)
  AND EXISTS (SELECT 1 FROM Pizzas   WHERE Id = seed.PizzaId);
