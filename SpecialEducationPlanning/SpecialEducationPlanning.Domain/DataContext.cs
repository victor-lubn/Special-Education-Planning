using Koa.Domain;
using Koa.Platform.Providers.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Entity.View;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Extensions;
using SpecialEducationPlanning
.Domain.Model.AzureSearchModel;
using SpecialEducationPlanning
.Domain.Service.Search;
using Action = SpecialEducationPlanning
.Domain.Entity.Action;
using Title = SpecialEducationPlanning
.Domain.Entity.Title;
using Version = SpecialEducationPlanning
.Domain.Entity.Version;

namespace SpecialEducationPlanning
.Domain
{

    public class DataContext : DbContext
    {

        public const int DefaultPropertyLength = 100;

        public const int LongPropertyLength = 500;

        public const int MaxPropertyLength = int.MinValue; //Flag value 

        public const int ShortPropertyLength = 20;

        public const int ShortTypePropertyLength = 10;

        public const int TelephoneNumberPropertyLegth = 30;

        private readonly IIdentityProvider identityProvider;

        private readonly ILogger<DataContext> logger;

        private readonly IAzureSearchManagementService searchManagementService;

        protected DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options, IIdentityProvider identityProvider,
            ILogger<DataContext> logger, IAzureSearchManagementService searchManagementService) : base(options)
        {
            this.identityProvider = identityProvider;
            this.logger = logger;
            this.searchManagementService = searchManagementService;
        }

        #region Properties 

        public bool CurrentUserFullAclAccess => GetCurrentUserFullAclAccess();

        public int CurrentUserId => GetCurrentUserId();

        public bool DisableUpdateAclOnSave { get; set; }

        #endregion

        #region Methods Public

        public override int SaveChanges()
        {
            SaveChangesOverride();

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            SaveChangesOverride();

            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public void Seed()
        {
            StaticData.SeedStaticData(this);
        }

        #endregion

        #region Methods Protected

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Acl
            modelBuilder.Entity<Acl>().Property(x => x.UserId).HasMaxLength(ShortPropertyLength);
            modelBuilder.Entity<Acl>().Property(x => x.EntityType).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Acl>().Property(x => x.EntityType).HasMaxLength(ShortPropertyLength);
            modelBuilder.Entity<Acl>().HasKey(acl => new { acl.UserId, acl.EntityType, acl.EntityId });

            // AuditLog
            modelBuilder.Entity<Action>().Property(x => x.EntityName).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Action>().Property(x => x.User).HasMaxLength(DefaultPropertyLength);

            //Log
            modelBuilder.Entity<Log>().Property(x => x.Level).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Log>().Property(x => x.Level).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Log>().HasIndex(x => x.TimeStamp);

            //Country
            modelBuilder.Entity<Country>().Property(x => x.KeyName).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Country>().HasIndex(country => country.KeyName).IsUnique();

            //Region
            modelBuilder.Entity<Region>().HasIndex(region => region.KeyName).IsUnique();
            modelBuilder.Entity<Region>().Property(x => x.KeyName).HasMaxLength(DefaultPropertyLength);

            //Area
            modelBuilder.Entity<Area>().HasIndex(area => area.KeyName).IsUnique();
            modelBuilder.Entity<Area>().Property(x => x.KeyName).HasMaxLength(DefaultPropertyLength);

            //Aiep
            modelBuilder.Entity<Aiep>().HasIndex(Aiep => Aiep.AiepCode).IsUnique();
            modelBuilder.Entity<Aiep>().Property(x => x.AiepCode).HasMaxLength(ShortPropertyLength);
            modelBuilder.Entity<Aiep>().Property(x => x.Name).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Aiep>().Property(x => x.Email).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Aiep>().Property(x => x.Address1).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Aiep>().Property(x => x.Address2).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Aiep>().Property(x => x.Address3).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Aiep>().Property(x => x.Address4).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Aiep>().Property(x => x.Address5).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Aiep>().Property(x => x.Address6).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Aiep>().Property(x => x.Postcode).HasMaxLength(ShortPropertyLength);
            modelBuilder.Entity<Aiep>().Property(x => x.PhoneNumber).HasMaxLength(TelephoneNumberPropertyLegth);
            modelBuilder.Entity<Aiep>().Property(x => x.FaxNumber).HasMaxLength(TelephoneNumberPropertyLegth);
            modelBuilder.Entity<Aiep>().Property(x => x.IpAddress).HasMaxLength(ShortPropertyLength);
            modelBuilder.Entity<Aiep>().Property(x => x.MediaServer).HasMaxLength(ShortPropertyLength);

            modelBuilder.Entity<Aiep>().HasMany(d => d.BuilderEducationerAieps).WithOne(d => d.Aiep)
                .HasForeignKey(d => d.AiepId).OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Aiep>().HasOne(x => x.Manager).WithMany().HasForeignKey(x => x.ManagerId);

            modelBuilder.Entity<Aiep>().HasOne(x => x.ReleaseInfo).WithMany(x => x.Aieps)
                .HasForeignKey(x => x.ReleaseInfoId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Aiep>()
                .HasMany(d => d.Projects)
                .WithOne(p => p.Aiep)
                .HasForeignKey(p => p.AiepId)
                .OnDelete(DeleteBehavior.SetNull);

            //Project
            modelBuilder.Entity<Project>().Property(x => x.CodeProject).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Project>().Property(x => x.KeyName).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Builder)
                .WithMany(b => b.Projects)
                .HasForeignKey(p => p.BuilderId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Project>()
                .HasMany(p => p.HousingSpecifications)
                .WithOne(b => b.Project)
                .HasForeignKey(b => b.ProjectId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Project>()
                .HasMany(t => t.ProjectTemplates)
                .WithOne(s => s.Project)
                .HasForeignKey(s => s.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            //HousingSpecification
            modelBuilder.Entity<HousingSpecification>().Property(x => x.Code).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<HousingSpecification>().Property(x => x.Name).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<HousingSpecification>()
                .HasMany(h => h.HousingTypes)
                .WithOne(p => p.HousingSpecification)
                .HasForeignKey(p => p.HousingSpecificationId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<HousingSpecification>()
                .HasMany(t => t.HousingSpecificationTemplates)
                .WithOne(s => s.HousingSpecification)
                .HasForeignKey(s => s.HousingSpecificationId)
                .OnDelete(DeleteBehavior.Cascade);

            //HousingType
            modelBuilder.Entity<HousingType>().Property(x => x.Code).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<HousingType>().Property(x => x.Name).HasMaxLength(DefaultPropertyLength);

            modelBuilder.Entity<HousingType>()
                .HasMany(e => e.Plans)
                .WithOne(p => p.HousingType) 
                .HasPrincipalKey(e => new { e.Id, e.HousingSpecificationId })
                .HasForeignKey(p => new { p.HousingTypeId, p.HousingSpecificationId })
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            //HousingSpecificationTemplates

            //ProjectTemplates

            //Plan
            modelBuilder.Entity<Plan>()
                .HasOne(d => d.EndUser)
                .WithMany(b => b.Plans);

            modelBuilder.Entity<Plan>()
                .HasOne(t => t.ProjectTemplates)
                .WithOne(p => p.Plan)
                .HasForeignKey<ProjectTemplates>(t => t.PlanId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Plan>()
                .HasOne(t => t.HousingSpecificationTemplates)
                .WithOne(p => p.Plan)
                .HasForeignKey<HousingSpecificationTemplates>(t => t.PlanId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Plan>()
                .HasMany(p => p.Versions)
                .WithOne(v => v.Plan)
                .HasForeignKey(v => v.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Plan>()
                .HasOne(p => p.MasterVersion)
                .WithOne()
                .HasForeignKey<Plan>(p => p.MasterVersionId);

            modelBuilder.Entity<Plan>()
                .HasOne(p => p.EducationToolOrigin)
                .WithMany()
                .HasForeignKey(p => p.EducationToolOriginId);

            modelBuilder.Entity<Plan>().Property(x => x.Title).HasMaxLength(ShortPropertyLength);
            modelBuilder.Entity<Plan>().Property(x => x.PlanCode).HasMaxLength(ShortPropertyLength);
            modelBuilder.Entity<Plan>().Property(x => x.PlanName).HasMaxLength(DefaultPropertyLength);

            modelBuilder.Entity<Plan>().Property(x => x.BuilderTradingName).HasMaxLength(LongPropertyLength);//.IsRequired();//

            modelBuilder.Entity<Plan>().Property(x => x.PlanType).HasMaxLength(LongPropertyLength);
            modelBuilder.Entity<Plan>().Property(x => x.CadFilePlanId).HasMaxLength(ShortPropertyLength);

            modelBuilder.Entity<Plan>().Navigation(e => e.EducationToolOrigin).AutoInclude();

            modelBuilder.Entity<EducationToolOrigin>().Property(x => x.Name).HasMaxLength(ShortTypePropertyLength);

            //Version
            modelBuilder.Entity<Version>().Property(x => x.KeyName).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Version>().Property(x => x.VersionNotes).HasMaxLength(LongPropertyLength);
            modelBuilder.Entity<Version>().Property(x => x.Range).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Version>().Property(x => x.ExternalCode).HasMaxLength(ShortPropertyLength);
            modelBuilder.Entity<Version>().Property(x => x.QuoteOrderNumber).HasMaxLength(ShortPropertyLength);
            modelBuilder.Entity<Version>().Property(x => x.AiepCode).HasMaxLength(ShortPropertyLength);
            modelBuilder.Entity<Version>().Property(x => x.EducationTool3DCPlanId).HasMaxLength(50);

            modelBuilder.Entity<Version>().Property(x => x.FittersPack3DCJobId).HasMaxLength(50);

            modelBuilder.Entity<Version>()
                .HasOne(p => p.FittersPackStatus)
                .WithMany()
                .HasForeignKey(p => p.FittersPackStatusId);

            modelBuilder.Entity<Version>()
                .HasMany(v => v.RomItems)
                .WithOne(r => r.Version)
                .HasForeignKey(r => r.VersionId)
                .OnDelete(DeleteBehavior.Cascade);
           

            //RomItem
            modelBuilder.Entity<RomItem>().Property(p => p.ItemName).IsRequired();

            modelBuilder.Entity<RomItem>().HasOne(r => r.Version).WithMany(v => v.RomItems)
                .HasForeignKey(r => r.VersionId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RomItem>().Property(p => p.ItemName).IsRequired()
                .HasMaxLength(DefaultPropertyLength);

            modelBuilder.Entity<RomItem>().Property(p => p.SerialNumber).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<RomItem>().Property(p => p.Sku).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<RomItem>().Property(p => p.Range).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<RomItem>().Property(p => p.Colour).HasMaxLength(DefaultPropertyLength);

            //SortingFiltering
            modelBuilder.Entity<SortingFiltering>().Property(x => x.EntityType).HasMaxLength(ShortPropertyLength);
            modelBuilder.Entity<SortingFiltering>().Property(x => x.PropertyName).HasMaxLength(DefaultPropertyLength);

            //Soundtrack
            modelBuilder.Entity<Soundtrack>().HasIndex(sound => sound.Code).IsUnique();

            //User

            modelBuilder.Entity<User>().HasOne(user => user.Aiep).WithMany(d => d.Educationers)
                .HasForeignKey(u => u.AiepId);

            modelBuilder.Entity<User>().HasOne(user => user.CurrentAiep).WithMany()
                .HasForeignKey(u => u.CurrentAiepId);

            modelBuilder.Entity<User>().HasIndex(user => user.UniqueIdentifier).IsUnique();

            modelBuilder.Entity<User>().HasMany(r => r.UserRoles).WithOne(ur => ur.User).HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //Builder

            modelBuilder.Entity<Builder>()
                .HasIndex(builder => new { builder.TradingName, builder.Postcode, builder.Address1}).IsUnique();

            modelBuilder.Entity<Builder>().HasMany(b => b.BuilderEducationerAieps).WithOne(b => b.Builder)
                .HasForeignKey(b => b.BuilderId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Builder>().Property(b => b.TradingName).IsRequired()
                .HasMaxLength(DefaultPropertyLength);

            modelBuilder.Entity<Builder>().Property(b => b.AccountNumber).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Builder>().Property(b => b.Postcode).HasMaxLength(ShortPropertyLength).IsRequired();
            modelBuilder.Entity<Builder>().Property(b => b.Address1).HasMaxLength(DefaultPropertyLength).IsRequired();
            modelBuilder.Entity<Builder>().Property(b => b.MobileNumber).HasMaxLength(TelephoneNumberPropertyLegth);
            modelBuilder.Entity<Builder>().Property(b => b.LandLineNumber).HasMaxLength(TelephoneNumberPropertyLegth);
            modelBuilder.Entity<Builder>().Property(b => b.Address0).HasMaxLength(LongPropertyLength);
            modelBuilder.Entity<Builder>().Property(b => b.Address2).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Builder>().Property(b => b.Address3).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Builder>().Property(b => b.CreationUser).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Builder>().Property(b => b.UpdateUser).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Builder>().Property(b => b.Email).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Builder>().Property(b => b.Notes).HasMaxLength(LongPropertyLength);
            modelBuilder.Entity<Builder>().Property(b => b.BuilderStatus).HasMaxLength(DefaultPropertyLength);

            //BuilderEducationerAiep
            modelBuilder.Entity<BuilderEducationerAiep>().HasIndex(x => new { x.BuilderId, x.AiepId })
                .IsUnique();

            modelBuilder.Entity<BuilderEducationerAiep>().Property(x => x.BuilderId).IsRequired();
            modelBuilder.Entity<BuilderEducationerAiep>().Property(x => x.AiepId).IsRequired();

            //EndUser
            modelBuilder.Entity<EndUser>().Property(x => x.Comment).HasMaxLength(LongPropertyLength);
            modelBuilder.Entity<EndUser>().Property(x => x.FirstName).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<EndUser>().Property(x => x.Surname).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<EndUser>().Property(x => x.Postcode).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<EndUser>().Property(x => x.MobileNumber).HasMaxLength(TelephoneNumberPropertyLegth);
            modelBuilder.Entity<EndUser>().Property(x => x.LandLineNumber).HasMaxLength(TelephoneNumberPropertyLegth);
            modelBuilder.Entity<EndUser>().Property(x => x.Address0).HasMaxLength(LongPropertyLength);
            modelBuilder.Entity<EndUser>().Property(x => x.Address1).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<EndUser>().Property(x => x.Address2).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<EndUser>().Property(x => x.Address3).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<EndUser>().Property(x => x.Address4).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<EndUser>().Property(x => x.Address5).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<EndUser>().Property(x => x.ContactEmail).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<EndUser>().Property(x => x.CreationUser).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<EndUser>().Property(x => x.UpdateUser).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<EndUser>().Property(x => x.FullName).HasMaxLength(LongPropertyLength);

            modelBuilder.Entity<EndUser>()
                .HasIndex(endUser => new { endUser.Surname, endUser.Postcode, endUser.Address1 }).IsUnique();

            //EndUserAiep
            modelBuilder.Entity<EndUserAiep>().HasIndex(x => new { x.AiepId, x.EndUserId }).IsUnique();

            modelBuilder.Entity<EndUserAiep>().HasOne(eud => eud.Aiep).WithMany(d => d.EndUserAieps)
                .HasForeignKey(d => d.AiepId);

            modelBuilder.Entity<EndUserAiep>().HasOne(eud => eud.EndUser).WithMany(eu => eu.EndUserAieps)
                .HasForeignKey(d => d.EndUserId);

            // Common
            modelBuilder.Entity<Comment>().Property(x => x.EntityName).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Comment>().Property(x => x.User).HasMaxLength(DefaultPropertyLength);

            //Catalog
            modelBuilder.Entity<Catalog>().Property(x => x.Name).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Catalog>().Property(x => x.Code).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<Catalog>().Property(c => c.Enabled).IsRequired();
            modelBuilder.Entity<Catalog>().HasIndex(catalog => catalog.Code).IsUnique();
            modelBuilder.Entity<Catalog>().Property(x => x.Range).HasMaxLength(DefaultPropertyLength);

            modelBuilder.Entity<Catalog>().HasMany(c => c.Versions).WithOne(c => c.Catalog)
                .HasForeignKey(c => c.CatalogId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Catalog>().HasMany(c => c.RomItems).WithOne(c => c.Catalog)
                .HasForeignKey(c => c.CatalogId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Catalog>()
                .HasOne(c => c.EducationToolOrigin)
                .WithMany()
                .HasForeignKey(c => c.EducationToolOriginId);

            //ReleaseInfo
            modelBuilder.Entity<ReleaseInfo>().Property(x => x.Title).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<ReleaseInfo>().Property(x => x.Subtitle).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<ReleaseInfo>().Property(x => x.Document).HasMaxLength(DefaultPropertyLength);
            modelBuilder.Entity<ReleaseInfo>().Property(x => x.DocumentPath).HasMaxLength(DefaultPropertyLength);

            //modelBuilder.Entity<ReleaseInfo>().HasIndex(releaseInfo =>
            //    new {releaseInfo.MajorVersion, releaseInfo.PatchVersion, releaseInfo.MinorVersion}).IsUnique();

            //UserReleaseInfo
            modelBuilder.Entity<UserReleaseInfo>().HasOne(ur => ur.User).WithMany(ur => ur.UserReleasesInfo)
                .HasForeignKey(d => d.UserId);

            modelBuilder.Entity<UserReleaseInfo>().HasOne(ur => ur.ReleaseInfo).WithMany(ur => ur.UserReleasesInfo)
                .HasForeignKey(d => d.ReleaseInfoId);

            //Role
            modelBuilder.Entity<Role>().Property(x => x.Name).IsRequired().HasMaxLength(DefaultPropertyLength);

            modelBuilder.Entity<Role>().HasMany(r => r.UserRoles).WithOne(ur => ur.Role).HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Role>().HasMany(r => r.PermissionRoles).WithOne(pr => pr.Role)
                .HasForeignKey(pr => pr.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>().HasIndex(role => role.Name).IsUnique();

            //Permission
            modelBuilder.Entity<Permission>().HasIndex(permission => permission.Name).IsUnique();

            modelBuilder.Entity<Permission>().HasMany(p => p.PermissionRoles).WithOne(pr => pr.Permission)
                .HasForeignKey(pr => pr.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Permission>().Property(x => x.Name).IsRequired().HasMaxLength(DefaultPropertyLength);

            modelBuilder.Entity<Permission>().HasMany(p => p.PermissionRoles).WithOne(pr => pr.Permission)
                .HasForeignKey(pr => pr.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);

            //PermissionRole
            modelBuilder.Entity<PermissionRole>().Property(x => x.PermissionId).IsRequired();
            modelBuilder.Entity<PermissionRole>().Property(x => x.RoleId).IsRequired();
            modelBuilder.Entity<PermissionRole>().HasIndex(x => new { x.PermissionId, x.RoleId }).IsUnique();

            //UserRole
            modelBuilder.Entity<UserRole>().HasIndex(x => new { x.UserId, x.RoleId }).IsUnique();

            //Sequences
            modelBuilder.HasSequence<int>("ExternalIdSequence").StartsAt(1).HasMax(99999999).HasMin(0).IncrementsBy(1)
                .IsCyclic();

            modelBuilder.HasSequence<int>("PlanCodeSequence").StartsAt(1).HasMax(99999).HasMin(0).IncrementsBy(1)
                .IsCyclic();

            // Master Tables
            modelBuilder.Entity<Title>().Property(x => x.TitleName).HasMaxLength(DefaultPropertyLength);

            //Filters
            this.ApplyFilters(modelBuilder);

            //ActionLogs
            modelBuilder.Entity<ActionLogs>().ToView("ActionLogs");

            //PropertyLength
            StringPropertyLength(modelBuilder);
        }

        #endregion

        #region Methods Private

        private IEnumerable<EntityEntry> AuditAndSoftDelete()
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.Entity is IAuditableEntity &&
                            (x.State == EntityState.Added || x.State == EntityState.Modified ||
                             x.State == EntityState.Deleted)).ToList();

            var addedEntries = new List<EntityEntry>();

            try
            {

                foreach (var entry in modifiedEntries)
                {
                    if (EntryAddOrModified(entry, identityProvider.Identity?.Identity))
                    {
                        addedEntries.Add(entry);
                    }

                    EntryDeleted(entry, identityProvider.Identity?.Identity);
                }
            }
            catch
            {
                foreach (var entry in modifiedEntries)
                {

                    if (EntryAddOrModified(entry, null))
                    {
                        addedEntries.Add(entry);
                    }

                    EntryDeleted(entry, null);
                }

            }
            return addedEntries;
        }

        private void AutoActionsCreate(IEnumerable<EntityEntry> addedEntries)
        {
            logger?.LogDebug("DataContext: AutoActionsCreate...");

            foreach (var entry in addedEntries)
            {
                if (entry.Entity is IAuditableEntity && entry.Entity is BaseEntity<int> baseEntityAdded)
                {
                    Set<Action>().Add(new Action
                    {
                        ActionType = ActionType.Create,
                        AdditionalInfo = entry.Entity is Version
                            ? (entry.Entity as Version).VersionNumber.ToString()
                            : string.Empty,
                        Date = DateTime.UtcNow,
                        EntityId = baseEntityAdded.Id,
                        EntityName = baseEntityAdded.GetType().Name,
                        User = identityProvider.Identity?.Identity?.Name
                    });
                }
            }
        }

        private void AutoBuilderAcl(IEnumerable<EntityEntry> modifiedBuilderEducationerAiepEntries)
        {
            logger?.LogDebug("DataContext: AutoBuilderAcl...");

            foreach (var entry in modifiedBuilderEducationerAiepEntries)
            {
                if (!(entry.Entity is BuilderEducationerAiep builderEducationerAiep))
                {
                    continue;
                }

                this.BuilderAiepAcl(builderEducationerAiep.BuilderId, builderEducationerAiep.AiepId);
            }
        }

        private void AutoCalculated()
        {
            logger?.LogDebug("DataContext: AutoCalculated...");

            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => (x.Entity is Builder || x.Entity is EndUser)
                            && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in modifiedEntries)
            {
                if (entry.Entity != null && (EntityState.Added == entry.State || EntityState.Modified == entry.State))
                {
                    if (entry.Entity is Builder builder)
                    {
                        builder.Normalise();
                    }

                    if (entry.Entity is EndUser endUser)
                    {
                        endUser.CalculateProperties();
                    }
                }
            }
        }

        private void AutoEducationerAcl(IEnumerable<EntityEntry> modifiedEducationerEntries)
        {
            logger?.LogDebug("DataContext: AutoEducationerAcl...");

            foreach (var entry in modifiedEducationerEntries)
            {
                if (!(entry.Entity is User Educationer))
                {
                    continue;
                }

               this.EducationerUpdateAcl(Educationer.Id);
            }
        }

        private void AutoEndUserAcl(IEnumerable<EntityEntry> modifiedEndUserAiepEntries)
        {
            foreach (var entry in modifiedEndUserAiepEntries)
            {
                if (!(entry.Entity is EndUserAiep endUserAiep))
                {
                    continue;
                }

                this.EndUserAiepAcl(endUserAiep.EndUserId, endUserAiep.AiepId);
            }
        }
        private void AutoProjectAcl(IEnumerable<System.Action> actions)
        {
            foreach (var action in actions)
            {
                action();
            }
        }

        private List<System.Action> ChangedAiepsProjectIds(List<EntityEntry> modifiedIEntityWithAclEntries)
        {
            var actions = new List<System.Action>();

            foreach (var entry in modifiedIEntityWithAclEntries)
            {
                if (entry.Entity is Project pro)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            actions.Add(() => this.ProjectAddAcl(pro.Id));
                            break;
                        case EntityState.Deleted:
                            break;
                        case EntityState.Modified:
                            var oldAiepId = entry.OriginalValues.GetValue<int>(nameof(Project.AiepId));
                            var newAiepId = entry.CurrentValues.GetValue<int>(nameof(Project.AiepId));
                            if (oldAiepId != newAiepId)
                            {
                                actions.Add(() => this.ProjectUpdateAcl(pro.Id));
                            }
                            break;
                    }

                }
               else if (entry.Entity is Plan plan)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            actions.Add(() => this.PlanAddAcl(plan.Id));
                            break;
                        case EntityState.Deleted:
                            break;
                    }
                }
                else if (entry.Entity is Version ver)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            actions.Add(() => this.VersionAddAcl(ver.Id));
                            break;
                        case EntityState.Deleted:
                            break;
                    }
                }
                else if (entry.Entity is EndUser eu)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            actions.Add(() => this.EndUserAddAcl(eu.Id));
                            break;
                        case EntityState.Deleted:
                            break;
                    }
                }
            }

            return actions;
        }


        private bool EntryAddOrModified(EntityEntry entry, IIdentity identity)
        {
            var now = DateTime.UtcNow;

            if (entry.Entity != null && (EntityState.Added == entry.State || EntityState.Modified == entry.State) &&
                entry.Entity is IAuditableEntity entityToAddModified &&
                entry.Entity is IEntity<int> baseEntityModified)
            {
                if (entityToAddModified.CreatedDate != DateTime.MaxValue)
                {
                    entityToAddModified.UpdatedDate = now;
                    entityToAddModified.UpdateUser = identity?.Name;
                }

                if (EntityState.Added != entry.State)
                {
                    Set<Action>().Add(new Action
                    {
                        ActionType = ActionType.Update,
                        AdditionalInfo = entry.Entity is Version
                            ? (entry.Entity as Version).VersionNumber.ToString()
                            : string.Empty,
                        Date = DateTime.UtcNow,
                        EntityId = baseEntityModified.Id,
                        EntityName = baseEntityModified.GetType().Name,
                        User = identity?.Name
                    });

                    return false;
                }

                entityToAddModified.CreatedDate = now;
                entityToAddModified.CreationUser = identity?.Name;

                return true;
            }

            return false;
        }

        private void EntryDeleted(EntityEntry entry, IIdentity identity)
        {
            if (entry.Entity != null && EntityState.Deleted == entry.State &&
                entry.Entity is ISoftDeleteEntity entityToDelete &&
                entry.Entity is BaseEntity<int> baseEntityDeleted)
            {
                entityToDelete.IsDeleted = true;
                entityToDelete.DeletedUser = identity.Name;

                Set<Action>().Add(new Action
                {
                    ActionType = ActionType.Delete,
                    AdditionalInfo = string.Empty,
                    Date = DateTime.UtcNow,
                    EntityId = baseEntityDeleted.Id,
                    EntityName = baseEntityDeleted.GetType().Name,
                    User = identity.Name
                });

                entry.State = EntityState.Modified;
            }
        }

        private bool GetCurrentUserFullAclAccess()
        {
            var userAppUserFullAclAccessClaim =
                ((ClaimsPrincipal)identityProvider.Identity)?.Claims.FirstOrDefault(c =>
                   c.Type == nameof(AppClaimType.AppUserFullAclAccess));

            return userAppUserFullAclAccessClaim != null && bool.Parse(userAppUserFullAclAccessClaim.Value);
        }

        private int GetCurrentUserId()
        {
            var userAppClaim =
                ((ClaimsPrincipal)identityProvider.Identity)?.Claims.FirstOrDefault(c =>
                   c.Type == nameof(AppClaimType.AppUserIdClaimType));

            if (userAppClaim == null)
            {
                return -1;
            }

            return int.Parse(userAppClaim.Value);
        }

        private void SaveChangesOverride()
        {
            var modifiedIEntityWithAclEntries = new List<EntityEntry>();
            var modifiedBuilderEducationerAiepEntries = new List<EntityEntry>();
           // var modifiedEndUserAiepEntries = new List<EntityEntry>();
            var modifiedEducationerEntries = new List<EntityEntry>();
            var modifiedOrCreatedSearchableEntities = new List<EntityEntry>();
            var deletedSearchableEntities = new List<EntityEntry>();
            var deletedDocumentsIds = new Dictionary<Type, IEnumerable<int>>();
            var isearchableByUserToUpdate = new Dictionary<Type, List<int>>();

            AutoCalculated();
            var addedIAuditableEntries = AuditAndSoftDelete();

            if (!DisableUpdateAclOnSave)
            {
                modifiedIEntityWithAclEntries = ChangeTracker.Entries()
                    .Where(x => x.Entity is IEntityWithAcl &&
                                (x.State == EntityState.Added || x.State == EntityState.Modified ||
                                 x.State == EntityState.Deleted)).ToList();

                modifiedBuilderEducationerAiepEntries = ChangeTracker.Entries()
                    .Where(x => x.Entity is BuilderEducationerAiep &&
                                (x.State == EntityState.Added || x.State == EntityState.Modified ||
                                 x.State == EntityState.Deleted)).ToList();

                modifiedEducationerEntries = this.ChangeTracker.Entries()
                                        .Where(x => x.Entity is User &&
                                            (
                                                x.State == EntityState.Added || x.State == EntityState.Deleted ||
                                                (x.State == EntityState.Modified && !x.OriginalValues["AiepId"].Equals(x.CurrentValues["AiepId"]))
                                            )
                                        ).ToList();

                foreach (var entry in modifiedEducationerEntries)
                {
                    var currentAiep = entry.Property("AiepId").CurrentValue;
                    var originalAiep = entry.Property("AiepId").OriginalValue;
                    if (originalAiep.IsNotNull() && currentAiep.IsNull())
                    {
                        entry.State = EntityState.Unchanged;
                    }

                }

                //TODO WHEN ENDUSERAiep IS USED 
                /* modifiedEndUserAiepEntries = ChangeTracker.Entries()
                    .Where(x => x.Entity is EndUserAiep &&
                                (x.State == EntityState.Added || x.State == EntityState.Modified ||
                                 x.State == EntityState.Deleted)).ToList(); */

                modifiedOrCreatedSearchableEntities = ChangeTracker.Entries()
                    .Where(e => e.Entity is ISearchable<int> &&
                                (e.State == EntityState.Modified || e.State == EntityState.Added)).ToList();

                deletedSearchableEntities = ChangeTracker.Entries()
                    .Where(e => e.Entity is ISearchable<int> && e.State == EntityState.Deleted).ToList();

                deletedDocumentsIds = searchManagementService.GetDeletedOrUpdatedDocumentsIds(this, deletedSearchableEntities);
                isearchableByUserToUpdate = AddISearchableByUserToUpdate(isearchableByUserToUpdate, 
                                            searchManagementService.GetISearchableDocumentsByUserToBeUpdated(this, deletedDocumentsIds));

                var updatedDocumentsIds = searchManagementService.GetDeletedOrUpdatedDocumentsIds(this, modifiedOrCreatedSearchableEntities);
                isearchableByUserToUpdate = AddISearchableByUserToUpdate(isearchableByUserToUpdate, 
                                            searchManagementService.GetISearchableDocumentsByUserToBeUpdated(this, updatedDocumentsIds));
            }

            var changedProjectEntries = ChangedAiepsProjectIds(modifiedIEntityWithAclEntries);
            base.SaveChanges();
            if (this.Database.CurrentTransaction != null) this.Database.CommitTransaction();
            AutoActionsCreate(addedIAuditableEntries);

            if (DisableUpdateAclOnSave)
            {
                return;
            }

            AutoProjectAcl(changedProjectEntries);
            AutoBuilderAcl(modifiedBuilderEducationerAiepEntries);
            AutoEducationerAcl(modifiedEducationerEntries);

            searchManagementService.DeleteDocuments(deletedDocumentsIds);
            var mergedOrUploadedDocumentIds = searchManagementService.MergeOrUploadDocuments(modifiedOrCreatedSearchableEntities);
            isearchableByUserToUpdate = AddISearchableByUserToUpdate(isearchableByUserToUpdate, 
                                        searchManagementService.GetISearchableDocumentsByUserToBeUpdated(this, mergedOrUploadedDocumentIds));
            searchManagementService.MergeOrUploadISearchableDocumentsByUser(this, isearchableByUserToUpdate);
        }

        private Dictionary<Type, List<int>> AddISearchableByUserToUpdate(Dictionary<Type, List<int>> searchableDocuments, Dictionary<Type, List<int>> newSearchableDocuments)
        {
            if (searchableDocuments.Any())
            {
                foreach (var document in newSearchableDocuments)
                {
                    if (searchableDocuments.ContainsKey(document.Key))
                    {
                        searchableDocuments[document.Key] = searchableDocuments[document.Key].Union(document.Value).ToList();
                    }
                    else
                    {
                        searchableDocuments.Add(document.Key, document.Value);
                    }
                }
            }
            else
            {
                searchableDocuments.AddRange(newSearchableDocuments);
            }
            return searchableDocuments;
        }

        private void StringPropertyLength(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties().Where(p => p.ClrType == typeof(string)))
                {
                    if (property.GetMaxLength() == null)
                    {
                        logger?.LogWarning(
                            $"ModelBuilder NonDefinedPropertyLength: {entityType.Name} - {property.Name}");

                        //TODO IS THIS STILL NEEDED?  ONCE MaxPropertyLength APPLIED property.SetMaxLength(LongPropertyLength);
                    }
                    else if (property.GetMaxLength() == MaxPropertyLength)
                    {
                        //TODO IS THIS STILL NEEDED?  ONCE MaxPropertyLength APPLIED  property.SetMaxLength(null);
                    }
                }
            }
        }

        #endregion

    }

}

