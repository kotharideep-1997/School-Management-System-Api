-- Replaces sp_Students_Insert with ClassId validation (run if you already created the old procedure).
USE school_managemnet_system;

DROP PROCEDURE IF EXISTS sp_Students_Insert;

DELIMITER //
CREATE PROCEDURE sp_Students_Insert(
    IN  p_FirstName VARCHAR(40),
    IN  p_LastName  VARCHAR(40),
    IN  p_RollNo    INT,
    IN  p_ClassId   INT,
    IN  p_Active    TINYINT(1),
    IN  p_CreatedDate DATETIME,
    OUT p_NewId INT
)
BEGIN
    IF p_ClassId IS NULL OR p_ClassId < 1 THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'ClassId is required and must be a positive integer.';
    END IF;

    IF NOT EXISTS (SELECT 1 FROM ClassMaster WHERE Id = p_ClassId) THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'ClassId does not exist in ClassMaster.';
    END IF;

    INSERT INTO Students (FirstName, LastName, RollNo, ClassId, Active, CreatedDate)
    VALUES (
        p_FirstName,
        p_LastName,
        p_RollNo,
        p_ClassId,
        p_Active,
        IFNULL(p_CreatedDate, CURRENT_TIMESTAMP)
    );
    SET p_NewId = LAST_INSERT_ID();
END //
DELIMITER ;
