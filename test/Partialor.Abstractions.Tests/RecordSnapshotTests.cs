namespace Partialor;

public class RecordSnapshotTests {
    private static VerifySettings Settings([CallerMemberName] string method = "") {
        var settings = new VerifySettings();
        settings.UseFileName(method);
        return settings;
    }
#if false
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
#endif

    [Test]
    public Task Custom_record_name() {
        var source = """
        using Partialor;

        namespace MySpace;

        /// <summary>
        /// Class:
        ///    [Partial(PartialClassName = "ModelPartialCustom")]
        ///    public class Model
        /// </summary>
        [Partial(PartialClassName = "ModelPartialCustom")]
        public record class Model(
            string Name,
            int Age
        ) {
            public string Other { get; set; }
            // public int Age2 => this.Age*2;
        }
        """;

        var runResult = TestHelper.GeneratorDriver(source, typeof(Partialor.PartialAttribute).Assembly)
            .GetRunResult()
            .GetFirstResult()
            ;
        var settings = Settings();
        return Verify(runResult, settings).UseDirectory("Results/Records");
    }
}