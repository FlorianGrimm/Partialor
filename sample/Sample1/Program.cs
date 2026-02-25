namespace Sample1;

public class Program {
    public static async Task Main(string[] args) {
        {
            Abc abc = new Abc() {
                A = "1",
                B = 1,
                C = 1,
            };
            PartialAbc partialAbc1 = new PartialAbc() {
                A = "two"
            };
            PartialAbc partialAbc2 = new PartialAbc() {
                B = 2
            };
            PartialAbc partialAbc3 = new PartialAbc() {
                B = 3,
                C = 3
            };

            var abc1 = partialAbc1.MergeWith(abc);
            if (!(abc1.A == "two")) { throw new InvalidOperationException(); }

            var abc2 = partialAbc2.MergeWith(abc);
            if (!(abc2.B == 2)) { throw new InvalidOperationException(); }

            var abc3 = partialAbc3.MergeWith(abc);
            if (!((abc3.B == 3) && (abc3.C == 3))) { throw new InvalidOperationException(); }
        }
        {
            Abc abc = new Abc() {
                A = "1",
                B = 1,
                C = 1,
            };
            PartialAbc partialAbc1 = new PartialAbc() {
                A = "two"
            };
            PartialAbc partialAbc2 = new PartialAbc() {
                B = 2
            };
            PartialAbc partialAbc3 = new PartialAbc() {
                B = 3,
                C = 3
            };

            partialAbc1.CopyTo(abc);
            if (!(abc.A == "two")) { throw new InvalidOperationException(); }

            partialAbc2.CopyTo(abc);
            if (!(abc.B == 2)) { throw new InvalidOperationException(); }

            partialAbc3.CopyTo(abc);
            if (!((abc.B == 3) && (abc.C == 3))) { throw new InvalidOperationException(); }
        }

        System.Console.Out.WriteLine("~ fin ~");
    }
}
