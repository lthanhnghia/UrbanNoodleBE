using Pgvector.EntityFrameworkCore;
using System.Collections.Generic;
using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Repository.Interface;
using UrbanNoodle.Service;
using Microsoft.EntityFrameworkCore;
namespace UrbanNoodle.Repository
{
    public class KnowledgeChunksRepository : IKnowledgeChunksRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KnowledgeChunksRepository> _logger;

        public KnowledgeChunksRepository(ApplicationDbContext context, ILogger<KnowledgeChunksRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<string>> SearchSimilarContextAsync(float[] queryVector)
        {
            var pgVector = new Pgvector.Vector(queryVector);
            
           
            return await _context.KnowledgeChunks
                .OrderBy(x => x.Embedding.CosineDistance(pgVector))
                .Take(3)
                .Select(x => x.Content) 
                .ToListAsync();
        }
    }
}
