# Voting System Project

## 1. Backend (ASP.NET Web API)

### Fő felelősség
- Az adatok kezelése, API végpontok biztosítása.

### Ajánlott technológiák
- **ASP.NET Core Web API**
- **Entity Framework Core** (adatbáziskezelés)
- **Identity** vagy **JWT** az autentikációhoz

## 2. Adatbázis

### Feladatok
- Adatok tárolása, például felhasználók, szavazások, szavazatok.

### Ajánlott adatbázis lehetőségek
- **SQL Server** vagy **PostgreSQL** (strukturált adatokhoz)
- **SQLite** (ha egyszerűbb, fájlalapú megoldás kell)

## 3. Frontend

### Webes alkalmazás

#### Ajánlott technológiák
- **React (TypeScript)** vagy **Angular**
- **Axios** vagy **Fetch API** a REST kérésekhez
- **JWT token** kezelés a hitelesítéshez

### Asztali alkalmazás

#### Ajánlott technológia
- **WPF** + **.NET MAUI**
- **HttpClient** a backend API hívásokhoz

### Mobil alkalmazás

#### Ajánlott technológia
- **Flutter** vagy **.NET MAUI**
- HTTP kérések és lokális token tárolás

---

## 2️⃣ Első lépések

### 1. Backend elindítása (ASP.NET Core Web API)

1. Hozz létre egy **ASP.NET Core Web API** projektet.
2. Állítsd be az **Entity Framework Core**-t és az adatbázist.
3. Készítsd el az **Identity** alapú hitelesítést (JWT tokenekkel).

#### 📌 Elsőként implementálandó API végpontok:
- ✔ **Regisztráció** (POST `/api/auth/register`)
- ✔ **Bejelentkezés** (POST `/api/auth/login`)
- ✔ **Aktív szavazások listázása** (GET `/api/votes/active`)
- ✔ **Szavazás leadása** (POST `/api/votes/{id}/vote`)
- ✔ **Lezárult szavazások listázása és szűrése** (GET `/api/votes/closed`)

### 2. Frontend elindítása (React/Angular)

1. Hozz létre egy **React** vagy **Angular** projektet.
2. Konfiguráld az **Axios** (React) vagy **HttpClient** (Angular) segítségével az API hívásokat.
3. Implementáld a bejelentkezési oldalt és a szavazások listázását.

#### 📌 Elsőként implementálandó oldalak:
- ✔ **Bejelentkezés** és **regisztráció**
- ✔ **Aktív szavazások listája**
- ✔ **Egy szavazás részletei** + **szavazás leadása**

---

## 3️⃣ Következő lépések

1. **Backend alapok elkészítése**
   - API végpontok, adatbázis
2. **Frontend első oldalak fejlesztése**
   - Belépés, szavazások megjelenítése
3. **Hitelesítés és jogosultságkezelés**
   - JWT tokenek kezelése
4. **Szavazás leadás és lezárt szavazások eredményeinek megjelenítése**
5. **Extra funkciók**
   - Szűrés, UI fejlesztés, optimalizáció

---

Ha bármelyik részben elakadnál, szólj, és segítek! 🚀😊
