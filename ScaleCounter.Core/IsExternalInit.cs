// Enables C# record types and `init`-only setters on netstandard2.0, which does
// not ship System.Runtime.CompilerServices.IsExternalInit. Internal so it never
// clashes with the runtime-provided type on modern consumers.
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
