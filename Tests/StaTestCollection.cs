using Xunit;

[CollectionDefinition("STA Tests", DisableParallelization = true)]
public class StaTestCollection : ICollectionFixture<StaFixture> { }

public class StaFixture { }

