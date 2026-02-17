This solution is a .NET-based data processing engine designed to synchronise anonymised data from the Join Dementia Research (JDR) database.

The primary goal is to create a staging version of JDR data in a secure environment. This staged data is then ready to be mapped to the **OMOP Common Data Model (CDM)** using tools such as **Carrot CDM**. Ultimately, this data is intended to be exposed to the **HDRUK** gateway via the **Bunny/Hutch** infrastructure (see [Hutch](https://github.com/Health-Informatics-UoN/hutch)).

---

### Solution Overview

The application is built using a clean, layered architecture to ensure maintainability and scalability. It is designed to run as a containerised job (AWS Fargate) either manually or on a fixed schedule.

#### Application Layers

*   **Nihr.Jdr.Dta.Job**: 
    The entry point of the application. It orchestrates the flow of data and manages configuration, logging, and dependency injection. Its main responsibility is to trigger the synchronisation process and manage the lifecycle of the job.
*   **Nihr.Jdr.Dta.Domain**: 
    Contains the core domain models and interfaces of the application. This includes the internal data models (such as `Person`, `Diagnosis`, and `Appointment`) and the interfaces that define functionality.
*   **Nihr.Jdr.Dta.Infrastructure**: 
    The layer that implements our domain interfaces and communicates with external systems. It contains the actual database logic (MySQL for reading, SQL Server for writing), repository implementations, and integration with secret management services for secure credential retrieval.

---

### How Data Synchronisation Works

The application performs an "Extract, Transform, and Load" (ETL) process using a streaming pattern to move data efficiently between systems.

#### 1. Data Retrieval
The process begins by connecting to the external JDR MySQL database. To handle large volumes of data, the application uses a streaming approach. Instead of fetching all records at once, it pulls them from the source one by one. By using a sequential ordering method (sorting by ID), the database can provide a continuous stream of data without the overhead of traditional page-skipping techniques.

#### 2. Transformation
As each record is received, the infrastructure layer transforms the JDR-specific data into our internal domain models as required by the HDRUK team.

#### 3. Batch Writing
To ensure the process is efficient, the transformed records are grouped into batches (for example, 500 at a time) before being written to the SQL Server staging database. For every record in a batch, the application checks whether it already exists in the destination. If the record is found, it is updated; otherwise, a new entry is created. After each batch is successfully saved, the application clears its internal tracking state. This ensures that the memory used by the application remains stable.