using System.Data;
using Dapper;

namespace Infrastructure.Data;

internal static class StoredProcedureHelper
{
    internal static CommandType Sp => CommandType.StoredProcedure;

    internal static DynamicParameters InsertOutNewId(
        Action<DynamicParameters> addInputs,
        string outParamName = "p_NewId")
    {
        var p = new DynamicParameters();
        addInputs(p);
        p.Add(outParamName, dbType: DbType.Int32, direction: ParameterDirection.Output);
        return p;
    }

    internal static int ReadNewId(DynamicParameters parameters, string outParamName = "p_NewId") =>
        parameters.Get<int>(outParamName);
}
