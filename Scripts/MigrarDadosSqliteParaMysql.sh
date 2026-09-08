#!/usr/bin/env bash
# =============================================================================
# Exporta os dados do banco SQLite antigo para INSERTs compativeis com MySQL.
#
# Rode NO SERVIDOR, com o volume do SQLite ainda existente (antes de remove-lo).
# Gera um arquivo .sql que voce carrega no MySQL novo.
#
#   ./Scripts/MigrarDadosSqliteParaMysql.sh [nome-do-volume] [arquivo-de-saida]
# =============================================================================
set -euo pipefail

VOLUME="${1:-}"
SAIDA="${2:-./dados-pizzaria-mysql.sql}"

if [ -z "$VOLUME" ]; then
    VOLUME="$(docker volume ls --format '{{.Name}}' | grep -m1 'sqlite_data' || true)"
fi

if [ -z "$VOLUME" ]; then
    echo "ERRO: nenhum volume com 'sqlite_data' no nome foi encontrado." >&2
    echo "Liste os volumes com 'docker volume ls' e passe o nome como 1o argumento." >&2
    exit 1
fi

echo "Lendo do volume: $VOLUME"

# O SQLite roda em modo WAL, entao copiamos os arquivos para /tmp dentro do
# container antes de ler -- assim o volume pode ser montado como read-only.
docker run --rm -i -v "$VOLUME":/data:ro alpine:3 sh -s <<'DENTRO' > "$SAIDA"
set -e
apk add --no-cache sqlite >/dev/null 2>&1
cp /data/pizzaria.db* /tmp/ 2>/dev/null || cp /data/pizzaria.db /tmp/

# NO_BACKSLASH_ESCAPES faz o MySQL interpretar as strings do mesmo jeito que o
# SQLite as escreveu (o quote() do SQLite nao escapa barra invertida).
echo "SET sql_mode='NO_BACKSLASH_ESCAPES';"
echo "SET FOREIGN_KEY_CHECKS=0;"
echo "DELETE FROM Vendas;"
echo "DELETE FROM Usuarios;"
echo "DELETE FROM Pizzas;"

sqlite3 /tmp/pizzaria.db <<'SQL'
.headers off
.mode list
SELECT 'INSERT INTO Pizzas (Id, Nome, Descricao, Preco) VALUES ('
       || Id || ', ' || quote(Nome) || ', ' || quote(Descricao) || ', ' || Preco || ');'
  FROM Pizzas ORDER BY Id;

SELECT 'INSERT INTO Usuarios (Id, Nome, Email, Telefone) VALUES ('
       || Id || ', ' || quote(Nome) || ', ' || quote(Email) || ', ' || quote(Telefone) || ');'
  FROM Usuarios ORDER BY Id;

SELECT 'INSERT INTO Vendas (Id, UsuarioId, PizzaId, Quantidade, ValorTotal, DataVenda) VALUES ('
       || Id || ', ' || UsuarioId || ', ' || PizzaId || ', ' || Quantidade || ', '
       || ValorTotal || ', ' || quote(DataVenda) || ');'
  FROM Vendas ORDER BY Id;
SQL

echo "SET FOREIGN_KEY_CHECKS=1;"
DENTRO

LINHAS="$(grep -c '^INSERT' "$SAIDA" || true)"
echo "Gerado: $SAIDA ($LINHAS INSERTs)"
echo
echo "Para carregar no MySQL novo:"
echo "  docker exec -i pizzaria-mysql mysql -u root -p'SUA_SENHA_ROOT' pizzaria < $SAIDA"
