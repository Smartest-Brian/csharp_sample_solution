-- 插入一個管理員帳號
INSERT INTO auth.users (username, email, password_hash, password_salt, roles, is_active)
VALUES ('admin',
        'admin@example.com',
        'cGFzc3dvcmRfaGFzaF9leGFtcGxl', -- 假設 base64 之後的 PBKDF2 hash
        'c2FsdF9leGFtcGxl', -- 假設 base64 之後的 salt
        ARRAY['admin', 'user'],
        true);

-- 插入一個一般使用者帳號
INSERT INTO auth.users (username, email, password_hash, password_salt, roles, is_active)
VALUES ('johndoe',
        'john.doe@example.com',
        'anNub25jZV9oYXNoX2V4YW1wbGU=', -- 假設 base64 PBKDF2 hash
        'c2FsdF9qb2hu', -- 假設 base64 salt
        ARRAY['user'],
        true);

-- 插入一個停用帳號
INSERT INTO auth.users (username, email, password_hash, password_salt, roles, is_active)
VALUES ('disabled_user',
        'disabled@example.com',
        'ZGlzYWJsZWRfaGFzaA==', -- 假設 base64 PBKDF2 hash
        'ZGlzYWJsZWRfc2FsdA==', -- 假設 base64 salt
        ARRAY['user'],
        false);



INSERT INTO countries (country_name, country_code2, country_code3, currency_code, timezone)
VALUES ('United States', 'US', 'USA', 'USD', 'America/New_York'),
       ('Japan', 'JP', 'JPN', 'JPY', 'Asia/Tokyo'),
       ('Taiwan', 'TW', 'TWN', 'TWD', 'Asia/Taipei'),
       ('Germany', 'DE', 'DEU', 'EUR', 'Europe/Berlin'),
       ('United Kingdom', 'GB', 'GBR', 'GBP', 'Europe/London'),
       ('Australia', 'AU', 'AUS', 'AUD', 'Australia/Sydney');



