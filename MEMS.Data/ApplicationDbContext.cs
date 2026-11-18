using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MEMS.Data.Models;

namespace MEMS.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<MunicipalUser>(options)
{
    public DbSet<Municipality> Municipalities { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Candidate> Candidates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Enforce unique URL slugs for multi-tenant routing
        modelBuilder.Entity<Municipality>()
            .HasIndex(m => m.Slug)
            .IsUnique();

        // Prevent SQL Server "multiple cascade paths" error on Candidates
        modelBuilder.Entity<Candidate>()
            .HasOne(c => c.Municipality)
            .WithMany(m => m.Candidates)
            .HasForeignKey(c => c.MunicipalityId)
            .OnDelete(DeleteBehavior.Restrict);

        // 1. Seed 3 Municipalities (Burnaby = 1, Surrey = 2, Richmond = 3)
        var electionDate = new DateTime(2026, 10, 17);
        var municipalities = new[]
        {
            new Municipality { MunicipalityId = 1, Name = "Burnaby", Slug = "burnaby", Province = "BC", ElectionDate = electionDate, Description = "City of Burnaby", LogoUrl = "/images/burnaby-logo.png" },
            new Municipality { MunicipalityId = 2, Name = "Surrey", Slug = "surrey", Province = "BC", ElectionDate = electionDate, Description = "The future lives here", LogoUrl = "/images/surrey-logo.png" },
            new Municipality { MunicipalityId = 3, Name = "Richmond", Slug = "richmond", Province = "BC", ElectionDate = electionDate, Description = "Island City, by Nature", LogoUrl = "/images/richmond-logo.png" }
        };
        modelBuilder.Entity<Municipality>().HasData(municipalities);

        // 2. Dynamically generate 3 Positions per Municipality (9 total)
        var positionTemplates = new[]
        {
            (Name: "Mayor", Seats: 1),
            (Name: "Councillor", Seats: 8),
            (Name: "School Trustee", Seats: 6)
        };

        var positions = new List<Position>();
        int posId = 1;
        foreach (var muni in municipalities)
        {
            foreach (var tmpl in positionTemplates)
            {
                positions.Add(new Position
                {
                    PositionId = posId++,
                    Name = tmpl.Name,
                    SeatsAvailable = tmpl.Seats,
                    MunicipalityId = muni.MunicipalityId
                });
            }
        }
        modelBuilder.Entity<Position>().HasData(positions);

        // 3. Dynamically generate 24 Candidates (8 per municipality across Mayor, Councillor, Trustee)
        var candidateData = new (string Name, string Bio, int MuniId, int PosId)[]
        {
            // Burnaby (MunicipalityId = 1; PositionIds: 1=Mayor, 2=Councillor, 3=Trustee)
            ("Elena Rostova", "Urban sustainability advocate focused on Burnaby town centre transit and green infrastructure.", 1, 1),
            ("Kenji Takahashi", "Former municipal budget analyst prioritizing affordable housing and small business support.", 1, 1),
            ("Nadia Al-Mansoor", "Community organizer championing accessible recreation centres and youth mentorship programs.", 1, 2),
            ("Trevor MacLeod", "Civil engineer dedicated to road safety improvements, bike lanes, and park preservation.", 1, 2),
            ("Mei-Ling Zhou", "Local entrepreneur working to streamline permitting and revitalize neighborhood commercial hubs.", 1, 2),
            ("Devon Brooks", "Environmental consultant advocating for urban tree canopy expansion and zero-emission fleets.", 1, 2),
            ("Simran Kaur", "Educator and parent focused on inclusive curriculum resources and seismic school upgrades.", 1, 3),
            ("Patrick O'Sullivan", "Retired school administrator committed to mental health supports and literacy initiatives.", 1, 3),

            // Surrey (MunicipalityId = 2; PositionIds: 4=Mayor, 5=Councillor, 6=Trustee)
            ("Marcus Vance", "Former city planner focused on sustainable transit expansion, community safety, and local business growth.", 2, 4),
            ("Amrit Gill", "Small business owner and civic activist championing affordable housing initiatives and municipal tax relief.", 2, 4),
            ("Linda Nguyen", "Advocating for green urban spaces, improved cycling networks, and revitalization of community recreational centres.", 2, 5),
            ("David Chen", "Focusing on transparent city budgeting, neighborhood watch initiatives, and infrastructure upgrades.", 2, 5),
            ("Gurpreet Sidhu", "Community volunteer working to expand public library hours and family recreation programs.", 2, 5),
            ("Rachel Morales", "Healthcare worker advocating for senior services, accessible sidewalks, and community clinics.", 2, 5),
            ("Priya Sharma", "Parent and STEM educator passionate about reducing classroom sizes and increasing funding for special education.", 2, 6),
            ("Robert O'Connor", "Retired high school principal dedicated to expanding modern library resources and mental health supports for students.", 2, 6),

            // Richmond (MunicipalityId = 3; PositionIds: 7=Mayor, 8=Councillor, 9=Trustee)
            ("Raymond Lau", "Civic leader dedicated to dike infrastructure resilience, agricultural land protection, and transit access.", 3, 7),
            ("Catherine Vance-Boyd", "Former chamber of commerce director focused on coastal flood protection and local job creation.", 3, 7),
            ("Jason Ho", "Traffic systems specialist working to ease bridge congestion and improve pedestrian crosswalks.", 3, 8),
            ("Fatima Zahra", "Social worker advocating for affordable family housing and multilingual community outreach.", 3, 8),
            ("lucas Kowalski", "Small business owner prioritizing waterfront revitalization and transparent city spending.", 3, 8),
            ("Grace Park", "Community arts coordinator supporting cultural festivals, public parks, and youth sports.", 3, 8),
            ("Daniel Wu", "Parent and software engineer championing modern classroom technology and STEM labs.", 3, 9),
            ("Hannah Lindqvist", "Early childhood educator focused on after-school childcare spaces and special needs funding.", 3, 9)
        };

        var candidates = candidateData.Select((c, index) =>
        {
            var slugName = c.Name.ToLower().Replace(" ", "-").Replace("'", "");
            return new Candidate
            {
                CandidateId = index + 1,
                Name = c.Name,
                Biography = c.Bio,
                PhotoUrl = $"/images/candidates/{slugName}.jpg",
                Website = $"https://{slugName.Replace("-", "")}.ca",
                Email = $"contact@{slugName.Replace("-", "")}.ca",
                MunicipalityId = c.MuniId,
                PositionId = c.PosId
            };
        }).ToArray();

        modelBuilder.Entity<Candidate>().HasData(candidates);
    }
}