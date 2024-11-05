// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Major Code Smell",
    "S3903:Types should be defined in named namespaces",
    Justification = "False positive: Sonar does not support yet the file scoped namespaces.")]
[assembly: SuppressMessage(
    "Critical Code Smell",
    "S2360:Optional parameters should not be used",
    Justification = "Do not bloat classes and interfaces with overloads if not justified.")]
[assembly: SuppressMessage(
    "Major Code Smell",
    "S3925:\"ISerializable\" should be implemented correctly",
    Justification = "Obsolete binary serialization is not used anymore.")]
[assembly: SuppressMessage(
    "Critical Code Smell",
    "S4039:Interface methods should be callable by derived types",
    Justification = "False positive: Default interface method implementation not supported")]
