﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Kephas.Injection.SystemComposition.Resources.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided conventions must implement {0}..
        /// </summary>
        public static string InvalidConventions {
            get {
                return ResourceManager.GetString("InvalidConventions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type &apos;{0}&apos; cannot be used as a metadata view. A metadata view must be a concrete class with a parameterless or dictionary constructor..
        /// </summary>
        public static string MetadataViewProvider_InvalidViewImplementation {
            get {
                return ResourceManager.GetString("MetadataViewProvider_InvalidViewImplementation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Export metadata for &apos;{0}&apos; is missing and no default value was supplied..
        /// </summary>
        public static string MetadataViewProvider_MissingMetadata {
            get {
                return ResourceManager.GetString("MetadataViewProvider_MissingMetadata", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The injector is disposed!.
        /// </summary>
        public static string SystemCompositionInjector_Disposed_Exception {
            get {
                return ResourceManager.GetString("SystemCompositionInjector_Disposed_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple eligible constructors declared for service implementation &apos;{0}&apos;. Use &apos;{1}&apos; to disambiguate among them..
        /// </summary>
        public static string SystemCompositionPartConventionsBuilder_AmbiguousCompositionConstructor {
            get {
                return ResourceManager.GetString("SystemCompositionPartConventionsBuilder_AmbiguousCompositionConstructor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There are no constructors defined for service implementation &apos;{0}&apos; with contract &apos;{1}&apos;..
        /// </summary>
        public static string SystemCompositionPartConventionsBuilder_MissingCompositionConstructor {
            get {
                return ResourceManager.GetString("SystemCompositionPartConventionsBuilder_MissingCompositionConstructor", resourceCulture);
            }
        }
    }
}
