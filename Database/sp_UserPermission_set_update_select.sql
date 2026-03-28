-- Deploy UserPermission set / update-active / select procedures (MySQL 8.x).
-- Run after PermissionMaster + UserPermission tables exist.
-- mysql -u root -p school_managemnet_system < sp_UserPermission_set_update_select.sql

USE school_managemnet_system;

DELIMITER //

DROP PROCEDURE IF EXISTS sp_UserPermission_Set//
DROP PROCEDURE IF EXISTS sp_UserPermission_UpdateActive//
DROP PROCEDURE IF EXISTS sp_UserPermission_SelectAll//
DROP PROCEDURE IF EXISTS sp_UserPermission_SelectByUserId//

CREATE PROCEDURE sp_UserPermission_Set(
    IN p_UserId       INT,
    IN p_PermissionId INT,
    IN p_IsActive     TINYINT(1)
)
BEGIN
    DECLARE v_now DATETIME DEFAULT UTC_TIMESTAMP();
    DECLARE v_cnt INT DEFAULT 0;

    SELECT COUNT(*) INTO v_cnt
    FROM UserPermission
    WHERE UserId = p_UserId AND PermissionId = p_PermissionId;

    IF v_cnt = 0 THEN
        INSERT INTO UserPermission (UserId, PermissionId, IsActive, CreatedAt, UpdatedAt)
        VALUES (p_UserId, p_PermissionId, p_IsActive, v_now, v_now);
    ELSE
        UPDATE UserPermission
        SET IsActive = p_IsActive, UpdatedAt = v_now
        WHERE UserId = p_UserId AND PermissionId = p_PermissionId;
    END IF;
END//

CREATE PROCEDURE sp_UserPermission_UpdateActive(
    IN p_UserId       INT,
    IN p_PermissionId INT,
    IN p_IsActive     TINYINT(1),
    OUT p_RowCount    INT
)
BEGIN
    UPDATE UserPermission
    SET IsActive = p_IsActive, UpdatedAt = UTC_TIMESTAMP()
    WHERE UserId = p_UserId AND PermissionId = p_PermissionId;
    SET p_RowCount = ROW_COUNT();
END//

CREATE PROCEDURE sp_UserPermission_SelectAll()
BEGIN
    SELECT
        up.Id,
        up.UserId,
        up.PermissionId,
        pm.PermissionName AS PermissionName,
        up.IsActive,
        up.CreatedAt,
        up.UpdatedAt
    FROM UserPermission up
    INNER JOIN PermissionMaster pm ON pm.Id = up.PermissionId
    ORDER BY up.UserId, up.PermissionId;
END//

CREATE PROCEDURE sp_UserPermission_SelectByUserId(IN p_UserId INT)
BEGIN
    SELECT
        up.Id,
        up.UserId,
        up.PermissionId,
        pm.PermissionName AS PermissionName,
        up.IsActive,
        up.CreatedAt,
        up.UpdatedAt
    FROM UserPermission up
    INNER JOIN PermissionMaster pm ON pm.Id = up.PermissionId
    WHERE up.UserId = p_UserId
    ORDER BY up.PermissionId;
END//

DELIMITER ;
