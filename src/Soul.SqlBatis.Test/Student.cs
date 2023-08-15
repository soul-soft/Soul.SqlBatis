using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Soul.SqlBatis.Test
{
	public class Address : INotifyPropertyChanged
	{
		private string _p;
		public string P
		{
			get
			{
				return _p;
			}
			set
			{
				_p = value;
				NotifyPropertyChanged();
			}
		}
		private string _c;
		public string C 
		{
			get 
			{
				return _c;
			}
			set
			{
				_c = value;
				NotifyPropertyChanged();

			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	[Table("students")]
	public class Student
	{
		public uint Id { get; set; }

		public string Name { get; set; }

		public string FirstName { get; set; }

		public DateTime CreationTime { get; set; }

		public JsonArray<Address> Address { get; set; }

		public override bool Equals(object? obj)
		{
			if (obj == null || !(obj is Student))
				return false;
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			var other = (Student)obj;
			return Id == other.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}
