namespace Nexu.Shared.Infrastructure
{
    public static class SanitizeHelpers
    {
        public static string RemoveDisallowedCharacters(string field)
        {
            return field.Replace("-", string.Empty).Replace(" ", string.Empty);
        }
    }
}
