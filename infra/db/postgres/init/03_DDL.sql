
CREATE SCHEMA auth AUTHORIZATION pg_database_owner;

-- 使用者表
CREATE TABLE IF NOT EXISTS auth.user_info (
    id               uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    username         citext NOT NULL UNIQUE,
    email            citext UNIQUE,
    password_hash    text   NOT NULL,   -- base64 儲存 PBKDF2 hash
    password_salt    text   NOT NULL,   -- base64 儲存 salt
    roles            text[] NOT NULL DEFAULT '{}',
    is_active        boolean NOT NULL DEFAULT true,
    last_login_at    timestamptz,
    created_at       timestamptz NOT NULL DEFAULT now(),
    updated_at       timestamptz NOT NULL DEFAULT now()
    );

-- Refresh Token 表
CREATE TABLE IF NOT EXISTS auth.user_refresh_token (
    id            uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id       uuid NOT NULL REFERENCES auth.user_info(id) ON DELETE CASCADE,
    token         text NOT NULL UNIQUE,
    expires_at    timestamptz NOT NULL,
    created_at    timestamptz NOT NULL DEFAULT now(),
    revoked_at    timestamptz
    );

CREATE INDEX IF NOT EXISTS idx_user_refresh_token_user ON auth.user_refresh_token(user_id);

-- 國家
CREATE TABLE country_info
(
    id            SERIAL PRIMARY KEY,
    country_name  VARCHAR(100) NOT NULL,
    country_code2 CHAR(2)      NOT NULL, -- ISO Alpha-2
    country_code3 CHAR(3)      NOT NULL, -- ISO Alpha-3
    currency_code CHAR(3)      NOT NULL, -- ISO 4217
    timezone      VARCHAR(50)  NOT NULL
);




