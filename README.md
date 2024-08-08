# Secure Messages API

## Overview

The Secure Messages API is a web application designed to facilitate secure messaging between users. It supports user authentication, message sending and receiving, and admin functionalities to manage users and view decrypted data. The API is built using ASP.NET Core and Entity Framework Core with a SQLite database, and the frontend is implemented with HTML, CSS, and JavaScript.

The project was completed on 30th May, 2024.

## Features

- User registration and login
- Message sending and receiving
- User profile update (name and password)
- User account deletion
- Admin functionalities to view decrypted data
- Real-time message updates with polling
- Error handling with user-friendly notifications

## Technologies Used

- **Backend:** ASP.NET Core, Entity Framework Core, SQLite
- **Frontend:** HTML, CSS, JavaScript, Toastr (for notifications), Axios (for API requests)
- **Authentication:** Custom authentication and authorization mechanisms
- **API Documentation:** Swagger/OpenAPI

## API Endpoints

### User Management

- **Register User**
  - `POST /api/user/register`
  - Parameters: `userName`, `password`

- **Login User**
  - `GET /api/user/login`
  - Parameters: `userId`, `userName`, `password`

- **Update User Name**
  - `PATCH /api/user/update/name`
  - Parameters: `id`, `oldName`, `newName`

- **Update User Password**
  - `PATCH /api/user/update/password`
  - Parameters: `id`, `oldPassword`, `newPassword`

- **Delete User**
  - `DELETE /api/user/delete`
  - Parameters: `id`, `name`, `password`

### Recipient Management

- **Validate Recipient**
  - `GET /api/recipient/validate`
  - Parameters: `recipientId`, `recipientName`

### Messaging

- **Add Message**
  - `POST /api/message/add`
  - Parameters: `messageText`, `senderId`, `senderName`, `receiverId`

- **Get Corresponding User Messages**
  - `GET /api/message/get`
  - Parameters: `id`, `name`, `recipientId`, `recipientName`, `lastMessageId`

### Admin Functions

- **Get Decrypted Data**
  - `GET /api/get/decrypted/all`
  - Parameters: `password`

- **Get User Count**
  - `GET /api/users/get`

## Frontend Functionality

- **User Actions** - Handles user login, logout, profile updates, and message sending.
- **Chat Interface** - Manages chat UI updates, message fetching, and user interactions.
