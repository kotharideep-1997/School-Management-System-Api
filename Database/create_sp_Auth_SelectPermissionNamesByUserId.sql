-- Run this once if login fails with: PROCEDURE ... sp_Auth_SelectPermissionNamesByUserId does not exist
-- Adjust database name if different from your connection string.

USE school_managemnet_system;

DROP PROCEDURE IF EXISTS sp_Auth_SelectPermissionNamesByUserId;

DELIMITER //
CREATE PROCEDURE sp_Auth_SelectPermissionNamesByUserId(IN p_UserId INT)
BEGIN
    SELECT pm.PermissionName
    FROM UserPermission up
    INNER JOIN PermissionMaster pm ON pm.Id = up.PermissionId
    WHERE up.UserId = p_UserId AND up.IsActive = 1 AND pm.IsActive = 1;
END //
DELIMITER ;
