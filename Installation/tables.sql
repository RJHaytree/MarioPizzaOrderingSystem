DROP DATABASE IF EXISTS backend_db;
CREATE DATABASE backend_db;
USE backend_db;

CREATE TABLE IF NOT EXISTS tbl_appLogin(
    ID INT AUTO_INCREMENT NOT NULL,
    username VARCHAR(30) NOT NULL,
    password VARCHAR(30) NOT NULL,
    employee_key VARCHAR(10) NOT NULL,
    PRIMARY KEY (ID),
    UNIQUE (employee_key)
)engine=innodb;

INSERT INTO tbl_appLogin VALUES(NULL,'Admin','Password123','6FFG65T44W');
INSERT INTO tbl_appLogin VALUES(NULL,'Staff','TestLogin123','FRTG65WSGT');
INSERT INTO tbl_appLogin VALUES(NULL,'Staff','TestingPass987','TG99909Y6E'); 