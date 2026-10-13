using System;
using System.Collections.Generic;

namespace SDG_draft
{
    public class Household
    {
        public string Name { get; } //read-only: 
        private int _waterBalance; 

        public int WaterBalance
        {
            get { return _waterBalance; }
            set
            {
               
                if (value < 0)
                {
                    _waterBalance = 0;
                }
                else
                {
                    _waterBalance = value;
                }
            }
        }
        public string Status => WaterBalance <= 50 ? "!! WATER SUPPLY LOW !!" : "Stable";

        public Household(string name, int initialWater)
        {
            Name = name;
            WaterBalance = initialWater;
        }


        public void DispenseWater(int amount)
        {
            if (amount > WaterBalance)
                Console.WriteLine($"Insufficient water. Only {WaterBalance}L remaining.");

            WaterBalance += amount;
        }

        
    }

    // 2. Dispenser Manager 
    public class WaterDispenserManager
    {
        private Dictionary<string, Household> _households = new Dictionary<string, Household>();
        private Stack<(string HouseholdName, int Amount)> _history = new Stack<(string HouseholdName, int Amount)>();

        public void RegisterHousehold(string name, int initialWater)
        {
            if (_households.ContainsKey(name))
            {
                Console.WriteLine($"'{name}' is already registered.");
                return;
            }

            _households[name] = new Household(name, initialWater);
            Console.WriteLine($"'{name}' registered with {initialWater}L.");
        }

        public void DisplayStatus()
        {
            Console.WriteLine("\n--- Current Household/s Water Supply ---");
            if (_households.Count == 0)
            {
                Console.WriteLine("(No households registered yet)");
                return;
            }

            foreach (var household in _households.Values)
            {
                
                Console.WriteLine($"- {household.Name}: {household.WaterBalance} Liters remaining ({household.Status})");
            }
            Console.WriteLine("------------------------------------------\n");
        }

        public void Dispense(string name, int liters)
        {
            if (!_households.TryGetValue(name, out var household))
            {
                Console.WriteLine("Household not found.");
                return;
            }

            try
            {
                household.DispenseWater(liters);
                _history.Push((name, liters)); 
                Console.WriteLine($"-> Success: Handed out {liters}L to {household.Name}. Remaining balance: {household.WaterBalance}L");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void UndoLastDispensation()
        {
            if (_history.Count == 0)
            {
                Console.WriteLine("Nothing to undo.");
                return;
            }

            var (name, amount) = _history.Pop();
            if (_households.TryGetValue(name, out var household))
            {
                household.DispenseWater(amount);
                Console.WriteLine($"Undo: Restored {amount}L to {household.Name}. New balance: {household.WaterBalance}L");
            }
        }
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            var manager = new WaterDispenserManager();

            manager.RegisterHousehold("Guevarra", 150);
            manager.RegisterHousehold("Reniva", 45);

            Console.WriteLine("=================================================");
            Console.WriteLine("                  O Mizu Dispenser ");
            Console.WriteLine("=================================================");

            bool running = true;
            while (running)
            {
                manager.DisplayStatus();

                Console.WriteLine("--- MENU OPTIONS ---");
                Console.WriteLine("1. Add a New Household");
                Console.WriteLine("2. Dispense Water");
                Console.WriteLine("3. Undo Last Dispensation");
                Console.WriteLine("4. Exit Application");
                Console.Write("Select an action (1-4): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        Console.Write("\nEnter Household Name: ");
                        string newHouse = Console.ReadLine();
                        Console.Write("Enter amount of allocated water (Liters): ");
                        if (int.TryParse(Console.ReadLine(), out int rations))
                        {
                            manager.RegisterHousehold(newHouse, rations);
                        }
                        else
                        {
                            Console.WriteLine("Invalid amount format.");
                        }
                        break;

                    case "2":
                        Console.Write("\nEnter Household Name to serve: ");
                        string serveId = Console.ReadLine();
                        Console.Write("Enter liters of water to dispense: ");
                        if (int.TryParse(Console.ReadLine(), out int liters))
                        {
                            manager.Dispense(serveId, liters);
                        }
                        else
                        {
                            Console.WriteLine("Invalid amount format.");
                        }
                        PauseConsole();
                        break;

                    case "3":
                        Console.Clear();
                        manager.UndoLastDispensation();
                        PauseConsole();
                        break;

                    case "4":
                        running = false;
                        Console.WriteLine("\nThank you for ensuring clean water safety. Exiting application...");
                        break;

                    default:
                        Console.Clear();
                        Console.WriteLine("-> Invalid option. Please select between 1 and 4.");
                        break;
                }
            }
        }

        private static void PauseConsole()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}