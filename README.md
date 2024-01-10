# DotNetLeagueAPI

This League Management Application is built using .NET 7.0, serving as a minimal Web API that leverages MSSQL for data storage. The system is designed to manage a league format with 16 teams divided into four groups A, B, C, D, each containing four teams. After three rounds of group matches, the administrator selects the best team from each group based on points, and they proceed to the semi-finals. The two winning teams from the semi-finals advance to the final, where the ultimate winner is determined. Match results are randomly generated within a range of 0 to 5 goals, with wins earning 3 points, losses 0 points, and draws 1 point. In case of a draw in the semi-finals or final, a random point is awarded to one team.


## How Entites interact 
![Screenshot 2024-01-05 000520](https://github.com/LeonitKrasniqi/DotNetLeague/assets/102996903/e14d9884-3668-473e-9e97-bb8ca1f6dc42)


## Role-Based Access Control

## Token Validity

The access token provided upon successful authentication is valid for 5 days. After this period, users will need to re-authenticate to obtain a new token.

### Admin

### In LeagueController:

GenerateTeamsIntoGroup - [HttpPost("generate-groups")]

GetGeneratedLeague - [HttpGet("league")]

GenerateGroupMatches - [HttpPost("generate-group-matches")]

GenerateFinalMatches - [HttpPost("generate-semifinal-matches")]

GenerateFinalMatch - [HttpPost("generate-final-match")]

GetWinner - [HttpGet("winner")]

### In TeamsController:

GetAll - [HttpGet]

Create - [HttpPost]

### User

### In LeagueController:

GetGeneratedLeague - [HttpGet("league")]

GetBestTeamsPerGroup - [HttpGet("best-teams-per-group")]

GetBestTeamsFromSemifinals - [HttpGet("best-teams-from-semifinals")]

GetWinner - [HttpGet("winner")]

### In TeamsController:

GetById - [HttpGet("{id:Guid}")]

Create - [HttpPost]


## Usage

When logged in with your token, users have the following capabilities:

### User Feature
#### Team Management
- Create Your Team: As a user, you can create your own team. However, please note that the number of players in your team must be within the range of 23 to 34.
- View Detailed Team: Users have the ability to view detailed information about their own team.

#### League Information
- View Groups: Users can see information about the groups in the league.
- View Group Results: Access the results of matches within the groups.
- View Semi-Final and Final Results: If applicable, users can view results of semi-final and final matches.


### Admin Feature
#### Team Managment
- View All Teams: Admins have the privilege to view details of all teams.
- Generate and Draw Teams Into Groups: Admins can generate and draw teams into groups for the league.

#### League Managment
- Generate Group Matches: Admins have the ability to generate matches within the groups.
- Generate Semi-Final Matches: Admins can generate semi-final matches.
- Generate Final Match: Admins can generate the final match.


#### Possible Issues
Admins might encounter the following issues:
- Cannot generate draw: This could happen if there is already a draw in the database or if there are not exactly 16 teams.
- Cannot generate games into the groups more than 3 times: There is a limit on the number of times games can be generated.
- Cannot generate semi-final matches: This may occur if there are not exactly four best teams.
- Cannot generate final match: If the latest two matches required for the final are not found, the final match cannot be generated.

Please be aware of these constraints and potential issues while using the DotNetLeague.API. If you face any challenges, refer to the error messages provided for more information 😋



## Package References

- **AutoMapper** - version 12.0.1
- **AutoMapper.Extensions.Microsoft.DependencyInjection** - version 12.0.1
- **Microsoft.AspNetCore.Authentication.JwtBearer** - version 7.0.13
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore** - version 7.0.12
- **Microsoft.AspNetCore.OpenApi** - version 7.0.11
- **Microsoft.EntityFrameworkCore.SqlServer** - version 7.0.13
- **Microsoft.EntityFrameworkCore.Tools** - version 7.0.13
- **Microsoft.IdentityModel.Tokens** - version 7.0.1
- **Swashbuckle.AspNetCore** - version 6.5.0
- **System.IdentityModel.Tokens.Jwt** - version 7.0.0


## Local Host Ports

The application is hosted in the following local host port:

- HTTPS: https://localhost:7294
