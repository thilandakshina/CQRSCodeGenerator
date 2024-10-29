Built a Simple CQRS Code Generator for .NET Projects!
Ever spent hours writing repetitive CQRS code ? I created a small app to automate it and save you the hassle!

✨ How it works: Just provide your entity class (like Customer, Product, or Order), and it automatically generates:

* Commands (Create, Update, Delete)
* Queries (GetById, GetAll)
* DTOs and Validators
* Handlers (ready for implementation)
* Organized folders with everything you need!(you may need to adjust namespaces and place files as needed**)
  
Example Entity Class:

```
public class AppointmentInfo
{
    public Guid AppointmentId { get; set; }
    public string DoctorName { get; set; }
    public string PatientContactNumber { get; set; }
    public string PatientEmail { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string TimeSlot { get; set; }
    public string Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
```

Before:
All CQRS code had to be manually written, leading to repetitive coding and potential errors.

After:
* Complete CQRS structure, with organized commands, queries, DTOs, validators, and handlers
* Built-in validations to ensure data integrity
* Fully generated and ready-to-use commands, queries, and structure

![image](https://github.com/user-attachments/assets/0ad422f1-4970-46f7-a4e1-05abec86cb93)

![image](https://github.com/user-attachments/assets/857a5200-ee55-4d7f-81f3-75e639e13114)

![image](https://github.com/user-attachments/assets/13992fd3-c9df-4027-a920-c2cab2e80d60)

![image](https://github.com/user-attachments/assets/fa1bb14f-9f18-4aff-a85e-4138dc69b2b1)

![image](https://github.com/user-attachments/assets/a3619fe4-ece7-4345-bbdd-d238372a06bc)

![image](https://github.com/user-attachments/assets/906ee8c3-3136-40de-9c40-7477174a6ec0)

using this, you can:

* Saves hours of coding time
* Keeps code consistent across the project
* Reduces copy-paste errors
* Lets you focus on business logic instead of boilerplate code
* Ideal for .NET developers working on microservices, clean architecture, and the CQRS pattern!

#dotnet #csharp #coding #microservices
