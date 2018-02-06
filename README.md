
sql_profiler is a simple and fast replacement for SQL Server Profiler. <br />
It's a .NET-Core command-line port of ExpressProfiler, and runs everywhere .NET Core runs. 
So it runs on 
- Windows (x86-32, x86-64)
- Linux (x86-64, ARM-32)
- Mac/OS-X. 

It's a fork of [ExpressProfiler](https://github.com/OleksiiKovalov/expressprofiler) for MS-SQL-Server ([actually a fork of a slightly modified version](https://github.com/ststeiger/ExpressProfiler) - working datetime-format for any language). <br />
I used it to port ExpressProfiler to .NET Core (command-line).


sql_profiler can be used with both Express and non-Express editions of SQL Server 2005/2008/2008r2/2012/2014.<br />
Installation or administrative rights are not required. <br />
Trace permission are required for the SQL-user.<br />


**Invocation**
```bash
./sql_profiler --server {computername\instance} --username WebAppWebServices --password TOP_SECRET --db "The DB you want to profile";
```

or from the project:

```bash
dotnet run sql_profiler --server {computername\instance} --username WebAppWebServices --password TOP_SECRET --db "The DB you want to profile";
```
If you omit the username, it will attempt to connect with integrated security.

[![Windows-Console-Profiler: This is Sparta !][1]][1]



**Grant rights:** 


```sql

-- To Grant access to a Windows Login
USE Master;
GO
GRANT ALTER TRACE TO [DomainNAME\WindowsLogin]
GO

-- To Grant access to a SQL Login
USE master;
GO
GRANT ALTER TRACE TO manvendra
GO

```

[(source)](https://www.mssqltips.com/sqlservertip/3559/how-to-grant-permissions-to-run-sql-server-profiler-for-a-non-system-admin-user/)


### Standalone Building

**Build for Windows x86-64:**
> dotnet restore -r win-x64<br />
> dotnet build -r win-x64<br />
> dotnet publish -f netcoreapp2.0 -c Release -r win-x64<br />

**Build for Windows x86-32:**
> dotnet restore -r win-x86<br />
> dotnet build -r win-x86<br />
> dotnet publish -f netcoreapp2.0 -c Release -r win-x86<br />

**Build for Linux x86-64:**
> dotnet restore -r linux-x64<br />
> dotnet build -r linux-x64<br />
> dotnet publish -f netcoreapp2.0 -c Release -r linux-x64<br />


**Build for Linux ARM-32 (Raspberry PI/Chromebook/Android):**
> dotnet restore -r linux-arm<br />
> dotnet build -r linux-arm<br />
> dotnet publish -f netcoreapp2.0 -c Release -r linux-arm<br />


**Build for Linux x86-32:**
> **not supported by framework**


  [1]: https://i.stack.imgur.com/IgYvq.png
