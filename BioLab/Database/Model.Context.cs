﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BioLab.Database
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class entities : DbContext
    {
        public entities()
            : base("name=entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<analyzer> analyzers { get; set; }
        public virtual DbSet<country> countries { get; set; }
        public virtual DbSet<ensurance_patients> ensurance_patients { get; set; }
        public virtual DbSet<ensurance> ensurances { get; set; }
        public virtual DbSet<order_research_objects> order_research_objects { get; set; }
        public virtual DbSet<order_services> order_services { get; set; }
        public virtual DbSet<order_statuses> order_statuses { get; set; }
        public virtual DbSet<order> orders { get; set; }
        public virtual DbSet<patient> patients { get; set; }
        public virtual DbSet<research_objects> research_objects { get; set; }
        public virtual DbSet<research_result_statuses> research_result_statuses { get; set; }
        public virtual DbSet<research_results> research_results { get; set; }
        public virtual DbSet<service> services { get; set; }
        public virtual DbSet<social_types> social_types { get; set; }
        public virtual DbSet<user_types> user_types { get; set; }
        public virtual DbSet<user> users { get; set; }
        public virtual DbSet<users_auth_tries> users_auth_tries { get; set; }
    }
}
