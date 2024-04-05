// Weaver // https://kybernetik.com.au/weaver // Copyright 2022 Kybernetik //

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Weaver.Examples")]
[assembly: AssemblyDescription("Examples for Weaver.")]
[assembly: AssemblyCompany("Kybernetik")]
[assembly: AssemblyProduct("Weaver")]
[assembly: AssemblyCopyright("Copyright © Kybernetik 2022")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("6.3.0.0")]

#if UNITY_EDITOR

[assembly: SuppressMessage("Style", "IDE0016:Use 'throw' expression",
    Justification = "Not supported by older Unity versions.")]
[assembly: SuppressMessage("Style", "IDE0018:Inline variable declaration",
    Justification = "Not supported by older Unity versions.")]
[assembly: SuppressMessage("Style", "IDE0019:Use pattern matching",
    Justification = "Not supported by older Unity versions.")]
[assembly: SuppressMessage("Style", "IDE0031:Use null propagation",
    Justification = "Not supported by older Unity versions.")]
[assembly: SuppressMessage("Style", "IDE0041:Use 'is null' check",
    Justification = "Not supported by older Unity versions.")]
[assembly: SuppressMessage("Style", "IDE0044:Make field readonly",
    Justification = "Using the [SerializeField] attribute on a private field means Unity will set it from serialized data.")]
[assembly: SuppressMessage("Code Quality", "IDE0051:Remove unused private members",
    Justification = "Unity messages can be private, but the IDE will not know that Unity can still call them.")]
[assembly: SuppressMessage("Code Quality", "IDE0052:Remove unread private members",
    Justification = "Unity messages can be private and don't need to be called manually.")]
[assembly: SuppressMessage("Style", "IDE0059:Value assigned to symbol is never used",
    Justification = "Inline variable declarations are not supported by older Unity versions.")]
[assembly: SuppressMessage("Style", "IDE0060:Remove unused parameter",
    Justification = "Unity messages sometimes need specific signatures, even if you don't use all the parameters.")]
[assembly: SuppressMessage("Style", "IDE0066:Convert switch statement to expression",
    Justification = "Not supported by older Unity versions.")]
[assembly: SuppressMessage("Style", "IDE0083:Use pattern matching",
    Justification = "Not supported by older Unity versions")]
[assembly: SuppressMessage("Style", "IDE0090:Use 'new(...)'",
    Justification = "Not supported by older Unity versions.")]
[assembly: SuppressMessage("Style", "IDE1005:Delegate invocation can be simplified.",
    Justification = "Not supported by older Unity versions.")]
[assembly: SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression",
    Justification = "Don't give code style advice in publically released code.")]
[assembly: SuppressMessage("Style", "IDE1006:Naming Styles",
    Justification = "Don't give code style advice in publically released code.")]

// This suppression doesn't seem to actually work so we need to put #pragma warning disable in every file :(
//[assembly: SuppressMessage("Code Quality", "CS0649:Field is never assigned to, and will always have its default value",
//    Justification = "Using the [SerializeField] attribute on a private field means Unity will set it from serialized data.")]

#endif
