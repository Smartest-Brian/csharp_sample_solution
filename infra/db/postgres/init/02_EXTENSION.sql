-- 建議先啟用 extension
CREATE EXTENSION IF NOT EXISTS pgcrypto; -- 提供 gen_random_uuid()
CREATE EXTENSION IF NOT EXISTS citext;   -- 大小寫不敏感的字串
