#!/bin/sh
set -eu

api_base_url="${API_BASE_URL:-}"
google_client_id="${GOOGLE_CLIENT_ID:-}"

escape_js() {
    printf '%s' "$1" | sed "s/[\\']/\\\\&/g"
}

cat > /usr/share/nginx/html/env.js <<EOF
window.planejadorConfig = {
  apiBaseUrl: '$(escape_js "$api_base_url")',
  googleClientId: '$(escape_js "$google_client_id")',
};
EOF
