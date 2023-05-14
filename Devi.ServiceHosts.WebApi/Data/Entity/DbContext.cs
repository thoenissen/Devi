using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

using Devi.ServiceHosts.WebApi.Data.Entity.Tables.Reminders;

using Microsoft.EntityFrameworkCore;

namespace Devi.ServiceHosts.WebApi.Data.Entity;

/// <summary>
/// DbContext
/// </summary>
public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    /// <summary>
    /// Connection string
    /// </summary>
    private static string _connectionString;

    #region Properties

    /// <summary>
    /// Connection string
    /// </summary>
    public string ConnectionString => _connectionString;

    /// <summary>
    /// Last error
    /// </summary>
    public Exception LastError { get; set; }

    #endregion // Properties

    #region DbContext

    /// <summary>
    /// <para>
    /// Override this method to configure the database (and other options) to be used for this context.
    /// This method is called for each instance of the context that is created.
    /// The base implementation does nothing.
    /// </para>
    /// <para>
    /// In situations where an instance of <see cref="T:Microsoft.EntityFrameworkCore.DbContextOptions"/> may or may not have been passed
    /// to the constructor, you can use <see cref="P:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.IsConfigured"/> to determine if
    /// the options have already been set, and skip some or all of the logic in
    /// <see cref="M:Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)"/>.
    /// </para>
    /// </summary>
    /// <param name="optionsBuilder">
    /// A builder used to create or modify options for this context. Databases (and other extensions)
    /// typically define extension methods on this object that allow you to configure the context.
    /// </param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_connectionString == null)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
                                          {
                                              ApplicationName = "Devi.ServiceHosts.WebApi",
                                              DataSource = Environment.GetEnvironmentVariable("DEVI_DB_DATA_SOURCE"),
                                              InitialCatalog = Environment.GetEnvironmentVariable("DEVI_DB_CATALOG"),
                                              MultipleActiveResultSets = false,
                                              IntegratedSecurity = false,
                                              UserID = Environment.GetEnvironmentVariable("DEVI_DB_USER"),
                                              Password = Environment.GetEnvironmentVariable("DEVI_DB_PASSWORD"),
                                              TrustServerCertificate = true
                                          };
            _connectionString = connectionStringBuilder.ConnectionString;
        }

        optionsBuilder.UseSqlServer(ConnectionString);
#if DEBUG
        optionsBuilder.LogTo(s => Debug.WriteLine(s));
#endif
        base.OnConfiguring(optionsBuilder);
    }

    /// <summary>
    /// Override this method to further configure the model that was discovered by convention from the entity types
    /// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1"/> properties on your derived context. The resulting model may be cached
    /// and re-used for subsequent instances of your derived context.
    /// </summary>
    /// <remarks>
    /// If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)"/>)
    /// then this method will not be run.
    /// </remarks>
    /// <param name="modelBuilder">
    /// The builder being used to construct the model for this context. Databases (and other extensions) typically
    /// define extension methods on this object that allow you to configure aspects of the model that are specific
    /// to a given database.
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OneTimeReminderEntity>();

        // Disabling cascade on delete
        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
                                               .SelectMany(obj => obj.GetForeignKeys())
                                               .Where(obj => obj.IsOwnership == false && obj.DeleteBehavior == DeleteBehavior.Cascade))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }

    #endregion // DbContext
}