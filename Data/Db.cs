using KickBlastStableLight.Helpers;
using KickBlastStableLight.Models;
using Microsoft.Data.Sqlite;

namespace KickBlastStableLight.Data;

public static class Db
{
    private static string GetConnectionString()
    {
        var dir = Path.Combine(AppContext.BaseDirectory, "Data");
        Directory.CreateDirectory(dir);
        var dbPath = Path.Combine(dir, "kickblast_stable.db");
        return $"Data Source={dbPath}";
    }

    public static void Init()
    {
        using var conn = new SqliteConnection(GetConnectionString());
        conn.Open();

        var sql = @"
CREATE TABLE IF NOT EXISTS Users(Id INTEGER PRIMARY KEY AUTOINCREMENT, Username TEXT NOT NULL UNIQUE, Password TEXT NOT NULL);
CREATE TABLE IF NOT EXISTS Athletes(Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, Age INTEGER NOT NULL, Plan TEXT NOT NULL, Weight REAL NOT NULL, TargetWeight REAL NOT NULL, Notes TEXT NOT NULL DEFAULT '');
CREATE TABLE IF NOT EXISTS MonthlyCalculations(Id INTEGER PRIMARY KEY AUTOINCREMENT, AthleteId INTEGER NOT NULL, AthleteName TEXT NOT NULL, Plan TEXT NOT NULL, Competitions INTEGER NOT NULL, CoachingHours INTEGER NOT NULL, TrainingCost REAL NOT NULL, CoachingCost REAL NOT NULL, CompetitionCost REAL NOT NULL, Total REAL NOT NULL, WeightStatus TEXT NOT NULL, MonthName TEXT NOT NULL, Year INTEGER NOT NULL, NextCompetitionDate TEXT NOT NULL, CreatedAt TEXT NOT NULL);
CREATE TABLE IF NOT EXISTS Pricing(Id INTEGER PRIMARY KEY, Beginner REAL NOT NULL, Intermediate REAL NOT NULL, Elite REAL NOT NULL, Competition REAL NOT NULL, CoachingRate REAL NOT NULL);
";
        using (var cmd = new SqliteCommand(sql, conn)) cmd.ExecuteNonQuery();

        using (var checkUser = new SqliteCommand("SELECT COUNT(*) FROM Users", conn))
        {
            var count = Convert.ToInt32(checkUser.ExecuteScalar());
            if (count == 0)
            {
                using var ins = new SqliteCommand("INSERT INTO Users(Username,Password) VALUES('dinidu','123456')", conn);
                ins.ExecuteNonQuery();
            }
        }

        using (var checkPricing = new SqliteCommand("SELECT COUNT(*) FROM Pricing WHERE Id=1", conn))
        {
            var count = Convert.ToInt32(checkPricing.ExecuteScalar());
            if (count == 0)
            {
                using var ins = new SqliteCommand("INSERT INTO Pricing(Id,Beginner,Intermediate,Elite,Competition,CoachingRate) VALUES(1,2400,3400,5000,1700,1300)", conn);
                ins.ExecuteNonQuery();
            }
        }

        using var checkAth = new SqliteCommand("SELECT COUNT(*) FROM Athletes", conn);
        if (Convert.ToInt32(checkAth.ExecuteScalar()) == 0)
        {
            var samples = new[]
            {
                new Athlete { Name = "Kamal", Age = 16, Plan = "Beginner", Weight = 56, TargetWeight = 55, Notes = "school" },
                new Athlete { Name = "Nimal", Age = 18, Plan = "Intermediate", Weight = 62, TargetWeight = 60, Notes = "regional" },
                new Athlete { Name = "Kasun", Age = 19, Plan = "Elite", Weight = 68, TargetWeight = 67, Notes = "state" },
                new Athlete { Name = "Tharindu", Age = 17, Plan = "Intermediate", Weight = 58, TargetWeight = 58, Notes = "focus cardio" },
                new Athlete { Name = "Ravindu", Age = 20, Plan = "Elite", Weight = 71, TargetWeight = 70, Notes = "pro" },
                new Athlete { Name = "Sahan", Age = 15, Plan = "Beginner", Weight = 50, TargetWeight = 50, Notes = "new" }
            };
            foreach (var a in samples) UpsertAthlete(a);
        }
    }

    public static bool ValidateLogin(string user, string pass)
    {
        using var conn = new SqliteConnection(GetConnectionString());
        conn.Open();
        using var cmd = new SqliteCommand("SELECT COUNT(*) FROM Users WHERE Username=@u AND Password=@p", conn);
        cmd.Parameters.AddWithValue("@u", user);
        cmd.Parameters.AddWithValue("@p", pass);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public static Pricing GetPricing()
    {
        using var conn = new SqliteConnection(GetConnectionString());
        conn.Open();
        using var cmd = new SqliteCommand("SELECT Id,Beginner,Intermediate,Elite,Competition,CoachingRate FROM Pricing WHERE Id=1", conn);
        using var r = cmd.ExecuteReader();
        if (r.Read())
        {
            return new Pricing
            {
                Id = r.GetInt32(0),
                Beginner = r.GetDecimal(1),
                Intermediate = r.GetDecimal(2),
                Elite = r.GetDecimal(3),
                Competition = r.GetDecimal(4),
                CoachingRate = r.GetDecimal(5)
            };
        }

        var p = new Pricing();
        SavePricing(p);
        return p;
    }

    public static void SavePricing(Pricing p)
    {
        using var conn = new SqliteConnection(GetConnectionString());
        conn.Open();
        using var cmd = new SqliteCommand(@"INSERT INTO Pricing(Id,Beginner,Intermediate,Elite,Competition,CoachingRate)
VALUES(1,@b,@i,@e,@c,@r)
ON CONFLICT(Id) DO UPDATE SET Beginner=@b, Intermediate=@i, Elite=@e, Competition=@c, CoachingRate=@r", conn);
        cmd.Parameters.AddWithValue("@b", p.Beginner);
        cmd.Parameters.AddWithValue("@i", p.Intermediate);
        cmd.Parameters.AddWithValue("@e", p.Elite);
        cmd.Parameters.AddWithValue("@c", p.Competition);
        cmd.Parameters.AddWithValue("@r", p.CoachingRate);
        cmd.ExecuteNonQuery();
    }

    public static List<Athlete> GetAthletes(string search, string plan)
    {
        var list = new List<Athlete>();
        using var conn = new SqliteConnection(GetConnectionString());
        conn.Open();

        var sql = "SELECT Id,Name,Age,Plan,Weight,TargetWeight,Notes FROM Athletes WHERE 1=1";
        if (!string.IsNullOrWhiteSpace(search)) sql += " AND Name LIKE @s";
        if (!string.IsNullOrWhiteSpace(plan) && plan != "All") sql += " AND Plan=@p";
        sql += " ORDER BY Name";

        using var cmd = new SqliteCommand(sql, conn);
        if (!string.IsNullOrWhiteSpace(search)) cmd.Parameters.AddWithValue("@s", $"%{search}%");
        if (!string.IsNullOrWhiteSpace(plan) && plan != "All") cmd.Parameters.AddWithValue("@p", plan);

        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Athlete
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1),
                Age = r.GetInt32(2),
                Plan = r.GetString(3),
                Weight = r.GetDecimal(4),
                TargetWeight = r.GetDecimal(5),
                Notes = r.GetString(6)
            });
        }
        return list;
    }

    public static void UpsertAthlete(Athlete a)
    {
        using var conn = new SqliteConnection(GetConnectionString());
        conn.Open();
        SqliteCommand cmd;
        if (a.Id <= 0)
        {
            cmd = new SqliteCommand("INSERT INTO Athletes(Name,Age,Plan,Weight,TargetWeight,Notes) VALUES(@n,@a,@p,@w,@t,@no)", conn);
        }
        else
        {
            cmd = new SqliteCommand("UPDATE Athletes SET Name=@n,Age=@a,Plan=@p,Weight=@w,TargetWeight=@t,Notes=@no WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", a.Id);
        }

        cmd.Parameters.AddWithValue("@n", a.Name);
        cmd.Parameters.AddWithValue("@a", a.Age);
        cmd.Parameters.AddWithValue("@p", a.Plan);
        cmd.Parameters.AddWithValue("@w", a.Weight);
        cmd.Parameters.AddWithValue("@t", a.TargetWeight);
        cmd.Parameters.AddWithValue("@no", a.Notes);
        cmd.ExecuteNonQuery();
    }

    public static void DeleteAthlete(int id)
    {
        using var conn = new SqliteConnection(GetConnectionString());
        conn.Open();
        using var cmd = new SqliteCommand("DELETE FROM Athletes WHERE Id=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public static void SaveCalculation(MonthlyCalculation c)
    {
        using var conn = new SqliteConnection(GetConnectionString());
        conn.Open();
        using var cmd = new SqliteCommand(@"INSERT INTO MonthlyCalculations(AthleteId,AthleteName,Plan,Competitions,CoachingHours,TrainingCost,CoachingCost,CompetitionCost,Total,WeightStatus,MonthName,Year,NextCompetitionDate,CreatedAt)
VALUES(@aid,@an,@p,@co,@ch,@t,@cc,@cmp,@tot,@ws,@m,@y,@nd,@ca)", conn);
        cmd.Parameters.AddWithValue("@aid", c.AthleteId);
        cmd.Parameters.AddWithValue("@an", c.AthleteName);
        cmd.Parameters.AddWithValue("@p", c.Plan);
        cmd.Parameters.AddWithValue("@co", c.Competitions);
        cmd.Parameters.AddWithValue("@ch", c.CoachingHours);
        cmd.Parameters.AddWithValue("@t", c.TrainingCost);
        cmd.Parameters.AddWithValue("@cc", c.CoachingCost);
        cmd.Parameters.AddWithValue("@cmp", c.CompetitionCost);
        cmd.Parameters.AddWithValue("@tot", c.Total);
        cmd.Parameters.AddWithValue("@ws", c.WeightStatus);
        cmd.Parameters.AddWithValue("@m", c.MonthName);
        cmd.Parameters.AddWithValue("@y", c.Year);
        cmd.Parameters.AddWithValue("@nd", c.NextCompetitionDate);
        cmd.Parameters.AddWithValue("@ca", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
    }

    public static List<MonthlyCalculation> GetHistory(string athlete, string month, int year)
    {
        var list = new List<MonthlyCalculation>();
        using var conn = new SqliteConnection(GetConnectionString());
        conn.Open();

        var sql = "SELECT Id,AthleteId,AthleteName,Plan,Competitions,CoachingHours,TrainingCost,CoachingCost,CompetitionCost,Total,WeightStatus,MonthName,Year,NextCompetitionDate,CreatedAt FROM MonthlyCalculations WHERE 1=1";
        if (!string.IsNullOrWhiteSpace(athlete) && athlete != "All") sql += " AND AthleteName=@a";
        if (!string.IsNullOrWhiteSpace(month) && month != "All") sql += " AND MonthName=@m";
        if (year > 0) sql += " AND Year=@y";
        sql += " ORDER BY Id DESC";

        using var cmd = new SqliteCommand(sql, conn);
        if (!string.IsNullOrWhiteSpace(athlete) && athlete != "All") cmd.Parameters.AddWithValue("@a", athlete);
        if (!string.IsNullOrWhiteSpace(month) && month != "All") cmd.Parameters.AddWithValue("@m", month);
        if (year > 0) cmd.Parameters.AddWithValue("@y", year);

        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(ReadCalculation(r));
        }

        return list;
    }

    public static Dictionary<string, string> GetDashboardStats()
    {
        var map = new Dictionary<string, string>
        {
            ["Athletes"] = "0",
            ["Calculations"] = "0",
            ["Revenue"] = CurrencyHelper.ToLkr(0),
            ["NextCompetition"] = DateHelper.GetSecondSaturday(DateTime.Now).ToString("dd MMM yyyy")
        };

        using var conn = new SqliteConnection(GetConnectionString());
        conn.Open();

        using var c1 = new SqliteCommand("SELECT COUNT(*) FROM Athletes", conn);
        map["Athletes"] = Convert.ToInt32(c1.ExecuteScalar()).ToString();

        using var c2 = new SqliteCommand("SELECT COUNT(*) FROM MonthlyCalculations", conn);
        map["Calculations"] = Convert.ToInt32(c2.ExecuteScalar()).ToString();

        using var c3 = new SqliteCommand("SELECT IFNULL(SUM(Total),0) FROM MonthlyCalculations", conn);
        var rev = Convert.ToDecimal(c3.ExecuteScalar());
        map["Revenue"] = CurrencyHelper.ToLkr(rev);

        return map;
    }

    public static List<MonthlyCalculation> GetRecentCalculations(int limit = 5)
    {
        var list = new List<MonthlyCalculation>();
        using var conn = new SqliteConnection(GetConnectionString());
        conn.Open();
        using var cmd = new SqliteCommand("SELECT Id,AthleteId,AthleteName,Plan,Competitions,CoachingHours,TrainingCost,CoachingCost,CompetitionCost,Total,WeightStatus,MonthName,Year,NextCompetitionDate,CreatedAt FROM MonthlyCalculations ORDER BY Id DESC LIMIT @l", conn);
        cmd.Parameters.AddWithValue("@l", limit);
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(ReadCalculation(r));
        return list;
    }

    private static MonthlyCalculation ReadCalculation(SqliteDataReader r)
    {
        return new MonthlyCalculation
        {
            Id = r.GetInt32(0),
            AthleteId = r.GetInt32(1),
            AthleteName = r.GetString(2),
            Plan = r.GetString(3),
            Competitions = r.GetInt32(4),
            CoachingHours = r.GetInt32(5),
            TrainingCost = r.GetDecimal(6),
            CoachingCost = r.GetDecimal(7),
            CompetitionCost = r.GetDecimal(8),
            Total = r.GetDecimal(9),
            WeightStatus = r.GetString(10),
            MonthName = r.GetString(11),
            Year = r.GetInt32(12),
            NextCompetitionDate = r.GetString(13),
            CreatedAt = r.GetString(14)
        };
    }
}
