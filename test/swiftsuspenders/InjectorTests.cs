﻿using System;
using NUnit.Framework;
using swiftsuspenders.support.injectees;
using swiftsuspenders.support.types;
using swiftsuspenders.support.enums;
using swiftsuspenders.errors;
using swiftsuspenders.support.providers;
using swiftsuspenders.typedescriptions;
using swiftsuspenders.mapping;
using swiftsuspenders.dependencyproviders;

namespace swiftsuspenders
{
	[TestFixture]
	public class InjectorTests
	{
		private Injector injector;
//		protected var receivedInjectorEvents : Array;

		[SetUp]
		public void setup()
		{
			injector = new Injector();
//			receivedInjectorEvents = [];
		}

		[TearDown]
		public void Teardown()
		{
			Injector.PurgeInjectionPointsCache();
			injector = null;
//			receivedInjectorEvents = null;
		}

		[Test]
		public void unmap_removes_mapping()
		{
			InterfaceInjectee injectee = new InterfaceInjectee();
			Clazz value = new Clazz();
			injector.Map(typeof(Interface)).ToValue(value);
			Assert.True(injector.Satisfies(typeof(Interface)));
			injector.Unmap(typeof(Interface));
			Assert.False(injector.Satisfies(typeof(Interface)));
		}

		[Test]
		public void injector_injects_bound_value_into_all_injectees()
		{
			ClassInjectee injectee = new ClassInjectee();
			ClassInjectee injectee2= new ClassInjectee();
			Clazz value = new Clazz();
			injector.Map(typeof(Clazz)).ToValue(value);
			injector.InjectInto(injectee);
			Assert.AreEqual(value, injectee.property, "Value should have been injected");
			injector.InjectInto(injectee2);
			Assert.AreEqual(injectee.property, injectee2.property, "Injected values should be equal");
		}

		[Test]
		public void map_value_by_interface()
		{
			InterfaceInjectee injectee = new InterfaceInjectee();
			Interface value = new Clazz();
			injector.Map(typeof(Interface)).ToValue(value);
			injector.InjectInto(injectee);
			Assert.AreEqual(value, injectee.property, "Value should have been injected");
		}

		[Test]
		public void map_named_value_by_class()
		{
			NamedClassInjectee injectee = new NamedClassInjectee();
			Clazz value = new Clazz();
			injector.Map(typeof(Clazz), NamedClassInjectee.NAME).ToValue(value);
			injector.InjectInto(injectee);
			Assert.AreEqual(value, injectee.property, "Named value should have been injected");
		}

		[Test]
		public void map_named_value_by_interface()
		{
			NamedInterfaceInjectee injectee = new NamedInterfaceInjectee();
			Interface value = new Clazz();
			injector.Map(typeof(Interface), InjectEnum.NAME).ToValue(value);
			injector.InjectInto(injectee);
			Assert.AreEqual(value, injectee.property, "Named value should have been injected");
		}

		[Test]
		public void map_falsy_value()
		{
			StringInjectee injectee = new StringInjectee();
			string value = "falsy";
			injector.Map(typeof(String)).ToValue(value);
			injector.InjectInto(injectee);
			Assert.AreEqual(value, injectee.property, "Value should have been injected");
		}

		[Test]
		public void mapped_value_is_not_injected_into()
		{
			RecursiveInterfaceInjectee injectee = new RecursiveInterfaceInjectee();
			InterfaceInjectee value = new InterfaceInjectee();
			injector.Map(typeof(InterfaceInjectee)).ToValue(value);
			injector.InjectInto(injectee);
			Assert.Null(value.property, "value shouldn't have been injected into");
		}

		[Test]
		public void map_multiple_interfaces_to_one_singleton_class()
		{
			MultipleSingletonsOfSameClassInjectee injectee = new MultipleSingletonsOfSameClassInjectee();
			injector.Map(typeof(Interface)).ToSingleton(typeof(Clazz));
			injector.Map(typeof(Interface2)).ToSingleton(typeof(Clazz));
			injector.InjectInto(injectee);
			Assert.NotNull(injectee.property1, "Singleton Value for 'property1' should have been injected");
			Assert.NotNull(injectee.property2, "Singleton Value for 'property2' should have been injected");
			Assert.AreNotEqual(injectee.property1, injectee.property2, "Singleton Values 'property1' and 'property2' should not be identical");
		}

		[Test]
		public void map_class_to_type_creates_new_instances()
		{
			ClassInjectee injectee = new ClassInjectee();
			ClassInjectee injectee2 = new ClassInjectee();
			injector.Map(typeof(Clazz)).ToType(typeof(Clazz));
			injector.InjectInto(injectee);
			Assert.NotNull(injectee.property, "Instance of Class should have been injected");
			injector.InjectInto(injectee2);
			Assert.AreNotEqual(injectee.property, injectee2.property, "Injected values should be different");
		}

		[Test]
		public void map_class_to_type_results_in_new_instances_being_injected_into()
		{
			ComplexClassInjectee injectee = new ComplexClassInjectee();
			Clazz value = new Clazz();
			injector.Map(typeof(Clazz)).ToValue(value);
			injector.Map(typeof(ComplexClazz)).ToType(typeof(ComplexClazz));
			injector.InjectInto(injectee);
			Assert.NotNull(injectee.property, "Complex Value should have been injected");
			Assert.AreEqual(value, injectee.property.value, "Nested value should have been injected");
		}

		[Test]
		public void map_interface_to_type()
		{
			InterfaceInjectee injectee = new InterfaceInjectee();
			injector.Map(typeof(Interface)).ToType(typeof(Clazz));
			injector.InjectInto(injectee);
			Assert.NotNull(injectee.property, "Instance of Class should have been injected");
		}
		
		[Test]
		public void map_class_to_type_by_name()
		{
			NamedClassInjectee injectee = new NamedClassInjectee();
			injector.Map(typeof(Clazz), NamedClassInjectee.NAME).ToType(typeof(Clazz));
			injector.InjectInto(injectee);
			Assert.NotNull(injectee.property, "Instance of named Class should have been injected");
		}
		
		[Test]
		public void map_interface_to_type_by_name()
		{
			NamedInterfaceInjectee injectee = new NamedInterfaceInjectee();
			injector.Map(typeof(Interface), InjectEnum.NAME).ToType(typeof(Clazz));
			injector.InjectInto(injectee);
			Assert.NotNull(injectee.property, "Instance of named Class should have been injected");
		}

		[Test]
		public void map_class_to_singleton_provides_single_instance()
		{
			ClassInjectee injectee = new ClassInjectee();
			ClassInjectee injectee2 = new ClassInjectee();
			injector.Map(typeof(Clazz)).ToSingleton(typeof(Clazz));
			injector.InjectInto(injectee);
			Assert.NotNull(injectee.property, "Instance of Class should have been injected");
			injector.InjectInto(injectee2);
			Assert.AreEqual(injectee.property, injectee2.property, "Injected values should be equal");
		}
		
		[Test]
		public void map_interface_to_singleton_provides_single_instance()
		{
			InterfaceInjectee injectee = new InterfaceInjectee();
			InterfaceInjectee injectee2 = new InterfaceInjectee();
			injector.Map(typeof(Interface)).ToSingleton(typeof(Clazz));
			injector.InjectInto(injectee);
			Assert.NotNull(injectee.property, "Instance of Class should have been injected");
			injector.InjectInto(injectee2);
			Assert.AreEqual(injectee.property, injectee2.property, "Injected values should be equal");
		}

		[Test]
		public void map_same_interface_with_different_names_to_different_singletons_provides_different_instances()
		{
			TwoNamedInterfaceFieldsInjectee injectee = new TwoNamedInterfaceFieldsInjectee();
			injector.Map(typeof(Interface), "Name1").ToSingleton(typeof(Clazz));
			injector.Map(typeof(Interface), "Name2").ToSingleton(typeof(Clazz2));
			injector.InjectInto(injectee);
			Assert.True(injectee.property1 is Clazz, "Property 'property1' should be of type 'Clazz'");
			Assert.True(injectee.property2 is Clazz2, "Property 'property2' should be of type 'Clazz2'");
			Assert.False(injectee.property1 == injectee.property2, "Properties 'property1' and 'property2' should have received different singletons");
		}
		
		[Test]
		public void setter_injection_fulfills_dependency()
		{
			SetterInjectee injectee = new SetterInjectee();
			SetterInjectee injectee2 = new SetterInjectee();
			injector.Map(typeof(Clazz)).ToType(typeof(Clazz));
			injector.InjectInto(injectee);
			Assert.NotNull(injectee.property, "Instance of Class should have been injected");
			injector.InjectInto(injectee2);
			Assert.False(injectee.property == injectee2.property, "Injected values should be different");
		}
		
		[Test]
		public void one_parameter_method_injection_receives_dependency()
		{
			OneParameterMethodInjectee injectee = new OneParameterMethodInjectee();
			OneParameterMethodInjectee injectee2 = new OneParameterMethodInjectee();
			injector.Map(typeof(Clazz)).ToType(typeof(Clazz));
			injector.InjectInto(injectee);
			Assert.NotNull(injectee.GetDependency(), "Instance of Class should have been injected");
			injector.InjectInto(injectee2);
			Assert.False(injectee.GetDependency() == injectee2.GetDependency(), "Injected values should be different");
		}
		
		[Test]
		public void one_named_parameter_method_injection_receives_dependency()
		{
			OneNamedParameterMethodInjectee injectee = new OneNamedParameterMethodInjectee();
			OneNamedParameterMethodInjectee injectee2 = new OneNamedParameterMethodInjectee();
			injector.Map(typeof(Clazz), InjectEnum.NAMED_DEP).ToType(typeof(Clazz));
			injector.InjectInto(injectee);
			Assert.NotNull(injectee.GetDependency(), "Instance of Class should have been injected for named Clazz parameter");
			injector.InjectInto(injectee2);
			Assert.False(injectee.GetDependency() == injectee2.GetDependency(), "Injected values should be different");
		}

		[Test]
		public void two_parameter_method_injection_receives_both_dependencies()
		{
			TwoParametersMethodInjectee injectee = new TwoParametersMethodInjectee();
			TwoParametersMethodInjectee injectee2 = new TwoParametersMethodInjectee();
			injector.Map(typeof(Clazz)).ToType(typeof(Clazz));
			injector.Map(typeof(Interface)).ToType(typeof(Clazz));
			injector.InjectInto(injectee);
			Assert.NotNull(injectee.GetDependency(), "Instance of Class should have been injected for unnamed Clazz parameter");
			Assert.NotNull(injectee.GetDependency2(), "Instance of Class should have been injected for unnamed Interface parameter");
			injector.InjectInto(injectee2);
			Assert.False(injectee.GetDependency() == injectee2.GetDependency(), "Injected values should be different");
			Assert.False(injectee.GetDependency2() == injectee2.GetDependency2(), "Injected values for Interface should be different");
		}
		
		[Test]
		public void two_named_parameter_method_injection_receives_both_dependencies()
		{
			TwoNamedParametersMethodInjectee injectee = new TwoNamedParametersMethodInjectee();
			TwoNamedParametersMethodInjectee injectee2 = new TwoNamedParametersMethodInjectee();
			injector.Map(typeof(Clazz), InjectEnum.NAMED_DEP).ToType(typeof(Clazz));
			injector.Map(typeof(Interface), InjectEnum.NAMED_DEP_2).ToType(typeof(Clazz));
			injector.InjectInto(injectee);
			Assert.NotNull(injectee.GetDependency(), "Instance of Class should have been injected for named Clazz parameter");
			Assert.NotNull(injectee.GetDependency2(), "Instance of Class should have been injected for named Interface parameter");
			injector.InjectInto(injectee2);
			Assert.False(injectee.GetDependency() == injectee2.GetDependency(), "Injected values should be different");
			Assert.False(injectee.GetDependency2() == injectee2.GetDependency2(), "Injected values for Interface should be different");
		}
		
		[Test]
		public void mixed_named_and_unnamed_parameters_in_method_injection_fulfilled()
		{
			MixedParametersMethodInjectee injectee = new MixedParametersMethodInjectee();
			MixedParametersMethodInjectee injectee2 = new MixedParametersMethodInjectee();
			injector.Map(typeof(Clazz), InjectEnum.NAMED_DEP).ToType(typeof(Clazz));
			injector.Map(typeof(Clazz)).ToType(typeof(Clazz));
			injector.Map(typeof(Interface), InjectEnum.NAMED_DEP_2).ToType(typeof(Clazz));
			injector.InjectInto(injectee);
			Assert.NotNull(injectee.GetDependency(), "Instance of Class should have been injected for named Clazz parameter");
			Assert.NotNull(injectee.GetDependency2(), "Instance of Class should have been injected for unnamed Clazz parameter");
			Assert.NotNull(injectee.GetDependency3(), "Instance of Class should have been injected for Interface");
			injector.InjectInto(injectee2);
			Assert.False(injectee.GetDependency() == injectee2.GetDependency(), "Injected values for named Clazz should be different");
			Assert.False(injectee.GetDependency2() == injectee2.GetDependency2(), "Injected values for unnamed Clazz should be different");
			Assert.False(injectee.GetDependency3() == injectee2.GetDependency3(), "Injected values for named Interface should be different");
		}
		
		[Test]
		public void one_parameter_constructor_injection_fulfilled()
		{
			injector.Map(typeof(Clazz));
			OneParameterConstructorInjectee injectee = injector.InstantiateUnmapped(typeof(OneParameterConstructorInjectee)) as OneParameterConstructorInjectee;
			Assert.NotNull(injectee.GetDependency(), "Instance of Class should have been injected for Clazz parameter");
		}

		[Test]
		public void two_parameter_constructor_injection_fulfilled()
		{
			injector.Map(typeof(Clazz));
			injector.Map(typeof(string)).ToValue("stringDependency");
			TwoParametersConstructorInjectee injectee = injector.InstantiateUnmapped(typeof(TwoParametersConstructorInjectee)) as TwoParametersConstructorInjectee;
			Assert.NotNull(injectee.GetDependency(), "Instance of Class should have been injected for named Clazz parameter");
			Assert.AreEqual(injectee.GetDependency2(), "stringDependency", "The String 'stringDependency' should have been injected for String parameter");
		}
		
		[Test]
		public void one_named_parameter_constructor_injection_fulfilled()
		{
			injector.Map(typeof(Clazz), InjectEnum.NAMED_DEPENDENCY).ToType(typeof(Clazz));
			OneNamedParameterConstructorInjectee injectee = injector.InstantiateUnmapped(typeof(OneNamedParameterConstructorInjectee)) as OneNamedParameterConstructorInjectee;
			Assert.NotNull(injectee.GetDependency(), "Instance of Class should have been injected for named Clazz parameter");
		}
		
		[Test]
		public void two_named_parameters_constructor_injection_fulfilled()
		{
			injector.Map(typeof(Clazz), InjectEnum.NAMED_DEPENDENCY).ToType(typeof(Clazz));
			injector.Map(typeof(string), InjectEnum.NAMED_DEPENDENCY_2).ToValue("stringDependency");
			TwoNamedParametersConstructorInjectee injectee = injector.InstantiateUnmapped(typeof(TwoNamedParametersConstructorInjectee)) as TwoNamedParametersConstructorInjectee;
			Assert.NotNull(injectee.GetDependency(), "Instance of Class should have been injected for named Clazz parameter");
			Assert.AreEqual(injectee.GetDependency2(), "stringDependency", "The String 'stringDependency' should have been injected for named String parameter");
		}
		
		[Test]
		public void mixed_named_and_unnamed_parameters_in_constructor_injection_fulfilled()
		{
			injector.Map(typeof(Clazz), InjectEnum.NAMED_DEP).ToType(typeof(Clazz));
			injector.Map(typeof(Clazz)).ToType(typeof(Clazz));
			injector.Map(typeof(Interface), InjectEnum.NAMED_DEP_2).ToType(typeof(Clazz));
			MixedParametersConstructorInjectee injectee = injector.InstantiateUnmapped(typeof(MixedParametersConstructorInjectee)) as MixedParametersConstructorInjectee;
			Assert.NotNull(injectee.GetDependency(), "Instance of Class should have been injected for named Clazz parameter");
			Assert.NotNull(injectee.GetDependency2(), "Instance of Class should have been injected for unnamed Clazz parameter");
			Assert.NotNull(injectee.GetDependency3(), "Instance of Class should have been injected for Interface");
		}

		[Test]
		public void named_array_injection_fulfilled()
		{
			string[] ac = new string[0];
			injector.Map(typeof(string[]), "namedCollection").ToValue(ac);
			NamedStringArrayInjectee injectee = injector.InstantiateUnmapped(typeof(NamedStringArrayInjectee)) as NamedStringArrayInjectee;
			Assert.NotNull(injectee.ac, "Instance 'ac' should have been injected for named ArrayCollection parameter");
			Assert.AreEqual(ac, injectee.ac, "Instance field 'ac' should be identical to local variable 'ac'");
		}

		[Test, ExpectedException(typeof(InjectorMissingMappingException))]
		public void halt_on_missing_interface_dependency()
		{
			injector.InjectInto(new InterfaceInjectee());
		}

		[Test]
		public void use_fallbackProvider_for_unmapped_dependency_if_given()
		{
			injector.fallbackProvider = new ProviderThatCanDoInterfaces(typeof(Clazz));
			ClassInjectee injectee = new ClassInjectee();
			injector.InjectInto(injectee);
			Assert.IsInstanceOf<Clazz>(injectee.property);
		}
		
		[Test, ExpectedException(typeof(InjectorMissingMappingException))]
		public void halt_on_missing_class_dependency_without_fallbackProvider()
		{
			ClassInjectee injectee = new ClassInjectee();
			injector.InjectInto(injectee);
		}
		
		[Test, ExpectedException(typeof(InjectorMissingMappingException))]
		public void halt_on_missing_named_dependency()
		{
			NamedClassInjectee injectee = new NamedClassInjectee();
			injector.InjectInto(injectee);
		}
		
		[Test]
		public void postConstruct_method_is_called()
		{
			ClassInjectee injectee = new ClassInjectee();
			Clazz value = new Clazz();
			injector.Map(typeof(Clazz)).ToValue(value);
			injector.InjectInto(injectee);
				
			Assert.True(injectee.someProperty);
		}

		[Test]
		public void postConstruct_method_with_arg_is_called_correctly()
		{
			injector.Map(typeof(Clazz));
			PostConstructWithArgInjectee injectee =
				injector.InstantiateUnmapped(typeof(PostConstructWithArgInjectee)) as PostConstructWithArgInjectee;
			Assert.IsInstanceOf<Clazz>(injectee.property);
		}

		[Test]
		public void postConstruct_methods_called_as_ordered()
		{
			OrderedPostConstructInjectee injectee = new OrderedPostConstructInjectee();
			injector.InjectInto(injectee);

			Assert.AreEqual(injectee.loadOrder, new int[]{1,2,3,4});
		}

		[Test]
		public void satisfies_is_false_for_unmapped_unnamed_interface()
		{
			Assert.False(injector.Satisfies(typeof(Interface)));
		}

		[Test]
		public void satisfies_is_false_for_unmapped_unnamed_class()
		{
			Assert.False(injector.Satisfies(typeof(Clazz)));
		}

		[Test]
		public void satisfies_is_false_for_unmapped_named_class()
		{
			Assert.False(injector.Satisfies(typeof(Clazz), "namedClass"));
		}

		[Test]
		public void satisfies_is_true_for_mapped_unnamed_class()
		{
			injector.Map(typeof(Clazz)).ToType(typeof(Clazz));
			Assert.True(injector.Satisfies(typeof(Clazz)));
		}

		[Test]
		public void satisfies_is_true_for_mapped_named_class()
		{
			injector.Map(typeof(Clazz), "namedClass").ToType(typeof(Clazz));
			Assert.True(injector.Satisfies(typeof(Clazz), "namedClass"));
		}

		[Test, ExpectedException(typeof(InjectorMissingMappingException))]
		public void get_instance_errors_for_unmapped_class()
		{
			injector.GetInstance(typeof(Clazz));
		}
		
		[Test]
		public void instantiateUnmapped_works_for_unmapped_class()
		{
			Assert.IsInstanceOf<Clazz>(injector.InstantiateUnmapped(typeof(Clazz)));
		}

		[Test, ExpectedException(typeof(InjectorMissingMappingException))]
		public void get_instance_errors_for_unmapped_named_class()
		{
			injector.GetInstance(typeof(Clazz), "namedClass");
		}

		[Test]
		public void getInstance_returns_mapped_value_for_mapped_unnamed_class()
		{
			Clazz clazz = new Clazz();
			injector.Map(typeof(Clazz)).ToValue(clazz);
			Assert.AreEqual(injector.GetInstance(typeof(Clazz)), clazz);
		}

		[Test]
		public void getInstance_returns_mapped_value_for_mapped_named_class()
		{
			Clazz clazz = new Clazz();
			injector.Map(typeof(Clazz), "namedClass").ToValue(clazz);
			Assert.AreEqual(injector.GetInstance(typeof(Clazz), "namedClass"), clazz);
		}

		[Test]
		public void unmapping_singleton_instance_removes_the_singleton()
		{
			injector.Map(typeof(Clazz)).ToSingleton(typeof(Clazz));
			ClassInjectee injectee1 = injector.InstantiateUnmapped<ClassInjectee>();
			injector.Unmap(typeof(Clazz));
			injector.Map(typeof(Clazz)).ToSingleton(typeof(Clazz));
			ClassInjectee injectee2 = injector.InstantiateUnmapped<ClassInjectee>();
			Assert.False(injectee1.property == injectee2.property, "injectee1.property is not the same instance as injectee2.property");
		}

		[Test, ExpectedException(typeof(InjectorInterfaceConstructionException))]
		public void instantiateUnmapped_on_interface_throws_InjectorInterfaceConstructionError()
		{
			injector.fallbackProvider = new ProviderThatCanDoInterfaces(typeof(Clazz));
			injector.InstantiateUnmapped(typeof(Interface));
		}

		[Test, ExpectedException(typeof(InjectorMissingMappingException))]
		public void getInstance_on_unmapped_interface_with_no_fallback_throws_InjectorMissingMappingError()
		{
			injector.GetInstance(typeof(Interface));
		}
		
		[Test, ExpectedException(typeof(InjectorMissingMappingException))]
		public void getInstance_on_unmapped_class_with_fallback_provider_that_doesnt_satisfy_throws_InjectorMissingMappingError()
		{
			injector.fallbackProvider = new MoodyProvider(false);
			injector.GetInstance (typeof(Clazz));
		}

		[Test]
		public void instantiateUnmapped_doesnt_throw_when_attempting_unmapped_optional_property_injection()
		{
			OptionalClassInjectee injectee = injector.InstantiateUnmapped<OptionalClassInjectee>();
			Assert.Null(injectee.property, "injectee mustn't contain Clazz instance");
		}

		[Test]
		public void getInstance_doesnt_throw_when_attempting_unmapped_optional_method_injection()
		{
			OptionalOneRequiredParameterMethodInjectee injectee =
					injector.InstantiateUnmapped<OptionalOneRequiredParameterMethodInjectee>();
			Assert.Null(injectee.GetDependency(), "injectee mustn't contain Interface instance");
		}

		[Test]
		public void soft_mapping_is_used_if_no_parent_injector_available()
		{
			injector.Map(typeof(Interface)).Softly().ToType(typeof(Clazz));
			Assert.NotNull(injector.GetInstance(typeof(Interface)));
		}

		[Test]
		public void parent_mapping_is_used_instead_of_soft_child_mapping()
		{
			Injector childInjector = injector.CreateChildInjector();
			injector.Map(typeof(Interface)).ToType(typeof(Clazz));
			childInjector.Map(typeof(Interface)).Softly().ToType(typeof(Clazz2));
			Assert.True(childInjector.GetInstance<Interface>() is Clazz);
		}

		[Test]
		public void local_mappings_are_used_in_own_injector()
		{
			injector.Map(typeof(Interface)).Locally().ToType(typeof(Clazz));
			Assert.NotNull(injector.GetInstance<Interface>());
		}

		[Test, ExpectedException(typeof(InjectorMissingMappingException))]
		public void local_mappings_arent_shared_with_child_injectors()
		{
			Injector childInjector = injector.CreateChildInjector();
			injector.Map(typeof(Interface)).Locally().ToType(typeof(Clazz));
			childInjector.GetInstance<Interface>();
		}

//		[Test]
//		public void injector_dispatches_POST_INSTANTIATE_event_during_instance_construction()
//		{
//			Assert.True(constructMappedTypeAndListenForEvent(injector.POST_INSTANTIATE));
//		}

		/*
		[Test]
		public void injector_dispatches_PRE_CONSTRUCT_event_during_instance_construction()
		{
			assertThat(constructMappedTypeAndListenForEvent(InjectionEvent.PRE_CONSTRUCT), isTrue());
		}

		[Test]
		public void injector_dispatches_POST_CONSTRUCT_event_after_instance_construction()
		{
			assertThat(constructMappedTypeAndListenForEvent(InjectionEvent.POST_CONSTRUCT), isTrue());
		}

		[Test]
		public void injector_events_after_instantiate_contain_created_instance()
		{
			function listener(event : InjectionEvent)
			{
				assertThat(event, hasPropertyWithValue('instance', isA(Clazz)));
			}
			injector.Map(typeof(Clazz));
			injector.addEventListener(InjectionEvent.POST_INSTANTIATE, listener);
			injector.addEventListener(InjectionEvent.PRE_CONSTRUCT, listener);
			injector.addEventListener(InjectionEvent.POST_CONSTRUCT, listener);
			const instance : Clazz = injector.GetInstance(Clazz);
		}

		[Test]
		public void injectInto_dispatches_PRE_CONSTRUCT_event_during_object_construction()
		{
			assertThat(injectIntoInstanceAndListenForEvent(InjectionEvent.PRE_CONSTRUCT), isTrue());
		}

		[Test]
		public void injectInto_dispatches_POST_CONSTRUCT_event_during_object_construction()
		{
			assertThat(injectIntoInstanceAndListenForEvent(InjectionEvent.POST_CONSTRUCT), isTrue());
		}

		[Test]
		public void injector_dispatches_PRE_MAPPING_CREATE_event_before_creating_new_mapping()
		{
			assertThat(createMappingAndListenForEvent(MappingEvent.PRE_MAPPING_CREATE));
		}

		[Test]
		public void injector_dispatches_POST_MAPPING_CREATE_event_after_creating_new_mapping()
		{
			assertThat(createMappingAndListenForEvent(MappingEvent.POST_MAPPING_CREATE));
		}

		[Test]
		public void injector_dispatches_PRE_MAPPING_CHANGE_event_before_changing_mapping_provider()
		{
			injector.Map(typeof(Clazz));
			listenToInjectorEvent(MappingEvent.PRE_MAPPING_CHANGE);
			injector.Map(typeof(Clazz)).AsSingleton();
			assertThat(receivedInjectorEvents.pop(), equalTo(MappingEvent.PRE_MAPPING_CHANGE));
		}

		[Test]
		public void injector_dispatches_POST_MAPPING_CHANGE_event_after_changing_mapping_provider()
		{
			injector.Map(typeof(Clazz));
			listenToInjectorEvent(MappingEvent.POST_MAPPING_CHANGE);
			injector.Map(typeof(Clazz)).AsSingleton();
			assertThat(receivedInjectorEvents.pop(), equalTo(MappingEvent.POST_MAPPING_CHANGE));
		}

		[Test]
		public void injector_dispatches_PRE_MAPPING_CHANGE_event_before_changing_mapping_strength()
		{
			injector.Map(typeof(Clazz));
			listenToInjectorEvent(MappingEvent.PRE_MAPPING_CHANGE);
			injector.Map(typeof(Clazz)).Softly();
			assertThat(receivedInjectorEvents.pop(), equalTo(MappingEvent.PRE_MAPPING_CHANGE));
		}

		[Test]
		public void injector_dispatches_POST_MAPPING_CHANGE_event_after_changing_mapping_strength()
		{
			injector.Map(typeof(Clazz));
			listenToInjectorEvent(MappingEvent.POST_MAPPING_CHANGE);
			injector.Map(typeof(Clazz)).Softly();
			assertThat(receivedInjectorEvents.pop(), equalTo(MappingEvent.POST_MAPPING_CHANGE));
		}

		[Test]
		public void injector_dispatches_PRE_MAPPING_CHANGE_event_before_changing_mapping_scope()
		{
			injector.Map(typeof(Clazz));
			listenToInjectorEvent(MappingEvent.PRE_MAPPING_CHANGE);
			injector.Map(typeof(Clazz)).Locally();
			assertThat(receivedInjectorEvents.pop(), equalTo(MappingEvent.PRE_MAPPING_CHANGE));
		}

		[Test]
		public void injector_dispatches_POST_MAPPING_CHANGE_event_after_changing_mapping_scope()
		{
			injector.Map(typeof(Clazz));
			listenToInjectorEvent(MappingEvent.POST_MAPPING_CHANGE);
			injector.Map(typeof(Clazz)).Locally();
			assertThat(receivedInjectorEvents.pop(), equalTo(MappingEvent.POST_MAPPING_CHANGE));
		}

		[Test]
		public void injector_dispatches_POST_MAPPING_REMOVE_event_after_removing_mapping()
		{
			injector.Map(typeof(Clazz));
			listenToInjectorEvent(MappingEvent.POST_MAPPING_REMOVE);
			injector.Unmap(Clazz);
			assertThat(receivedInjectorEvents.pop(), equalTo(MappingEvent.POST_MAPPING_REMOVE));
		}

		[Test]
		public void injector_throws_when_trying_to_create_mapping_for_same_type_from_pre_mapping_create_handler()
		{
			var errorThrown : Boolean;
			injector.addEventListener(MappingEvent.PRE_MAPPING_CREATE,
				function(event : MappingEvent) : void
			{
				try
				{
					injector.Map(typeof(Clazz));
				}
				catch (error : InjectorError)
				{
					errorThrown = true;
				}
			});
			injector.Map(typeof(Clazz));
			assertThat(errorThrown, isTrue());
		}

		[Test]
		public void injector_throws_when_trying_to_create_mapping_for_same_type_from_post_mapping_create_handler()
		{
			var errorThrown : Boolean;
			injector.addEventListener(MappingEvent.POST_MAPPING_CREATE,
				function(event : MappingEvent) : void
				{
					try
					{
						injector.Map(typeof(Clazz)).Locally();
					}
					catch (error : InjectorError)
					{
						errorThrown = true;
					}
				});
			injector.Map(typeof(Clazz));
			assertThat(errorThrown, isTrue());
		}
		*/

//		private bool constructMappedTypeAndListenForEvent(Delegate eventType)
//		{
//			injector.Map(typeof(Clazz));
//			listenToInjectorEvent(eventType);
//			injector.GetInstance<Clazz>();
//			return receivedInjectorEvents.pop() == eventType;
//		}
		
		/*
		private function injectIntoInstanceAndListenForEvent(eventType : String) : Boolean
		{
			const injectee : ClassInjectee = new ClassInjectee();
			injector.Map(typeof(Clazz)).ToValue(new Clazz());
			listenToInjectorEvent(eventType);
			injector.InjectInto(injectee);
			return receivedInjectorEvents.pop() == eventType;
		}

		private function createMappingAndListenForEvent(eventType : String) : Boolean
		{
			listenToInjectorEvent(eventType);
			injector.Map(typeof(Clazz));
			return receivedInjectorEvents.pop() == eventType;
		}
		*/

//		private void listenToInjectorEvent(event eventType)
//		{
//			/*
//			injector.addEventListener(eventType, function(event : Event) : void
//			{
//				receivedInjectorEvents.push(event.type);
//			});
//			*/
//		}

		// Hmm, optional parameters injection for provider
//		[Test]
//		public void injector_makes_inject_parameters_available_to_providers()
//		{
//			UnknownParametersUsingProvider provider = new UnknownParametersUsingProvider();
//			injector.Map(typeof(Clazz)).ToProvider(provider);
//			injector.InstantiateUnmapped(UnknownInjectParametersListInjectee);
//			assertThat(provider.parameterValue, equalTo('true,str,123'));
//		}

		// TypeDescription Tests
		/*
		[Test]
		public void injector_uses_manually_supplied_type_description_for_field()
		{
			TypeDescription description = new TypeDescription();
			description.AddFieldInjection("property", typeof(Clazz));
			injector.AddTypeDescription(NamedClassInjectee, description);
			injector.Map(typeof(Clazz));
			NamedClassInjectee injectee = injector.InstantiateUnmapped(NamedClassInjectee);
			Assert.IsInstanceOf<Clazz>(injectee.property);
		}
		
		[Test]
		public void injector_uses_manually_supplied_type_description_for_method()
		{
			const description : TypeDescription = new TypeDescription();
			description.AddMethodInjection('setDependency', [Clazz]);
			injector.AddTypeDescription(OneNamedParameterMethodInjectee, description);
			injector.Map(typeof(Clazz));
			const injectee : OneNamedParameterMethodInjectee =
				injector.InstantiateUnmapped(OneNamedParameterMethodInjectee);
			assertThat(injectee.GetDependency(), isA(Clazz));
		}

		[Test]
		public void injector_uses_manually_supplied_type_description_for_ctor()
		{
			const description : TypeDescription = new TypeDescription(false);
			description.setConstructor([Clazz]);
			injector.AddTypeDescription(OneNamedParameterConstructorInjectee, description);
			injector.Map(typeof(Clazz));
			const injectee : OneNamedParameterConstructorInjectee =
				injector.InstantiateUnmapped(OneNamedParameterConstructorInjectee);
			assertThat(injectee.GetDependency(), isA(Clazz));
		}

		[Test]
		public void injector_uses_manually_supplied_type_description_for_PostConstruct_method()
		{
			const description : TypeDescription = new TypeDescription();
			description.addPostConstructMethod('doSomeStuff', [Clazz]);
			injector.AddTypeDescription(PostConstructWithArgInjectee, description);
			injector.Map(typeof(Clazz));
			const injectee : PostConstructWithArgInjectee =
				injector.InstantiateUnmapped(PostConstructWithArgInjectee);
			assertThat(injectee.property, isA(Clazz));
		}
		*/

		// Funky post construct methods
		/*
		[Test]
		public void injector_executes_injected_PostConstruct_method_vars()
		{
			var callbackInvoked : Boolean;
			injector.Map(Function).ToValue(function() : void {callbackInvoked = true});
			injector.InstantiateUnmapped(PostConstructInjectedVarInjectee);
			assertThat(callbackInvoked, isTrue());
		}

		[Test]
		public void injector_executes_injected_PostConstruct_method_vars_in_injectee_scope()
		{
			injector.Map(Function).ToValue(function() : void {this.property = new Clazz();});
			const injectee : PostConstructInjectedVarInjectee =
				injector.InstantiateUnmapped(PostConstructInjectedVarInjectee);
			assertThat(injectee.property, isA(Clazz));
		}
		*/

		[Test]
		public void unmapping_singleton_provider_invokes_PreDestroy_methods_on_singleton()
		{
			injector.Map(typeof(Clazz)).AsSingleton();
			Clazz singleton = injector.GetInstance<Clazz>();
			Assert.False (singleton.preDestroyCalled);
			injector.Unmap(typeof(Clazz));
			Assert.True (singleton.preDestroyCalled);
		}

		[Test]
		public void destroyInstance_invokes_PreDestroy_methods_on_instance()
		{
			Clazz target = new Clazz();
			Assert.False (target.preDestroyCalled);
			injector.DestroyInstance(target);
			Assert.True (target.preDestroyCalled);
		}

		[Test]
		public void teardown_destroys_all_singletons()
		{
			injector.Map(typeof(Clazz)).AsSingleton();
			injector.Map(typeof(Interface)).ToSingleton(typeof(Clazz));
			Clazz singleton1 = injector.GetInstance<Clazz>();
			Clazz singleton2 = injector.GetInstance(typeof(Interface)) as Clazz;
			Assert.False (singleton1.preDestroyCalled);
			Assert.False (singleton2.preDestroyCalled);
			injector.Teardown();
			Assert.True (singleton1.preDestroyCalled);
			Assert.True (singleton2.preDestroyCalled);
		}

		[Test]
		public void teardown_destroys_all_instances_it_injected_into()
		{
			Clazz target1 = new Clazz();
			injector.InjectInto(target1);
			injector.Map(typeof(Clazz));
			Clazz target2 = injector.GetInstance<Clazz>();
			Assert.False (target1.preDestroyCalled);
			Assert.False (target2.preDestroyCalled);
			injector.Teardown();
			Assert.True (target1.preDestroyCalled);
			Assert.True (target2.preDestroyCalled);
		}
		
		[Test]
		public void fallbackProvider_is_null_by_default()
		{
			Assert.Null(injector.fallbackProvider);
		}

		[Test]
		public void satisfies_isTrue_if_fallbackProvider_satisifies()
		{
			injector.fallbackProvider = new MoodyProvider(true);
			Assert.True(injector.Satisfies(typeof(Clazz)));
		}
		
		[Test]
		public void satisfies_isFalse_if_fallbackProvider_doesnt_satisfy()
		{
			injector.fallbackProvider = new MoodyProvider(false);
			Assert.False(injector.Satisfies(typeof(Clazz)));
		}

		[Test]
		public void satisfies_returns_false_without_error_if_fallback_provider_cannot_satisfy_request()
		{
			injector.fallbackProvider = new MoodyProvider(false);
			Assert.False(injector.Satisfies<Interface>());
		}
		
		[Test]
		public void satisfies_returns_true_without_error_if_interface_requested_from_ProviderThatCanDoInterfaces()
		{
			injector.fallbackProvider = new ProviderThatCanDoInterfaces(typeof(Clazz));
			Assert.True(injector.Satisfies<Interface>());
		}

		//TODO: Add this test as we are checking for base types, note: object is mapped and shouldn't be
//		[Test]
//		public void satisfies_returns_false_for_unmapped_common_base_types()
//		{
//			injector.fallbackProvider = new MoodyProvider(true);
//			Type[] baseTypes = new Type[]{typeof(bool), typeof(int), typeof(float), typeof(uint), typeof(object), typeof(double), typeof(string)};
//			// yes, loops in tests are bad, but this test case is already 1000 lines long!
//			for (uint i = 0; i < baseTypes.Length; i++)
//			{
//				Assert.False(injector.Satisfies(baseTypes[i]));
//			}
//		}
		
		[Test]
		public void satisfiesDirectly_isTrue_if_fallbackProvider_satisifies()
		{
			injector.fallbackProvider = new MoodyProvider(true);
			Assert.True(injector.SatisfiesDirectly(typeof(Clazz)));
		}

		[Test]
		public void satisfiesDirectly_isFalse_if_no_local_fallbackProvider()
		{
			injector.fallbackProvider = new MoodyProvider(true);
			Injector childInjector = injector.CreateChildInjector();
			Assert.False(childInjector.SatisfiesDirectly(typeof(Clazz)));
		}
		
		[Test]
		public void instantiateUnmapped_returns_new_instance_even_if_mapped_instance_exists()
		{
			Clazz mappedValue = new Clazz();
			injector.Map(typeof(Clazz)).ToValue(mappedValue);
			Clazz instance = injector.InstantiateUnmapped<Clazz>();
			Assert.AreNotEqual(instance, mappedValue);
		}

		[Test]
		public void hasMapping_returns_true_for_parent_mappings()
		{
			injector.Map(typeof(Clazz)).ToValue(new Clazz());
			Injector childInjector = injector.CreateChildInjector();
			Assert.True(childInjector.HasMapping(typeof(Clazz)));
		}
		
		[Test]
		public void hasMapping_returns_true_for_local_mappings()
		{
			injector.Map(typeof(Clazz)).ToValue(new Clazz());
			Assert.True(injector.HasMapping(typeof(Clazz)));
		}
		
		[Test]
		public void hasMapping_returns_false_where_mapping_doesnt_exist()
		{
			Assert.False(injector.HasMapping(typeof(Clazz)));
		}
		
		[Test]
		public void hasDirectMapping_returns_false_for_parent_mappings()
		{
			injector.Map(typeof(Clazz)).ToValue(new Clazz());
			Injector childInjector = injector.CreateChildInjector();
			Assert.False(childInjector.HasDirectMapping(typeof(Clazz)));
			
		}

		[Test]
		public void hasDirectMapping_returns_true_for_local_mappings()
		{
			injector.Map(typeof(Clazz)).ToValue(new Clazz());
			Assert.True(injector.HasDirectMapping(typeof(Clazz)));
		}
		
		[Test]
		public void getOrCreateNewInstance_provides_mapped_value_where_mapping_exists()
		{
			injector.Map(typeof(Clazz)).AsSingleton();
			Clazz instance1 = injector.GetOrCreateNewInstance(typeof(Clazz)) as Clazz;
			Clazz instance2 = injector.GetOrCreateNewInstance(typeof(Clazz)) as Clazz;
			Assert.AreEqual(instance1, instance2);
		}

		[Test]
		public void getOrCreateNewInstance_instantiates_new_instance_where_no_mapping_exists()
		{
			Clazz instance1 = injector.GetOrCreateNewInstance(typeof(Clazz)) as Clazz;
			Assert.IsInstanceOf<Clazz>(instance1);
		}
		
		[Test]
		public void getOrCreateNewInstance_instantiates_new_instances_each_time_where_no_mapping_exists()
		{
			Clazz instance1 = injector.GetOrCreateNewInstance(typeof(Clazz)) as Clazz;
			Clazz instance2 = injector.GetOrCreateNewInstance(typeof(Clazz)) as Clazz;
			Assert.AreNotEqual(instance1, instance2);
		}

		[Test]
		public void satisfies_doesnt_use_fallbackProvider_from_ancestors_if_blockParentFallbackProvider_is_set()
		{
			injector.fallbackProvider = new ProviderThatCanDoInterfaces(typeof(Clazz));
			Injector childInjector = injector.CreateChildInjector();
			childInjector.blockParentFallbackProvider = true;
			Assert.False(childInjector.Satisfies<Clazz>());
		}
		
		[Test, ExpectedException(typeof(InjectorMissingMappingException))]
		public void getInstance_doesnt_use_fallbackProvider_from_ancestors_if_blockParentFallbackProvider_is_set()
		{
			injector.fallbackProvider = new ProviderThatCanDoInterfaces(typeof(Clazz));
			Injector childInjector = injector.CreateChildInjector();
			childInjector.blockParentFallbackProvider = true;
			childInjector.GetInstance(typeof(Clazz));
		}

		// Not an XML test...
		[Test]
		public void performXMLConfiguredConstructorInjectionWithOneNamedParameter()
		{
			injector = new Injector();
			injector.Map(typeof(Clazz), InjectEnum.NAMED_DEPENDENCY).ToType(typeof(Clazz));
			OneNamedParameterConstructorInjectee injectee = injector.InstantiateUnmapped<OneNamedParameterConstructorInjectee>();
			Assert.NotNull(injectee.GetDependency(), "Instance of Class should have been injected for named Clazz parameter");
		}

		[Test]
		public void performOtherMappingInjection()
		{
			InjectionMapping mapping = injector.Map(typeof(Interface));
			mapping.ToSingleton(typeof(Clazz));
			injector.Map(typeof(Interface2)).ToProvider(new OtherMappingProvider(mapping));
			MultipleSingletonsOfSameClassInjectee injectee = injector.InstantiateUnmapped<MultipleSingletonsOfSameClassInjectee>();
			Assert.AreEqual(injectee.property1, injectee.property2, "Instance field 'property1' should be identical to Instance field 'property2'");
		}

		[Test]
		public void performNamedOtherMappingInjection()
		{
			InjectionMapping mapping = injector.Map(typeof(Interface));
			mapping.ToSingleton(typeof(Clazz));
			injector.Map(typeof(Interface2)).ToProvider(new OtherMappingProvider(mapping));
			injector.Map(typeof(Interface), "name1").ToProvider(new OtherMappingProvider(mapping));
			injector.Map(typeof(Interface2), "name2").ToProvider(new OtherMappingProvider(mapping));
			MultipleNamedSingletonsOfSameClassInjectee injectee = injector.InstantiateUnmapped<MultipleNamedSingletonsOfSameClassInjectee>();
			Assert.AreEqual(injectee.property1, injectee.property2, "Instance field 'property1' should be identical to Instance field 'property2'");
			Assert.AreEqual(injectee.property1, injectee.namedProperty1, "Instance field 'property1' should be identical to Instance field 'namedProperty1'");
			Assert.AreEqual(injectee.property1, injectee.namedProperty2, "Instance field 'property1' should be identical to Instance field 'namedProperty2'");
		}

		[Test]
		public void two_parameters_constructor_injection_with_constructor_injected_dependencies_fulfilled()
		{
			injector.Map(typeof(Clazz));
			injector.Map(typeof(OneParameterConstructorInjectee));
			injector.Map(typeof(TwoParametersConstructorInjectee));
			injector.Map(typeof(String)).ToValue("stringDependency");

			TwoParametersConstructorInjecteeWithConstructorInjectedDependencies injectee = 
				injector.InstantiateUnmapped<TwoParametersConstructorInjecteeWithConstructorInjectedDependencies>();
			Assert.NotNull(injectee.GetDependency1(), "Instance of Class should have been injected for OneParameterConstructorInjectee parameter");
			Assert.NotNull(injectee.GetDependency2(), "Instance of Class should have been injected for TwoParametersConstructorInjectee parameter");
		}
	}
}
