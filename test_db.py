import os
import psycopg2
import sys
from urllib.parse import urlsplit, urlunsplit

db_url = os.getenv("DB_URL")

if not db_url:
    print("Missing DB_URL environment variable.")
    sys.exit(1)

parts = urlsplit(db_url)
masked_netloc = parts.netloc.split("@")[-1]
safe_url = urlunsplit((parts.scheme, masked_netloc, parts.path, parts.query, parts.fragment))

try:
    print(f"Connecting to: {safe_url}")
    conn = psycopg2.connect(db_url)
    print("Successfully connected to database!")
    
    cur = conn.cursor()
    cur.execute("SELECT version();")
    version = cur.fetchone()
    print(f"PostgreSQL version: {version[0]}")
    
    # Check if tables exist
    cur.execute("SELECT table_name FROM information_schema.tables WHERE table_schema='public';")
    tables = cur.fetchall()
    print("Tables in DB:")
    for table in tables:
        print(f"- {table[0]}")

    # Check Roles
    try:
        cur.execute('SELECT "Name" FROM "AspNetRoles";')
        roles = cur.fetchall()
        print("Roles in AspNetRoles:")
        for role in roles:
            print(f"- {role[0]}")
    except Exception as e:
        print(f"Could not query AspNetRoles: {e}")

    conn.close()
except Exception as e:
    print(f"Connection failed: {e}")
    sys.exit(1)
