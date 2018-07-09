using System.Collections.Generic;
using System.Linq;
using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects.Implementations
{
    public class MetadataProcedures30 : MetadataProcedures25, ISupportsComment
    {
        public MetadataProcedures30(IMetadata metadata, ISqlHelper sqlHelper)
            : base(metadata, sqlHelper)
        { }

        protected override string ProcedureCommandText => @"
select trim(P.RDB$PROCEDURE_NAME) as RDB$PROCEDURE_NAME,
       P.RDB$PROCEDURE_ID,
       P.RDB$PROCEDURE_INPUTS,
       P.RDB$PROCEDURE_OUTPUTS,
       P.RDB$DESCRIPTION,
       P.RDB$PROCEDURE_SOURCE,
       trim(P.RDB$OWNER_NAME) as RDB$OWNER_NAME,
       P.RDB$PROCEDURE_TYPE,
       P.RDB$SYSTEM_FLAG,
       trim(P.RDB$ENGINE_NAME) as RDB$ENGINE_NAME,
       trim(P.RDB$ENTRYPOINT) as RDB$ENTRYPOINT,
       trim(P.RDB$PACKAGE_NAME) as RDB$PACKAGE_NAME,
       P.RDB$PRIVATE_FLAG
  from RDB$PROCEDURES P";

        protected override string ProcedureParameterCommandText => @"
select trim(PP.RDB$PARAMETER_NAME) as RDB$PARAMETER_NAME,
       trim(PP.RDB$PROCEDURE_NAME) as RDB$PROCEDURE_NAME,
       PP.RDB$PARAMETER_NUMBER,
       PP.RDB$PARAMETER_TYPE,
       trim(PP.RDB$FIELD_SOURCE) as RDB$FIELD_SOURCE,
       PP.RDB$DESCRIPTION,
       PP.RDB$SYSTEM_FLAG,
       PP.RDB$DEFAULT_SOURCE,
       PP.RDB$COLLATION_ID,
       iif(coalesce(PP.RDB$NULL_FLAG, 0) = 0, 0, 1) as RDB$NULL_FLAG,
       PP.RDB$PARAMETER_MECHANISM,
       trim(PP.RDB$FIELD_NAME) as RDB$FIELD_NAME,
       trim(PP.RDB$RELATION_NAME) as RDB$RELATION_NAME,
       trim(PP.RDB$PACKAGE_NAME) as RDB$PACKAGE_NAME
  from RDB$PROCEDURE_PARAMETERS PP";

        public override void FinishInitialization()
        {
            base.FinishInitialization();

            foreach (var procedureParameter in ProcedureParameters.Values)
            {
                if (procedureParameter.PackageName != null)
                {
                    procedureParameter.Package =
                        Metadata
                            .MetadataPackages
                            .PackagesByName[procedureParameter.PackageName];
                }
            }

            foreach (var procedure in ProceduresById.Values)
            {
                if (procedure.PackageName != null)
                {
                    procedure.Package =
                        Metadata
                            .MetadataPackages
                            .PackagesByName[procedure.PackageName];
                }
            }
        }

        IEnumerable<CommandGroup> ISupportsComment.Handle(IMetadata other, IComparerContext context)
        {
            var result = new CommandGroup().Append(HandleComment(ProceduresByName, other.MetadataProcedures.ProceduresByName, x => x.ProcedureName, "PROCEDURE", x => x.PackageName != null ? new[] { x.PackageName, x.ProcedureName } : new[] { x.ProcedureName }, context, x => HandleCommentNested(x.ProcedureParameters.OrderBy(y => y.ParameterNumber), other.MetadataProcedures.ProcedureParameters, (a, b) => new ProcedureParameterKey(a, b), x.ProcedureName, y => y.ParameterName, "PARAMETER", y => new[] { y.ParameterName }, context)));
            if (!result.IsEmpty)
            {
                yield return result;
            }
        }
    }
}