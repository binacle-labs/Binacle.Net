---
title: Users API
menu_title: Users
meta_description: >-
  The User Management API in Binacle.Net v1.3.x, available once the Service Module is on. Auth tokens, admin-
  only management, and the rules.
nav:
  parent: API
  order: 4
  icon: 👥
---

The User Management API is available after enabling the Service Module, along with the Authentication API endpoint.

---
## 📑 Contents
- [🔑 Auth Token](#-auth-token)
- [👥 User Management (Admin Only)](#-user-management-admin-only)
- [🔑 User Management Rules](#-user-management-rules)
- [📧 Email and Password Requirements](#-email-and-password-requirements)
- [🔒 Recommended Practice: Creating a New Admin](#-recommended-practice-creating-a-new-admin)

---

## 🔑 Auth Token
`POST /api/auth/token`
Authenticate and obtain a JWT token for making the calls without rate limits.

**Request Example**
```json
{
  "email": "test@test.com",
  "password": "testpassword"
}
```

**Response Example**
```json
{
  "tokenType": "Bearer",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
  "expiresIn": 3600
}
```

---

## 👥 User Management (Admin Only)

### Create User
`POST /api/users`

**Request Example**
```json
{
  "email": "useremail@domain.com",
  "password": "userspassword"
}
```

### Update User
`PUT /api/users/{email}`

**Request Example**
```json
{
  "type": "User", // User/Admin
  "status": "Active" // Active/Inactive
}
```

### Change Password
`PATCH /api/users/{email}`

**Request Example**
```json
{
  "newPassword": "newpassword"
}
```

### Delete User
`DELETE /api/users/{email}`

---

## 🔑 User Management Rules
- ✔️ Admin users are the only ones who can manage other users, including creation, deletion, activation, 
deactivation, and promotion/demotion.
- ✔️ Only active users can interact with the API without rate limits.
- ✔️ Delete performs a soft-delete and ensures the user remains in the database but cannot interact with the API. 
 Once soft-deleted, the user cannot be restored.
- ✔️ A new user with the same email can be created after a soft-deletion.

--- 

## 📧 Email and Password Requirements
To maintain security and consistency, the following requirements must be met:

- **Email**: Must be a valid email format. It will not be used for communication or marketing purposes.
- **Password**: Must be at least 10 characters long.

---

## 🔒 Recommended Practice: Creating a New Admin
For enhanced security, follow these steps:

1. **Create a New Admin User**: After the Service Module is configured and the default admin is set, create a new user account and promote it to admin status.
2. **Deactivate the Default Admin**: Once a new admin is established, deactivate the default admin account. This reduces the risk of exposing default credentials and enhances system security.
