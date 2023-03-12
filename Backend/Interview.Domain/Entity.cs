namespace Interview.Domain;

public abstract class Entity
{
    public Entity()
    {
    }

    public Entity(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }
}
