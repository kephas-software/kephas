#if NETSTANDARD2_1

// https://stackoverflow.com/questions/64749385/predefined-type-system-runtime-compilerservices-isexternalinit-is-not-defined

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}

#endif