# Role/position names on the TR server: client terms vs. player usage, and a naming scheme for PreBot

Claim tested: "Role names are mixed: 'Top, Orman, Orta, ADC, Destek': half player slang, half Turkish client language. They match neither the client (Üst / Orman / Orta / Alt / Destek) nor players (top / jungle / mid / adc / sup). 'PREBOT ORMAN' in the lobby name also looks odd."

Method note: the client terms below come from Riot's own Turkish (tr_TR) localization files for the current League client, as mirrored by CommunityDragon. Build: `16.22.8348910+branch.releases-16-22` ([content-metadata](https://raw.communitydragon.org/latest/content-metadata.json)), which is the live patch as of Nov 2026. That makes them primary-source strings, not screenshots or guesses. Evidence on community usage is much thinner (see Gaps).

Current PreBot state, for reference ([TurkishNames.cs](file:///C:/Users/Alper/OneDrive/Masaüstü/LoL%20Pre%20Bot%20Projesi/src/PreBot.Core/Domain/TurkishNames.cs), [Lobby.cs](file:///C:/Users/Alper/OneDrive/Masaüstü/LoL%20Pre%20Bot%20Projesi/src/PreBot.Core/Domain/Lobby.cs), [PreModule.cs](file:///C:/Users/Alper/OneDrive/Masaüstü/LoL%20Pre%20Bot%20Projesi/src/PreBot.Discord/Bot/PreModule.cs)):
- UI and Discord choice labels: Top, Orman, Orta, ADC, Destek, Fark etmez.
- Lobby-name words: TOP, ORMAN, ORTA, ADC, DESTEK, and **FILL** for fill. So fill has two different names inside PreBot itself ("Fark etmez" in the UI, "FILL" in the lobby name).
- Rank labels: Demir, Bronz, Gümüş, Altın, Platin, Zümrüt, Elmas, **Master+**. "Master+" is English among Turkish tier names, the same kind of mix the claim points out for roles.
- Lobby format: `PREBOT {ROLE} {4-digit code}`, ASCII only, e.g. "PREBOT DESTEK 4821". The hint text is "Oyundaysan: Özel oyuna katıl → …".

## 1. Official Turkish client terms for positions, fill, queues and custom games

### Takeaway
The TR client's position picker uses exactly **Üst / Orman / Orta / Alt / Destek**, with **Doldur** for fill. Champ select and custom-game slots use the longer forms **Üst Koridor / Orman / Orta Koridor / Alt Koridor / Destek**. The client never calls the bot-lane position "ADC" or "Nişancı". "Nişancı" is the champion class (Marksman), not the position. Fill is "Doldur", not "Herhangi" or "Fark etmez". The queue is "Dereceli Esnek", and custom games are "Özel Oyun".

### Cited Findings
- Party/lobby position picker (the screen where you choose primary and secondary roles before queueing): `parties_position_top` "Üst", `parties_position_jungle` "Orman", `parties_position_middle` / `_mid` "Orta", `parties_position_bottom` / `_bot` "Alt", `parties_position_utility` / `_support` "Destek", `parties_position_fill` and `parties_primary_fill` "Doldur", `parties_position_unselected` "Seçili Değil". Other labels: `parties_primary_position` "Birincil", `parties_secondary_position` "İkincil", `parties_exclude_position` "Hariç tut". The client calls a role "konum" (e.g. "Bütün grup üyelerinin sıraya girmeden önce konumlarını seçmesi gerekiyor.") — [rcp-fe-lol-parties tr_TR trans.json](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-parties/global/tr_tr/trans.json)
- Custom-game team slots use the long forms: `custom_game_position_top` "Üst Koridor", `_jungle` "Orman", `_middle` "Orta Koridor", `_bottom` "Alt Koridor", `_utility` "Destek", `_unselected` "Seçilmemiş" — [rcp-fe-lol-parties tr_TR](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-parties/global/tr_tr/trans.json)
- Champ select: the assigned-position banner is uppercase short form (`summoner_assigned_position_top` "ÜST", `_jungle` "ORMAN", `_middle` "ORTA", `_bottom` "ALT", `_utility` "DESTEK"), with the title "Atandığın konum:". Favorites and filters use the long form ("Üst Koridor için Favori Seç", "Orta Koridor…", "Alt Koridor…", "Orman…", "Destek…"). Autofill is `assigned_position_title_autofill` "Otomatik Doldurma". The role-swap prompt is "Rol Değiştirilsin Mi?" — [rcp-fe-lol-champ-select tr_TR trans.json](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-champ-select/global/tr_tr/trans.json)
- The jungle *player* is "Ormancı" in client text ("Ormancı rolünde sihirdar büyülerinden birinin Çarp olması gerekir", "Ormancılara Çarp büyüsü otomatik olarak verilir") — [rcp-fe-lol-champ-select tr_TR](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-champ-select/global/tr_tr/trans.json); [rcp-fe-lol-parties tr_TR](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-parties/global/tr_tr/trans.json)
- "Nişancı" is the client's word for the Marksman champion class, e.g. `MATCH_HISTORY_SUMMARY_RECENT_ROLE_MARKSMAN` "Nişancı". It is not a position label — [rcp-fe-lol-match-history tr_TR](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-match-history/global/tr_tr/trans.json)
- Queue names:
  - Flex: `parties_game_type_ranked_flex` "Dereceli Esnek" ([parties](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-parties/global/tr_tr/trans.json)); `QUEUE_NAME_RANKED_FLEX_SR` "Dereceli Esnek" ([leagues](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-leagues/global/tr_tr/trans.json)).
  - Solo/duo has two spellings: "Dereceli Tek/Çift" in parties and "Dereceli Tekli/Çiftli sıra" in leagues (same sources).
- Custom games: `parties_game_type_practice` "Özel", buttons "ÖZEL OYUN KUR" / "ÖZEL OYUNA KATIL", list title "Özel Oyun Lobisi" — [rcp-fe-lol-parties tr_TR](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-parties/global/tr_tr/trans.json)
- Riot TR's own editorial text uses the same vocabulary in prose: "ormancı", "orman", "destek", "orta koridor", "otomatik doldurma", queues "Esnek" and "Tek/Çift" ("/geliştir: 2026'da Dereceli", 2025-12-01) — [leagueoflegends.com/tr-tr dev-ranked-2026](https://www.leagueoflegends.com/tr-tr/news/dev/dev-ranked-2026/)
- Riot TR support: the queues are "Tekli/Çiftli" and "Esnek". Flex allows parties of 1, 2, 3 or 5 ("4 kişilik gruplar derecesiz karşılaşmalar oynamak zorundadır") — [Riot Support TR: ranked tiers, divisions and queues](https://support.riotgames.com/tr-tr/league-of-legends/gameplay/ranked-tiers-divisions-and-queues)

### Inferences
- The claim's "client" column (Üst / Orman / Orta / Alt / Destek) is **accurate** for the main position picker. A stricter version would add that the client uses the "… Koridor" forms in champ select and custom-game slots, and "Doldur" for fill.
- PreBot's "Fark etmez" is neither the client word (Doldur) nor player slang (fill). It is natural Turkish, but no surface of the client or the community says it.
- PreBot's existing "Özel oyuna katıl" wording matches the client's "ÖZEL OYUNA KATIL" button exactly. That part is consistent and should be kept.

### Gaps
- I did not find TR client screenshots or video footage to confirm visually how these strings render (e.g. whether the position picker shows text labels or only icons on hover). The localization files are authoritative for the wording, not for layout.

## 2. What Turkish players actually say and write

### Takeaway
In the forum and LFG text I could reach, Turkish players write role names in English/slang (top, jungle/jg/jungler, mid, adc, sup/support). In casual posts I found no use of "üst", "alt" or "destek" as role names. "Orman/ormancı" is the one Turkish role word with real crossover: Riot TR, TR-localized stat sites and some long-form community writing use it. The evidence base is small and partly old (2013–2017 forum threads plus search snippets), so treat this as directional, not measured.

### Cited Findings
- DonanımHaber "lane önerisi" thread (May 2013), first page of about 20 posts. Role words were English: "support" (~12), "adc" (~5), "jungle/jungler" (~5), "mid/midlane", "top". Examples: "Support lazım oluyor birlikte oynayabiliriz.", "jungle da beceremiyorum sanirim", "Top git, turret altı farm yap sadece." No "üst", "orman", "orta", "alt", "destek", "ormancı" or "nişancı" appeared — [forum.donanimhaber.com/lane-onerisi--74700600](https://forum.donanimhaber.com/lane-onerisi--74700600)
- DonanımHaber LFG thread titled "TR Server Flex e pre lazım" (Oct 2017). The queue is called "flex" three times and "esnek" once ("şuan esnek ligin ne ?"). Rank is given as "Elmas 5" — [forum.donanimhaber.com/tr-server-flex-e-pre-lazim--129212783](https://forum.donanimhaber.com/tr-server-flex-e-pre-lazim--129212783)
- Techolay forum post (search snippet; the page itself returned 403): "Ben JG ve ADC giriyorum o ben JG olduğumda mid, ADC olduğumda SUP alıyor." This shows the abbreviations JG, ADC, SUP and mid in casual Turkish — [techolay.net: LoL için öğretici rehber önerisi](https://techolay.net/sosyal/konu/lol-icin-ogretici-rehber-onerisi.121254/?amp=1)
- An Ekşi Sözlük entry (search summary only; the page returned 403) discusses warding duties using "ormancı" and "yukarı koridor". So Turkish role words do appear in long-form community writing — [eksisozluk.com/entry/26202159](https://eksisozluk.com/entry/26202159)
- TR-localized third-party build sites follow Riot's term, e.g. "Trundle Orman Build LoL Rünler, Öğeler, Sayaçlar" — [metasrc.com/tr Trundle jungle](https://www.metasrc.com/tr/lol/build/trundle/jungle)
- PreBot's own design note (an internal developer observation, not external evidence) says groups already open public custom games named like "FLEX 5. LAZIM" — [Lobby.cs](file:///C:/Users/Alper/OneDrive/Masaüstü/LoL%20Pre%20Bot%20Projesi/src/PreBot.Core/Domain/Lobby.cs)

### Inferences
- Turkish LoL talk splits by register:
  - Client UI, Riot editorial and stat-site localization: Turkish (Üst/Orman/Orta/Alt/Destek; ormancı).
  - Casual chat and LFG: English loanwords and abbreviations (top, jg/jungle, mid, adc/bot, sup/support, fill), inside Turkish sentences ("sup lazım", "jg lazım", "flex 5. lazım").
- The claim's "player" column (top / jungle / mid / adc / sup) matches this evidence. Two forms should be added: "jg" (the very common short form for jungle) and "support" (written out about as often as "sup").
- The claim is somewhat **overstated** in implying that "Orman" is purely client language. Orman/ormancı is the one Turkish role word players do encounter and sometimes use. "Orta" and "Destek" crossover is less evidenced. "Üst" and "Alt" as spoken role names have no support in what I found.
- Where Turkish players need the bot-lane carry, they say "adc" (or "bot"). Using "Alt" as a role label would be clear to the client but unnatural in LFG copy.

### Gaps
- I found no quantitative frequency data (e.g. counts across Turkish Discord LFG channels, X/Twitter or Twitch chat) comparing "jungle/jg" vs "orman". The Discord LFG channels of Turkish servers (e.g. the "League of Legends TR" and "League of Legends Türkiye" servers listed on discord.com/servers) are not indexable, and I did not read them.
- Technopat and Ekşi Sözlük returned HTTP 403, so their usage is known only from search snippets.
- I did not check Turkish streamer/YouTube content (titles, speech) or TCL casting vocabulary in Turkish. The TCL coverage I found was English-language (Sheep Esports), so it says nothing about Turkish usage.

## 3. Rank (tier) names: client vs. community

### Takeaway
The current TR client and Riot TR support use Demir, Bronz, Gümüş, Altın, Platin, Zümrüt, Elmas, **Ustalık** (Master), **Üstatlık** (Grandmaster) and **Şampiyonluk** (Challenger), plus "Derecesiz" (Unranked). Third-party Turkish guides say "Büyük Ustalık" for Grandmaster, which is not what the client says. In the client, "Ustalık" also means champion mastery. That collision is a fair argument for PreBot to keep the English "Master+" label, but only if it is presented on purpose.

### Cited Findings
- Riot Support TR lists the ten tiers in order: "Demir, Bronz, Gümüş, Altın, Platin, Zümrüt, Elmas, Ustalık, Üstatlık, Şampiyonluk" — [Riot Support TR](https://support.riotgames.com/tr-tr/league-of-legends/gameplay/ranked-tiers-divisions-and-queues)
- Client strings: `challenge_level_iron` "Demir", `_bronze` "Bronz", `_silver` "Gümüş", `_gold` "Altın", `_platinum` "Platin", `_diamond` "Elmas", `_master` "Ustalık", `_grandmaster` "Üstatlık", `_challenger` "Şampiyonluk", `_none` "Derecesiz" — [rcp-fe-lol-postgame tr_TR](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-postgame/global/tr_tr/trans.json). Challenger is shown as "ŞAMPİYONLUK AŞAMASI", Unranked as "Derecesiz" — [rcp-fe-lol-leagues tr_TR](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-leagues/global/tr_tr/trans.json)
- "Zümrüt Aşaması" is described as sitting between Platin and Elmas (TFT string, same naming) — [rcp-fe-lol-postgame / leagues tr_TR](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-leagues/global/tr_tr/trans.json)
- The same client uses "Ustalık" for champion mastery: "{{level}}. Ustalık Seviyesi", "Ustalık Skoru" — [rcp-fe-lol-postgame tr_TR](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-postgame/global/tr_tr/trans.json)
- Riot TR editorial: "Ustalık ve üzeri aşamalarda", "Şampiyonluk", "Bronz/Gümüş/Altın/Platin Aşaması" — [leagueoflegends.com/tr-tr dev-ranked-2026](https://www.leagueoflegends.com/tr-tr/news/dev/dev-ranked-2026/)
- Conflict: Red Bull TR's guide (per search summary, not fetched) lists "…Ustalık, Büyük Ustalık ve Şampiyonluk". The search tool also noted that Riot's pages use "Üstatlık" instead — [redbull.com/tr-tr dereceli sistemi](https://www.redbull.com/tr-tr/league-of-legends-dereceli-sistemi); vs. [Riot Support TR](https://support.riotgames.com/tr-tr/league-of-legends/gameplay/ranked-tiers-divisions-and-queues)
- Community example: a player states his rank as "Elmas 5", using the Turkish tier name — [DonanımHaber 2017](https://forum.donanimhaber.com/tr-server-flex-e-pre-lazim--129212783)

### Inferences
- PreBot's Demir…Elmas labels match the client exactly.
- "Master+" is English among them. The client form would be "Ustalık+", but "Ustalık" collides with champion mastery, and "Master" is what players say. So "Master+" is defensible, provided the mix is a deliberate rule (see section 5).
- Tier names are proper nouns that every player sees on their own profile, so client Turkish names are well understood. Position names in the client ("Üst", "Alt") are labels players rarely say aloud. That asymmetry justifies treating ranks and roles differently.

### Gaps
- No frequency data on community use of English tier names ("plat", "gold", "dia", "emerald", "master") vs. Turkish ones ("platin", "altın", "elmas", "zümrüt"). I have only a single "Elmas 5" example.

## 4. Custom-game lobby name constraints, visibility and search

### Takeaway
In the current client (16.22) a custom-game name is limited to **30 characters** (input `maxlength="30"`). The trimmed name must be at least 3 characters, must pass a bad-word filter, and must not duplicate an existing name. Public and password-protected lobbies both appear in the "Özel Oyun Lobisi" list, which has a **fuzzy, ranked search** over lobby name and owner name. "PREBOT DESTEK 4821" (18 chars) fits comfortably. I found no ASCII-only rule, but ASCII remains the safer choice for typing and search.

### Cited Findings
- The custom-game name input in the parties plugin has `id="custom-game-name"`, default value `defaultGameName` and `maxlength="30"`. The client rejects names whose trimmed length is under 3 (`e.value.trim().length<3` → `nameError "length"`) — [rcp-fe-lol-parties.js (16.22)](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-parties/global/default/rcp-fe-lol-parties.js)
- Related TR strings:
  - name label "Ad"
  - default name "{{summonerName}} oyunu"
  - length error "…en az 3 karakter uzunluğunda olmalı."
  - bad-word error "…bir veya daha fazla geçersiz kelime mevcut…"
  - duplicate error "''{{name}}'' eşsiz değil!"
  - password field "Şifre (Opsiyonel)"

  Oddly, the TR errors call the lobby name "Oyuncu adı" (player name), which is a translation quirk — [rcp-fe-lol-parties tr_TR](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-parties/global/tr_tr/trans.json)
- The lobby list still exists in the 16.22 client:
  - title "Özel Oyun Lobisi"
  - columns "Lobi Adı" / "Oyun Tipi" / "Oyun Sahibi"
  - search box "Oyun bul"
  - filter "Sadece özel lobileri göster", which in code keeps only `hasPassword` lobbies, so locked lobbies are listed too
  - join prompt "Katılmak için şifre gerekli"

  Sources: [tr_TR strings](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-parties/global/tr_tr/trans.json); [rcp-fe-lol-parties.js](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-parties/global/default/rcp-fe-lol-parties.js)
- How list search works: the list builds a fuzzy-search index with keys `lobbyName` (weight 1) and `ownerDisplayName` (weight 0.5). The search library defaults are `location:0, distance:100, threshold:.6`, i.e. Fuse.js/Bitap-style matching. While a search string is present, results come back in relevance order instead of sorted by name — [rcp-fe-lol-parties.js](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-parties/global/default/rcp-fe-lol-parties.js)
- A third-party tournament-code tool also notes that the lobby name is what shows in the custom games list — [npm lol-tournament-code](https://www.npmjs.com/package/lol-tournament-code)

### Inferences
- **Length.** Every candidate fits easily. "PREBOT SUPPORT 4821" is 19 characters and "PREBOT JUNGLE 4821" is 18, well under 30. Length is not a reason to keep Turkish words.
- **Search is loose.** Threshold 0.6 is permissive. Searching "PREBOT" or the 4-digit code surfaces the right lobby near the top, but near-misses (other PREBOT lobbies, similar numbers) will also be listed. The unique code does the identifying; the role word is mainly for humans browsing the list.
- **The Turkish dotted/dotless I problem.** On a Turkish Q keyboard, typing an uppercase I with Caps Lock or Shift+i produces "İ", so a creator typing "MID" or "FILL" may produce "MİD" or "FİLL". JavaScript's non-locale `toLowerCase()` turns "İ" into "i̇" (i plus a combining dot), not "i". Fuzzy matching will probably still find the lobby, but exact-looking names can drift. This applies to the current fallback word "FILL" and to any "MID" or "JUNGLE"-style word containing I. Words without I (TOP, JG, ADC, SUP, DESTEK, ORMAN, ORTA) avoid it. This is a reasoned risk; I did not test it in a live client.
- **Turkish characters.** I found no ASCII whitelist in the UI code. The default name embeds the player's name, and TR Riot IDs may contain Turkish letters, so non-ASCII is probably accepted. ASCII-only is still the right call: it avoids the İ/ı and Ü/U typing variance and keeps search predictable. Its cost is that client Turkish words degrade ("Üst" becomes "UST"), which is another argument against using client terms in lobby names.
- **Visibility.** Because locked lobbies are listed and searchable by anyone on the TR server, the lobby name is effectively public copy. Whatever role word goes there should read naturally to a random TR player browsing the list. In LFG-style names that is slang ("JG", "SUP", "5. LAZIM"), not "ORMAN".

### Gaps
- Server-side validation (allowed characters, the duplicate-name scope, the exact bad-word list) is not visible in client code and was not tested.
- I could not confirm whether the custom-game list is region-wide or limited somehow, or how many lobbies it shows.
- No live-client test of how "MİD" vs "MID" searches actually rank.

## 5. Verdict on the claim and recommended naming scheme

### Takeaway
The claim is **largely correct**. PreBot's set takes Top and ADC from players and Orman, Orta and Destek from the client. The client column (Üst/Orman/Orta/Alt/Destek) is exact, and the player column (top/jungle/mid/adc/sup) is supported. The claim also misses three bigger inconsistencies:
- Fill is "Fark etmez" in the UI but "FILL" in the lobby name, and neither equals the client's "Doldur".
- Ranks mix Turkish tier names with "Master+".
- The queue is "Flex" in player speech but "Dereceli Esnek" in the client.

Its one overstatement is that "Orman" is not purely client language. "PREBOT ORMAN looks odd" is a reasonable judgment, since LFG-style lobby names use slang, but it is subjective and unmeasured. **Recommendation:** use one rule, "player words as the label, client word as the helper", applied the same way on every surface.

### Cited Findings
- Client uses Üst/Orman/Orta/Alt/Destek/Doldur (party position picker) and Üst Koridor/Orta Koridor/Alt Koridor in champ select and custom slots — [parties tr_TR](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-parties/global/tr_tr/trans.json); [champ-select tr_TR](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-champ-select/global/tr_tr/trans.json)
- Casual Turkish posts use support/sup, adc, jungle/JG, mid, top and "flex" — [DonanımHaber 2013](https://forum.donanimhaber.com/lane-onerisi--74700600); [DonanımHaber 2017](https://forum.donanimhaber.com/tr-server-flex-e-pre-lazim--129212783); [techolay snippet](https://techolay.net/sosyal/konu/lol-icin-ogretici-rehber-onerisi.121254/?amp=1)
- PreBot currently uses "Fark etmez" in UI and Discord choices but "FILL" in lobby names, and "Master+" among Turkish tiers — [TurkishNames.cs](file:///C:/Users/Alper/OneDrive/Masaüstü/LoL%20Pre%20Bot%20Projesi/src/PreBot.Core/Domain/TurkishNames.cs); [Lobby.cs](file:///C:/Users/Alper/OneDrive/Masaüstü/LoL%20Pre%20Bot%20Projesi/src/PreBot.Core/Domain/Lobby.cs)
- Lobby names are capped at 30 characters and searchable by fuzzy match in a public list — [rcp-fe-lol-parties.js](https://raw.communitydragon.org/latest/plugins/rcp-fe-lol-parties/global/default/rcp-fe-lol-parties.js)

### Inferences
Recommended scheme (one rule everywhere: **the primary label is what players type in LFG; the client term is a secondary hint wherever there is room**):

| Role (enum) | Web UI label | Discord choice name (value unchanged) | Lobby word (ASCII, no "I") | Client term, as a hint/tooltip |
|---|---|---|---|---|
| Top | Top | `Top (Üst)` | TOP | Üst Koridor |
| Jungle | Jungle | `Jungle (Orman)` | JG | Orman |
| Mid | Mid | `Mid (Orta)` | MID* | Orta Koridor |
| Adc | ADC | `ADC (Alt)` | ADC | Alt Koridor |
| Support | Support (or "Sup" in tight spots) | `Support (Destek)` | SUP | Destek |
| Fill | Fill | `Fill (Doldur)` | FILL* | Doldur |

\* MID and FILL contain "I", which carries the Turkish İ typing risk from section 4. That risk is acceptable because the 4-digit code is the real identifier. A more cautious lobby format would drop the role word entirely ("PREBOT 4821") and keep the role in the notification text.

Reasons for this scheme:
- **Lobby names are public copy read by random TR players browsing "Özel Oyun Lobisi"**, so they should look like the player-made LFG lobbies already there ("FLEX 5. LAZIM", "JG LAZIM"). "PREBOT JG 4821" or "PREBOT SUP 4821" reads like LFG. "PREBOT ORMAN 4821" reads like a label.
- **Client terms survive ASCII badly.** "Üst" becomes "UST", and "ALT" also means "sub/lower" in everyday Turkish. Slang terms are already ASCII.
- **Bilingual Discord choice names** ("Jungle (Orman)") let both groups recognize the option. They also probably let users find it by typing either word. This is an inference about Discord's choice filtering, which I did not verify here. The choice *values* ("Jungle", "Fill") stay unchanged, so no data migration is needed.
- **Fill:** pick one word. "Fill" is the player term. "Doldur" is the client term, and the client's own autofill wording is "Otomatik Doldurma". "Fark etmez" is friendly Turkish, but neither side uses it. If it is kept, it should only be a subtitle ("Fill — fark etmez"), never the only label. In all cases, the UI and lobby name must use the same word.
- **Ranks:** keep the client Turkish tier names (Demir…Elmas), which match what players see on their profiles. Keep "Master+" deliberately rather than "Ustalık+", because "Ustalık" also means champion mastery in the TR client. Optionally show "Master+ (Ustalık ve üzeri)" once in a tooltip. This intentionally treats ranks and roles differently, and that is consistent with the asymmetry in section 3.
- **Queue and button wording:**
  - Casual copy: "Flex".
  - When telling users what to click in the client, use the client strings exactly: "Dereceli Esnek", "ÖZEL OYUNA KATIL" (PreBot already says "Özel oyuna katıl").
  - If the client is ever referenced in instructions, match its labels: "Oyun bul" (the custom-list search box) and "Şifre".
- **Alternative**, if the product prefers to mirror the client fully: Üst / Orman / Orta / Alt / Destek / Doldur in the UI. In that case the lobby names should still use slang or no role word, because of ASCII degradation and public-list readability. Do not leave the current mix, where Top and ADC are slang, Orman, Orta and Destek are client words, and fill is a third invented word.

### Gaps
- No user testing or survey data on which labels TR Flex players find clearest. The recommendation rests on register evidence (client vs. casual posts) plus reasoning, not measured preference.
- Discord's slash-command choice filtering (whether typing "orman" matches "Jungle (Orman)") and `name_localizations` behavior were not verified in this research.
