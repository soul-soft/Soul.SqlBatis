using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Soul.SqlBatis.Infrastructure
{
	public static class ActivatorUtilities
	{
		public static Func<IDataRecord, T> Create<T>(IDataRecord record)
		{

		}

		private static List<FieldEntry> GetFieldEntries(IDataRecord record)
		{
			var list = new List<FieldEntry>();
			for (int i = 0; i < record.FieldCount; i++)
			{
				var name = record.GetName(i);
				var type = record.GetFieldType(i);
				list.Add(new FieldEntry(type, name));
			}
			return list;
		}

		private static IEnumerable<BinddingEntry> GetBinddingEntries(Type entityType,List<FieldEntry> entries)
		{
			var list = new List<BinddingEntry>();
			if (entries.Count == 1 && DefaultDataConverter.TryGetConverter(entries[0].Type, out _))
			{
				yield return new BinddingEntry(entityType, entries[0].Type);
			}
			else if()
			{

			}
		}

		private static bool TryGetDataConverter()
		{
			typeof(DefaultDataConverter)
		}

		class BinddingEntry
		{
			public string MemberName { get; }
			public string ColumnName { get; }
			public Type MemberType { get; }
			public Type ColumnType { get; }
			public BinddingEntry(Type memberType, Type columnType)
			{
				MemberType = memberType;
				ColumnType = columnType;
			}
			public BinddingEntry(string memberName, string columnName, Type memberType, Type columnType)
			{
				MemberName = memberName;
				ColumnName = columnName;
				MemberType = memberType;
				ColumnType = columnType;
			}
		}

		class FieldEntry
		{
			public Type Type { get; }
			public string Name { get; }

			public FieldEntry(Type type, string name)
			{
				Type = type;
				Name = name;
			}
		}
	}
}
