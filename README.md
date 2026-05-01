![Header image](https://github.com/DougChisholm/App-Mod-Booster/blob/main/repo-header-booster.png)

# Access-Booster
A project to show how GitHub coding agent can turn a legacy MS Access database app into a working proof-of-concept for a cloud native Azure replacement.

Steps to modernise an app:

1. Fork this repo (then make private if uploading any PII)
2. Download the ps1 script to a local windows machine that has MS Access installed
3. Run the ps1 script in terminal (in the same folder as the access db file) to get all the database objects as text files.
4. Upload all those text files (of various file extensions) into the "input folder" in your repo (powershell -ExecutionPolicy Bypass -File .\access2sql.ps1)
5. Open the coding agents at top of the screen, select Access-Booster agent and ask it to "Modernise my access db"

The agent will create a project with a SQL Lite DB than can be run in Codespaces or your local machine. If the POC has all the functionality
matchign the legacy app then your Azure Account Team can help you migrate it to a PaaS solution.

Supporting slides for Microsoft Employees:
[Here](<https://microsofteur-my.sharepoint.com/:p:/g/personal/dchisholm_microsoft_com/IQAY41LQ12fjSIfFz3ha4hfFAZc7JQQuWaOrF7ObgxRK6f4?e=p6arJs>)
