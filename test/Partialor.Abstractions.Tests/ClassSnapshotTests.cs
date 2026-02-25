namespace Partialor;

public class ClassSnapshotTests {
    [Test]
    public async Task VerifyChecksTest() {
        await VerifyTUnit.VerifyChecks.Run();
    }

    private static VerifySettings Settings([CallerMemberName] string method = "") {
        var settings = new VerifySettings();
        settings.UseFileName(method);
        return settings;
    }

    [Test]
    public Task Normal() {
        var source = """
        namespace MySpace;

        [Partialor.Partial()]
        public class Model
        {
            /// <summary>
            /// Name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Age
            /// </summary>
            public int Age { get; set; }
        }
        """;

        var runResult = TestHelper.GeneratorDriver(source, typeof(Partialor.PartialAttribute).Assembly)
            .GetRunResult()
            .GetFirstResult();
        var settings = Settings();
        return Verify(runResult, settings).UseDirectory("Results/Classes");
    }

    [Test]
    public Task Custom_class_name() {
        var source = """
        using Partialor;

        namespace MySpace;

        /// <summary>
        /// Class:
        ///    [Partial(PartialClassName = "ModelPartialCustom")]
        ///    public class Model
        /// </summary>
        [Partial(PartialClassName = "ModelPartialCustom")]
        public class Model
        {
            /// <summary>
            /// input:
            ///    public string Name { get; set; }
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Age
            /// </summary>
            public int Age { get; set; }
        }
        """;

        var runResult = TestHelper.GeneratorDriver(source, typeof(Partialor.PartialAttribute).Assembly)
            .GetRunResult()
            .GetFirstResult()
            ;
        var settings = Settings();
        return Verify(runResult, settings).UseDirectory("Results/Classes");
    }
}