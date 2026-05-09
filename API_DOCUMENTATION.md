# Smart Attendance GPS - API Documentation

## Base URL
- Development: `https://localhost:7192`
- Production: `[Your Production URL]`

## Table of Contents
1. [Authentication](#authentication)
2. [Employee Endpoints](#employee-endpoints)
3. [Admin Endpoints](#admin-endpoints)
4. [Face Recognition Endpoints](#face-recognition-endpoints)

---

## Authentication

All endpoints (except login and change password) require JWT Bearer token authentication.

### Header Format
```
Authorization: Bearer {your-jwt-token}
```

---

## 1. Authentication Endpoints

### 1.1 Login
**Endpoint:** `POST /api/auth/login`

**Description:** Authenticate user and receive JWT token.

**Request Body:**
```json
{
  "email": "string",
  "password": "string"
}
```

**Response (200 OK):**
```json
{
  "token": "string",
  "id": "string",
  "email": "string",
  "employeeName": "string",
  "employeePhone": "string",
  "roles": ["string"],
  "companies": ["string"]
}
```

**Error Responses:**
- `401 Unauthorized`: Invalid email or password

**Example:**
```json
// Request
{
  "email": "john.doe@example.com",
  "password": "SecurePass123!"
}

// Response
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "email": "john.doe@example.com",
  "employeeName": "John Doe",
  "employeePhone": "+1234567890",
  "roles": ["Employee"],
  "companies": ["Company A", "Company B"]
}
```

---

### 1.2 Change Password
**Endpoint:** `POST /api/auth/change-password`

**Description:** Change user password without requiring the old password (password reset).

**Request Body:**
```json
{
  "email": "string",
  "newPassword": "string"
}
```

**Validation Rules:**
- Email: Required, valid email format
- NewPassword: Required

**Response (200 OK):**
```json
"Password changed successfully"
```

**Error Responses:**
- `404 Not Found`: User not found
- `400 Bad Request`: Password change failed

**Example:**
```json
// Request
{
  "email": "john.doe@example.com",
  "newPassword": "NewSecurePass456!"
}
```

---

## 2. Employee Endpoints

**Authentication Required:** Yes (Bearer Token)

---

### 2.1 Check-In
**Endpoint:** `POST /api/employee/checkin`

**Description:** Record employee check-in with GPS coordinates. The user ID is extracted from the JWT token.

**Request Body:**
```json
{
  "latitude": "number (double)",
  "longitude": "number (double)"
}
```

**Response (200 OK):**
```json
"Check-in successful."
```

**Error Responses:**
- `400 Bad Request`: Failed to check-in. Coordinates may be out of range or user already checked-in.
- `401 Unauthorized`: Invalid or missing token

**Example:**
```json
// Request
{
  "latitude": 30.033333,
  "longitude": 31.233334
}
```

---

### 2.2 Check-Out
**Endpoint:** `POST /api/employee/checkout`

**Description:** Record employee check-out with GPS coordinates. The user ID is extracted from the JWT token.

**Request Body:**
```json
{
  "latitude": "number (double)",
  "longitude": "number (double)"
}
```

**Response (200 OK):**
```json
"Check-out successful."
```

**Error Responses:**
- `400 Bad Request`: Failed to check-out. Coordinates may be out of range or no active check-in found.
- `401 Unauthorized`: Invalid or missing token

**Example:**
```json
// Request
{
  "latitude": 30.033333,
  "longitude": 31.233334
}
```

---

### 2.3 Get Attendance Report
**Endpoint:** `GET /api/employee/attendance/report`

**Description:** Retrieve attendance records for a user within a date range.

**Query Parameters:**
- `userId` (optional): User ID. If not provided, uses the authenticated user's ID from the token.
- `fromDate` (required): Start date (DateTime format: `YYYY-MM-DD` or `YYYY-MM-DDTHH:mm:ss`)
- `toDate` (optional): End date (DateTime format). Defaults to current date if not provided.

**Response (200 OK):**
```json
[
  {
    "attendanceId": "number (int)",
    "checkInDate": "string (DateTime)",
    "checkOutDate": "string (DateTime)",
    "duration": "number (double, hours)",
    "companyName": "string"
  }
]
```

**Error Responses:**
- `400 Bad Request`:
  - User ID cannot be empty
  - From date cannot be in the future
  - To date cannot be before from date
- `404 Not Found`:
  - User not found
  - No attendance records found for the given dates
- `401 Unauthorized`: Invalid or missing token

**Example:**
```
GET /api/employee/attendance/report?fromDate=2024-01-01&toDate=2024-01-31
```

**Response:**
```json
[
  {
    "attendanceId": 1,
    "checkInDate": "2024-01-15T08:00:00",
    "checkOutDate": "2024-01-15T17:00:00",
    "duration": 9.0,
    "companyName": "Company A"
  },
  {
    "attendanceId": 2,
    "checkInDate": "2024-01-16T08:30:00",
    "checkOutDate": "2024-01-16T17:30:00",
    "duration": 9.0,
    "companyName": "Company B"
  }
]
```

---

### 2.4 Get User Profile
**Endpoint:** `GET /api/employee/users/Profile`

**Description:** Retrieve user profile information.

**Query Parameters:**
- `userId` (optional): User ID. If not provided, uses the authenticated user's ID from the token.

**Response (200 OK):**
```json
{
  "id": "string",
  "email": "string",
  "employeeName": "string",
  "employeePhone": "string",
  "roles": ["string"],
  "companies": ["string"]
}
```

**Error Responses:**
- `404 Not Found`: User not found
- `401 Unauthorized`: Invalid or missing token

**Example:**
```
GET /api/employee/users/Profile
```

**Response:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "email": "john.doe@example.com",
  "employeeName": "John Doe",
  "employeePhone": "+1234567890",
  "roles": ["Employee"],
  "companies": ["Company A", "Company B"]
}
```

---

## 3. Admin Endpoints

**Authentication Required:** Yes (Bearer Token)
**Role Required:** Admin

---

### 3.1 Get All Users
**Endpoint:** `GET /api/admin/users`

**Description:** Retrieve all users in the system.

**Response (200 OK):**
```json
[
  {
    "id": "string",
    "email": "string",
    "employeeName": "string",
    "employeePhone": "string",
    "roles": ["string"],
    "companies": ["string"]
  }
]
```

**Error Responses:**
- `401 Unauthorized`: Invalid or missing token
- `403 Forbidden`: User does not have Admin role

**Example Response:**
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "john.doe@example.com",
    "employeeName": "John Doe",
    "employeePhone": "+1234567890",
    "roles": ["Employee"],
    "companies": ["Company A"]
  },
  {
    "id": "660e8400-e29b-41d4-a716-446655440001",
    "email": "admin@example.com",
    "employeeName": "Admin User",
    "employeePhone": "+1234567891",
    "roles": ["Admin", "Employee"],
    "companies": ["Company A", "Company B"]
  }
]
```

---

### 3.2 Get User By ID
**Endpoint:** `GET /api/admin/users/{id}`

**Description:** Retrieve a specific user by their ID.

**Path Parameters:**
- `id` (required): User ID (string)

**Response (200 OK):**
```json
{
  "id": "string",
  "email": "string",
  "employeeName": "string",
  "employeePhone": "string",
  "roles": ["string"],
  "companies": ["string"]
}
```

**Error Responses:**
- `404 Not Found`: User not found
- `401 Unauthorized`: Invalid or missing token
- `403 Forbidden`: User does not have Admin role

**Example:**
```
GET /api/admin/users/550e8400-e29b-41d4-a716-446655440000
```

---

### 3.3 Create User
**Endpoint:** `POST /api/admin/users`

**Description:** Create a new user (employee).

**Request Body:**
```json
{
  "email": "string",
  "password": "string",
  "employeeName": "string",
  "employeePhone": "string"
}
```

**Validation Rules:**
- Email: Required, valid email format
- Password: Required, must contain:
  - At least 6 characters
  - At least one uppercase letter
  - At least one lowercase letter
  - At least one number
  - At least one special character (@$!%*?&)
- EmployeeName: Required
- EmployeePhone: Valid phone format

**Response (201 Created):**
```json
{
  "id": "string",
  "email": "string",
  "employeeName": "string",
  "employeePhone": "string",
  "roles": ["string"],
  "companies": ["string"]
}
```

**Location Header:** `/api/admin/users/{id}`

**Error Responses:**
- `400 Bad Request`:
  - Validation errors
  - A user with this email already exists
  - User creation failed
- `401 Unauthorized`: Invalid or missing token
- `403 Forbidden`: User does not have Admin role

**Example:**
```json
// Request
{
  "email": "jane.smith@example.com",
  "password": "SecurePass123!",
  "employeeName": "Jane Smith",
  "employeePhone": "+1234567892"
}

// Response
{
  "id": "770e8400-e29b-41d4-a716-446655440002",
  "email": "jane.smith@example.com",
  "employeeName": "Jane Smith",
  "employeePhone": "+1234567892",
  "roles": [],
  "companies": []
}
```

---

### 3.4 Update User
**Endpoint:** `PUT /api/admin/users/{id}`

**Description:** Update an existing user's information.

**Path Parameters:**
- `id` (required): User ID (string)

**Request Body:**
```json
{
  "email": "string",
  "employeeName": "string",
  "employeePhone": "string"
}
```

**Validation Rules:**
- Email: Required, valid email format
- EmployeeName: Required
- EmployeePhone: Required

**Response (200 OK):**
```json
{
  "id": "string",
  "email": "string",
  "employeeName": "string",
  "employeePhone": "string",
  "roles": ["string"],
  "companies": ["string"]
}
```

**Error Responses:**
- `404 Not Found`: User not found
- `400 Bad Request`: Validation errors
- `401 Unauthorized`: Invalid or missing token
- `403 Forbidden`: User does not have Admin role

**Example:**
```json
// Request
PUT /api/admin/users/770e8400-e29b-41d4-a716-446655440002

{
  "email": "jane.smith.updated@example.com",
  "employeeName": "Jane Smith Updated",
  "employeePhone": "+1234567893"
}
```

---

### 3.5 Assign Role to User
**Endpoint:** `POST /api/admin/users/{id}/role`

**Description:** Assign a role (Admin or Employee) to a user.

**Path Parameters:**
- `id` (required): User ID (string)

**Request Body:**
```json
{
  "role": "string"
}
```

**Valid Roles:**
- `Admin`
- `Employee`

**Response (200 OK):**
```json
"Role {role} assigned to user {id}"
```

**Error Responses:**
- `400 Bad Request`:
  - Invalid role. Only 'Admin' or 'Employee' roles can be assigned
  - Failed to assign role (user not found, role already assigned, or invalid role)
- `401 Unauthorized`: Invalid or missing token
- `403 Forbidden`: User does not have Admin role

**Example:**
```json
// Request
POST /api/admin/users/770e8400-e29b-41d4-a716-446655440002/role

{
  "role": "Admin"
}

// Response
"Role Admin assigned to user 770e8400-e29b-41d4-a716-446655440002"
```

---

### 3.6 Delete User
**Endpoint:** `DELETE /api/admin/users/{id}`

**Description:** Delete a user from the system.

**Path Parameters:**
- `id` (required): User ID (string)

**Response (200 OK):**
```json
"User With ID ({id}) deleted successfully"
```

**Error Responses:**
- `404 Not Found`: User not found
- `401 Unauthorized`: Invalid or missing token
- `403 Forbidden`: User does not have Admin role

**Example:**
```
DELETE /api/admin/users/770e8400-e29b-41d4-a716-446655440002

Response: "User With ID (770e8400-e29b-41d4-a716-446655440002) deleted successfully"
```

---

### 3.7 Assign User to Companies
**Endpoint:** `POST /api/admin/users/{userId}/assign-companies`

**Description:** Assign a user to one or more companies.

**Path Parameters:**
- `userId` (required): User ID (string)

**Request Body:**
```json
[1, 2, 3]
```
(Array of company IDs as integers)

**Response (200 OK):**
```json
"User successfully assigned to selected companies."
```

**Error Responses:**
- `400 Bad Request`:
  - At least one company must be provided
  - User could not be assigned to company (already assigned or company doesn't exist)
- `401 Unauthorized`: Invalid or missing token
- `403 Forbidden`: User does not have Admin role

**Example:**
```json
// Request
POST /api/admin/users/550e8400-e29b-41d4-a716-446655440000/assign-companies

[1, 2, 3]

// Response
"User successfully assigned to selected companies."
```

---

### 3.8 Unassign User from Company
**Endpoint:** `POST /api/admin/users/{userId}/unassign-company/{companyId}`

**Description:** Remove a user's assignment from a specific company.

**Path Parameters:**
- `userId` (required): User ID (string)
- `companyId` (required): Company ID (integer)

**Response (200 OK):**
```json
"User {userId} successfully unassigned from company {companyId}."
```

**Error Responses:**
- `400 Bad Request`: User or Company not found, or the user is not assigned to this company
- `401 Unauthorized`: Invalid or missing token
- `403 Forbidden`: User does not have Admin role

**Example:**
```
POST /api/admin/users/550e8400-e29b-41d4-a716-446655440000/unassign-company/2

Response: "User 550e8400-e29b-41d4-a716-446655440000 successfully unassigned from company 2."
```

---

## 4. Face Recognition Endpoints

**Authentication Required:** Yes (Bearer Token)
**Role Required:** Admin

---

### 4.1 Enroll Face
**Endpoint:** `POST /api/face/enroll`

**Description:** Enroll a user's face for face verification. The face image should be sent as a base64-encoded string.

**Request Body:**
```json
{
  "userId": "string",
  "faceImage": "string (base64-encoded image)"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "string"
}
```

**Error Responses:**
- `400 Bad Request`:
  - Invalid user ID
  - Face image is required
  - Face enrollment failed (invalid image, no face detected, etc.)
- `401 Unauthorized`: Invalid or missing token
- `403 Forbidden`: User does not have Admin role

**Example:**
```json
// Request
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "faceImage": "/9j/4AAQSkZJRgABAQEAYABgAAD..." // base64-encoded JPEG or PNG
}

// Response
{
  "success": true,
  "message": "Face enrolled successfully"
}
```

**Notes:**
- Face image must be in JPEG or PNG format
- Image should be base64-encoded
- Only one face should be visible in the image

---

### 4.2 Verify Face
**Endpoint:** `POST /api/face/verify`

**Description:** Verify a user's face against their enrolled face.

**Request Body:**
```json
{
  "userId": "string",
  "faceImage": "string (base64-encoded image)"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "string",
  "data": {
    "isVerified": true,
    "similarity": 0.95
  }
}
```

**Error Responses:**
- `400 Bad Request`:
  - Invalid user ID
  - Face image is required
  - Verification failed (no enrolled face, no face detected, etc.)
- `401 Unauthorized`: Invalid or missing token
- `403 Forbidden`: User does not have Admin role

**Example:**
```json
// Request
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "faceImage": "/9j/4AAQSkZJRgABAQEAYABgAAD..." // base64-encoded JPEG or PNG
}

// Response
{
  "success": true,
  "message": "Face verification completed",
  "data": {
    "isVerified": true,
    "similarity": 0.95
  }
}
```

**Notes:**
- Similarity score ranges from 0.0 to 1.0
- Higher similarity indicates a better match
- Typical verification threshold is 0.75-0.85

---

### 4.3 Delete Face Enrollment
**Endpoint:** `DELETE /api/face/enroll/{userId}`

**Description:** Delete a user's enrolled face data.

**Path Parameters:**
- `userId` (required): User ID (string)

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Face enrollment deleted successfully"
}
```

**Error Responses:**
- `400 Bad Request`: Invalid user ID
- `404 Not Found`: User not found or no face enrolled
- `401 Unauthorized`: Invalid or missing token
- `403 Forbidden`: User does not have Admin role

**Example:**
```
DELETE /api/face/enroll/550e8400-e29b-41d4-a716-446655440000

Response:
{
  "success": true,
  "message": "Face enrollment deleted successfully"
}
```

---

### 4.4 Check Enrollment Status
**Endpoint:** `GET /api/face/enroll/status/{userId}`

**Description:** Check if a user has enrolled their face.

**Path Parameters:**
- `userId` (required): User ID (string)

**Response (200 OK):**
```json
{
  "success": true,
  "userId": "string",
  "hasEnrollment": true
}
```

**Error Responses:**
- `400 Bad Request`: Invalid user ID
- `401 Unauthorized`: Invalid or missing token
- `403 Forbidden`: User does not have Admin role

**Example:**
```
GET /api/face/enroll/status/550e8400-e29b-41d4-a716-446655440000

Response:
{
  "success": true,
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "hasEnrollment": true
}
```

---

### 4.5 Toggle Face Verification
**Endpoint:** `PUT /api/face/verification/toggle`

**Description:** Enable or disable face verification for a specific user.

**Request Body:**
```json
{
  "userId": "string",
  "isEnabled": true
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Face verification enabled successfully",
  "userId": "string",
  "isEnabled": true
}
```

**Error Responses:**
- `400 Bad Request`:
  - Invalid user ID
  - Cannot enable face verification (user not found or no face enrolled)
  - Cannot disable face verification (user not found)
- `401 Unauthorized`: Invalid or missing token
- `403 Forbidden`: User does not have Admin role

**Example:**
```json
// Request
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "isEnabled": true
}

// Response
{
  "success": true,
  "message": "Face verification enabled successfully",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "isEnabled": true
}
```

**Notes:**
- Face verification can only be enabled if the user has already enrolled their face
- Disabling face verification does not delete the enrolled face data

---

## Error Response Format

All error responses follow a consistent format:

```json
{
  "message": "Error description"
}
```

Or for validation errors:

```json
{
  "errors": {
    "fieldName": ["Error message 1", "Error message 2"]
  }
}
```

---

## HTTP Status Codes

- `200 OK`: Request successful
- `201 Created`: Resource created successfully
- `400 Bad Request`: Invalid request data or business logic error
- `401 Unauthorized`: Missing or invalid authentication token
- `403 Forbidden`: User doesn't have required permissions
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

---

## Authentication Flow

1. **Login**: Call `/api/auth/login` with email and password
2. **Receive Token**: Store the JWT token from the response
3. **Subsequent Requests**: Include the token in the `Authorization` header:
   ```
   Authorization: Bearer {token}
   ```
4. **Token Expiration**: If you receive a 401 response, the token may have expired. Re-login to get a new token.

---

## Important Notes

1. **GPS Coordinates**:
   - Latitude and Longitude must be within the allowed range for check-in/check-out
   - The backend validates these coordinates against configured company locations

2. **Date Format**:
   - Use ISO 8601 format for dates: `YYYY-MM-DD` or `YYYY-MM-DDTHH:mm:ss`
   - All dates are in UTC

3. **User ID**:
   - Most employee endpoints can work without explicitly passing userId
   - The system will extract the userId from the JWT token
   - Admin endpoints allow specifying userId for managing other users

4. **Face Images**:
   - Must be base64-encoded
   - Supported formats: JPEG, PNG
   - Recommended resolution: 640x480 or higher
   - Face should be clearly visible and well-lit

5. **Role-Based Access**:
   - Admin endpoints require "Admin" role
   - Employee endpoints can be accessed by both "Admin" and "Employee" roles
   - Use the role assignment endpoint to grant Admin privileges

---

## Testing with Postman/Insomnia

### Step 1: Login
```
POST https://localhost:7192/api/auth/login
Content-Type: application/json

{
  "email": "your-email@example.com",
  "password": "your-password"
}
```

### Step 2: Copy the Token
From the response, copy the `token` value.

### Step 3: Set Authorization Header
For all subsequent requests, add:
```
Authorization: Bearer {paste-your-token-here}
```

---

## Mobile App Implementation Recommendations

### 1. Token Management
- Store the JWT token securely (e.g., Secure Storage, Keychain)
- Implement automatic token refresh or re-login on 401 errors
- Clear token on logout

### 2. GPS Handling
- Request location permissions from the user
- Use high-accuracy GPS for check-in/check-out
- Show user's current location before submitting
- Handle cases where GPS is unavailable or denied

### 3. Face Recognition
- Capture high-quality images with good lighting
- Convert images to base64 before sending
- Implement retry logic for failed verifications
- Provide clear feedback to users during enrollment/verification

### 4. Error Handling
- Display user-friendly error messages
- Implement retry logic for network failures
- Handle validation errors gracefully
- Log errors for debugging

### 5. Offline Support
- Queue check-in/check-out requests when offline
- Sync when connection is restored
- Show offline indicator to users

### 6. Date Handling
- Convert between local time and UTC for attendance reports
- Display dates in user's local timezone
- Handle different date formats properly

---

## Seeded Data

Based on the `DbSeeder.cs` file, the following data is seeded:

### Admin User
- **Email**: admin@attendance.com
- **Password**: Admin@123
- **Role**: Admin

Use these credentials for initial testing and development.

---

## Support

For technical support or questions about the API, please contact the development team.

**Last Updated**: 2026-02-09
