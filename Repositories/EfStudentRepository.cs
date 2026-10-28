using estudiantes_icinf.Data;
using estudiantes_icinf.Models;
using Microsoft.EntityFrameworkCore;

namespace estudiantes_icinf.Repositories;

public class EfStudentRepository : IStudentRepository
{
    private readonly AppDbContext _db;

    public EfStudentRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Student>> GetAllAsync()
    {
        return await _db.Students.ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        return await _db.Students.FindAsync(id);
    }

    public async Task<Student?> GetByEmailAsync(string email, Guid? excludeId = null)
    {
        return await _db.Students
            .FirstOrDefaultAsync(s => s.Email == email && s.Id != excludeId);
    }

    public async Task<Student> AddAsync(CreateStudentDto dto)
    {
        var student = new Student
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Age = dto.Age,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Students.Add(student);
        await _db.SaveChangesAsync();
        return student;
    }

    public async Task<Student?> UpdateAsync(Guid id, UpdateStudentDto dto)
    {
        var student = await _db.Students.FindAsync(id);
        if (student is null) return null;

        if (dto.Name is not null) student.Name = dto.Name;
        if (dto.Email is not null) student.Email = dto.Email;
        if (dto.Age is not null) student.Age = dto.Age.Value;
        student.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return student;
    }

    public async Task DeleteAsync(Guid id)
    {
        var student = await _db.Students.FindAsync(id);
        if (student is null) return;

        _db.Students.Remove(student);
        await _db.SaveChangesAsync();
    }
}