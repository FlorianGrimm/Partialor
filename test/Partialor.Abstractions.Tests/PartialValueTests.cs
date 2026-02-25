namespace Partialor;

public class PartialValueTests {
    [Test]
    public async Task PartialValueClass() {
        TestClass sut = new();
        sut.A = Partialor.PartialValue<int>.WithValue(1);
        await Assert.That(sut.A.HasValue).IsTrue();
        await Assert.That(sut.A.Value).IsEqualTo(1);
        await Assert.That(sut.A.TryGetValue(out var x) ? x : 0).IsEqualTo(1);

        await Assert.That(sut.B.HasValue).IsFalse();
        Assert.Throws<NullReferenceException>(() => { _ = sut.B.Value; });
        await Assert.That(sut.B.TryGetValue(out _)).IsFalse();

        sut.C = 1;
        await Assert.That(sut.C.Value).IsEqualTo(1);

        sut.C = new PartialNoValue();
        await Assert.That(sut.C.HasValue).IsFalse();
    }
}

public class TestClass {
    public PartialValue<int> A { get; set; }
    public PartialValue<int> B { get; set; }
    public PartialValue<int> C { get; set; }
}
