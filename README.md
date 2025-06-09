# Team Ranking System API

A .NET 5 Web API for managing teams, matches, and calculating team rankings based on match results.

## Features

- Create, read, update, and delete teams
- Create, read, update, and delete matches between teams
- Automatically calculates team rankings based on match results
- Prevents teams from playing against themselves
- Prevents deletion of teams that are part of existing matches

- ## Technologies Used

- .NET 5
- Entity Framework Core
- AutoMapper
- SQL Server
- RESTful API principles

## Prerequisites

- [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
- SQL Server (LocalDB or any SQL instance)
  
### Setup Instructions

1. Clone the repository:
   ```bash
   git clone https://github.com/LVarsaminov/TeamRankingSystem.git
   cd TeamRankingSystem

##API Endpoints
Method  Endpoint   Description 
-GET	/api/teams	 Get all teams
-GET	/api/teams/{id}	 Get team by ID
-POST	/api/teams	Create a new team
-PUT	/api/teams/{id}	Update an existing team
-DELETE	/api/teams/{id}	Delete a team
-GET /api/teams/rankingList Get the ranking list for all teams

-GET	/api/matches	Get all matches
-GET	/api/matches/{id}	Get match by ID
-POST	/api/matches	Create a new match
-PUT	/api/matches/{id}	Update a match
-DELETE	/api/matches/{id}	Delete a match
