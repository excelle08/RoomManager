# RoomManager

This is a comprehensive course design written in C# running on .NET Core platform.

Be aware, bugs and security exploits may present due to the fact that this project is written in a damn hurry.

### How to run?

1. Install MySQL and [dotNET Core](http://dot.net)
2. Import database structure to MySQL (Usually by running `mysql -uroot -p < dbinit.sql` and `mysql -uroot -p <data.sql` under `RoomManager/db`
3. Go to `RoomManager/RoomManager/Models/Database.cs` and modify the database account settings.
4. Go to `RoomManager/RoomManager` directory, run this:
  ```shell
  dotnet restore;
  dotnet run
  ```
5. Visit 127.0.0.1:5000
6. Username: admin, password: admin
