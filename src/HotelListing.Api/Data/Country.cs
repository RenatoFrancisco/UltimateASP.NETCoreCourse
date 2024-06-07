namespace HotelListing.Api.Data;

public class Country : Entity
{
    public string? Name { get; set; }
    public string? ShortName { get; set; }

    public virtual IList<Hotel>? Hotels { get; set; }
}
