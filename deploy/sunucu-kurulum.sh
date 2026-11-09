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
  sudo usermod -aG docker "$USER"
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
  sudo iptables -I INPUT 1 -p tcp --dport 80 -j ACCEPT
  sudo iptables -I INPUT 1 -p tcp --dport 443 -j ACCEPT
  sudo iptables -I INPUT 1 -p udp --dport 443 -j ACCEPT
  if command -v netfilter-persistent >/dev/null 2>&1; then sudo netfilter-persistent save; fi
fi

echo "== Proje =="
if [ ! -d prebot ]; then
  git clone https://github.com/AlperCna/prebot.git prebot
fi
cd prebot
git pull --ff-only

if [ ! -f .env ]; then
  printf "Sitenin adresi (ornek: prebot-alper.westeurope.cloudapp.azure.com): "; read -r DOMAIN
  printf "Iletisim e-postasi (gizlilik metninde gorunur): "; read -r EMAIL
  printf "Isletmeci adi (gizlilik metninde 'veri sorumlusu' olarak gorunur): "; read -r OPERATOR
  printf "Sunucunun yeri (ornek: Avrupa (Azure, Hollanda)): "; read -r LOCATION
  printf "Davet kodu (bos birakirsan uretilir): "; read -r CODE
  [ -n "$CODE" ] || CODE="flex-$(shuf -i 1000-9999 -n 1)"
  cp deploy/.env.example .env
  sed -i "s|^DOMAIN=.*|DOMAIN=$DOMAIN|; s|^PUSH_SUBJECT=.*|PUSH_SUBJECT=mailto:$EMAIL|; s|^SITE_CONTACT_EMAIL=.*|SITE_CONTACT_EMAIL=$EMAIL|; s|^SITE_OPERATOR_NAME=.*|SITE_OPERATOR_NAME=$OPERATOR|; s|^SITE_HOSTING_LOCATION=.*|SITE_HOSTING_LOCATION=$LOCATION|; s|^SITE_INVITE_CODE=.*|SITE_INVITE_CODE=$CODE|" .env
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
