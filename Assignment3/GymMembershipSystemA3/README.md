# Gym Membership Management System

CPIS 358 - Assignment 3

## Project Description

This project is a Gym Membership Management System built using
ASP.NET Core MVC, Entity Framework Core, and SQLite.

The system allows members to register, log in, view membership
plans, request subscriptions, and check gym occupancy.

Administrators can manage members, membership plans,
subscriptions, and gym attendance.

## Technologies Used

- ASP.NET Core MVC (.NET 10)
- C#
- Entity Framework Core
- SQLite
- HTML
- CSS
- JavaScript

## How to Run the Project

1. Install the .NET 10 SDK.
2. Open the GymMembershipSystemA3 project folder.
3. Open Terminal in this folder.
4. If gymmembership.db does not already exist, copy
   gymmembership-demo.db and rename the copy to gymmembership.db.
5. Run:

   dotnet run

6. Open the local URL displayed in Terminal.

## Demo Accounts

Admin Account:

Email: demo.admin@example.com
Password: DemoGym2026!

Member 1:

Email: demo.member1@example.com
Password: DemoGym2026!

Member 2:

Email: demo.member2@example.com
Password: DemoGym2026!

These accounts are for demonstration purposes only.

## Sample Database

The project includes gymmembership-demo.db with:

- 3 registered accounts
- 3 membership plans
- 4 subscription records
- 3 gym visit records

The database contains fictional demonstration data.

## Main Features

- User registration and login
- Cookie authentication
- Session management
- Membership plans
- Subscription management
- Member management
- Gym check-in and check-out
- Gym occupancy monitoring
- Search members and subscriptions
- Admin dashboard