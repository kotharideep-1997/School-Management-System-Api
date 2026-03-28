-- Widen/narrow ClassMaster label column to match API (1–5 chars: 5A, 12B, 10A, …).
-- Run after backup if you have existing data longer than 5 characters (truncate or fix rows first).

USE school_managemnet_system;

ALTER TABLE ClassMaster
    MODIFY COLUMN `Class` VARCHAR(5) NOT NULL;

-- Recreate insert procedure so parameter matches column (if you use procedures from repo).
DROP PROCEDURE IF EXISTS sp_ClassMaster_Insert;

DELIMITER //
CREATE PROCEDURE sp_ClassMaster_Insert(
    IN  p_Class     VARCHAR(5),
    IN  p_Strength  TINYINT UNSIGNED,
    IN  p_CreatedAt DATETIME,
    IN  p_UpdatedAt DATETIME,
    IN  p_IsActive  TINYINT(1),
    OUT p_NewId INT
)
BEGIN
    INSERT INTO ClassMaster (`Class`, Strength, CreatedAt, UpdatedAt, IsActive)
    VALUES (p_Class, p_Strength, p_CreatedAt, p_UpdatedAt, p_IsActive);
    SET p_NewId = LAST_INSERT_ID();
END //
DELIMITER ;
