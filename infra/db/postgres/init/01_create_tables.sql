CREATE TABLE products
(
    id          BIGINT PRIMARY KEY,
    name        TEXT        NOT NULL,
    price_cents INTEGER     NOT NULL,
    updated_at  TIMESTAMPTZ NOT NULL DEFAULT NOW()
);