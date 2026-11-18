using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MEMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Municipalities",
                columns: table => new
                {
                    MunicipalityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Province = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ElectionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipalities", x => x.MunicipalityId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MunicipalityId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Municipalities_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalTable: "Municipalities",
                        principalColumn: "MunicipalityId");
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    PositionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SeatsAvailable = table.Column<int>(type: "int", nullable: false),
                    MunicipalityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.PositionId);
                    table.ForeignKey(
                        name: "FK_Positions_Municipalities_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalTable: "Municipalities",
                        principalColumn: "MunicipalityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candidates",
                columns: table => new
                {
                    CandidateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Biography = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    MunicipalityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidates", x => x.CandidateId);
                    table.ForeignKey(
                        name: "FK_Candidates_Municipalities_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalTable: "Municipalities",
                        principalColumn: "MunicipalityId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Candidates_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Municipalities",
                columns: new[] { "MunicipalityId", "Description", "ElectionDate", "LogoUrl", "Name", "Province", "Slug" },
                values: new object[,]
                {
                    { 1, "City of Burnaby", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "/images/burnaby-logo.png", "Burnaby", "BC", "burnaby" },
                    { 2, "The future lives here", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "/images/surrey-logo.png", "Surrey", "BC", "surrey" },
                    { 3, "Island City, by Nature", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "/images/richmond-logo.png", "Richmond", "BC", "richmond" }
                });

            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "PositionId", "MunicipalityId", "Name", "SeatsAvailable" },
                values: new object[,]
                {
                    { 1, 1, "Mayor", 1 },
                    { 2, 1, "Councillor", 8 },
                    { 3, 1, "School Trustee", 6 },
                    { 4, 2, "Mayor", 1 },
                    { 5, 2, "Councillor", 8 },
                    { 6, 2, "School Trustee", 6 },
                    { 7, 3, "Mayor", 1 },
                    { 8, 3, "Councillor", 8 },
                    { 9, 3, "School Trustee", 6 }
                });

            migrationBuilder.InsertData(
                table: "Candidates",
                columns: new[] { "CandidateId", "Biography", "Email", "MunicipalityId", "Name", "PhotoUrl", "PositionId", "Website" },
                values: new object[,]
                {
                    { 1, "Urban sustainability advocate focused on Burnaby town centre transit and green infrastructure.", "contact@elenarostova.ca", 1, "Elena Rostova", "/images/candidates/elena-rostova.jpg", 1, "https://elenarostova.ca" },
                    { 2, "Former municipal budget analyst prioritizing affordable housing and small business support.", "contact@kenjitakahashi.ca", 1, "Kenji Takahashi", "/images/candidates/kenji-takahashi.jpg", 1, "https://kenjitakahashi.ca" },
                    { 3, "Community organizer championing accessible recreation centres and youth mentorship programs.", "contact@nadiaalmansoor.ca", 1, "Nadia Al-Mansoor", "/images/candidates/nadia-al-mansoor.jpg", 2, "https://nadiaalmansoor.ca" },
                    { 4, "Civil engineer dedicated to road safety improvements, bike lanes, and park preservation.", "contact@trevormacleod.ca", 1, "Trevor MacLeod", "/images/candidates/trevor-macleod.jpg", 2, "https://trevormacleod.ca" },
                    { 5, "Local entrepreneur working to streamline permitting and revitalize neighborhood commercial hubs.", "contact@meilingzhou.ca", 1, "Mei-Ling Zhou", "/images/candidates/mei-ling-zhou.jpg", 2, "https://meilingzhou.ca" },
                    { 6, "Environmental consultant advocating for urban tree canopy expansion and zero-emission fleets.", "contact@devonbrooks.ca", 1, "Devon Brooks", "/images/candidates/devon-brooks.jpg", 2, "https://devonbrooks.ca" },
                    { 7, "Educator and parent focused on inclusive curriculum resources and seismic school upgrades.", "contact@simrankaur.ca", 1, "Simran Kaur", "/images/candidates/simran-kaur.jpg", 3, "https://simrankaur.ca" },
                    { 8, "Retired school administrator committed to mental health supports and literacy initiatives.", "contact@patrickosullivan.ca", 1, "Patrick O'Sullivan", "/images/candidates/patrick-osullivan.jpg", 3, "https://patrickosullivan.ca" },
                    { 9, "Former city planner focused on sustainable transit expansion, community safety, and local business growth.", "contact@marcusvance.ca", 2, "Marcus Vance", "/images/candidates/marcus-vance.jpg", 4, "https://marcusvance.ca" },
                    { 10, "Small business owner and civic activist championing affordable housing initiatives and municipal tax relief.", "contact@amritgill.ca", 2, "Amrit Gill", "/images/candidates/amrit-gill.jpg", 4, "https://amritgill.ca" },
                    { 11, "Advocating for green urban spaces, improved cycling networks, and revitalization of community recreational centres.", "contact@lindanguyen.ca", 2, "Linda Nguyen", "/images/candidates/linda-nguyen.jpg", 5, "https://lindanguyen.ca" },
                    { 12, "Focusing on transparent city budgeting, neighborhood watch initiatives, and infrastructure upgrades.", "contact@davidchen.ca", 2, "David Chen", "/images/candidates/david-chen.jpg", 5, "https://davidchen.ca" },
                    { 13, "Community volunteer working to expand public library hours and family recreation programs.", "contact@gurpreetsidhu.ca", 2, "Gurpreet Sidhu", "/images/candidates/gurpreet-sidhu.jpg", 5, "https://gurpreetsidhu.ca" },
                    { 14, "Healthcare worker advocating for senior services, accessible sidewalks, and community clinics.", "contact@rachelmorales.ca", 2, "Rachel Morales", "/images/candidates/rachel-morales.jpg", 5, "https://rachelmorales.ca" },
                    { 15, "Parent and STEM educator passionate about reducing classroom sizes and increasing funding for special education.", "contact@priyasharma.ca", 2, "Priya Sharma", "/images/candidates/priya-sharma.jpg", 6, "https://priyasharma.ca" },
                    { 16, "Retired high school principal dedicated to expanding modern library resources and mental health supports for students.", "contact@robertoconnor.ca", 2, "Robert O'Connor", "/images/candidates/robert-oconnor.jpg", 6, "https://robertoconnor.ca" },
                    { 17, "Civic leader dedicated to dike infrastructure resilience, agricultural land protection, and transit access.", "contact@raymondlau.ca", 3, "Raymond Lau", "/images/candidates/raymond-lau.jpg", 7, "https://raymondlau.ca" },
                    { 18, "Former chamber of commerce director focused on coastal flood protection and local job creation.", "contact@catherinevanceboyd.ca", 3, "Catherine Vance-Boyd", "/images/candidates/catherine-vance-boyd.jpg", 7, "https://catherinevanceboyd.ca" },
                    { 19, "Traffic systems specialist working to ease bridge congestion and improve pedestrian crosswalks.", "contact@jasonho.ca", 3, "Jason Ho", "/images/candidates/jason-ho.jpg", 8, "https://jasonho.ca" },
                    { 20, "Social worker advocating for affordable family housing and multilingual community outreach.", "contact@fatimazahra.ca", 3, "Fatima Zahra", "/images/candidates/fatima-zahra.jpg", 8, "https://fatimazahra.ca" },
                    { 21, "Small business owner prioritizing waterfront revitalization and transparent city spending.", "contact@lucaskowalski.ca", 3, "lucas Kowalski", "/images/candidates/lucas-kowalski.jpg", 8, "https://lucaskowalski.ca" },
                    { 22, "Community arts coordinator supporting cultural festivals, public parks, and youth sports.", "contact@gracepark.ca", 3, "Grace Park", "/images/candidates/grace-park.jpg", 8, "https://gracepark.ca" },
                    { 23, "Parent and software engineer championing modern classroom technology and STEM labs.", "contact@danielwu.ca", 3, "Daniel Wu", "/images/candidates/daniel-wu.jpg", 9, "https://danielwu.ca" },
                    { 24, "Early childhood educator focused on after-school childcare spaces and special needs funding.", "contact@hannahlindqvist.ca", 3, "Hannah Lindqvist", "/images/candidates/hannah-lindqvist.jpg", 9, "https://hannahlindqvist.ca" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_MunicipalityId",
                table: "AspNetUsers",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_MunicipalityId",
                table: "Candidates",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_PositionId",
                table: "Candidates",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Municipalities_Slug",
                table: "Municipalities",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_MunicipalityId",
                table: "Positions",
                column: "MunicipalityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Candidates");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Municipalities");
        }
    }
}
