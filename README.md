# 🚀 Data Quality Validation Service (C# / .NET Core)

This repository demonstrates a **robust and architecturally mature solution** for data contract validation. It is designed to be integrated as a high-availability **backend microservice**, essential for high-volume systems like DraftKings.

***

## 💡 Introduction and Context

This solution showcases expertise in **designing and implementing scalable, data-centric backend services** using C# and the .NET Core ecosystem. The core approach focuses on high architectural standards to ensure **scalability, maintainability, and data integrity**.

Key operational strengths demonstrated:

* **Architectural Design (OOP/DI):** Leveraging **OOP** principles and **Dependency Injection (DI)** for high extensibility and low coupling.
* **Asynchronous Scalability:** Utilizing the **`async/await`** pattern and **`Task.WhenAll`** for concurrent task execution and maximizing throughput.
* **Data Integrity Enforcement:** Developing specialized rules to act as a **data quality gatekeeper** before persistence.
* **Quality Assurance (Testing):** Maintaining strict code quality standards by implementing **100% unit test coverage** using **XUnit** and **Moq**.

***

## 🎯 Problem Solved: Data Pipeline Integrity

The upstream Python ETL Pipeline generates concession data records (including an LLM-derived *Sentiment Score*). This C# service acts as a **Gatekeeper** to enforce critical quality rules *before* data is stored in the database (PostGIS), preventing downstream corruption.

***

## 🔑 Key Architectural Features (Senior Level)

The project design focuses on **extensibility**, **low coupling**, and **asynchronous scalability**.

### 1. Dependency Injection (DI) and OOP

* **`IValidator<T>` Interface:** Defines the **OOP contract** for any business rule. This allows for the creation of new rules (e.g., `RegionValidator`) without modifying the core orchestrator code.

    

[Image of Dependency Inversion Principle Diagram]


* **`DataValidationService`:** This service implements the **Dependency Injection (DI)** pattern, receiving all rules (`IEnumerable<IValidator<T>>`) in its constructor. This facilitates testing, maintenance, and service configuration in environments like ASP.NET Core.

### 2. Asynchronous Scalability

* **`ValidateAsync` and `Task.WhenAll`:** All validations are executed **asynchronously** and **in parallel**. The orchestrator uses `Task.WhenAll` to wait for all rules to complete, **maximizing throughput** and minimizing process latency.

### 3. Quality and Test Coverage

* **Rigorous Testing:** The project uses **XUnit** for unit testing the logic.
* **Mocks (`Moq`):** The **Moq** framework is used to test the `DataValidationService`, isolating the orchestration service from individual rule logic (clean and fast tests).

***

## 📋 Implemented Quality Contract

The service implements two critical business rules:

| Rule | Field | C# Class | Purpose |
| :--- | :--- | :--- | :--- |
| **Numeric Format** | `CveNumber` | `CveFormatValidator` | Validates the 5-digit numeric format (e.g., `12345`). |
| **Sentiment Range** | `SentimentScore` | `SentimentScoreValidator` | Ensures the LLM-derived score is within the accepted range of **-2.0 to 2.0**. |

***

## 📂 Project Structure

The code is divided into two .NET Core projects:

* **`DataQuality.Core`**: Contains the data model (`ConcessionDataRecord.cs`), the interface (`IValidator.cs`), specific rules, and the orchestrator (`DataValidationService.cs`).
* **`DataQuality.Tests`**: Contains the unit tests for all rules and for the service orchestration.

***

## ▶️ How to Run Unit Tests

To verify that the entire architecture and logic function correctly (100% test coverage), follow these steps:

1.  Clone the repository.
2.  Navigate to the solution root folder (`DataQualityChecker_DotNet`).
3.  Execute the command to compile and run all tests:

```bash
dotnet test

✅ Expected Outcome
A total of 9 successful tests verifying the integrity of the rules and the functionality of the Service Manager.

3. 💾 Git Commands for Uploading the Code
Open your terminal in the project root folder and execute the following sequence of commands to perform the initial commit and push the code to GitHub.

# 1. Initialize Git (if you haven't already)
git init

# 2. Stage all trackable files (the .gitignore file excludes bin/ and obj/)
git add .

# 3. Commit the changes with a descriptive message
git commit -m "feat: Initial implementation of Data Quality Validation Service (C#/.NET Core) with Async DI and Unit Tests"

# 4. Connect your local repository to your remote GitHub repository
git remote add origin <YOUR_GITHUB_REPO_URL>

# 5. Push the code to the main branch
git push -u origin main
# (If your primary branch is named 'master', use 'git push -u origin master' instead)
