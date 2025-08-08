using System;
using System.Collections.Generic;
using System.Globalization;

namespace Assignment3
{
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category}.");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Mobile Money] Processed {transaction.Amount:C} for {transaction.Category}.");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Crypto Wallet] Processed {transaction.Amount:C} for {transaction.Category}.");
        }
    }

    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction Applied. New Balance: {Balance:C}");
        }
    }

    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
            }
            else
            {
                Balance -= transaction.Amount;
                Console.WriteLine($"Transaction of {transaction.Amount:C} applied. New Balance: {Balance:C}");
            }
        }
    }

    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            var account = new SavingsAccount("SA-001", 1000m);

            var t1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
            var t2 = new Transaction(2, DateTime.Now, 200m, "Utilities");
            var t3 = new Transaction(3, DateTime.Now, 50m, "Entertainment");

            ITransactionProcessor mobile = new MobileMoneyProcessor();
            ITransactionProcessor bank = new BankTransferProcessor();
            ITransactionProcessor crypto = new CryptoWalletProcessor();

            mobile.Process(t1);
            account.ApplyTransaction(t1);

            bank.Process(t2);
            account.ApplyTransaction(t2);

            crypto.Process(t3);
            account.ApplyTransaction(t3);

            
            _transactions.AddRange(new[] { t1, t2, t3 });


            PrintTransactionHistory();
        }

        private void PrintTransactionHistory()
        {
            Console.WriteLine("\n=== Transaction History ===");
            Console.WriteLine($"{"ID",-5} {"Date",-20} {"Amount",-10} {"Category",-15}");
            Console.WriteLine(new string('-', 55));

            foreach (var tx in _transactions)
            {
                Console.WriteLine($"{tx.Id,-5} {tx.Date,-20} {tx.Amount,-10:C} {tx.Category,-15}");
            }
        }
    }

    internal class Finance_Mangement_System
    {
        static void Main(string[] args)
        { 

            CultureInfo customCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            customCulture.NumberFormat.CurrencySymbol = "¢";
            CultureInfo.CurrentCulture = customCulture;


            var app = new FinanceApp();
            app.Run();
        }
    }
}
