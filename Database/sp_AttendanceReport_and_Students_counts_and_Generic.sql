-- Stored procedures replacing raw SELECTs in:
--   AttendanceReportRepository (day rows + filtered grid)
--   StudentRepository (count active in class, count by roll)
--   GenericRepository (whitelist: Students, Users only)
-- Run after USE your_database;

USE school_managemnet_system;

-- ---------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_AttendanceReport_SelectDayRows;
DELIMITER //
CREATE PROCEDURE sp_AttendanceReport_SelectDayRows(
    IN p_From DATE,
    IN p_ToExclusive DATE
)
BEGIN
    SELECT s.RollNo,
           s.FirstName,
           s.LastName,
           cm.`Class` AS Class,
           sa.AttendanceDate,
           CASE LOWER(TRIM(sa.Status))
               WHEN 'present' THEN 'Present'
               WHEN 'absent' THEN 'Absent'
               ELSE TRIM(IFNULL(sa.Status, ''))
           END AS Attendance
    FROM StudentAttendances sa
    INNER JOIN Students s ON s.Id = sa.StudentId
    INNER JOIN ClassMaster cm ON cm.Id = s.ClassId
    WHERE sa.AttendanceDate >= p_From
      AND sa.AttendanceDate < p_ToExclusive
    ORDER BY s.RollNo;
END //
DELIMITER ;

-- ---------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_AttendanceReport_SelectGridFiltered;
DELIMITER //
CREATE PROCEDURE sp_AttendanceReport_SelectGridFiltered(
    IN p_AttFrom DATE,
    IN p_AttToExclusive DATE,
    IN p_NameLike VARCHAR(255),
    IN p_RollNo INT
)
BEGIN
    SELECT s.RollNo,
           s.FirstName,
           s.LastName,
           cm.`Class` AS Class,
           sa.AttendanceDate,
           CASE LOWER(TRIM(sa.Status))
               WHEN 'present' THEN 'Present'
               WHEN 'absent' THEN 'Absent'
               ELSE TRIM(IFNULL(sa.Status, ''))
           END AS Attendance
    FROM StudentAttendances sa
    INNER JOIN Students s ON s.Id = sa.StudentId
    INNER JOIN ClassMaster cm ON cm.Id = s.ClassId
    WHERE sa.AttendanceDate >= p_AttFrom
      AND sa.AttendanceDate < p_AttToExclusive
      AND (
          p_NameLike IS NULL
          OR s.FirstName LIKE p_NameLike
          OR s.LastName LIKE p_NameLike
          OR CONCAT(TRIM(IFNULL(s.FirstName, '')), ' ', TRIM(IFNULL(s.LastName, ''))) LIKE p_NameLike
      )
      AND (p_RollNo IS NULL OR s.RollNo = p_RollNo)
    ORDER BY sa.AttendanceDate DESC, s.RollNo;
END //
DELIMITER ;

-- ---------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_Students_CountActiveInClass;
DELIMITER //
CREATE PROCEDURE sp_Students_CountActiveInClass(
    IN p_ClassId INT,
    IN p_ExcludeStudentId INT
)
BEGIN
    SELECT COUNT(*) AS Cnt
    FROM Students
    WHERE ClassId = p_ClassId
      AND Active = 1
      AND (p_ExcludeStudentId IS NULL OR Id <> p_ExcludeStudentId);
END //
DELIMITER ;

-- ---------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_Students_CountByClassAndRollNo;
DELIMITER //
CREATE PROCEDURE sp_Students_CountByClassAndRollNo(
    IN p_ClassId INT,
    IN p_RollNo INT,
    IN p_ExcludeStudentId INT
)
BEGIN
    SELECT COUNT(*) AS Cnt
    FROM Students
    WHERE ClassId = p_ClassId
      AND RollNo = p_RollNo
      AND (p_ExcludeStudentId IS NULL OR Id <> p_ExcludeStudentId);
END //
DELIMITER ;

-- ---------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_Generic_SelectAll;
DELIMITER //
CREATE PROCEDURE sp_Generic_SelectAll(IN p_Table VARCHAR(64))
BEGIN
    IF p_Table = 'Students' THEN
        SELECT * FROM Students;
    ELSEIF p_Table = 'Users' THEN
        SELECT * FROM Users;
    ELSE
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'sp_Generic_SelectAll: table not allowed (use Students or Users).';
    END IF;
END //
DELIMITER ;

-- ---------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_Generic_SelectById;
DELIMITER //
CREATE PROCEDURE sp_Generic_SelectById(IN p_Table VARCHAR(64), IN p_Id INT)
BEGIN
    IF p_Table = 'Students' THEN
        SELECT * FROM Students WHERE Id = p_Id LIMIT 1;
    ELSEIF p_Table = 'Users' THEN
        SELECT * FROM Users WHERE Id = p_Id LIMIT 1;
    ELSE
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'sp_Generic_SelectById: table not allowed (use Students or Users).';
    END IF;
END //
DELIMITER ;
