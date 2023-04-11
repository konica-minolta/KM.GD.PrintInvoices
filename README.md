# KM.GD.PrintInvoices
 
## Create the Database
Tools → NuGet Package Manager → Package Manager Console).
Use the command:
PM> Add-Migration Initial    

Add-Migration [name of migration] -Context [name of]

## This will create a Migrations

 Add-Migration SeparateImageTable -Context CameraContext



In the Package Manager Console use the command:
PM> Update-Database

Update-Database -Context [Name of]

## This will update DB tables   

Update-Database -Context CameraContext


## TO BUILD

Use Node version  v14.19.1

Download NVM to manage version

https://github.com/coreybutler/nvm-windows/releases


