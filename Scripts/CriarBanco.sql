-- =============================================
-- Pizzaria - Schema SQLite
-- =============================================

-- Tabela de Pizzas
CREATE TABLE IF NOT EXISTS Pizzas (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    Nome        TEXT    NOT NULL,
    Descricao   TEXT    NOT NULL,
    Preco       REAL    NOT NULL
);

-- Tabela de Usuarios
CREATE TABLE IF NOT EXISTS Usuarios (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    Nome        TEXT    NOT NULL,
    Email       TEXT    NOT NULL,
    Telefone    TEXT    NOT NULL
);

-- Tabela de Vendas
CREATE TABLE IF NOT EXISTS Vendas (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    UsuarioId   INTEGER NOT NULL REFERENCES Usuarios(Id),
    PizzaId     INTEGER NOT NULL REFERENCES Pizzas(Id),
    Quantidade  INTEGER NOT NULL,
    ValorTotal  REAL    NOT NULL,
    DataVenda   TEXT    NOT NULL
);

-- Dados de exemplo (só insere se tabela estiver vazia)
INSERT INTO Pizzas (Nome, Descricao, Preco)
    SELECT 'Margherita', 'Molho de tomate, mussarela e manjericao', 35.00
    WHERE NOT EXISTS (SELECT 1 FROM Pizzas);

INSERT INTO Pizzas (Nome, Descricao, Preco)
    SELECT 'Calabresa', 'Calabresa, cebola e mussarela', 38.00
    WHERE (SELECT COUNT(*) FROM Pizzas) < 2;

INSERT INTO Pizzas (Nome, Descricao, Preco)
    SELECT 'Quatro Queijos', 'Mussarela, parmesao, gorgonzola e provolone', 42.00
    WHERE (SELECT COUNT(*) FROM Pizzas) < 3;

INSERT INTO Pizzas (Nome, Descricao, Preco)
    SELECT 'Portuguesa', 'Presunto, ovo, cebola, azeitona e mussarela', 40.00
    WHERE (SELECT COUNT(*) FROM Pizzas) < 4;

INSERT INTO Usuarios (Nome, Email, Telefone)
    SELECT 'Joao Silva', 'joao@email.com', '11999990001'
    WHERE NOT EXISTS (SELECT 1 FROM Usuarios);

INSERT INTO Usuarios (Nome, Email, Telefone)
    SELECT 'Maria Souza', 'maria@email.com', '11999990002'
    WHERE (SELECT COUNT(*) FROM Usuarios) < 2;

INSERT INTO Vendas (UsuarioId, PizzaId, Quantidade, ValorTotal, DataVenda)
    SELECT 1, 1, 2, 70.00, datetime('now')
    WHERE NOT EXISTS (SELECT 1 FROM Vendas);

INSERT INTO Vendas (UsuarioId, PizzaId, Quantidade, ValorTotal, DataVenda)
    SELECT 2, 3, 1, 42.00, datetime('now')
    WHERE (SELECT COUNT(*) FROM Vendas) < 2;
