# Backend Setup (MacOS)

```bash
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
brew install postgresql

brew services start postgresql
createuser postgres
createdb MementoMori

psql

alter user postgres with encyrpted password 'SuperSecretPassword';
grant all privileges on database "MementoMori" to postgres;
\q

cd ./MementoMori.API
dotnet tool install --global dotnet-ef
dotnet ef database update

dotnet run
```

# Frontend Setup (MacOS)

```bash
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.40.2/install.sh | bash
\. "$HOME/.nvm/nvm.sh"

npm i
npm run dev
```
