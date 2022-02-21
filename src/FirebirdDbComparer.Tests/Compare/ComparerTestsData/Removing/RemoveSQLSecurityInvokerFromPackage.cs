using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.Tests.Compare.ComparerTestsData.Removing;

public class RemoveSQLSecurityInvokerFromPackage : ComparerTests.TestCaseStructure
{
    public override bool IsCompatibleWithVersion(TargetVersion targetVersion)
    {
        return targetVersion.AtLeast(TargetVersion.Version40);
    }

    public override string Source => @"
set term ^;

create package pkg
as
begin
end^

set term ;^
";

    public override string Target => @"
set term ^;

create package pkg
sql security invoker
as
begin
end^

set term ;^
";
}
