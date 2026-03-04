\set ON_ERROR_STOP on

-- Creates missing databases (case-sensitive) if they don't exist.
-- Safe to run multiple times.

SELECT format('CREATE DATABASE %I;', 'UsersDb')
WHERE NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'UsersDb') \gexec

SELECT format('CREATE DATABASE %I;', 'CartDb')
WHERE NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'CartDb') \gexec

SELECT format('CREATE DATABASE %I;', 'FavoriteDb')
WHERE NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'FavoriteDb') \gexec

SELECT format('CREATE DATABASE %I;', 'CatalogDb')
WHERE NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'CatalogDb') \gexec

SELECT format('CREATE DATABASE %I;', 'Orders')
WHERE NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'Orders') \gexec

SELECT format('CREATE DATABASE %I;', 'StorageDb')
WHERE NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'StorageDb') \gexec

