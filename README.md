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
├── Scripts/             # Scripts SQL e utilitários
│   ├── CriarBanco.sql               # Schema + seed inicial
│   ├── ResetDiario.sql              # Repopula os dados de demonstração
│   ├── AgendadorReset.sh            # Dispara o reset todo dia
│   └── MigrarDadosSqliteParaMysql.sh
│
├── Program.cs               # Configuração da aplicação
├── docker-compose.yml       # Ambiente LOCAL — sobe direto depois do clone
├── docker-compose.prod.yml  # Ambiente do SERVIDOR — Traefik, .env, reset diário
├── .env.example             # Modelo do .env de produção (o local não usa .env)
└── appsettings.json         # Connection string usada pelo `dotnet run`
```

---

## 🚀 Tecnologias Utilizadas

- .NET 9  
- ASP.NET Core Web API  
- ADO.NET  
- MySQL 8.4 (driver [MySqlConnector](https://mysqlconnector.net/))  
- Swagger (OpenAPI)  

---

## 🗄️ Banco de Dados

O schema tem **3 tabelas**, criadas automaticamente na primeira execução da API
a partir de `Scripts/CriarBanco.sql`:

| Tabela | Descrição |
|---|---|
| `Pizzas` | Cardápio — `Id`, `Nome`, `Descricao`, `Preco` |
| `Usuarios` | Clientes — `Id`, `Nome`, `Email`, `Telefone` |
| `Vendas` | Pedidos — `Id`, `UsuarioId` (FK), `PizzaId` (FK), `Quantidade`, `ValorTotal`, `DataVenda` |

`Vendas` é a tabela associativa: N:1 com `Usuarios` e N:1 com `Pizzas`, com
foreign keys InnoDB. O script é idempotente (`CREATE TABLE IF NOT EXISTS` +
`INSERT IGNORE`), então pode rodar em toda subida sem duplicar dados.

---

## 🔧 Como Executar o Projeto

O repositório tem **dois arquivos de compose**, e é isso que faz o clone rodar
sem configuração:

| Arquivo | Onde | Precisa de `.env`? |
|---|---|---|
| `docker-compose.yml` | sua máquina | ❌ não — já vem com credenciais de desenvolvimento |
| `docker-compose.prod.yml` | servidor (usado pelo deploy) | ✅ sim — escrito a partir dos secrets |

---

### 1. Clonar e subir

```bash
git clone https://github.com/LuizRosa-Aulas/Pizzaria.API.git
cd Pizzaria.API
docker compose up -d --build
```

É só isso: **não há `.env` para preencher nem senha para inventar**. Sobem dois
containers — `pizzaria-mysql-dev` e `pizzaria-api-dev` — e a API espera o
healthcheck do banco passar antes de iniciar, criando o schema sozinha a partir
de `Scripts/CriarBanco.sql`.

| Endereço | O que é |
|---|---|
| http://localhost:8080/swagger | Swagger da API |
| http://localhost:8080/api/pizzas | já responde com o cardápio de exemplo |
| `localhost:3307` | MySQL — usuário `pizzaria`, senha `pizzaria_dev` |

> A porta do MySQL local é a **3307**, não a 3306: quem já tem MySQL ou XAMPP
> instalado na máquina tem a 3306 ocupada, e o `docker compose up` morreria no
> bind da porta.

Se ainda assim alguma porta estiver ocupada, dá para trocar sem editar arquivo:

```bash
API_PORT=8081 MYSQL_PORT=3316 docker compose up -d --build
```

Dia a dia:

```bash
docker compose logs -f pizzaria-api   # acompanhar a API
docker compose down                   # para tudo, preserva o banco
docker compose down -v                # apaga também o volume: recomeça do zero
```

---

### 2. Ou rodar a API na máquina (Visual Studio / `dotnet run`)

Sobe só o banco no Docker e deixa a API na sua máquina, com breakpoint e tudo:

```bash
docker compose up -d pizzaria-mysql
dotnet run
```

Swagger em http://localhost:5051/swagger. As credenciais do `appsettings.json`
são as mesmas do compose local, então **não há nada para ajustar**:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3307;Database=pizzaria;User Id=pizzaria;Password=pizzaria_dev;"
}
```

---

### 3. Por que as senhas de desenvolvimento estão versionadas

O `docker-compose.yml` traz `pizzaria_dev` e `root_dev` em texto claro de
propósito: esse banco só escuta em `127.0.0.1`, é descartável e existe para a
aula subir sem configuração nenhuma.

**Nada disso vale para o servidor.** Lá o ambiente é o `docker-compose.prod.yml`,
que **não tem senha padrão** — se o `.env` não existir, o `docker compose` falha
na hora em vez de subir um banco exposto com senha conhecida.

---

### 4. Ambiente de produção (`docker-compose.prod.yml`)

É o arquivo que o deploy usa, e o único que precisa de `.env`:

```bash
docker compose -f docker-compose.prod.yml up -d --build
```

Sobre o local, ele acrescenta: os labels do Traefik, a rede externa `web`, a
porta 3306 publicada e o container `pizzaria-seed` do reset diário.

#### O `.env` (só produção)

```bash
cp .env.example .env
```

```env
MYSQL_ROOT_PASSWORD=sua_senha_root
MYSQL_DATABASE=pizzaria
MYSQL_USER=pizzaria
MYSQL_PASSWORD=sua_senha_do_app

RESET_HORA=03:00
RESET_AO_SUBIR=false
TZ=America/Sao_Paulo
```

> O `.env` está no `.gitignore` **e** no `--exclude` do rsync: nunca vai para o
> repositório e não é sobrescrito pela sincronização de arquivos.

**No servidor você não cria nada à mão** — o deploy escreve o `/opt/apps/Pizzaria.API/.env`
a partir dos secrets do GitHub. Cadastre em *Settings → Secrets and variables → Actions*:

| Nome | Tipo | Obrigatório |
|---|---|---|
| `MYSQL_ROOT_PASSWORD` | Secret | ✅ |
| `MYSQL_PASSWORD` | Secret | ✅ |
| `RESET_HORA` | Variable | opcional (padrão `03:00`) |
| `TZ` | Variable | opcional (padrão `America/Sao_Paulo`) |

Sem os dois secrets o deploy **para antes de mexer no servidor**, com a mensagem
dizendo qual falta. O conteúdo do `.env` vai pelo stdin do `ssh` (não pela linha
de comando) e o arquivo é criado com `umask 077`.

> ⚠️ `MYSQL_ROOT_PASSWORD` e `MYSQL_PASSWORD` só são aplicados na **primeira**
> subida, quando o volume `mysql_data` é criado. Trocar o secret depois não troca
> a senha do banco — para rotacionar, altere direto no MySQL:
>
> ```bash
> docker exec -it pizzaria-mysql mysql -u root -p >   -e "ALTER USER 'pizzaria'@'%' IDENTIFIED BY 'nova_senha';"
> ```
>
> e atualize o secret para que os próximos deploys gerem o `.env` coerente.

---

## 🐬 Acessar o banco pelo MySQL Workbench

A porta 3306 está publicada na internet, então a conexão é direta:

1. *MySQL Connections* → **+**
2. **Connection Method:** `Standard (TCP/IP)`
3. Preencha:

| Campo | Valor |
|---|---|
| Hostname | `pizzaria.viniciusguedes.cloud` (ou o IP do servidor) |
| Port | `3306` |
| Username | `root` (acesso total) ou `pizzaria` (só o schema da app) |
| Password | a senha correspondente no `.env` |
| Default Schema | `pizzaria` |

*Test Connection* → *OK*.

> ⚠️ **A porta está aberta para a internet.** A imagem `mysql:8.4` cria
> `root@'%'`, ou seja o root responde de qualquer IP — as senhas do `.env` são a
> única barreira. Recomendações, em ordem de custo/benefício:
>
> - senhas longas e exclusivas (não reaproveite de outro serviço);
> - `ufw allow from SEU_IP to any port 3306` em vez de liberar para todos;
> - `fail2ban` com o filtro `mysqld-auth` para cortar força bruta;
> - para fechar de vez, troque `"3306:3306"` por `"127.0.0.1:3306:3306"` no
>   `docker-compose.prod.yml` e conecte pelo Workbench com
>   *Standard TCP/IP over SSH*.

Para acesso pelo terminal do servidor:

```bash
docker exec -it pizzaria-mysql mysql -u pizzaria -p pizzaria
```

---

## 🔁 Reset diário dos dados

> Este container só existe no `docker-compose.prod.yml` — no ambiente local o
> banco não é resetado sozinho.

O container `pizzaria-seed` **apaga tudo e repopula** o banco todo dia no horário
de `RESET_HORA`, deixando sempre:

- **5 pizzas** sorteadas de um catálogo de 15, preço aleatório entre 30,00 e 60,00
- **5 usuários** sorteados de uma lista de 15, telefone aleatório
- **10 vendas** com cliente, pizza, quantidade (1 a 4) e data (últimos 30 dias)
  aleatórios

O `ValorTotal` de cada venda é **calculado** (`Preco × Quantidade`), não sorteado,
para os totais fecharem com o cardápio. `TRUNCATE` zera o `AUTO_INCREMENT`, então
os Ids voltam a começar em 1 a cada reset.

| Variável | Padrão | O que faz |
|---|---|---|
| `RESET_HORA` | `03:00` | Horário do reset, formato `HH:MM` |
| `RESET_AO_SUBIR` | `false` | `true` reseta também ao subir o container |
| `TZ` | `America/Sao_Paulo` | Fuso usado para interpretar `RESET_HORA` |

```bash
# Ver o agendamento e o histórico de resets
docker logs -f pizzaria-seed

# Forçar um reset agora, sem esperar o horário (no servidor)
docker exec -i pizzaria-mysql mysql -u pizzaria -p pizzaria < Scripts/ResetDiario.sql

# O mesmo na sua máquina, contra o banco local
docker exec -i pizzaria-mysql-dev mysql -u pizzaria -ppizzaria_dev pizzaria < Scripts/ResetDiario.sql

# Desligar o reset diário (servidor)
docker compose -f docker-compose.prod.yml stop pizzaria-seed
```

> ⚠️ **Este container destrói dados todo dia.** Ele existe porque o ambiente é de
> demonstração/aula. Se algum dia o banco passar a guardar dado que importa,
> remova o serviço `pizzaria-seed` do `docker-compose.prod.yml`.

---

## 🔄 Migração do SQLite antigo

> ⚠️ Faz sentido apenas se você **desligar o reset diário** — senão os dados
> migrados são apagados no próximo `RESET_HORA`.

Versões anteriores usavam SQLite (`/data/pizzaria.db` num volume Docker). Para
trazer os dados existentes:

```bash
# 1. No servidor, com o volume do SQLite ainda existente:
./Scripts/MigrarDadosSqliteParaMysql.sh

# 2. Subir o MySQL novo:
docker compose -f docker-compose.prod.yml up -d --build

# 3. Carregar os dados (substitui os dados de exemplo):
docker exec -i pizzaria-mysql mysql -u root -p'SUA_SENHA_ROOT' pizzaria < dados-pizzaria-mysql.sql

# 4. Só depois de conferir, remover o volume antigo:
docker volume rm pizzariaapi_sqlite_data
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

**Senha 1/2** `zQN(H>`
File -> decrypt db_2026-01-23.sql.enc`
