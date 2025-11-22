using DotNet.Testcontainers.Containers;
using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Infrastructure.Employees;
using LocadoraDeAutomoveis.Infrastructure.Groups;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Testcontainers.MsSql;

namespace LocadoraDeAutomoveis.Tests.Integration.Shared;

[TestClass]
public abstract class TestFixture
{
    protected TestAppDbContext dbContext = null!;

    protected UserManager<User> userManager = null!;
    protected EmployeeRepository employeeRepository = null!;
    protected GroupRepository groupRepository = null!;

    private static IDatabaseContainer? dbContainer = null!;

    private static string? applicationConnectionString;

    private const string DatabaseName = "LocadoraDeAutomoveisDbTests";

    [AssemblyInitialize]
    public static async Task Setup(TestContext _)
    {
        dbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithName("locadora-de-automoveis-db-testes")
            .WithPassword("SenhaSuperSecreta1!")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithCleanUp(true)
            .Build();

        await StartDatabaseAsync();
    }

    [AssemblyCleanup]
    public static async Task Teardown() => await ShutdownDatabaseAsync();

    [TestInitialize]
    public virtual void ConfigurarTestes()
    {
        if (dbContainer is null || string.IsNullOrWhiteSpace(applicationConnectionString))
        {
            throw new InvalidOperationException("O banco de dados não foi inicializado corretamente.");
        }

        this.dbContext = TestAppDbContext.CreateDbContext(applicationConnectionString);

        ConfigureTables(this.dbContext);

        this.userManager = CreateUserManager(this.dbContext);
        this.employeeRepository = new(this.dbContext);
        this.groupRepository = new(this.dbContext);

        BuilderSetup.SetCreatePersistenceMethod<User>(u => this.userManager.CreateAsync(u).GetAwaiter().GetResult());

        BuilderSetup.SetCreatePersistenceMethod<Employee>(e => this.employeeRepository.AddAsync(e).GetAwaiter().GetResult());
        BuilderSetup.SetCreatePersistenceMethod<IList<Employee>>(e => this.employeeRepository.AddMultiplyAsync(e).GetAwaiter().GetResult());

        BuilderSetup.SetCreatePersistenceMethod<Group>(g => this.groupRepository.AddAsync(g).GetAwaiter().GetResult());
        BuilderSetup.SetCreatePersistenceMethod<IList<Group>>(g => this.groupRepository.AddMultiplyAsync(g).GetAwaiter().GetResult());

    }

    private static async Task StartDatabaseAsync()
    {
        if (dbContainer is null)
        {
            return;
        }

        await dbContainer.StartAsync();

        SqlConnectionStringBuilder masterConnectionStringBuilder =
           new(dbContainer.GetConnectionString())
           {
               InitialCatalog = "master"
           };

        await using (SqlConnection masterConnection =
                     new(masterConnectionStringBuilder.ConnectionString))
        {
            await masterConnection.OpenAsync();

            await using SqlCommand createDbCommand = masterConnection.CreateCommand();
            createDbCommand.CommandText =
                $"""
                IF DB_ID(N'{DatabaseName}') IS NULL
                BEGIN
                    CREATE DATABASE [{DatabaseName}];
                END
                """;

            await createDbCommand.ExecuteNonQueryAsync();
        }

        SqlConnectionStringBuilder appConnectionStringBuilder =
            new(dbContainer.GetConnectionString())
            {
                InitialCatalog = DatabaseName
            };

        applicationConnectionString = appConnectionStringBuilder.ConnectionString;
    }

    private static async Task ShutdownDatabaseAsync()
    {
        if (dbContainer is null)
        {
            return;
        }

        await dbContainer.StopAsync();
        await dbContainer.DisposeAsync();
        dbContainer = null;
        applicationConnectionString = null;
    }

    private static void ConfigureTables(TestAppDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();

        dbContext.Roles.RemoveRange(dbContext.Roles);
        dbContext.Users.RemoveRange(dbContext.Users);

        dbContext.TestEntities.RemoveRange(dbContext.TestEntities);
        dbContext.Employees.RemoveRange(dbContext.Employees);

        dbContext.SaveChanges();
    }

    private static UserManager<User> CreateUserManager(AppDbContext dbContext)
    {
        UserStore<User, Role, AppDbContext, Guid> userStore = new(dbContext);

        List<IUserValidator<User>> userValidators = [];
        List<IPasswordValidator<User>> passwordValidators = [new PasswordValidator<User>()];

        ServiceProvider serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        UserManager<User> userManager = new(
            userStore,
            new OptionsWrapper<IdentityOptions>(new IdentityOptions()),
            new PasswordHasher<User>(),
            userValidators,
            passwordValidators,
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            serviceProvider,
            new Logger<UserManager<User>>(serviceProvider.GetRequiredService<ILoggerFactory>())
        );

        return userManager;
    }
}
