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

    public Guid Id { get; protected set; }
}
