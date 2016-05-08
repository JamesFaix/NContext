﻿namespace NContext.Tests.Specs.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FakeItEasy;

    using Machine.Specifications;

    using NContext.Text;
    
    public class with_a_derived_Dictionary_of_Object_values : when_sanitizing_objects_with_ObjectGraphSanitizer
    {
        Establish context = () =>
            {
                TextSanitizer = A.Fake<ISanitizeText>();

                A.CallTo(() => TextSanitizer.SanitizeHtmlFragment(A<string>._))
                    .Returns(_SanitizedValue);

                _Data = new Dictionary<String, Object>
                {
                    {"AuthorId", Guid.Empty},
                    {"FirstName", "Daniel"},
                    {"LastName", "Gioulakis"},
                    {"Email", null}
                };
            };

        Because of = () => Sanitize(_Data);

        It should_sanitize_only_dictionary_string_values = 
            () => _Data.Select(item => item.Value).ShouldContainOnly(Guid.Empty, _SanitizedValue, _SanitizedValue, null);

        static IDictionary<String, Object> _Data;

        const String _SanitizedValue = "ncontext";
    }
}