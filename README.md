# ASP.NET Core 2.2 Starter Kit

ASP.NET Core 2.2 Starter Kit
I built this project in ASP.NET Core 2.2 with database first approach and Identity 2.0
I implemented many basic functionalities that we need to implement in every project again and again before start the project. With the help of this project, you just need to clone and rename the solution and start on actual funtionality. So, definatily it will save your time.

# Work Done
I implement the following functionalities in initial commit:

1. Role base authorization
2. Database, roles and admin account initialization (Code first approach). Database will create automatically on app first run
3. Admin rights to admin@yopmail.com (default username: admin & password: Super@123)
4. Identity 2.0
5. External login (login with Google)
6. Prevent user to login after register (sign up), admin approval required
7. Lock user for 15 minutes after 3 invalid login attempt
8. Sending email to user for email verification, reset password and account approved by admin.
9. User management - admin can update any user's profile info, login password, user permissions and can delete account.
10. More minor features

## Getting Started

Clone this repository then open solution in visual studio. Go to build > clean solution to clean the solution. Again go to Build menu and click on Rebuild Solution to restore nuget packages. Finaly click on Run button or press F5.

### Prerequisites

You must have below things ready on your system
.NET Core 2.2 
SQL Server
Visual Studio 2017 or above

### Installing

## Software
**Visual Studio 2017**
Download link: https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community

Installation help:
> https://docs.microsoft.com/en-us/visualstudio/install/install-visual-studio

**Microsoft SQL Server LocalDB**
Download link: 
_For 32-bit operating system_: http://download.microsoft.com/download/8/D/D/8DD7BDBA-CEF7-4D8E-8C16-D9F69527F909/ENU/x86/SqlLocaLDB.MSI

_For 64-bit operating system_: http://download.microsoft.com/download/8/D/D/8DD7BDBA-CEF7-4D8E-8C16-D9F69527F909/ENU/x64/SqlLocalDB.MSI

Installation help:
> https://www.sqlshack.com/install-microsoft-sql-server-express-localdb/


##How to change connection strings ##
Open your solution in Visual Studio 2017. In root directory find appsettings.json. Double click on this file and you will see the existing connection strings that you need to change. 
 
*Connection strings format:*
*_"Server=(localdb)\\[instance];Database=[database]; Trusted_Connection=True;MultipleActiveResultSets=true"_*
 
*[instance]* = Your SQL LocalDB instance 
*[database]* = Name of your database 
 
###Example:###
Instance name = mssqllocaldb and Database = myDatabase 
*_"Server=(localdb)\\mssqllocaldb;Database=myDatabase; Trusted_Connection=True;MultipleActiveResultSets=true"_*

###Example for SQL Server###
*_"Server=192.168.1.91;Database=ServiceAssetTracker-dev;Trusted_Connection=True;User ID=sa; Password=SQL_Password;MultipleActiveResultSets=true"_*

## Running the Project
1. Clone this project
2. Open ServiceAssetTracker.sln file with Visual Studio 2017
3. Go to Build > Clean to clean project
4. Go to Build > Rebuild to restore missing packages and location
5. Run solution by pressing F5

NOTE:
>Please make sure that all required software are installed and verify the system requirements before run the project to avoid any error.

## Built With

ASP.NET Core 2.2 MVC
Microsoft .NET Framework 4.7
Database Code First Approach
Bootstrap

## Contributing

Please contribute to this project to make an easy way to start any project.

## Authors

* **Kuldeep S. Gill** - *Initial work*

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Will update soon
