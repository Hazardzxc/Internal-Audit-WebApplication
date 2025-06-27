
# Internal Audit Web Application – Setup Module

This project is a **web-based application prototype** developed to improve the internal audit process at universities. 
It focuses on the **Data and Working Paper Setup Module** that allows efficient configuration and planning of audit sessions.

Developed by: Sarun Jearawattanakul  
Faculty of Information and Communication Technology, Mahidol University  
Academic Year: 2023

---

## 📌 Project Overview

Traditional manual auditing in universities is often paper-based, time-consuming, and error-prone.  
This application offers a centralized, digital solution to streamline and standardize the internal audit preparation process.

Key features include:
- Data setting for faculties, audit issues, and auditors
- Working paper configuration for audit sessions
- Preview and preparation of digital working papers

---

## 🎯 Objectives

- Develop a centralized web application for internal university audits
- Reduce repetitive configuration and human error
- Improve audit readiness and consistency across departments

---

## 🧰 Tech Stack

- Backend: ASP.NET (.NET Framework)
- Frontend: HTML/CSS/JavaScript
- Database: SQL Server
- Tools: Visual Studio, SQL Server Management Studio

---

## 🗃️ Modules Included

1. **Faculty Setting** – Add and manage departments to be audited
2. **Auditor Management** – Assign roles and responsibilities
3. **Audit Issue Configuration** – Set up audit topics and subtopics
4. **Working Paper Setup** – Prepare audit documentation templates
5. **Preview Module** – Generate and view structured working papers

---

## 📌 Database Highlights

- `AuditIssueMain` / `AuditIssueSub` – Define topics and subtopics
- `AuditManagement` – Represents an audit session
- `AuditManagementSub` – Links sessions to specific issues
- `UserAuditor` – Tracks assigned auditors
- `Faculty` – Stores faculty and department information

---

## 🚧 Known Limitations

- No sorting/filtering feature in audit topic list
- Resistance to transitioning from manual to digital among users

---

## 🔮 Future Improvements

- Add execution and report modules
- Implement sorting, search, and filtering features
- Introduce multi-user access control
- Dashboard analytics and reporting

---

## 📸 Demo

Please refer to the `Demo` folder or presentation slides for screenshots and workflow examples.

---

## 📄 License

This project is for educational and academic demonstration purposes only.

---

Thank you for visiting this project!
