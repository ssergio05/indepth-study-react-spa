# Individual In-Depth Study: React SPA vs ASP.NET Core MVC
**Student:** Sergio González (S2605887 / s2605887@student.hb.se)  
**Course:** Development of Web Applications (C1UT1B), 1.5 ECTS, Autumn 2026  
**Track:** Track A (Frontend SPA - React v19 with Vite)  

## Project Overview
This repository contains the source code and micro-artifact implementation for my individual in-depth study report at the University of Borås. It builds directly upon the Laboratory Assignment 2 library management system, extending the C# .NET 10 backend with a read-only REST API endpoint (/api/books), CORS policy, and automatic database seeding, paired with an interactive React SPA client consuming the JSON data.

## Structure
- /lab2-library-app: ASP.NET Core MVC backend and API controllers.
- /lab2-library-app/client: React SPA frontend built with Vite and Axios.

## How to Run the Project Locally

### 1. Run the Backend (.NET API)
1. Open a terminal and navigate to the backend project directory:
   cd lab2-library-app
2. Start the ASP.NET Core application (which automatically seeds the SQLite database with 10 sample books):
   dotnet run --launch-profile https
   *(The API will be available at https://localhost:7164)*

### 2. Run the Frontend (React SPA)
1. Open a second terminal and navigate to the React client folder:
   cd lab2-library-app/client
2. Install the required dependencies:
   npm install
   *(On Windows PowerShell, if scripts are restricted, you can use npm.cmd install)*
3. Start the Vite development server:
   npm run dev
   *(Or npm.cmd run dev. The React application will run at http://localhost:5173)*