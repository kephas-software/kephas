﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Kephas.Resources.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to Cannot get the implementation type for the provided abstract type info &apos;{0}&apos;..
        /// </summary>
        public static string ActivatorBase_CannotGetImplementationType_Exception {
            get {
                return ResourceManager.GetString("ActivatorBase_CannotGetImplementationType_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot instantiate the type &apos;{0}&apos; because it could not be resolved to an implementation type..
        /// </summary>
        public static string ActivatorBase_CannotInstantiateAbstractTypeInfo_Exception {
            get {
                return ResourceManager.GetString("ActivatorBase_CannotInstantiateAbstractTypeInfo_Exception", resourceCulture);
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
        ///   Looks up a localized string similar to Multiple application services registered for the contract {0} and the override priority does not allow a proper service resolution. Identified eligible parts: {1}..
        /// </summary>
        public static string AmbiguousOverrideForAppServiceContract {
            get {
                return ResourceManager.GetString("AmbiguousOverrideForAppServiceContract", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bootstrapping the application....
        /// </summary>
        public static string App_BootstrapAsync_Bootstrapping_Message {
            get {
                return ResourceManager.GetString("App_BootstrapAsync_Bootstrapping_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Configuring the ambient services....
        /// </summary>
        public static string App_BootstrapAsync_ConfiguringAmbientServices_Message {
            get {
                return ResourceManager.GetString("App_BootstrapAsync_ConfiguringAmbientServices_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Errors occurred during the application configuration procedure, please check the exception details for more information..
        /// </summary>
        public static string App_BootstrapAsync_ErrorDuringConfiguration_Exception {
            get {
                return ResourceManager.GetString("App_BootstrapAsync_ErrorDuringConfiguration_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Errors occurred during the application forced shut down procedure, please check the exception details for more information..
        /// </summary>
        public static string App_BootstrapAsync_ErrorDuringForcedShutdown_Exception {
            get {
                return ResourceManager.GetString("App_BootstrapAsync_ErrorDuringForcedShutdown_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Errors occurred during the application bootstrap procedure, please check the exception details for more information..
        /// </summary>
        public static string App_BootstrapAsync_ErrorDuringInitialization_Exception {
            get {
                return ResourceManager.GetString("App_BootstrapAsync_ErrorDuringInitialization_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Initializing the app manager....
        /// </summary>
        public static string App_BootstrapAsync_InitializingAppManager_Message {
            get {
                return ResourceManager.GetString("App_BootstrapAsync_InitializingAppManager_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The application bootstrapped successfully..
        /// </summary>
        public static string App_BootstrapAsync_StartComplete_Message {
            get {
                return ResourceManager.GetString("App_BootstrapAsync_StartComplete_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The application shut down successfully. Goodbye!.
        /// </summary>
        public static string App_ShutdownAsync_Complete_Message {
            get {
                return ResourceManager.GetString("App_ShutdownAsync_Complete_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Errors occurred during the application shutdown procedure, please check the exception details for more information..
        /// </summary>
        public static string App_ShutdownAsync_ErrorDuringFinalization_Exception {
            get {
                return ResourceManager.GetString("App_ShutdownAsync_ErrorDuringFinalization_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Shutting down the application....
        /// </summary>
        public static string App_ShutdownAsync_ShuttingDown_Message {
            get {
                return ResourceManager.GetString("App_ShutdownAsync_ShuttingDown_Message", resourceCulture);
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
        ///   Looks up a localized string similar to The specified contract type &apos;{0}&apos; is not assignable from the service contract &apos;{1}&apos;..
        /// </summary>
        public static string AppServiceContractTypeDoesNotMatchServiceContract {
            get {
                return ResourceManager.GetString("AppServiceContractTypeDoesNotMatchServiceContract", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Application service contracts exported as open generics do not support metadata attributes and they will be ignored (&apos;{contractType}&apos;). Instead, try to pass metadata through generic parameter types..
        /// </summary>
        public static string AppServiceConventionsRegistrarBase_AsOpenGenericDoesNotSupportMetadataAttributes_Warning {
            get {
                return ResourceManager.GetString("AppServiceConventionsRegistrarBase_AsOpenGenericDoesNotSupportMetadataAttributes_" +
                        "Warning", resourceCulture);
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
        ///   Looks up a localized string similar to Cannot resolve priorities of service &apos;{0}&apos; for two instances: {1} and {2}..
        /// </summary>
        public static string CompositionHelper_ToDictionary_CannotResolveServicePriority_Exception {
            get {
                return ResourceManager.GetString("CompositionHelper_ToDictionary_CannotResolveServicePriority_Exception", resourceCulture);
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
        ///   Looks up a localized string similar to Setting a value is not supported by the default implementation of the configuration. Override this in a derived class to provide an implementation..
        /// </summary>
        public static string ConfigurationBase_SettingValueNotSupported_Exception {
            get {
                return ResourceManager.GetString("ConfigurationBase_SettingValueNotSupported_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The stored settings type &apos;{0}&apos; differs from the requested settings type &apos;{1}&apos;..
        /// </summary>
        public static string ConfigurationStoreSettingsProvider_SettingsTypeMismatch_Exception {
            get {
                return ResourceManager.GetString("ConfigurationStoreSettingsProvider_SettingsTypeMismatch_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot change the identity in the context once it has been set..
        /// </summary>
        public static string Context_CannotChangeIdentity_Exception {
            get {
                return ResourceManager.GetString("Context_CannotChangeIdentity_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The context type &apos;{0}&apos; must be an instatiable class..
        /// </summary>
        public static string ContextFactory_CreateContext_ContextTypeMustBeInstatiable {
            get {
                return ResourceManager.GetString("ContextFactory_CreateContext_ContextTypeMustBeInstatiable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Arguments may not be null when instatiating context of type &apos;{0}&apos;..
        /// </summary>
        public static string ContextFactory_CreateContext_NonNullArguments {
            get {
                return ResourceManager.GetString("ContextFactory_CreateContext_NonNullArguments", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find a matching constructor for signature {0} in type &apos;{1}&apos;..
        /// </summary>
        public static string ContextFactory_GetCreatorFunc_CannotFindMatchingConstructor {
            get {
                return ResourceManager.GetString("ContextFactory_GetCreatorFunc_CannotFindMatchingConstructor", resourceCulture);
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
        ///   Looks up a localized string similar to Duplicate type with full name &apos;{0}&apos; in &apos;{1}&apos;..
        /// </summary>
        public static string DynamicAssemblyInfo_AddType_Duplicate_Exception {
            get {
                return ResourceManager.GetString("DynamicAssemblyInfo_AddType_Duplicate_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type &apos;{0}&apos; cannot be a base of itself..
        /// </summary>
        public static string DynamicTypeInfo_AddBaseType_TypeCannotBeABaseOfItself_Exception {
            get {
                return ResourceManager.GetString("DynamicTypeInfo_AddBaseType_TypeCannotBeABaseOfItself_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Duplicate member with name &apos;{0}&apos; in &apos;{1}&apos;..
        /// </summary>
        public static string DynamicTypeInfo_AddMember_Duplicate_Exception {
            get {
                return ResourceManager.GetString("DynamicTypeInfo_AddMember_Duplicate_Exception", resourceCulture);
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
        ///   Looks up a localized string similar to Cannot identify the constructed service type of {0} from {1}..
        /// </summary>
        public static string LiteRegistrationBuilder_CannotIdentifyConstructedServiceType_Exception {
            get {
                return ResourceManager.GetString("LiteRegistrationBuilder_CannotIdentifyConstructedServiceType_Exception", resourceCulture);
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
        ///   Looks up a localized string similar to Cannot merge the task result, the task is not completed..
        /// </summary>
        public static string OperationResult_Merge_TaskNotCompleteException {
            get {
                return ResourceManager.GetString("OperationResult_Merge_TaskNotCompleteException", resourceCulture);
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
