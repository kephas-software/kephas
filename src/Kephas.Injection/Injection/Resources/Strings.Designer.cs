﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kephas.Injection.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Kephas.Injection.Resources.Strings", typeof(Strings).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple application services registered for the contract &apos;{0}&apos; and the override priority does not allow a proper service resolution. Registrations: {1}..
        /// </summary>
        internal static string AmbiguousOverrideForAppServiceContract {
            get {
                return ResourceManager.GetString("AmbiguousOverrideForAppServiceContract", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple contract declarations for {0}. {1} will be overwritten by {2}..
        /// </summary>
        internal static string AppServiceCollectionMultipleContractDeclarationsWithSameType {
            get {
                return ResourceManager.GetString("AppServiceCollectionMultipleContractDeclarationsWithSameType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified contract type &apos;{0}&apos; is not assignable from the service contract &apos;{1}&apos;..
        /// </summary>
        internal static string AppServiceContractTypeDoesNotMatchServiceContract {
            get {
                return ResourceManager.GetString("AppServiceContractTypeDoesNotMatchServiceContract", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The contract type {0} must be assignable from the declaration type {1}..
        /// </summary>
        internal static string AppServiceInfoBuilderContractTypeMismatch {
            get {
                return ResourceManager.GetString("AppServiceInfoBuilderContractTypeMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No contract type provided for {0}..
        /// </summary>
        internal static string InjectorBuilder_RegisterService_InvalidContractType {
            get {
                return ResourceManager.GetString("InjectorBuilder_RegisterService_InvalidContractType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple application services registered for the contract &apos;{contractType}&apos;. Strategy is {resolutionStrategy}, registrations: {registrations}..
        /// </summary>
        internal static string MultipleRegistrationsForAppServiceContract {
            get {
                return ResourceManager.GetString("MultipleRegistrationsForAppServiceContract", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The service with the requested criteria was not found..
        /// </summary>
        internal static string OrderedServiceFactoryCollection_service_with_requested_criteria_not_found {
            get {
                return ResourceManager.GetString("OrderedServiceFactoryCollection_service_with_requested_criteria_not_found", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The service {0} is either not registered or it is not registered with an instance..
        /// </summary>
        internal static string ServiceInstanceNotRegistered {
            get {
                return ResourceManager.GetString("ServiceInstanceNotRegistered", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The service provider is not available, it is now being built..
        /// </summary>
        internal static string ServiceProviderIsBeingBuilt {
            get {
                return ResourceManager.GetString("ServiceProviderIsBeingBuilt", resourceCulture);
            }
        }
    }
}
