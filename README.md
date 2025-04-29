# VotingSystem 🗳️

[![Build](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com/krisz09/VotingSystem)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

> Egy modern szavazó rendszer ASP.NET Core Web API és React frontend technológiákkal.

---

## Tartalomjegyzék

- [Projekt Áttekintés](#projekt-áttekintés)
- [Technológiák](#technológiák)
- [Funkciók](#funkciók)
- [Telepítés](#telepítés)
- [Használat](#használat)
- [Tervek a jövőre](#tervek-a-jövőre)
- [Licensz](#licensz)

---

## Projekt Áttekintés

A VotingSystem célja egy biztonságos, könnyen használható, web-alapú szavazási platform megvalósítása. A rendszer képes felhasználókezelésre, aktív szavazások listázására, szavazatok leadására és lezárult szavazások eredményeinek megtekintésére.

### Projektstruktúra

| Projekt                  | Leírás                                          |
|---------------------------|-------------------------------------------------|
| `VotingSystem.WebApi`     | ASP.NET Core Web API backend                    |
| `VotingSystem.DataAccess` | EF Core adatbázis kezelő réteg                  |
| `VotingSystem.Shared`     | Közös DTO-k, modellek, validációk               |
| `votingsystemreact`       | React + TypeScript frontend alkalmazás         |

---

## Technológiák

### Backend
- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core
- Identity + JWT Token alapú hitelesítés
- SQL Server / SQLite

### Frontend
- React (TypeScript)
- Axios az API kommunikációhoz
- JWT alapú hitelesítés kezelése (localStorage)
- React Router DOM

---

## Funkciók

✔️ Felhasználói regisztráció és bejelentkezés  
✔️ Aktív szavazások megjelenítése  
✔️ Szavazatok leadása  
✔️ Lezárult szavazások eredményeinek böngészése  
✔️ JWT hitelesítés + token frissítés kezelése  

---

## Telepítés

### 1. Backend (ASP.NET Core Web API)

```bash
git clone https://github.com/krisz09/VotingSystem.git
cd VotingSystem/VotingSystem.WebApi
dotnet ef database update
dotnet run
```

## Használat

### Legfontosabb API végpontok

| HTTP Módszer | Végpont                     | Leírás                       |
|--------------|------------------------------|-------------------------------|
| `POST`       | `/api/auth/register`         | Új felhasználó regisztrációja |
| `POST`       | `/api/auth/login`            | Bejelentkezés és token szerzés|
| `GET`        | `/api/votes/active`          | Aktív szavazások lekérdezése  |
| `POST`       | `/api/votes/{id}/vote`       | Szavazat leadása              |
| `GET`        | `/api/votes/closed`          | Lezárt szavazások megtekintése|


## Fő Funkciók

✔️ Felhasználói regisztráció és bejelentkezés JWT hitelesítéssel  
✔️ Aktív szavazások listázása  
✔️ Szavazatok leadása  
✔️ Lezárult szavazások megtekintése  
✔️ JWT hitelesítés + token frissítés kezelése  
✔️ API hívások hibakezelése

## Tervek a jövőre

- [ ] Adminisztrátor felület szavazások kezelésére
- [ ] Szavazások időzítése és automatikus lezárása
- [ ] E-mail értesítések szavazásokkal kapcsolatban
- [ ] További autentikációs opciók (pl. Google OAuth)
- [ ] PWA támogatás (Progressive Web App)
- [ ] Mobil alkalmazás támogatás (React Native vagy MAUI)
- [ ] Reszponzív dizájn mobil eszközökre
- [ ] Teljes unit és integrációs tesztlefedettség
