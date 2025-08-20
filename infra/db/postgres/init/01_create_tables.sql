CREATE TABLE countries
(
    id            SERIAL PRIMARY KEY,
    country_name  VARCHAR(100) NOT NULL,
    country_code2 CHAR(2)      NOT NULL, -- ISO Alpha-2
    country_code3 CHAR(3)      NOT NULL, -- ISO Alpha-3
    currency_code CHAR(3)      NOT NULL, -- ISO 4217
    timezone      VARCHAR(50)  NOT NULL
);