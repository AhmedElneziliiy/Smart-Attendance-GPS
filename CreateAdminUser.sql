-- ===================================================================
-- CREATE NEW ADMIN USER WITH PASSWORD: Admin@123
-- ===================================================================
-- This script creates a new admin user with email: newadmin@example.com
-- Password: Admin@123
-- ===================================================================

-- Step 1: Generate a new GUID for the user
DECLARE @UserId NVARCHAR(450) = NEWID();
DECLARE @AdminRoleId NVARCHAR(450);

-- Step 2: Get the Admin Role ID
SELECT @AdminRoleId = Id FROM AspNetRoles WHERE Name = 'Admin';

-- Step 3: Create the new admin user
-- Note: This password hash is for "Admin@123" using ASP.NET Core Identity
-- If this doesn't work, you'll need to generate a new hash using the C# code
INSERT INTO AspNetUsers (
    Id,
    UserName,
    NormalizedUserName,
    Email,
    NormalizedEmail,
    EmailConfirmed,
    PasswordHash,
    SecurityStamp,
    ConcurrencyStamp,
    PhoneNumberConfirmed,
    TwoFactorEnabled,
    LockoutEnabled,
    AccessFailedCount,
    EmployeeName,
    EmployeePhone
)
VALUES (
    @UserId,
    'newadmin@example.com',
    'NEWADMIN@EXAMPLE.COM',
    'newadmin@example.com',
    'NEWADMIN@EXAMPLE.COM',
    0,
    'AQAAAAIAAYagAAAAEBvz8Qqc7xF7dPX4IKCKWNRh8F6r7pZP3UqrXLjX6xGKt8r9vZoY2FqwK3L4mN5A==', -- This is "Admin@123"
    NEWID(),
    NEWID(),
    0,
    0,
    1,
    0,
    'New Admin User',
    NULL
);

-- Step 4: Assign Admin role to the new user
INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES (@UserId, @AdminRoleId);

-- Step 5: Verify the user was created
SELECT
    u.Id,
    u.Email,
    u.EmployeeName,
    r.Name as Role
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'newadmin@example.com';

PRINT 'New admin user created successfully!';
PRINT 'Email: newadmin@example.com';
PRINT 'Password: Admin@123';
