-- Fixes: "Field 'Created_At' doesn't have a default value" on insert.
-- Run on school_managemnet_system (adjust USE if needed).

USE school_managemnet_system;

-- 1) Table: defaults so inserts always get timestamps even if a caller omits columns
ALTER TABLE `StudentAttendances`
    MODIFY COLUMN `Created_At` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    MODIFY COLUMN `Updated_At` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;

-- 2) Procedure: always coalesce NULL params before INSERT
DROP PROCEDURE IF EXISTS sp_StudentAttendances_Insert;

DELIMITER //
CREATE PROCEDURE sp_StudentAttendances_Insert(
    IN  p_StudentId      INT,
    IN  p_AttendanceDate DATETIME,
    IN  p_Status         VARCHAR(20),
    OUT p_NewId INT
)
BEGIN
    INSERT INTO StudentAttendances (StudentId, AttendanceDate, Status, Created_At, Updated_At)
    VALUES (p_StudentId, p_AttendanceDate, p_Status, UTC_TIMESTAMP(), UTC_TIMESTAMP());
    SET p_NewId = LAST_INSERT_ID();
END //
DELIMITER ;
