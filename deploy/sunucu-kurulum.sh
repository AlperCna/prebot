#!/bin/sh
# Installs PreBot on a fresh Ubuntu server (e.g. Oracle Cloud Always Free, ARM or x86).
# Run on the server as the default user (ubuntu):
#   curl -fsSL https://raw.githubusercontent.com/AlperCna/prebot/master/deploy/sunucu-kurulum.sh -o kurulum.sh && sh kurulum.sh
# It asks for the domain (e.g. prebot.duckdns.org), a contact e-mail and an invite code, then starts the site.
set -eu

echo "== Docker kuruluyor =="
if ! command -v docker >/dev/null 2>&1; then
  curl -fsSL https://get.docker.com | sudo sh
  sudo usermod -aG docker "$USER"
fi

echo "== 80 ve 443 portlari aciliyor (Oracle'in Ubuntu imaji varsayilan olarak kapatir) =="
if sudo iptables -L INPUT -n | grep -q "REJECT"; then
  sudo iptables -I INPUT 1 -p tcp --dport 80 -j ACCEPT
  sudo iptables -I INPUT 1 -p tcp --dport 443 -j ACCEPT
  sudo iptables -I INPUT 1 -p udp --dport 443 -j ACCEPT
  if command -v netfilter-persistent >/dev/null 2>&1; then sudo netfilter-persistent save; fi
fi

echo "== Proje indiriliyor =="
if [ ! -d prebot ]; then
  git clone https://github.com/AlperCna/prebot.git prebot
fi
cd prebot
git pull --ff-only

if [ ! -f .env ]; then
  printf "Alan adi (ornek: prebot.duckdns.org): "; read -r DOMAIN
  printf "Iletisim e-postasi (gizlilik metninde gorunur): "; read -r EMAIL
  printf "Isletmeci adi (gizlilik metninde 'veri sorumlusu' olarak gorunur): "; read -r OPERATOR
  printf "Davet kodu (bos birakirsan uretilir): "; read -r CODE
  [ -n "$CODE" ] || CODE="flex-$(shuf -i 1000-9999 -n 1)"
  cp deploy/.env.example .env
  sed -i "s|^DOMAIN=.*|DOMAIN=$DOMAIN|; s|^PUSH_SUBJECT=.*|PUSH_SUBJECT=mailto:$EMAIL|; s|^SITE_CONTACT_EMAIL=.*|SITE_CONTACT_EMAIL=$EMAIL|; s|^SITE_OPERATOR_NAME=.*|SITE_OPERATOR_NAME=$OPERATOR|; s|^SITE_INVITE_CODE=.*|SITE_INVITE_CODE=$CODE|" .env
  echo "Davet kodu: $CODE  (.env dosyasinda saklandi)"
fi

echo "== Site baslatiliyor (ilk derleme birkac dakika surer) =="
sudo docker compose up -d --build
echo "Bitti. Birkac dakika icinde https://$(grep '^DOMAIN=' .env | cut -d= -f2) adresinde acilir."
