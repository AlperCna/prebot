#!/bin/sh
# Installs PreBot on a fresh Ubuntu server (Azure for Students, Oracle, any small VPS; x64).
# Run on the server as the default user:
#   curl -fsSL https://raw.githubusercontent.com/AlperCna/prebot/master/deploy/sunucu-kurulum.sh -o kurulum.sh && sh kurulum.sh
# It asks for the address (e.g. prebot-alper.westeurope.cloudapp.azure.com), a contact e-mail, a name and an
# invite code, then downloads the ready-made image from GitHub and starts the site with automatic HTTPS.
# Running it again later updates the site to the newest version (your settings and data are kept).
set -eu

echo "== Docker =="
if ! command -v docker >/dev/null 2>&1; then
  curl -fsSL https://get.docker.com | sudo sh
  sudo usermod -aG docker "${USER:-$(id -un)}"
fi

echo "== Takas alani (1 GB RAM'li sunucularda bellek tasmasin diye) =="
MEM_MB=$(awk '/MemTotal/ { print int($2 / 1024) }' /proc/meminfo)
if [ "$MEM_MB" -lt 2048 ] && ! swapon --show | grep -q '/swapfile'; then
  sudo fallocate -l 2G /swapfile
  sudo chmod 600 /swapfile
  sudo mkswap /swapfile
  sudo swapon /swapfile
  grep -q '/swapfile' /etc/fstab || echo '/swapfile none swap sw 0 0' | sudo tee -a /etc/fstab >/dev/null
fi

echo "== Guvenlik duvari: 80 ve 443 (bazi imajlar varsayilan olarak kapatir) =="
if sudo iptables -L INPUT -n 2>/dev/null | grep -q "REJECT"; then
  # -C checks first, so running the script again does not add the same rule twice.
  for rule in "-p tcp --dport 80" "-p tcp --dport 443" "-p udp --dport 443"; do
    # shellcheck disable=SC2086
    sudo iptables -C INPUT $rule -j ACCEPT 2>/dev/null || sudo iptables -I INPUT 1 $rule -j ACCEPT
  done
  if command -v netfilter-persistent >/dev/null 2>&1; then sudo netfilter-persistent save; fi
fi

echo "== Proje =="
if [ ! -d prebot ]; then
  git clone https://github.com/AlperCna/prebot.git prebot
fi
cd prebot
git pull --ff-only

# Writes KEY=value into .env. The value is escaped for sed, so names like "Alper & Co" stay as typed.
set_env() {
  value=$(printf '%s' "$2" | sed 's/[\\&|]/\\&/g')
  sed -i "s|^$1=.*|$1=$value|" .env
}

if [ ! -f .env ]; then
  printf "Sitenin adresi (ornek: prebot-alper.westeurope.cloudapp.azure.com): "; read -r DOMAIN
  printf "Iletisim e-postasi (gizlilik metninde gorunur): "; read -r EMAIL
  printf "Isletmeci adi (gizlilik metninde 'veri sorumlusu' olarak gorunur): "; read -r OPERATOR
  printf "Sunucunun yeri (ornek: Avrupa (Azure, Hollanda)): "; read -r LOCATION
  printf "Davet kodu (bos birakirsan tahmin edilemez bir kod uretilir): "; read -r CODE
  # 12 random letters/digits: the invite code must not be guessable (the login also counts failed tries).
  [ -n "$CODE" ] || CODE="flex-$(tr -dc 'abcdefghjkmnpqrstuvwxyz23456789' </dev/urandom | head -c 12)"
  cp deploy/.env.example .env
  set_env DOMAIN "$DOMAIN"
  set_env PUSH_SUBJECT "mailto:$EMAIL"
  set_env SITE_CONTACT_EMAIL "$EMAIL"
  set_env SITE_OPERATOR_NAME "$OPERATOR"
  set_env SITE_HOSTING_LOCATION "$LOCATION"
  set_env SITE_INVITE_CODE "$CODE"
  echo "Davet kodu: $CODE  (.env dosyasinda saklandi)"
fi

echo "== Hazir imaj indiriliyor ve site baslatiliyor =="
if sudo docker compose pull web; then
  sudo docker compose up -d --no-build
else
  echo "Hazir imaj indirilemedi, sunucuda derleniyor (birkac dakika surer)..."
  sudo docker compose up -d --build
fi

DOMAIN_NOW=$(grep '^DOMAIN=' .env | cut -d= -f2)
echo ""
echo "Bitti. 1-2 dakika icinde https://$DOMAIN_NOW adresinde acilir (HTTPS sertifikasi otomatik alinir)."
echo "Davet kodu: $(grep '^SITE_INVITE_CODE=' .env | cut -d= -f2)"
echo "Guncellemek icin bu komutu tekrar calistirman yeterli."
