namespace HotelListing.Api.Data;

public abstract class Entity
{
    public int Id { get; set; }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj == null || GetType() != obj.GetType())
            return false;

        Entity other = (Entity)obj;
        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}

