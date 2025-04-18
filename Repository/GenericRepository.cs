﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RoomFInder.Data;
using RoomFInder.Interface;
using RoomFInder.Models;

namespace RoomFInder.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T: class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }
    public async Task<IEnumerable<T>> GetActiveAsync()
    {
        if (!typeof(IRecStatusEntity).IsAssignableFrom(typeof(T)))
        {
            throw new InvalidOperationException("Entity does not support RecStatus.");
        }

        return await FindAsync(e => ((IRecStatusEntity)e).RecStatus == "A");
    }

    public async Task AddAsync(T entity)
    {
         await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync(); 
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.RemoveRange(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Comment>> GetCommentsByRoomIdAsync(int roomId)
    {
        if (typeof(T) == typeof(Comment))
        {
            return await _context.Comments
                .Where(c => c.RoomId == roomId)
                .ToListAsync();
        }

        throw new InvalidOperationException("This method is not supported for the current entity type.");
    }
    }
