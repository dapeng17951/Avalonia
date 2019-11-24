// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Reactive.Subjects;
using Xunit;

namespace Avalonia.Base.UnitTests
{
    public class AvaloniaObjectTests_Validation
    {
        [Fact]
        public void Registration_Throws_If_DefaultValue_Fails_Validation()
        {
            Assert.Throws<ArgumentException>(() =>
                new StyledProperty<int>(
                    "BadDefault",
                    typeof(Class1),
                    new StyledPropertyMetadata<int>(101),
                    validate: Class1.ValidateFoo));
        }

        [Fact]
        public void Metadata_Override_Throws_If_DefaultValue_Fails_Validation()
        {
            Assert.Throws<ArgumentException>(() => Class1.FooProperty.OverrideDefaultValue<Class2>(101));
        }

        [Fact]
        public void SetValue_Throws_If_Fails_Validation()
        {
            var target = new Class1();

            Assert.Throws<ArgumentException>(() => target.SetValue(Class1.FooProperty, 101));
        }

        [Fact]
        public void Reverts_To_DefaultValue_If_Binding_Fails_Validation()
        {
            var target = new Class1();
            var source = new Subject<int>();

            target.Bind(Class1.FooProperty, source);
            source.OnNext(150);

            Assert.Equal(11, target.GetValue(Class1.FooProperty));
        }

        [Fact]
        public void Reverts_To_DefaultValue_Even_In_Presence_Of_Other_Bindings()
        {
            var target = new Class1();
            var source1 = new Subject<int>();
            var source2 = new Subject<int>();

            target.Bind(Class1.FooProperty, source1);
            target.Bind(Class1.FooProperty, source2);
            source1.OnNext(42);
            source2.OnNext(150);

            Assert.Equal(11, target.GetValue(Class1.FooProperty));
        }

        private class Class1 : AvaloniaObject
        {
            public static readonly StyledProperty<int> FooProperty =
                AvaloniaProperty.Register<Class1, int>(
                    "Qux",
                    defaultValue: 11,
                    validate: ValidateFoo);

            public static bool ValidateFoo(int value)
            {
                return value < 100;
            }
        }

        private class Class2 : AvaloniaObject
        {
        }
    }
}