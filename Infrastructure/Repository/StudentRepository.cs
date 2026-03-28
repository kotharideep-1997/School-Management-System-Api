using Application.DTO;
using Application.IRepository;
using Dapper;
using Domain.Models;
using Infrastructure.Data;
using System.Data;

public class StudentRepository : IStudnetRepository
{
    private readonly IDbConnection _db;

    public StudentRepository(DbConnectionFactory factory)
    {
        _db = factory.CreateConnection();
    }

    public async Task<int> AddAsync(Student student)
    {
        var p = StoredProcedureHelper.InsertOutNewId(dp =>
        {
            dp.Add("p_FirstName", student.FirstName);
            dp.Add("p_LastName", student.LastName);
            dp.Add("p_RollNo", student.RollNo);
            dp.Add("p_ClassId", student.ClassId);
            dp.Add("p_Active", student.Active);
            dp.Add("p_CreatedDate", (DateTime?)null);
        });

        await _db.ExecuteAsync("sp_Students_Insert", p, commandType: StoredProcedureHelper.Sp);
        return StoredProcedureHelper.ReadNewId(p);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Students WHERE Id = @Id";
        return await _db.ExecuteAsync(sql, new { Id = id }) > 0;
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await _db.QueryAsync<Student>("sp_Students_SelectAll", commandType: StoredProcedureHelper.Sp);
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _db.QueryFirstOrDefaultAsync<Student>(
            "sp_Students_SelectById",
            new { p_Id = id },
            commandType: StoredProcedureHelper.Sp);
    }

    public async Task<PagedResultDto<Student>> GetPagedAsync(PagedRequestDto request)
    {
        var parameters = new
        {
            p_Offset = (request.PageNumber - 1) * request.PageSize,
            p_PageSize = request.PageSize
        };

        using var multi = await _db.QueryMultipleAsync(
            "sp_Students_SelectPaged",
            parameters,
            commandType: StoredProcedureHelper.Sp);

        var data = await multi.ReadAsync<Student>();
        var totalCount = await multi.ReadSingleAsync<int>();

        return new PagedResultDto<Student>
        {
            Data = data,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<IEnumerable<Student>> SearchAsync(StudentSearchRequestDto request)
    {
        var parameters = new
        {
            p_Name = string.IsNullOrWhiteSpace(request.Name) ? null : request.Name.Trim(),
            p_RollNo = request.RollNo,
            p_Offset = (request.PageNumber - 1) * request.PageSize,
            p_PageSize = request.PageSize
        };

        return await _db.QueryAsync<Student>(
            "sp_Students_Search",
            parameters,
            commandType: StoredProcedureHelper.Sp);
    }

    public async Task<bool> UpdateAsync(Student student)
    {
        const string sql = """
            UPDATE Students
            SET FirstName = @FirstName,
                LastName = @LastName,
                RollNo = @RollNo,
                ClassId = @ClassId,
                Active = @Active
            WHERE Id = @Id
            """;

        return await _db.ExecuteAsync(sql, student) > 0;
    }
}
