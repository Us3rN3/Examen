namespace VivesRental.Models.Order
{
    public class CustomerGroupKey : IEquatable<CustomerGroupKey>
    {
        public Guid CustomerId { get; set; }
        public string CustomerFirstName { get; set; } = "";
        public string CustomerLastName { get; set; } = "";
        public string CustomerEmail { get; set; } = "";

        public override bool Equals(object? obj)
        {
            return Equals(obj as CustomerGroupKey);
        }

        public bool Equals(CustomerGroupKey? other)
        {
            if (other == null) return false;
            return CustomerId == other.CustomerId;
            // Je kan ook meer velden meenemen als nodig, maar CustomerId is vaak uniek genoeg
        }

        public override int GetHashCode()
        {
            return CustomerId.GetHashCode();
        }
    }

}
