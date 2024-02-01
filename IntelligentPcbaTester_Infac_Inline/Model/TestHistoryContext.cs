using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentPcbaTester
{
    public class TestHistoryContext : DbContext
    {
        public DbSet<TestHistory> TestHistories { get; set; }
        public DbSet<HistoryGroup> HistoryGroups { get; set; }

        public TestHistoryContext()
            : base(new SQLiteConnection(new SQLiteConnectionStringBuilder() { DataSource = "D:\\ElozPlugin\\history.db" }.ConnectionString), true)
        {
        }
    }
}
