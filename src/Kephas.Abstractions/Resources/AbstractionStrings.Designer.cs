﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kephas.Resources {
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
    public class AbstractionStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AbstractionStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Kephas.Resources.AbstractionStrings", typeof(AbstractionStrings).Assembly);
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
        ///   Looks up a localized string similar to &apos;{0}&apos; has at least two constructors with the same length which can be resolved through composition: ({1}) and ({2})..
        /// </summary>
        public static string AmbientServices_AmbiguousConstructors_Exception {
            get {
                return ResourceManager.GetString("AmbientServices_AmbiguousConstructors_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No injection constructor could be identified for &apos;{0}&apos;. The following parameters could not be resolved: {1}..
        /// </summary>
        public static string AmbientServices_MissingCompositionConstructor_Exception {
            get {
                return ResourceManager.GetString("AmbientServices_MissingCompositionConstructor_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The required service &apos;{0}&apos; is not registered, or the factory returned a null value..
        /// </summary>
        public static string AmbientServices_RequiredServiceNotRegistered_Exception {
            get {
                return ResourceManager.GetString("AmbientServices_RequiredServiceNotRegistered_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided service implementation &apos;{0}&apos; is not convertible to the service type &apos;{1}&apos;..
        /// </summary>
        public static string AmbientServices_ServiceTypeAndImplementationMismatch_Exception {
            get {
                return ResourceManager.GetString("AmbientServices_ServiceTypeAndImplementationMismatch_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided service instance &apos;{0}&apos; is not convertible to the service type &apos;{1}&apos;..
        /// </summary>
        public static string AmbientServices_ServiceTypeAndServiceInstanceMismatch_Exception {
            get {
                return ResourceManager.GetString("AmbientServices_ServiceTypeAndServiceInstanceMismatch_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The service type &apos;{0}&apos; must be a super type of the contract type &apos;{1}&apos;..
        /// </summary>
        public static string AmbientServices_ServiceTypeMustBeSuperOfContractType_Exception {
            get {
                return ResourceManager.GetString("AmbientServices_ServiceTypeMustBeSuperOfContractType_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot load assembly &apos;{assembly}&apos;. See the exception for more information..
        /// </summary>
        public static string AppRuntimeBase_CannotLoadAssembly_Exception {
            get {
                return ResourceManager.GetString("AppRuntimeBase_CannotLoadAssembly_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple constructors marked with {0} are declared for service implementation &apos;{1}&apos; with contract &apos;{2}&apos;..
        /// </summary>
        public static string AppServiceMultipleInjectConstructors {
            get {
                return ResourceManager.GetString("AppServiceMultipleInjectConstructors", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not add the item {0} to the concurrent collection..
        /// </summary>
        public static string ConcurrentCollection_CannotAddItem_Exception {
            get {
                return ResourceManager.GetString("ConcurrentCollection_CannotAddItem_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The application&apos;s finalize procedure was canceled, at {0:s}..
        /// </summary>
        public static string DefaultAppManager_FinalizeCanceled_Exception {
            get {
                return ResourceManager.GetString("DefaultAppManager_FinalizeCanceled_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The application&apos;s finalize procedure encountered an exception at {0:s}..
        /// </summary>
        public static string DefaultAppManager_FinalizeFaulted_Exception {
            get {
                return ResourceManager.GetString("DefaultAppManager_FinalizeFaulted_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The application&apos;s initialize procedure was canceled, at {0:s}..
        /// </summary>
        public static string DefaultAppManager_InitializeCanceled_Exception {
            get {
                return ResourceManager.GetString("DefaultAppManager_InitializeCanceled_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The application&apos;s initialize procedure encountered an exception at {0:s}..
        /// </summary>
        public static string DefaultAppManager_InitializeFaulted_Exception {
            get {
                return ResourceManager.GetString("DefaultAppManager_InitializeFaulted_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurred when invoking subscription callback for &apos;{event}&apos;..
        /// </summary>
        public static string DefaultEventHub_ErrorWhenInvokingSubscriptionCallback {
            get {
                return ResourceManager.GetString("DefaultEventHub_ErrorWhenInvokingSubscriptionCallback", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No media type class  found for &apos;{0}&apos;. Resolution: define a class implementing {1} with the indicated media type..
        /// </summary>
        public static string DefaultMediaTypeProvider_NotFound_Exception {
            get {
                return ResourceManager.GetString("DefaultMediaTypeProvider_NotFound_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple services found for {0}/{1}..
        /// </summary>
        public static string DefaultNamedServiceProvider_GetNamedService_AmbiguousMatch_Exception {
            get {
                return ResourceManager.GetString("DefaultNamedServiceProvider_GetNamedService_AmbiguousMatch_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No service found for {0}/{1}..
        /// </summary>
        public static string DefaultNamedServiceProvider_GetNamedService_NoServiceFound_Exception {
            get {
                return ResourceManager.GetString("DefaultNamedServiceProvider_GetNamedService_NoServiceFound_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A serializer for the media type &apos;{0}&apos; was not found..
        /// </summary>
        public static string DefaultSerializationService_SerializerNotFound_Exception {
            get {
                return ResourceManager.GetString("DefaultSerializationService_SerializerNotFound_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot load all defined types from {assembly}. Only the loadable types will be considered..
        /// </summary>
        public static string DefaultTypeLoader_GetLoadableDefinedTypes_ReflectionTypeLoadException {
            get {
                return ResourceManager.GetString("DefaultTypeLoader_GetLoadableDefinedTypes_ReflectionTypeLoadException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot load all exported types from {assembly}. Only the loadable types will be considered..
        /// </summary>
        public static string DefaultTypeLoader_GetLoadableExportedTypes_ReflectionTypeLoadException {
            get {
                return ResourceManager.GetString("DefaultTypeLoader_GetLoadableExportedTypes_ReflectionTypeLoadException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot load exported type {type} from {assembly}. Only the loadable types will be considered..
        /// </summary>
        public static string DefaultTypeLoader_GetLoadableExportedTypes_TypeLoadException {
            get {
                return ResourceManager.GetString("DefaultTypeLoader_GetLoadableExportedTypes_TypeLoadException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple types found for &apos;{0}&apos; : {1}..
        /// </summary>
        public static string DefaultTypeResolver_ResolveType_AmbiguousMatch_Exception {
            get {
                return ResourceManager.GetString("DefaultTypeResolver_ResolveType_AmbiguousMatch_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type &apos;{0}&apos; could not be found..
        /// </summary>
        public static string DefaultTypeResolver_ResolveType_NotFound_Exception {
            get {
                return ResourceManager.GetString("DefaultTypeResolver_ResolveType_NotFound_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Errors occurred when trying to resolve type &apos;{type}&apos;..
        /// </summary>
        public static string DefaultTypeResolver_ResolveTypeCore_Exception {
            get {
                return ResourceManager.GetString("DefaultTypeResolver_ResolveTypeCore_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An item with the same key is already added..
        /// </summary>
        public static string DuplicateKeyException_Message {
            get {
                return ResourceManager.GetString("DuplicateKeyException_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot invoke non-delegate value of {0}; it has a type of {1}..
        /// </summary>
        public static string ExpandoBase_CannotInvokeNonDelegate_Exception {
            get {
                return ResourceManager.GetString("ExpandoBase_CannotInvokeNonDelegate_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple nodes found for value &apos;{0}&apos;..
        /// </summary>
        public static string GraphBaseOfTNodeValue_AmbiguousMatchForValue_Exception {
            get {
                return ResourceManager.GetString("GraphBaseOfTNodeValue_AmbiguousMatchForValue_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The injectable type &apos;{0}&apos; must be an instantiable class..
        /// </summary>
        public static string InjectableFactory_Create_InjectableTypeMustBeInstantiable {
            get {
                return ResourceManager.GetString("InjectableFactory_Create_InjectableTypeMustBeInstantiable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Arguments may not be null when instantiating injectable of type &apos;{0}&apos;..
        /// </summary>
        public static string InjectableFactory_Create_NonNullArguments {
            get {
                return ResourceManager.GetString("InjectableFactory_Create_NonNullArguments", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find a matching constructor for signature {0} in type &apos;{1}&apos;..
        /// </summary>
        public static string InjectableFactory_GetCreatorFunc_CannotFindMatchingConstructor {
            get {
                return ResourceManager.GetString("InjectableFactory_GetCreatorFunc_CannotFindMatchingConstructor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The required service of type &apos;{0}&apos; was not provided..
        /// </summary>
        public static string InjectorBuilderBase_RequiredServiceMissing_Exception {
            get {
                return ResourceManager.GetString("InjectorBuilderBase_RequiredServiceMissing_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Circular dependency involving &apos;{0}&apos; detected..
        /// </summary>
        public static string LazyFactory_CircularDependency_Exception {
            get {
                return ResourceManager.GetString("LazyFactory_CircularDependency_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to One of {0} or {1} must be set in service registration..
        /// </summary>
        public static string LiteRegistrationBuilder_InvalidRegistration_Exception {
            get {
                return ResourceManager.GetString("LiteRegistrationBuilder_InvalidRegistration_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No implementation found for service type &apos;{0}&apos;..
        /// </summary>
        public static string NoImplementationForServiceType_Exception {
            get {
                return ResourceManager.GetString("NoImplementationForServiceType_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} is intended to be replaced by a proper implementation..
        /// </summary>
        public static string NullServiceExceptionMessage {
            get {
                return ResourceManager.GetString("NullServiceExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided comparer function produces cycles in the sorted graph. This indicates that there are cyclic dependencies which need to be broken in order to produce an ordered set. Check the following nodes: {0}..
        /// </summary>
        public static string PartialOrderedSet_BadComparer_ProducesCycles_Exception {
            get {
                return ResourceManager.GetString("PartialOrderedSet_BadComparer_ProducesCycles_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Argument &apos;{0}&apos; must not be null nor empty..
        /// </summary>
        public static string Requires_NotNullOrEmpty_EmptyArgument_Exception {
            get {
                return ResourceManager.GetString("Requires_NotNullOrEmpty_EmptyArgument_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No get accessor for property &apos;{0}&apos; in &apos;{1}&apos;..
        /// </summary>
        public static string RuntimePropertyInfo_GetValue_Exception {
            get {
                return ResourceManager.GetString("RuntimePropertyInfo_GetValue_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No set accessor for property &apos;{0}&apos; in &apos;{1}&apos;..
        /// </summary>
        public static string RuntimePropertyInfo_SetValue_Exception {
            get {
                return ResourceManager.GetString("RuntimePropertyInfo_SetValue_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There are multiple members with the name &apos;{0}&apos; in &apos;{1}&apos;. Please use {2} to disambiguate between them..
        /// </summary>
        public static string RuntimeTypeInfo_AmbiguousMember_Exception {
            get {
                return ResourceManager.GetString("RuntimeTypeInfo_AmbiguousMember_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} provided in the [{1}] attribute must implement have a public constructor accepting a single type parameter..
        /// </summary>
        public static string RuntimeTypeInfo_CreateRuntimeTypeInfo_InvalidConstructor_Exception {
            get {
                return ResourceManager.GetString("RuntimeTypeInfo_CreateRuntimeTypeInfo_InvalidConstructor_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} provided in the [{1}] attribute must implement {2}..
        /// </summary>
        public static string RuntimeTypeInfo_CreateRuntimeTypeInfo_InvalidType_Exception {
            get {
                return ResourceManager.GetString("RuntimeTypeInfo_CreateRuntimeTypeInfo_InvalidType_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Member {0} not found in {1}..
        /// </summary>
        public static string RuntimeTypeInfo_MemberNotFound_Exception {
            get {
                return ResourceManager.GetString("RuntimeTypeInfo_MemberNotFound_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No public constructor defined for &apos;{0}&apos;, cannot create instances..
        /// </summary>
        public static string RuntimeTypeInfo_NoPublicConstructorDefined_Exception {
            get {
                return ResourceManager.GetString("RuntimeTypeInfo_NoPublicConstructorDefined_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Property {0} not found or is not accessible in {1}..
        /// </summary>
        public static string RuntimeTypeInfo_PropertyNotFound_Exception {
            get {
                return ResourceManager.GetString("RuntimeTypeInfo_PropertyNotFound_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected {0} instead of {1} media type in the serialization context..
        /// </summary>
        public static string Serialization_MediaTypeMismatch_Exception {
            get {
                return ResourceManager.GetString("Serialization_MediaTypeMismatch_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} should export a public property {1}: {2} to be able to create a serialization context..
        /// </summary>
        public static string SerializationExtensions_GetContextFactory_CannotGetContextFactory {
            get {
                return ResourceManager.GetString("SerializationExtensions_GetContextFactory_CannotGetContextFactory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot resolve priorities of service &apos;{0}&apos; for two instances: {1} and {2}..
        /// </summary>
        public static string ServiceEnumerableExtensions_ToDictionary_CannotResolveServicePriority_Exception {
            get {
                return ResourceManager.GetString("ServiceEnumerableExtensions_ToDictionary_CannotResolveServicePriority_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Either the implementation type, the factory, or the service instance must be provided for &apos;{0}&apos;. Check also whether the {1} value should be set to {2}..
        /// </summary>
        public static string ServiceRegistrationBuilder_InstancingNotProvided_Exception {
            get {
                return ResourceManager.GetString("ServiceRegistrationBuilder_InstancingNotProvided_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The service {0} is registered both as multiple and single service..
        /// </summary>
        public static string ServiceRegistry_MismatchedMultipleServiceRegistration_Exception {
            get {
                return ResourceManager.GetString("ServiceRegistry_MismatchedMultipleServiceRegistration_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No provider found for settings type &apos;{settingsType}&apos;..
        /// </summary>
        public static string SettingsProviderSelector_NoProviderFoundForSettingsType {
            get {
                return ResourceManager.GetString("SettingsProviderSelector_NoProviderFoundForSettingsType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The task is not completed..
        /// </summary>
        public static string TaskNotCompletedException_Message {
            get {
                return ResourceManager.GetString("TaskNotCompletedException_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} of &apos;{1}&apos; is not completed..
        /// </summary>
        public static string TransitionMonitor_AssertIsCompleted_Exception {
            get {
                return ResourceManager.GetString("TransitionMonitor_AssertIsCompleted_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} of &apos;{1}&apos; is not completed successfully..
        /// </summary>
        public static string TransitionMonitor_AssertIsCompletedSuccessfully_Exception {
            get {
                return ResourceManager.GetString("TransitionMonitor_AssertIsCompletedSuccessfully_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} of &apos;{1}&apos; is not in progress..
        /// </summary>
        public static string TransitionMonitor_AssertIsInProgress_Exception {
            get {
                return ResourceManager.GetString("TransitionMonitor_AssertIsInProgress_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} of &apos;{1}&apos; is already started..
        /// </summary>
        public static string TransitionMonitor_AssertIsNotStarted_Exception {
            get {
                return ResourceManager.GetString("TransitionMonitor_AssertIsNotStarted_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unhandled exception in process..
        /// </summary>
        public static string UnhandledException_InProcess_Message {
            get {
                return ResourceManager.GetString("UnhandledException_InProcess_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unhandled exception terminating the process..
        /// </summary>
        public static string UnhandledException_Terminating_Message {
            get {
                return ResourceManager.GetString("UnhandledException_Terminating_Message", resourceCulture);
            }
        }
    }
}
