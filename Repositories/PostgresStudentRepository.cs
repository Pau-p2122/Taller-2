using Microsoft.EntityFrameworkCore;
using estudiantes_icinf.Models;
using estudiantes_icinf.Repositories;

public class PostgresStudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;

    public PostgresStudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await _context.Students.ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        return await _context.Students.FindAsync(id);
    }

    public async Task<Student?> GetByEmailAsync(string email, Guid? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.Email == email && s.Id != excludeId.Value);
        }
        return await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<Student> AddAsync(CreateStudentDto dto)
    {
        // Aplicando Transacción explícita (Requisito del taller)
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var student = new Student
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email
                // Ajusta las propiedades según lo que tenga tu DTO y modelo Student actual
            };

            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();

            // Confirmamos la transacción si todo sale bien
            await transaction.CommitAsync();
            return student;
        }
        catch (Exception)
        {
            // Revertimos cambios si ocurre un fallo (Rollback)
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Student> UpdateAsync(Guid id, UpdateStudentDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) throw new KeyNotFoundException("Estudiante no encontrado");

            if (dto.Name != null) student.Name = dto.Name;
            if (dto.Email != null) student.Email = dto.Email;

            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return student;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}