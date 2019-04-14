# Oracle4Net [![NuGet Version](https://img.shields.io/nuget/v/Oracle4Net.svg?style=flat)](https://www.nuget.org/packages/Oracle4Net/)
Simple Oracle Database connector for Microsoft .NET Core based on official [Oracle Data Provider for .NET (ODP.NET) Core](https://www.nuget.org/packages/Oracle.ManagedDataAccess.Core/).

## Features
* .NET Standard 2.0 Library
* Easy installation using [NuGet](https://www.nuget.org/packages/Oracle4Net/)
* Console Logging
* Oracle Exception Handling

## Example
```csharp
class Program
{
	static void Main(string[] args)
	{
		const string dataSource = "(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=XEPDB1)))";
		OracleDatabase db = new OracleDatabase();
		System.Console.WriteLine(db.IsConnected());
		try
		{
			db.Connect("hr", "password", dataSource);
			System.Console.WriteLine(db.ExecuteSQLCount("employees", "department_id = 50"));
			db.Disconnect();
		}
		catch (OracleDatabaseException)
		{
			System.Console.WriteLine("Oracle Exception");
		}
        System.Console.WriteLine(db.IsConnected());
	}
}
```

## Issues
Found a bug? Have suggestion? Please report in the [Issues Tracker](https://github.com/jtsoya539/Oracle4Net/issues).