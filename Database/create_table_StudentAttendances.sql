-- Creates attendance table expected by stored procedures and Dapper (name must match: StudentAttendances).
-- Run against your school DB (fix USE if your database name differs).

USE school_managemnet_system;

CREATE TABLE IF NOT EXISTS `StudentAttendances` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `StudentId` INT NOT NULL,
    `AttendanceDate` DATETIME NOT NULL,
    `Status` VARCHAR(20) NOT NULL,
    `Created_At` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `Updated_At` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`Id`),
    KEY `IX_StudentAttendances_StudentId` (`StudentId`),
    KEY `IX_StudentAttendances_AttendanceDate` (`AttendanceDate`),
    CONSTRAINT `FK_StudentAttendances_Students`
        FOREIGN KEY (`StudentId`) REFERENCES `Students` (`Id`)
        ON DELETE CASCADE
        ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
