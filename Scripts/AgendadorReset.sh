#!/usr/bin/env bash
# =============================================================================
# Agendador do reset diario dos dados de demonstracao.
#
# Roda no container pizzaria-seed: dorme ate a hora marcada, aplica o
# Scripts/ResetDiario.sql e volta a dormir. O horario alvo e recalculado a cada
# volta, entao nao acumula atraso.
#
# Variaveis (via .env / docker-compose):
#   MYSQL_HOST       host do banco            (default: pizzaria-mysql)
#   MYSQL_DATABASE   nome do banco            (default: pizzaria)
#   MYSQL_USER       usuario                  (default: pizzaria)
#   MYSQL_PWD        senha (lida pelo cliente mysql, nao aparece em ps)
#   RESET_HORA       HH:MM do reset           (default: 03:00)
#   RESET_AO_SUBIR   true = reseta ao iniciar (default: false)
#   TZ               fuso                     (default: UTC)
# =============================================================================
set -uo pipefail

HOST="${MYSQL_HOST:-pizzaria-mysql}"
BANCO="${MYSQL_DATABASE:-pizzaria}"
USUARIO="${MYSQL_USER:-pizzaria}"
HORA="${RESET_HORA:-03:00}"
SQL="/scripts/ResetDiario.sql"

log() { echo "[$(date '+%Y-%m-%d %H:%M:%S %Z')] $*"; }

if [ ! -f "$SQL" ]; then
    log "ERRO: $SQL nao encontrado. O volume ./Scripts foi montado?"
    exit 1
fi

if ! date -d "today ${HORA}" >/dev/null 2>&1; then
    log "ERRO: RESET_HORA invalida: '${HORA}'. Use o formato HH:MM."
    exit 1
fi

executar_reset() {
    if ! mysql --default-character-set=utf8mb4 -h "$HOST" -u "$USUARIO" "$BANCO" < "$SQL"; then
        log "ERRO: o reset falhou (o banco esta acessivel? a senha esta correta?)"
        return 1
    fi

    local totais
    totais="$(mysql -N -B --default-character-set=utf8mb4 -h "$HOST" -u "$USUARIO" "$BANCO" -e "
        SELECT CONCAT(
            (SELECT COUNT(*) FROM Usuarios), ' usuarios, ',
            (SELECT COUNT(*) FROM Pizzas),   ' pizzas, ',
            (SELECT COUNT(*) FROM Vendas),   ' vendas'
        );" 2>/dev/null)"

    log "reset concluido -- ${totais:-contagem indisponivel}"
}

log "agendador iniciado -- reset diario as ${HORA} (TZ=${TZ:-UTC}), banco ${USUARIO}@${HOST}/${BANCO}"

if [ "${RESET_AO_SUBIR:-false}" = "true" ]; then
    log "RESET_AO_SUBIR=true -- aplicando o reset agora"
    executar_reset || true
fi

while true; do
    agora="$(date +%s)"
    alvo="$(date -d "today ${HORA}" +%s)"
    if [ "$alvo" -le "$agora" ]; then
        alvo="$(date -d "tomorrow ${HORA}" +%s)"
    fi

    espera=$(( alvo - agora ))
    log "proximo reset em ${espera}s ($(date -d "@${alvo}" '+%Y-%m-%d %H:%M:%S %Z'))"
    sleep "$espera"

    executar_reset || true
done
