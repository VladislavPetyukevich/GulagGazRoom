namespace Interview.Backend;

public static class GenericExt
{
    public static string GetNotFoundMessage<T>(this T? self, string entityName, Guid id)
    {
        return self != null ? string.Empty : $"Not found {entityName} with id [{id}]";
    }
}
