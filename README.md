# 🍕 Pizzaria.API

API REST para gerenciamento de uma pizzaria, com CRUD completo de pizzas, usuários e vendas. Desenvolvida em **.NET 9** utilizando **ADO.NET (SQL puro)**, proporcionando maior controle sobre as queries e melhor entendimento do acesso a dados.

---

## 🌐 Demo Online

A API está disponível para testes em:

👉 http://pizzaria-api.viniciusguedes.cloud/swagger

Você pode utilizar ferramentas como **Postman**, **Insomnia** ou o próprio navegador via Swagger.

---

## 🔗 Repositórios

- 🔙 Backend (API):  
👉 https://github.com/LuizRosa-Aulas/Pizzaria.API  

- 🎨 Frontend (UI):  
👉 https://github.com/LuizRosa-Aulas/Pizzaria.UI

---

## 📋 Sobre o Projeto

Este projeto simula o backend de uma pizzaria, permitindo:

- Cadastro de pizzas  
- Cadastro de usuários  
- Registro de vendas  

A aplicação foi construída com foco educacional, ideal para aprendizado de:

- Consumo de APIs REST  
- Estruturação de backend  
- Acesso a banco de dados sem ORM  

---

## 🏗️ Arquitetura

O projeto segue uma **arquitetura em 3 camadas**, simples e didática:

- **Controllers**  
  Responsáveis por receber as requisições HTTP e retornar as respostas.

- **Repositories**  
  Responsáveis pelo acesso ao banco de dados utilizando **SQL puro (ADO.NET)**.

- **Models**  
  Representação das entidades do sistema.

---

## 📁 Estrutura do Projeto

```bash
Pizzaria.API/
│
├── Controllers/         # Endpoints REST
│   ├── PizzasController.cs
│   ├── UsuariosController.cs
│   └── VendasController.cs
│
├── Models/              # Entidades
│   ├── Pizza.cs
│   ├── Usuario.cs
│   └── Venda.cs
│
├── Repositories/        # Acesso a dados (SQL puro com ADO.NET)
│   ├── PizzaRepository.cs
│   ├── UsuarioRepository.cs
│   └── VendaRepository.cs
│
├── Scripts/             # Scripts SQL para criação do banco
│   └── CriarBanco.sql
│
├── Program.cs           # Configuração da aplicação
└── appsettings.json     # Connection string e configurações
```

---

## 🚀 Tecnologias Utilizadas

- .NET 9  
- ASP.NET Core Web API  
- ADO.NET  
- SQL Server  
- Swagger (OpenAPI)  

---

## 🔧 Como Executar o Projeto

### 1. Clonar o repositório

```bash
git clone https://github.com/LuizRosa-Aulas/pizzaria-api.git
```

---

### 2. Configurar o banco de dados

- Execute o script:

```bash
Scripts/CriarBanco.sql
```

- Configure a **connection string** no `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=SEU_SERVIDOR;Database=PizzariaDB;Trusted_Connection=True;"
}
```

---

### 3. Executar a aplicação

```bash
dotnet run
```

---

### 4. Acessar o Swagger (local)

```bash
https://localhost:xxxx/swagger
```

---

## 📌 Endpoints Principais

### 🍕 Pizzas
- `GET /api/pizzas`
- `GET /api/pizzas/{id}`
- `POST /api/pizzas`
- `PUT /api/pizzas/{id}`
- `DELETE /api/pizzas/{id}`

---

### 👤 Usuários
- `GET /api/usuarios`
- `POST /api/usuarios`

---

### 🛒 Vendas
- `GET /api/vendas`
- `POST /api/vendas`

---

## 🎯 Objetivo Educacional

Este projeto foi desenvolvido para:

- Ensinar conceitos de API REST  
- Servir como base para exercícios de consumo de API (frontend, Postman, etc.)  

---

## 🕵️ Desafio das Estrelas (atividade de aula)

> ⚠️ Esta seção faz parte de um **jogo investigativo educacional** (*Desafio das Estrelas*).
> O valor abaixo **não é uma credencial real** do sistema — é apenas uma pista do jogo.

Perito, você seguiu os logs até aqui. 🎯

**1ª metade da chave do arquivo lacrado:** `zQN(H>`

A **2ª metade** está escondida no **front-end** da *Pizzaria.UI* (dica: `Ctrl+U` ou F12).
Junte as duas metades e rode `decrypt db_2026-01-23.sql.enc` no console forense.
