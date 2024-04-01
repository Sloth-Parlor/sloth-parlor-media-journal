[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$PostgresHost,

    [Parameter(Mandatory=$false)]
    [switch]$ResetAppUserPassword = $false
)

# Define the database and user details
$dbName = "sp_media_journal"
$dbUser = "appuser"

# Generate a new GUID for the password
$password = [guid]::NewGuid().ToString()

# Define the SQL commands to check for the existence of the database and user
$checkDbExistsCommand = "SELECT 1 FROM pg_database WHERE datname = '$dbName';"
$checkUserExistsCommand = "SELECT 1 FROM pg_roles WHERE rolname = '$dbUser';"

# Check if the database exists
$dbExists = psql -q -h $PostgresHost -U $env:PGUSER -t -c $checkDbExistsCommand

# If the database doesn't exist, create it
if (-not $dbExists) {
    psql -q -h $PostgresHost -U $env:PGUSER -c "CREATE DATABASE $dbName;"
}

# Check if the user exists
$userExists = psql -q -h $PostgresHost -U $env:PGUSER -t -c $checkUserExistsCommand

# If the user doesn't exist, create it
if (-not $userExists) {
    psql -q -h $PostgresHost -U $env:PGUSER -c "CREATE USER $dbUser WITH PASSWORD '$password';"
}

# If the user was just created or the ResetAppUserPassword flag is set, set the password
if (-not $userExists -or $ResetAppUserPassword) {
    psql -q -h $PostgresHost -U $env:PGUSER -c "ALTER USER $dbUser WITH PASSWORD '$password';"
    Write-Host "The password for the $dbUser user is: $password"
}

# Grant all privileges on the database to the user
psql -q -h $PostgresHost -U $env:PGUSER -c @"

ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL PRIVILEGES ON TABLES TO $dbUser;

GRANT ALL PRIVILEGES ON DATABASE $dbName TO $dbUser;
GRANT ALL PRIVILEGES ON SCHEMA public TO $dbUser;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO $dbUser;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO $dbUser;
GRANT ALL PRIVILEGES ON ALL FUNCTIONS IN SCHEMA public TO $dbUser;

DO
`$`$
BEGIN
    IF EXISTS (SELECT FROM pg_catalog.pg_class c JOIN pg_catalog.pg_namespace n ON n.oid = c.relnamespace WHERE n.nspname = 'public' AND c.relname = '__EFMigrationsHistory') THEN
        EXECUTE 'GRANT ALL PRIVILEGES ON TABLE public."__EFMigrationsHistory" TO ' || quote_ident('$dbUser');
    END IF;
END
`$`$;

"@

