using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public abstract class EmergencyUnit
{
    public string Name { get; }
    public int Speed { get; }

    public EmergencyUnit(string name, int speed)
    {
        Name = name;
        Speed = speed;
    }

    public abstract bool CanHandle(string incidentType);
    public abstract void RespondToIncident(Incident incident);
}

public class Police : EmergencyUnit
{
    public Police(string name, int speed) : base(name, speed) { }
    public override bool CanHandle(string incidentType) => incidentType.Equals("Crime", StringComparison.OrdinalIgnoreCase);
    public override void RespondToIncident(Incident incident) => Console.WriteLine($"{Name} responding to a crime at {incident.Location}.");
}

public class Firefighter : EmergencyUnit
{
    public Firefighter(string name, int speed) : base(name, speed) { }
    public override bool CanHandle(string incidentType) => incidentType.Equals("Fire", StringComparison.OrdinalIgnoreCase);
    public override void RespondToIncident(Incident incident) => Console.WriteLine($"{Name} extinguishing the fire at {incident.Location}.");
}

public class Ambulance : EmergencyUnit
{
    public Ambulance(string name, int speed) : base(name, speed) { }
    public override bool CanHandle(string incidentType) => incidentType.Equals("Medical", StringComparison.OrdinalIgnoreCase);
    public override void RespondToIncident(Incident incident) => Console.WriteLine($"{Name} treating patients at {incident.Location}.");
}

public class SearchAndRescue : EmergencyUnit
{
    public SearchAndRescue(string name, int speed) : base(name, speed) { }
    public override bool CanHandle(string incidentType) => incidentType.Equals("Search", StringComparison.OrdinalIgnoreCase) || incidentType.Equals("Rescue", StringComparison.OrdinalIgnoreCase);
    public override void RespondToIncident(Incident incident) => Console.WriteLine($"{Name} is conducting a search and rescue at {incident.Location}.");
}

public class Incident
{
    public string Type { get; }
    public string Location { get; }
    public int Difficulty { get; }

    public Incident(string type, string location, int difficulty)
    {
        Type = type;
        Location = location;
        Difficulty = difficulty;
    }
}

class EmergencySimulation
{
    private static readonly Random random = new Random();
    private static readonly string[] incidentTypes = { "Crime", "Fire", "Medical", "Search", "Rescue" };
    private static readonly string[] locations = { "Downtown", "Residential Area", "Industrial Zone", "Mountain Area", "Lake" };
    private static List<EmergencyUnit> units = new List<EmergencyUnit>();
    private static int score = 0;

    static void Main(string[] args)
    {
        InitializeUnits();
        Console.WriteLine("Emergency Response Simulation");
        Console.WriteLine("----------------------------");

        for (int round = 1; round <= 5; round++)
        {
            Console.WriteLine($"\n--- Round {round} ---");
            Incident currentIncident = GenerateIncident();
            Console.WriteLine($"Incident: {currentIncident.Type} at {currentIncident.Location} (Difficulty: {currentIncident.Difficulty})");

            EmergencyUnit selectedUnit = GetUserSelectedUnit(currentIncident.Type);

            if (selectedUnit != null)
            {
                HandleIncident(currentIncident, selectedUnit);
            }
            else
            {
                Console.WriteLine("No unit selected for this incident.");
            }

            Console.WriteLine($"Current Score: {score}");
        }

        Console.WriteLine("\n--- Simulation Ended ---");
        Console.WriteLine($"Final Score: {score}");
    }

    static void InitializeUnits()
    {
        units.Add(new Police("Police Unit 1", 80));
        units.Add(new Police("Police Unit 2", 75));
        units.Add(new Firefighter("Firefighter Unit 1", 60));
        units.Add(new Firefighter("Firefighter Unit 2", 65));
        units.Add(new Ambulance("Ambulance Unit 1", 90));
        units.Add(new Ambulance("Ambulance Unit 2", 85));
        units.Add(new SearchAndRescue("SAR Unit 1", 70));
    }

    static Incident GenerateIncident()
    {
        string type = incidentTypes[random.Next(incidentTypes.Length)];
        string location = locations[random.Next(locations.Length)];
        int difficulty = random.Next(1, 4);
        return new Incident(type, location, difficulty);
    }

    static EmergencyUnit GetUserSelectedUnit(string incidentType)
    {
        Console.WriteLine("\nAvailable Units:");
        for (int i = 0; i < units.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {units[i].Name} ({units[i].GetType().Name}, Speed: {units[i].Speed}) - Can Handle: {(units[i].CanHandle(incidentType) ? "Yes" : "No")}");
        }

        Console.Write("Choose a unit by number (or 0 to skip): ");
        if (int.TryParse(Console.ReadLine(), out int choice))
        {
            if (choice >= 1 && choice <= units.Count)
            {
                return units[choice - 1];
            }
            else if (choice == 0)
            {
                return null;
            }
        }
        Console.WriteLine("Invalid input.");
        return null;
    }

    static void HandleIncident(Incident incident, EmergencyUnit unit)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        if (unit.CanHandle(incident.Type))
        {
            unit.RespondToIncident(incident);
            stopwatch.Stop();
            long responseTime = stopwatch.ElapsedMilliseconds;
            int pointsEarned = CalculateScore(incident.Difficulty, unit.Speed, responseTime);
            score += pointsEarned;
            Console.WriteLine($"Response Time: {responseTime}ms. Earned +{pointsEarned} points.");
        }
        else
        {
            Console.WriteLine($"The {unit.Name} cannot handle this type of incident.");
            score -= 5;
            Console.WriteLine("-5 points.");
        }
    }

    static int CalculateScore(int difficulty, int unitSpeed, long responseTime)
    {
        int basePoints = difficulty * 10;
        int speedBonus = unitSpeed / 10;
        int timePenalty = (int)(responseTime / 200);
        timePenalty = Math.Min(timePenalty, basePoints + speedBonus);
        int finalScore = basePoints + speedBonus - timePenalty;
        return Math.Max(0, finalScore);
    }
}
