

# how to setup database

1. install a MySQL server
2. create a database
3. add it to the .env file
4. run the following command to create the tables
5. dotnet ef database update

if db does not work initially try:
dotnet tool install --global dotnet-ef

workbench crashing when trying to run/start follow these steps
Step 1. In your operating system go to the services app Step 
2. In the service app look for mySQL80 Step 
3. Start the service if it has been stopped Step 
4. Now test the connection to see if it works

as a hail marry try and restart the program the DB and run the program