using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment3
{
    // a. Generic Repository
    public class Repository<T>
    {
        private readonly List<T> items = new();

        public void Add(T item) => items.Add(item);

        public List<T> GetAll() => new(items);

        public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);

        public bool Remove(Func<T, bool> predicate)
        {
            var item = items.FirstOrDefault(predicate);
            if (item != null)
            {
                items.Remove(item);
                return true;
            }
            return false;
        }
    }

    // b. Patient
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString() => $"ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
    }

    // c. Prescription
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString() =>
            $"Prescription ID: {Id}, Medication: {MedicationName}, Date: {DateIssued:dd/MM/yyyy}";
    }

    // g. HealthSystemApp
    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new();

        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "Alice Smith", 30, "Female"));
            _patientRepo.Add(new Patient(2, "Bob Johnson", 45, "Male"));
            _patientRepo.Add(new Patient(3, "Carol Lee", 28, "Female"));

            _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Now.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(2, 1, "Ibuprofen", DateTime.Now.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(3, 2, "Metformin", DateTime.Now.AddDays(-7)));
            _prescriptionRepo.Add(new Prescription(4, 3, "Lisinopril", DateTime.Now.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(5, 2, "Atorvastatin", DateTime.Now.AddDays(-1)));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap = _prescriptionRepo
                .GetAll()
                .GroupBy(p => p.PatientId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId) =>
            _prescriptionMap.TryGetValue(patientId, out var prescriptions)
                ? prescriptions
                : new List<Prescription>();

        public Patient? GetPatientById(int id)
        {
            return _patientRepo.GetById(p => p.Id == id);
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("\n=== All Patients ===");
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine(patient);
            }
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            var patient = GetPatientById(id);
            if (patient == null)
            {
                Console.WriteLine("Patient not found.");
                return;
            }

            Console.WriteLine($"\n=== Prescriptions for {patient.Name} ===");
            var prescriptions = GetPrescriptionsByPatientId(id);
            if (prescriptions.Count == 0)
            {
                Console.WriteLine("No prescriptions found.");
            }
            else
            {
                foreach (var prescription in prescriptions)
                {
                    Console.WriteLine(prescription);
                }
            }
        }
    }

    internal class Health_Management_System
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Health Management System!");

            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();

            app.PrintAllPatients();

            int patientId;
            while (true)
            {
                Console.Write("\nEnter a Patient ID to view prescriptions: ");

                if (!int.TryParse(Console.ReadLine(), out patientId))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                if (app.GetPatientById(patientId) == null)
                {
                    Console.WriteLine("Patient not found. Please try again.");
                    continue;
                }

                app.PrintPrescriptionsForPatient(patientId);
                break; // Exit loop after showing prescriptions
            }
        }
    }
}
