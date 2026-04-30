-- =============================================
-- Script para criar o banco de dados e as tabelas
-- Execute: mysql -u root -p < CriarBanco.sql
-- =============================================

CREATE DATABASE IF NOT EXISTS PizzariaDb;

USE PizzariaDb;

-- Tabela de Pizzas
CREATE TABLE IF NOT EXISTS Pizzas (
    Id          INT             AUTO_INCREMENT PRIMARY KEY,
    Nome        VARCHAR(100)    NOT NULL,
    Descricao   VARCHAR(500)    NOT NULL,
    Preco       DECIMAL(18,2)   NOT NULL
);

-- Tabela de Usuarios
CREATE TABLE IF NOT EXISTS Usuarios (
    Id          INT             AUTO_INCREMENT PRIMARY KEY,
    Nome        VARCHAR(100)    NOT NULL,
    Email       VARCHAR(200)    NOT NULL,
    Telefone    VARCHAR(20)     NOT NULL
);

-- Tabela de Vendas
CREATE TABLE IF NOT EXISTS Vendas (
    Id          INT             AUTO_INCREMENT PRIMARY KEY,
    UsuarioId   INT             NOT NULL,
    PizzaId     INT             NOT NULL,
    Quantidade  INT             NOT NULL,
    ValorTotal  DECIMAL(18,2)   NOT NULL,
    DataVenda   DATETIME        NOT NULL,
    FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id),
    FOREIGN KEY (PizzaId)   REFERENCES Pizzas(Id)
);

-- Dados de exemplo
INSERT INTO Pizzas (Nome, Descricao, Preco) VALUES
    ('Margherita',      'Molho de tomate, mussarela e manjericao',   35.00),
    ('Calabresa',       'Calabresa, cebola e mussarela',            38.00),
    ('Quatro Queijos',  'Mussarela, parmesao, gorgonzola e provolone', 42.00),
    ('Portuguesa',      'Presunto, ovo, cebola, azeitona e mussarela', 40.00);

INSERT INTO Usuarios (Nome, Email, Telefone) VALUES
    ('Joao Silva',      'joao@email.com',   '11999990001'),
    ('Maria Souza',     'maria@email.com',  '11999990002');

INSERT INTO Vendas (UsuarioId, PizzaId, Quantidade, ValorTotal, DataVenda) VALUES
    (1, 1, 2, 70.00,  NOW()),
    (2, 3, 1, 42.00,  NOW());
