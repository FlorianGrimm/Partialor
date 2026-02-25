using System.Runtime.CompilerServices;
using VerifyTests;

namespace Partialor.Configuration;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init() => VerifySourceGenerators.Initialize();
}