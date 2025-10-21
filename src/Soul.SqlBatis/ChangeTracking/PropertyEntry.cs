using Soul.SqlBatis.Metadata;

namespace Soul.SqlBatis.ChangeTracking
{

    public class PropertyEntry : IPropertyEntry
    {

        internal PropertyEntry(EntityEntry entityEntry, IProperty metadata)
        {
            Metadata = metadata;
            EntityEntry = entityEntry;
        }

        public IProperty Metadata { get; private set; }

        public EntityEntry EntityEntry { get; private set; }

        public object OriginalValue
        {
            get
            {
                return EntityEntry.GetOriginalValue(Metadata);
            }
        }


        public object CurrentValue
        {
            get
            {
                return EntityEntry.GetCurrentValue(Metadata);
            }
        }

        public bool IsModified
        {
            get
            {
                if (CurrentValue == null && OriginalValue == null)
                {
                    return false;
                }
                if (CurrentValue == null && OriginalValue != null)
                {
                    return true;
                }
           
                return !CurrentValue.Equals(OriginalValue);
            }
        }
    }
}
