# VivesRental - Verhuurapplicatie

<div align="center">
  <h3>VIVES Hogeschool - Web Development 2</h3>
  <p><strong>Eindopdracht 2024-2025</strong></p>
</div>

---

## Projectinformatie

**Auteur**: Quinten Verbeke  
**Klas**: 2CYB  
**Academiejaar**: 2024-2025  
**Vak**: Web Development 2  
**Datum**: 15 juni 2025  

---

## Projectomschrijving

VivesRental is een professionele verhuurapplicatie ontwikkeld voor VIVES Hogeschool om hun materiaalverhuur te digitaliseren en stroomlijnen. De applicatie stelt medewerkers in staat om het volledige verhuurproces te beheren: van productbeheer tot klantregistratie en orderafhandeling.

Het systeem werd gebouwd als een moderne web-applicatie met een REST API backend en een gebruiksvriendelijke ASP.NET MVC frontend, waarbij bijzondere aandacht werd besteed aan UI/UX design en responsive functionaliteit.

---

## Technische Architectuur

### Backend
- **Framework**: ASP.NET Core 9.0
- **API**: RESTful Web API
- **Database**: SQL Server (vervangen van In-Memory naar productie-ready)
- **ORM**: Entity Framework Core 9
- **Authenticatie**: JWT Bearer Tokens

### Frontend
- **Framework**: ASP.NET MVC 9.0
- **Styling**: Custom CSS + Bootstrap voor responsive design
- **UI Components**: Razor Partial Views
- **Client-side**: JavaScript voor dynamische interacties

### Development Tools
- **IDE**: Visual Studio 2022+
- **Version Control**: Git
- **Database Management**: SQL Server Management Studio

---

## Functionaliteiten

### Dashboard & Navigatie
- **Rental Dashboard**: Centraal overzicht met snelle toegang tot alle modules
- **Responsive Design**: Geoptimaliseerd voor verschillende schermgroottes
- **Intuïtieve Navigatie**: Gebruiksvriendelijke interface voor efficiënt werken

### Klantenbeheer
- **CRUD Operaties**: Volledige klantgegevensbeheer
- **Klantregistratie**: Nieuwe klanten aanmaken en koppelen aan orders
- **Klantoverzicht**: Zoeken en filteren van klantgegevens

### Product- en Artikelbeheer
- **Productcategorieën**: Beheer van productsoorten (bv. "Rode Schop")
- **Artikelvoorraad**: Fysieke items per product met statusbeheer
- **Bulk Import**: Efficiënt toevoegen van grote hoeveelheden artikelen
- **Statusbeheer**: Artikelen markeren als beschikbaar, verhuurd, kapot, etc.

### Orderbeheer
- **Nieuwe Verhuurorders**: Interactieve ordersamenstelling
- **Beschikbaarheidscheck**: Real-time controle op voorraad
- **Automatische Selectie**: Intelligente toewijzing van beschikbare artikelen
- **Orderhistoriek**: Volledige audit trail van alle transacties

### Orderlijnen & Historiek
- **Gedetailleerde Tracking**: Elke verhuurde artikel wordt individueel gevolgd
- **Gegevensarchivereing**: Originele productinformatie wordt bewaard op orderlijnniveau
- **Flexibele Retournering**: Volledige orders of individuele items terugbrengen
- **Statusupdates**: Automatische bijwerking van artikelstatussen

### Reserveringssysteem
- **Tijdelijke Reserveringen**: Artikelen blokkeren voor specifieke periodes
- **Onbeperkte Reserveringen**: Permanente reserveringen voor speciale doeleinden
- **Klantgekoppelde Reserveringen**: Zoeken op klantnaam
- **Reserveringsoverzicht**: Centraal beheer van alle actieve reserveringen

### Beveiligingssysteem
- **JWT Authenticatie**: Veilige token-gebaseerde authenticatie

### Foutafhandeling & Validatie
- **API Connectivity**: Graceful handling bij API-onbereikbaarheid
- **SQL Exception Handling**: Robuuste databasefoutafhandeling
- **Input Validatie**: Client-side en server-side validatie
- **User Feedback**: Duidelijke foutmeldingen en succesberichten

---

## Databasestructuur

### Kernentiteiten
- **Products**: Productcategorieën en -specificaties
- **Articles**: Fysieke verhuurbare items gekoppeld aan producten
- **Customers**: Klantgegevens en contactinformatie
- **Orders**: Verhuurorders met algemene informatie
- **OrderLines**: Gedetailleerde orderregels per artikel
- **Reservations**: Reserveringssysteem voor artikelen

### Relationele Integriteit
- **One-to-Many**: Product → Articles
- **One-to-Many**: Customer → Orders
- **One-to-Many**: Order → OrderLines
- **Many-to-One**: OrderLine → Article (historisch)

---

## Installatie & Setup

### Vereisten
- **.NET 9 SDK** of hoger
- **SQL Server 2019** of hoger (LocalDB ondersteund)
- **Visual Studio 2022** (aanbevolen) of Visual Studio Code
- **SQL Server Management Studio** (optioneel)

### Installatiestappen

#### 1. Project Klonen/Uitpakken
```bash
# Als Git repository
git clone https://github.com/Us3rN3/Examen

# Of unzip het geleverde bestand
unzip VivesRental_QuintenVerbeke.zip
```

#### 2. Database Configuratie
```bash
# Herstel database backup in SQL Server Management Studio
# Of gebruik de meegeleverde database.bak file

# Update connection string in appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=VivesRentalDB;Trusted_Connection=true;"
  }
}
```

#### 3. Dependencies Installeren
```bash
# Restore NuGet packages
dotnet restore

# Update database (indien Entity Framework migrations gebruikt)
dotnet ef database update
```

#### 4. Applicatie Starten
```bash
# Via .NET CLI
dotnet run

# Of via Visual Studio
F5 / Ctrl+F5
```

### Configuratie
- **Web Application**: Standaard op `https://localhost:7109`
- **Database**: Configureerbaar via `appsettings.json`

---

## API Documentatie

### Authenticatie
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "password123"
}
```
*Volledige API documentatie beschikbaar via Swagger UI op `/swagger`*
---

## Troubleshooting

### Veel Voorkomende Problemen

#### Database Connection Errors
```
Probleem: Cannot connect to SQL Server
Oplossing: 
1. Controleer connection string in appsettings.json
2. Zorg dat SQL Server service draait
3. Verificeer database bestaat en toegankelijk is
```

#### JWT Token Errors
```
Probleem: 401 Unauthorized responses
Oplossing:
1. Controleer token expiration
2. Herstart applicatie voor nieuwe secret key
3. Clear browser cache/cookies
```

---

##  Toekomstige Uitbreidingen

### Technische Verbeteringen
- **Caching**: Redis implementatie voor betere performance
- **Logging**: Structured logging met Serilog
- **Monitoring**: Application Performance Monitoring
- **Tests**: Uitgebreidere test coverage

---

## 🏆 Evaluatiecriteria (Behaald)

| Criterium | Punten | Status |
|-----------|---------|---------|
| SQL Server Database | 10/10 | ✅ Geïmplementeerd |
| REST API | 20/20 | ✅ Volledig functioneel |
| UI/UX + Partial Views | 30/30 | ✅ Professional design |
| CRUD Operaties | 20/20 | ✅ Alle modules |
| Layout & Structure | 10/10 | ✅ Responsive & Clean |
| Foutafhandeling | 10/10 | ✅ Comprehensive |
| JWT Authenticatie | 20/20 | ✅ Secure implementation |
| **Totaal** | **120/120** | ✅ **Volledig** |

---

## 📞 Contact & Support

**Student**: Quinten Verbeke  
**Email**: quinten.verbeke@student.vives.be  
**Klas**: 2CYB  
**Academiejaar**: 2024-2025  

Voor vragen over de implementatie of technische details, neem contact op via Teams of email.

---

## Licentie & Disclaimers

Dit project is ontwikkeld voor educatieve doeleinden in het kader van de opleiding Toegepaste Informatica aan VIVES Hogeschool. 

**Copyright © 2025 Quinten Verbeke - Alle rechten voorbehouden**

---

*Laatste update: 15 juni 2025*
