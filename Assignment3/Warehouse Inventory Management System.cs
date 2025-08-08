using System;
using System.Collections.Generic;

namespace Assignment3
{
    // a. Marker interface
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // b. ElectronicItem
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString() =>
            $"[Electronic] ID: {Id}, Name: {Name}, Brand: {Brand}, Warranty: {WarrantyMonths} months, Quantity: {Quantity}";
    }

    // c. GroceryItem
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString() =>
            $"[Grocery] ID: {Id}, Name: {Name}, Expiry: {ExpiryDate:dd/MM/yyyy}, Quantity: {Quantity}";
    }

    // e. Custom Exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // d. Generic Inventory Repository
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
        }

        public List<T> GetAllItems() => new(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");
            var item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    // f. WareHouseManager
    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            try
            {
                _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
                _electronics.AddItem(new ElectronicItem(2, "Smartphone", 20, "Samsung", 12));
                _electronics.AddItem(new ElectronicItem(3, "Headphones", 15, "Sony", 18));

                _groceries.AddItem(new GroceryItem(1, "Milk", 30, DateTime.Now.AddDays(7)));
                _groceries.AddItem(new GroceryItem(2, "Bread", 25, DateTime.Now.AddDays(3)));
                _groceries.AddItem(new GroceryItem(3, "Eggs", 50, DateTime.Now.AddDays(10)));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SeedData Error] {ex.Message}");
            }
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine(item);
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Stock increased for item ID {id}. New quantity: {item.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IncreaseStock Error] {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Item with ID {id} removed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RemoveItem Error] {ex.Message}");
            }
        }

        public InventoryRepository<ElectronicItem> Electronics => _electronics;
        public InventoryRepository<GroceryItem> Groceries => _groceries;
    }

    // Main
    internal class Warehouse_Inventory_Management_System
    {
        static void Main(string[] args)
        {
            var manager = new WareHouseManager();

            // i. Seed data
            manager.SeedData();

            // ii. Print all grocery items
            Console.WriteLine("Grocery Items:");
            manager.PrintAllItems(manager.Groceries);

            // iii. Print all electronic items
            Console.WriteLine("\nElectronic Items:");
            manager.PrintAllItems(manager.Electronics);

            // iv. Try to add a duplicate item
            Console.WriteLine("\nAttempting to add duplicate electronic item:");
            try
            {
                manager.Electronics.AddItem(new ElectronicItem(1, "Tablet", 5, "Apple", 12));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"[DuplicateItemException] {ex.Message}");
            }

            // v. Try to remove a non-existent item
            Console.WriteLine("\nAttempting to remove non-existent grocery item:");
            manager.RemoveItemById(manager.Groceries, 99);

            // vi. Try to update with invalid quantity
            Console.WriteLine("\nAttempting to update electronic item with invalid quantity:");
            try
            {
                manager.Electronics.UpdateQuantity(2, -5);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"[InvalidQuantityException] {ex.Message}");
            }
        }
    }
}
